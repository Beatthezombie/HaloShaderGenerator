﻿#include "helpers\final_composite_helper.hlsli"

uniform float4 fill_color : register(c1);

float4 main(in float3 color : COLOR) : COLOR
{
    return float4(color * fill_color.r, 1.0f);
}
