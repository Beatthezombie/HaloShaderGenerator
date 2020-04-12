﻿#ifndef _INPUT_OUTPUT_HLSLI
#define _INPUT_OUTPUT_HLSLI

struct VS_OUTPUT_ALBEDO
{
    float4 position : SV_Position;
    float2 texcoord : TEXCOORD;
    float4 normal : TEXCOORD1;
    float3 binormal : TEXCOORD2;
    float3 tangent : TEXCOORD3;
    float3 camera_dir : TEXCOORD4;
};

struct VS_OUTPUT_STATIC_PRT
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	float3 tangent : TEXCOORD5;
	float3 camera_dir : TEXCOORD6;
	float4 prt_radiance_vector : TEXCOORD7;
	float3 extinction_factor : COLOR;
	float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_ACTIVE_CAMO
{
	float4 position : SV_Position;
    float4 texcoord2 : TEXCOORD1;
	float2 texcoord1 : TEXCOORD0;
};

struct VS_OUTPUT_SFX_DISTORT
{
	float4 position : SV_Position;
	float4 texcoord : TEXCOORD;
	float distortion : TEXCOORD1;
};

struct PS_OUTPUT_ALBEDO
{
	float4 diffuse;
	float4 normal;
	float4 unknown;
};

struct PS_OUTPUT_DEFAULT
{
    float4 low_frequency;
    float4 high_frequency;
	float4 unknown;
};

#endif