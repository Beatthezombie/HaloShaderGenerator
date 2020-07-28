﻿#ifndef _BLACK_POINT_HLSLI
#define _BLACK_POINT_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/definition_helper.hlsli"
#include "..\helpers\apply_hlsl_fixes.hlsli"

void black_point_on(inout float alpha, float black_point)
{
    // black_point is the darkest pixel determined in the vertex shader
    // i'd assume something there is wrong
    
    if (!APPLY_HLSL_FIXES)
    {
        float white_point = 1.0f;
        
        float new_alpha = saturate((alpha - black_point) * (1 / (-black_point + 0.5f * (1 - -black_point))));
        float bwPoint = white_point + black_point;
        float mid_point = bwPoint * 0.5f; // (white_point + black_point) / 2.0f
        bwPoint = saturate(alpha - (bwPoint * 0.5f));
        alpha = mid_point * new_alpha + bwPoint;
    }
}

#endif