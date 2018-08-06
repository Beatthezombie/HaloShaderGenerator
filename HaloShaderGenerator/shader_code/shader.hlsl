﻿#define shader_template

#include "registers/shader.hlsli"

#include "methods/albedo.hlsli"
#include "helpers/color_processing.hlsli"

//TODO: These must be in the correct order for the registers to align, double check this
#include "methods\bump_mapping.hlsli"
#include "methods\alpha_test.hlsli"
#include "methods\specular_mask.hlsli"
#include "methods\material_model.hlsli"
#include "methods\environment_mapping.hlsli"
#include "methods\self_illumination.hlsli"
#include "methods\blend_mode.hlsli"
#include "methods\parallax.hlsli"
#include "methods\misc.hlsli"

struct VS_OUTPUT_ALBEDO
{
    float4 TexCoord : TEXCOORD;
    float4 TexCoord1 : TEXCOORD1;
    float4 TexCoord2 : TEXCOORD2;
    float4 TexCoord3 : TEXCOORD3;
};

struct PS_OUTPUT_ALBEDO
{
    float4 Diffuse;
    float4 Normal;
    float4 Unknown;
};

PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_ALBEDO input) : COLOR
{
    float2 texcoord = input.TexCoord.xy;
    float2 texcoord_tiled = input.TexCoord.zw;
    float3 tangentspace_x = input.TexCoord3.xyz;
    float3 tangentspace_y = input.TexCoord2.xyz;
    float3 tangentspace_z = input.TexCoord1.xyz;
    float3 unknown = input.TexCoord1.w;

    float3 diffuse;
    float alpha;
	{
        float4 diffuse_alpha = calc_albedo_ps(texcoord);
        diffuse = diffuse_alpha.xyz;
        alpha = diffuse_alpha.w;
    }
    diffuse = bungie_color_processing(diffuse);
    
    float3 normal = calc_bumpmap_ps(tangentspace_x, tangentspace_y, tangentspace_z, texcoord);

    PS_OUTPUT_ALBEDO output;
    output.Diffuse = blend_type(float4(diffuse, alpha));
    output.Normal = blend_type(float4(normal_export(normal), alpha));
    output.Unknown = unknown.xxxx;
    return output;
}

struct VS_OUTPUT_ACTIVE_CAMO
{
    float4 vPos : VPOS;
    float4 TexCoord : TEXCOORD0;
    float4 TexCoord1 : TEXCOORD1;
};

struct PS_OUTPUT_DEFAULT
{
    float4 HighFrequency;
    float4 LowFrequency;
    float4 Unknown;
};

float4 export_low_frequency(float4 input)
{
    float alpha = input.w;
    float3 color = input.xyz;

    return float4(color / g_exposure.y, alpha * g_exposure.z);
}

float4 export_high_frequency(float4 input)
{
    float alpha = input.w;
    float3 color = input.xyz;

    return float4(color, alpha * g_exposure.w);
}

PS_OUTPUT_DEFAULT entry_active_camo(VS_OUTPUT_ACTIVE_CAMO input) : COLOR
{
    //note: vpos is in range [0, viewportsize]
    // add a half pixel offset
    float2 vpos = input.vPos.xy;
    float2 screen_location = vpos + 0.5; // half pixel offset
    float2 texel_size = float2(1.0, 1.0) / texture_size.xy;
    float2 screen_coord = screen_location * texel_size; // converts to [0, 1] range

    // I'm not sure what is happening here with these three
    // but I think its a depth value, and this is a kind of
    // clamp of the effect at a distance
    float unknown0 = 0.5 - input.TexCoord1.w;
    float unknown1 = 1.0 / input.TexCoord1.w;
    float unknown2 = unknown0 >= 0 ? 2.0 : unknown1;

    // not sure where these values come from
    // however, the aspect ratio is 16:9 multiplied by 4
    float2 unknown3 = input.TexCoord.xy * k_ps_active_camo_factor.yz / float2(64, 36);

    float2 texcoord = unknown3 * unknown2 + screen_coord;

    float4 sample = tex2D(scene_ldr_texture, texcoord);
    float3 color = sample.xyz;

    float alpha = k_ps_active_camo_factor.x;
    
    PS_OUTPUT_DEFAULT output;
    output.HighFrequency = export_high_frequency(float4(color, alpha));
    output.LowFrequency = export_low_frequency(float4(color, alpha));
    output.Unknown = float4(0, 0, 0, 0);
    return output;
}
