#ifndef ECHOS_CRY_LEAF_SWAY_INCLUDED
#define ECHOS_CRY_LEAF_SWAY_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Gentle canopy motion: radians, world units, and radians per second.
static const float LeafSwayRotation = 0.0105; // 0.6 degrees
static const float LeafSwayTranslation = 0.035;
static const float LeafSwaySpeed = 1.0;

float2 LeafSwayAngles(float time)
{
    float3 originWS = TransformObjectToWorld(float3(0, 0, 0));
    float phase = dot(originWS.xz, float2(0.73, 1.17));
    return sin(float2(time * LeafSwaySpeed + phase,
        time * LeafSwaySpeed * 0.79 + phase + 1.3)) * LeafSwayRotation;
}

float3 RotateLeafSway(float3 value, float2 angles)
{
    float2 s = sin(angles);
    float2 c = cos(angles);
    value.yz = float2(c.x * value.y - s.x * value.z, s.x * value.y + c.x * value.z);
    value.xy = float2(c.y * value.x - s.y * value.y, s.y * value.x + c.y * value.y);
    return value;
}

float3 LeafSwayPosition(float3 positionOS, float time)
{
    float2 angles = LeafSwayAngles(time);
    float3 positionWS = TransformObjectToWorld(RotateLeafSway(positionOS, angles));
    float3 originWS = TransformObjectToWorld(float3(0, 0, 0));
    float phase = dot(originWS.xz, float2(0.73, 1.17));
    positionWS += float3(sin(time * LeafSwaySpeed + phase),
        0.25 * sin(time * LeafSwaySpeed * 0.61 + phase),
        sin(time * LeafSwaySpeed * 0.83 + phase + 1.8)) * LeafSwayTranslation;
    return TransformWorldToObject(positionWS);
}

void ApplyLeafSway(inout float3 positionOS, inout float3 normalOS, inout float4 tangentOS)
{
    float2 angles = LeafSwayAngles(_Time.y);
    positionOS = LeafSwayPosition(positionOS, _Time.y);
    normalOS = RotateLeafSway(normalOS, angles);
    tangentOS.xyz = RotateLeafSway(tangentOS.xyz, angles);
}
#endif
