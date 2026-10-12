"""Render deterministic 100 BPM rhythm mixes; retain the original songs.

Dependencies: numpy, scipy, soundfile, imageio-ffmpeg. The project-local
Temp/audio-tools directory is searched first. Run with the bundled Python.
"""
import json
import argparse
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "Temp/audio-tools"))
import imageio_ffmpeg
import numpy as np
import soundfile as sf
from scipy import signal
from scipy.ndimage import median_filter

TRACKS = {
    "level-one": ("LevelOneSong.mp3", 100, 128, .028),
    "level-two": ("LevelTwoSong.mp3", 95, 144, .030),
    "level-three": ("LevelThreeSong.mp3", 100, 256, .028),
    "level-four": ("LevelFourSong.mp3", 100, 224, .028),
    "level-five": ("LevelFiveSong.mp3", 98, 272, .028),
    "main-menu": ("TitleSong.wav", 100, 256, 0.),
    "town": ("TownSong.wav", 100, 176, 0.),
}


def percussion(sr):
    rng = np.random.default_rng(100)

    def noise(duration, low, high):
        t = np.arange(round(duration * sr)) / sr
        raw = rng.standard_normal(len(t))
        sos = signal.butter(2, [low, high], btype="bandpass", fs=sr, output="sos")
        filtered = signal.sosfilt(sos, raw)
        filtered /= max(np.max(np.abs(filtered)), 1e-9)
        return t, filtered

    t, click = noise(.25, 1500, 6000)
    frequency = 52 + 88 * np.exp(-t / .012)
    phase = 2 * np.pi * np.cumsum(frequency) / sr
    attack = np.minimum(t / .0015, 1)
    kick = (np.sin(phase) + .12 * np.sin(2 * phase)) * np.exp(-t / .065) * attack
    kick += .2 * click * np.exp(-t / .006) * attack
    kick *= np.minimum((.25 - t) / .01, 1)
    kick /= np.max(np.abs(kick))

    t, snap = noise(.18, 1200, 8500)
    attack = np.minimum(t / .0007, 1)
    snare = (.85 * snap * np.exp(-t / .037) +
             .24 * np.sin(2 * np.pi * 185 * t) * np.exp(-t / .026)) * attack
    snare *= np.minimum((.18 - t) / .01, 1)
    snare /= np.max(np.abs(snare))

    t, shimmer = noise(.09, 6500, 14000)
    hat = shimmer * np.exp(-t / .012) * np.minimum(t / .0005, 1)
    hat *= np.minimum((.09 - t) / .01, 1)
    hat /= np.max(np.abs(hat))
    return kick, snare, hat


def render(track="level-one"):
    filename, source_bpm, BEATS, LEAD_SECONDS = TRACKS[track]
    BPM = 98 if track == "level-five" else 100
    SOURCE = ROOT / "Assets/Audio/Music" / filename
    OUTPUT = SOURCE.with_name(SOURCE.stem + "_Rhythm.wav")
    PREVIEW = ROOT / "Temp/audio-analysis" / (SOURCE.stem + "_Rhythm_Preview.wav")
    original, sr = sf.read(SOURCE, dtype="float32", always_2d=True)
    samples_per_beat = sr * 60 / BPM
    frames = round(samples_per_beat * BEATS)
    offset = round(LEAD_SECONDS * sr)
    source_frames = round(sr * 60 / source_bpm * BEATS)
    music = original[offset:offset + source_frames].copy()
    assert len(music) == source_frames, "Source must contain the full musical loop."
    assert BEATS % 4 == 0
    if source_bpm != BPM:
        # FFmpeg atempo preserves pitch; timestamps are rebuilt at the new speed.
        PREVIEW.parent.mkdir(parents=True, exist_ok=True)
        trimmed = PREVIEW.parent / (SOURCE.stem + "_Trimmed.wav")
        stretched = PREVIEW.parent / (SOURCE.stem + "_Tempo.wav")
        sf.write(trimmed, music, sr, subtype="FLOAT")
        subprocess.run([imageio_ffmpeg.get_ffmpeg_exe(), "-hide_banner", "-loglevel", "error", "-y",
                        "-i", str(trimmed), "-af", f"atempo={BPM / source_bpm:.12f},apad",
                        "-t", str(frames / sr), "-c:a", "pcm_f32le", str(stretched)], check=True)
        music, stretched_sr = sf.read(stretched, dtype="float32", always_2d=True)
        assert stretched_sr == sr and len(music) == frames

    # Level Two's dominant musical pulse falls on the opposite eighth-note phase.
    # Rotate the complete loop so it coincides with the gameplay quarter-note grid.
    music_advance_seconds = .3 if track == "level-two" else 0.
    if music_advance_seconds:
        music = np.roll(music, -round(music_advance_seconds * sr), axis=0)
        # A shifted full mix still contains competing syncopated drum accents.
        # Reduce its percussive layer with stereo-linked harmonic/percussive masks
        # before rebuilding a clear attack pattern on the gameplay grid.
        _, _, spectrum = signal.stft(music, fs=sr, nperseg=2048, noverlap=1536, axis=0)
        magnitude = np.abs(spectrum).mean(axis=1)
        harmonic = median_filter(magnitude, size=(1, 31))
        percussive = median_filter(magnitude, size=(31, 1))
        mask = harmonic ** 2 / (harmonic ** 2 + percussive ** 2 + 1e-12)
        spectral_gain = .15 + .85 * mask
        music = np.stack([signal.istft(spectrum[:, channel, :] * spectral_gain,
            fs=sr, nperseg=2048, noverlap=1536)[1][:frames]
            for channel in range(music.shape[1])], axis=1)
        sf.write(PREVIEW.parent / (SOURCE.stem + "_AlignedMusic.wav"), music, sr, subtype="FLOAT")

    # Leave room for the percussion while retaining the original melody/stereo image.
    bass = signal.sosfilt(signal.butter(2, 150, fs=sr, output="sos"), music, axis=0)
    # Level Three's source is louder; balance it against the established mixes
    # so its added attack accents remain audible through the original arrangement.
    strong_backbeats = track in ("level-two", "level-three", "level-four", "level-five", "main-menu")
    source_gain = min(1., .22 / np.sqrt(np.mean(original ** 2))) if strong_backbeats else 1.
    music = (music - .18 * bass) * .74 * source_gain
    # A short fade at the join avoids a click without crossfading or moving beat positions.
    seam = round(.004 * sr)
    music[:seam] *= np.linspace(0, 1, seam)[:, None]
    music[-seam:] *= np.linspace(1, 0, seam)[:, None]
    kick, snare, hat = percussion(sr)
    drums = np.zeros_like(music)
    duck = np.ones(frames, dtype=np.float32)
    for beat in range(BEATS):
        start = round(beat * samples_per_beat)
        # Four distinct quarter-note pulses; snare backbeats mark special attacks.
        kick_level = [.46, .36, .40, .36][beat % 4]
        drums[start:start + len(kick)] += (kick * kick_level)[:, None]
        if beat % 4 in (1, 3):
            snare_level = .38 if strong_backbeats else .29
            drums[start:start + len(snare)] += (snare * snare_level)[:, None]
        drums[start:start + len(hat)] += (hat * .075)[:, None]
        t = np.arange(round(.15 * sr)) / sr
        reduction = .30 * np.exp(-t / .045) * np.minimum(t / .001, 1)
        duck[start:start + len(t)] -= reduction

    mixed = music * duck[:, None] + drums
    # Measure oversampled peaks before scaling, preserving sharp drum transients.
    true_peak = np.max(np.abs(signal.resample_poly(mixed, 4, 1, axis=0)))
    gain = min(1., 10 ** (-1.2 / 20) / true_peak)
    mixed *= gain
    rng = np.random.default_rng(128)
    dither = (rng.random(mixed.shape) - rng.random(mixed.shape)) / 65536
    sf.write(OUTPUT, mixed + dither, sr, subtype="PCM_16")
    PREVIEW.parent.mkdir(parents=True, exist_ok=True)
    preview_frames = round(samples_per_beat * 32)
    sf.write(PREVIEW, mixed[:preview_frames] + dither[:preview_frames], sr, subtype="PCM_16")

    # Verify the exported file, beat grid and peaks after quantization.
    exported, exported_sr = sf.read(OUTPUT, always_2d=True)
    assert exported_sr == sr and len(exported) == frames
    assert np.isfinite(exported).all() and np.max(np.abs(exported)) < 1
    true_peak_db = 20 * np.log10(np.max(np.abs(signal.resample_poly(exported, 4, 1, axis=0))))
    assert true_peak_db <= -1.0, true_peak_db
    # Confirm the attack stem has a strong transient inside 20 ms of every beat.
    onset_peaks_ms = []
    for beat in range(BEATS):
        start = round(beat * samples_per_beat)
        transient = np.abs(drums[start:start + round(.02 * sr), 0])
        assert np.max(transient) > .25
        onset_peaks_ms.append(float(np.argmax(transient) * 1000 / sr))
    report = {"source": str(SOURCE.relative_to(ROOT)), "output": str(OUTPUT.relative_to(ROOT)),
              "bpm": BPM, "beats": BEATS, "bars": BEATS // 4,
              "source_bpm": source_bpm, "tempo_speed_ratio": BPM / source_bpm,
              "music_advance_seconds": music_advance_seconds,
              "source_gain": float(source_gain), "backbeat_snare_level": .38 if strong_backbeats else .29,
              "source_duration_seconds": len(original) / sr, "leading_trim_seconds": LEAD_SECONDS,
              "duration_seconds": len(exported) / sr, "sample_rate": sr, "samples_per_beat": samples_per_beat,
              "frames": len(exported), "true_peak_dbfs_4x": float(true_peak_db),
              "rms_dbfs": float(20 * np.log10(np.sqrt(np.mean(exported ** 2)))),
              "latest_attack_peak_ms": max(onset_peaks_ms), "clipped_samples": 0}
    result = subprocess.run([imageio_ffmpeg.get_ffmpeg_exe(), "-hide_banner", "-i", str(OUTPUT),
                             "-af", "loudnorm=I=-16:TP=-1:LRA=11:print_format=json", "-f", "null", "-"],
                            capture_output=True, text=True, check=True)
    last_json = result.stderr[result.stderr.rfind("{"):]
    report["loudness_analysis"] = json.JSONDecoder().raw_decode(last_json)[0]
    (PREVIEW.parent / (SOURCE.stem + "-rhythm-mix-report.json")).write_text(json.dumps(report, indent=2))
    print(json.dumps(report, indent=2))


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--track", choices=list(TRACKS), default="level-one")
    render(parser.parse_args().track)
