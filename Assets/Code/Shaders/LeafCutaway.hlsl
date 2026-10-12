#ifndef ECHOS_CRY_LEAF_CUTAWAY_INCLUDED
#define ECHOS_CRY_LEAF_CUTAWAY_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Set per camera immediately before rendering. No player means no cutaway.
float4 _LeafCutawayAnchor; // xyz: world position, w: enabled
float4 _LeafCutawaySettings; // radius in world units, edge softness, minimum world Y, height blend

void ApplyLeafCutaway(float4 positionCS)
{
    if (_LeafCutawayAnchor.w < 0.5)
        return;

    float2 uv = positionCS.xy / _ScaledScreenParams.xy;
    float depth = positionCS.z;
#if !UNITY_REVERSED_Z
    depth = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, depth);
#endif
    float3 positionWS = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);
    float3 anchorVS = TransformWorldToView(_LeafCutawayAnchor.xyz);
    float3 fragmentVS = TransformWorldToView(positionWS);

    // A camera-facing circle is round on screen, including orthographic isometric views.
    float radius = max(_LeafCutawaySettings.x, 0.001);
    float softness = clamp(_LeafCutawaySettings.y, 0.001, radius);
    float radialMask = 1.0 - smoothstep(radius - softness, radius,
        length(fragmentVS.xy - anchorVS.xy));
    float heightMask = smoothstep(_LeafCutawaySettings.z,
        _LeafCutawaySettings.z + max(_LeafCutawaySettings.w, 0.001), positionWS.y);
    // View-space forward is negative Z: only remove leaves in front of the anchor.
    float inFront = step(anchorVS.z + 0.01, fragmentVS.z);
    float visibility = 1.0 - radialMask * heightMask * inFront;

    // Ordered Bayer dithering: stable at rest, fully clear at the center.
    const float bayer[16] = {
         0,  8,  2, 10,
        12,  4, 14,  6,
         3, 11,  1,  9,
        15,  7, 13,  5
    };
    uint2 pixel = (uint2)positionCS.xy & 3u;
    float threshold = (bayer[pixel.y * 4u + pixel.x] + 0.5) / 16.0;
    clip(visibility - threshold);
}
#endif