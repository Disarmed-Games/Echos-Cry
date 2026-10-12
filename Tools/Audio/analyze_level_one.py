"""Inspect the source track against the game's 100 BPM attack grid."""
import json
import argparse
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "Temp/audio-tools"))
import numpy as np
import soundfile as sf
from scipy import signal

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("source", nargs="?", default="Assets/Audio/Music/LevelOneSong.mp3")
parser.add_argument("--bpm", type=float, default=100)
args = parser.parse_args()
source = ROOT / args.source
audio, sr = sf.read(source, dtype="float32", always_2d=True)
mono = audio.mean(axis=1)
analysis_sr = 11025
mono = signal.resample_poly(mono, analysis_sr, sr)
hop = 110
frequencies, times, spectrum = signal.stft(mono, analysis_sr, nperseg=1024, noverlap=1024-hop)
magnitude = np.abs(spectrum)
bands = {}
for name, low, high in [("full", 35, 5000), ("bass", 35, 220), ("attack", 700, 5000)]:
    band = np.log1p(100 * magnitude[(frequencies >= low) & (frequencies < high)])
    flux = np.maximum(np.diff(band, axis=1, prepend=band[:, :1]), 0).sum(axis=0)
    flux /= max(float(np.percentile(flux, 90)), 1e-8)
    bands[name] = flux

def grid_score(bpm, phase, start=2, end=None, band="full"):
    period = 60 / bpm
    grid = np.arange(phase, len(audio)/sr, period)
    grid = grid[(grid >= start) & (grid < (len(audio)/sr-2 if end is None else end))]
    # Tolerate 20 ms onset smearing from the analysis window.
    values = [np.interp(grid + shift, times, bands[band]) for shift in [-.02, -.01, 0, .01, .02]]
    return float(np.mean(np.max(values, axis=0)))

candidates = []
for bpm in np.arange(80, 141, .1):
    phases = np.arange(0, 60/bpm, .01)
    scores = [grid_score(bpm, phase) for phase in phases]
    best = int(np.argmax(scores))
    candidates.append((float(max(scores)), float(bpm), float(phases[best])))
candidates.sort(reverse=True)
fixed = [(grid_score(args.bpm, phase), float(phase)) for phase in np.arange(0, 60/args.bpm, .002)]
fixed.sort(reverse=True)
segments = []
for start in range(0, int(len(audio)/sr)-10, 10):
    scores = [(grid_score(args.bpm, phase, start, start+10), float(phase)) for phase in np.arange(0, 60/args.bpm, .002)]
    segments.append({"start": start, "phase": max(scores)[1]})
report = {"sample_rate": sr, "samples": len(audio), "duration": len(audio)/sr,
          "peak": float(np.max(np.abs(audio))), "rms": float(np.sqrt(np.mean(audio**2))),
          "tempo_candidates": candidates[:12], "grid_bpm": args.bpm, "grid_phase": fixed[:5], "segment_phases": segments}
report["leading_signal_seconds"] = {str(threshold): float(np.flatnonzero(np.max(np.abs(audio), axis=1) > threshold)[0] / sr)
                                    for threshold in [.0001, .001, .005]}
report["band_alignment"] = {}
for band in bands:
    phase_scores = [(grid_score(args.bpm, phase, band=band), float(phase))
                    for phase in np.arange(0, 60 / args.bpm, .002)]
    score, phase = max(phase_scores)
    report["band_alignment"][band] = {"best_phase_seconds": phase, "best_score": score,
        "on_beat_score": grid_score(args.bpm, 0, band=band),
        "half_beat_score": grid_score(args.bpm, 30 / args.bpm, band=band)}
out = ROOT / "Temp/audio-analysis"
out.mkdir(parents=True, exist_ok=True)
(out / (source.stem + "-source-analysis.json")).write_text(json.dumps(report, indent=2))
print(json.dumps(report, indent=2))
