"""Create a tight Excellent-hit accent from the original drum sample."""
from pathlib import Path
import math
import struct
import wave

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Assets/Audio/Effects/Player/Drums/Drum.wav"
OUTPUT = ROOT / "Assets/Audio/Effects/Player/Quality Hit/Excellent Drum Crack.wav"

with wave.open(str(SOURCE), "rb") as source:
    channels, width, rate, frames, _, _ = source.getparams()
    if width != 2:
        raise ValueError("Expected 16-bit PCM drum source")
    samples = struct.unpack(f"<{frames * channels}h", source.readframes(frames))

length = min(frames, round(rate * 0.180))
result = []
for frame in range(length):
    seconds = frame / rate
    # Preserve the initial crack; suppress the ringing body and tail.
    envelope = math.exp(-max(0.0, seconds - 0.025) / 0.045)
    envelope *= min(1.0, frame / max(1, round(rate * 0.0005)))
    envelope *= min(1.0, (length - 1 - frame) / max(1, round(rate * 0.012)))
    for channel in range(channels):
        result.append(round(samples[frame * channels + channel] * envelope))

with wave.open(str(OUTPUT), "wb") as output:
    output.setparams((channels, width, rate, 0, "NONE", "not compressed"))
    output.writeframes(struct.pack(f"<{len(result)}h", *result))
print(f"Created {OUTPUT.name}: {length / rate:.3f}s")
