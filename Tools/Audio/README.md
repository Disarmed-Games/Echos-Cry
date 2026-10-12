# Rhythm mixes

Level Five and the main menu also use the stronger 0.38 snare accents on beats
2 and 4. LevelFiveSong retains the scene's 98 BPM and loops for 272 beats
(68 bars, 166.530604 seconds), with 28 ms leading padding removed. Beat sample
positions are rounded individually to avoid cumulative timing drift at 98 BPM.
TitleSong retains 100 BPM and its 256-beat loop (64 bars, 153.6 seconds), with no
leading trim. Their original source files are unchanged. Full-track analysis
confirms the corresponding tempos and beat alignment; both mixes have no
clipped samples and about -1.20 dBTP. Integrated loudness is -15.32 LUFS for
Level Five and -16.49 LUFS for TitleSong. Render with `--track level-five` or
`--track main-menu`; analyze Level Five with `--bpm 98`.

`Assets/Audio/Music/LevelOneSong_Rhythm.wav` retains the original music, with
quarter-note kick and hi-hat pulses and snare accents on beats 2 and 4, matching
the special-attack beats in `TempoConductor`. A brief reduction in the music
under each transient makes the attack pulse clearer.

The source analysis detects 100 BPM. The mix removes 28 ms from the beginning
of the MP3 and exports exactly 128 beats (32 bars, 76.8 seconds) at 48 kHz.
Short fades at the loop boundary prevent clicks. The Unity importer uses PCM
to preserve the exact sample count and avoid compression padding. The original
`LevelOneSong.mp3` is unchanged. LevelOneScene and Enemy Test Scene use this mix.

Measured output: -16.42 LUFS integrated, -1.20 dBTP, no clipped samples.
The added attack transients peak within 1.53 ms of their quarter-note grid.
These checks do not replace a listening and gameplay test in Unity.

The same percussion treatment is also applied to `LevelTwoSong_Rhythm.wav` and
`TownSong_Rhythm.wav`. LevelTwoSong's measured source tempo is 95 BPM; FFmpeg's
pitch-preserving `atempo` filter brings it to the scene's 100 BPM grid. Its loop
is 144 beats (36 bars, 86.4 seconds), with 30 ms of MP3 leading padding removed.
TownSong stays at its original 100 BPM and 176 beats (44 bars, 105.6 seconds),
with no leading trim or tempo change. LevelTwoScene and TownScene use their
respective new mixes; TutorialScene also uses the Town mix. Both original
source assets are unchanged.

| Mix | Integrated loudness | True peak | Clipped samples |
| --- | --- | --- | --- |
| Level Two | -17.35 LUFS | -1.81 dBTP | 0 |
| Town | -15.43 LUFS | -1.21 dBTP | 0 |
| Level Three | -15.61 LUFS | -1.20 dBTP | 0 |
| Level Four | -16.45 LUFS | -1.21 dBTP | 0 |

`LevelThreeSong_Rhythm.wav` keeps the source's 100 BPM, removes 28 ms of
leading padding, and loops for 256 beats (64 bars, 153.6 seconds). The original
arrangement is reduced by about 2.56 dB before mixing to give the percussion
room. Beats 2 and 4 use a snare level of 0.38 versus 0.29 in the earlier mixes
(about 2.35 dB stronger). LevelThreeScene uses the new mix; the original MP3 is
unchanged. Full-track analysis confirms 100 BPM, with every analyzed 10-second
segment within 20 ms of the beat grid, without accumulated drift. The added
attack transients peak within 1.53 ms of the grid.

## Reproduce

Level Two phase correction: the tempo-adjusted original music's dominant pulse
was 300 ms after the added drums, placing it halfway between gameplay beats.
The renderer now circularly advances the complete underlying loop by 300 ms
before adding percussion. Independent analysis of `LevelTwoSong_AlignedMusic.wav`
confirms 100 BPM, with all analyzed 10-second segments within 20 ms of the grid.
The loop remains 144 beats / 86.4 seconds and uses the existing asset GUID.
The pre-correction mix is retained in `Temp/audio-analysis/LevelTwoSong_Rhythm_BeforePhaseFix.wav`.

Following gameplay feedback, Level Two also reduces the original mix's
percussive layer with a stereo-linked harmonic/percussive median-filter mask
(retaining 15% of purely percussive components). This reduces competing
syncopated accents while preserving the harmonic arrangement. The rebuilt
quarter-note pattern now uses the stronger 0.38 beat 2/4 snare accents.
File analysis and code inspection cannot confirm perceived alignment in live
gameplay; the updated mix still needs the player's in-game check.

`LevelFourSong_Rhythm.wav` uses the same stronger beat 2/4 accents as Level
Three. It retains 100 BPM, removes 28 ms of leading padding, and loops for
224 beats (56 bars, 134.4 seconds). LevelFourScene uses this mix, and the
original MP3 is unchanged. Render it with `mix_level_one.py --track level-four`.

Install `requirements.txt` with Python 3.12. Alternatively, the scripts can use
dependencies installed in the project-local ignored `Temp/audio-tools` folder.
Run `analyze_level_one.py [source-path]` to inspect tempo/phase and
`mix_level_one.py --track level-one`, `--track level-two`, `--track level-three`, or `--track town` to
render a track. The default track remains Level One. Rendering is deterministic
and does not overwrite the originals. Detailed per-track analysis and a
19.2-second preview are written to `Temp/audio-analysis`.
