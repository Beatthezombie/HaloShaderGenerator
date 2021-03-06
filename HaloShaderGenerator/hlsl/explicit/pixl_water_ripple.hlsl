﻿
#define k_water_ripple_update 0
#define k_water_ripple_apply 1
#define k_water_ripple_slope 2
#define k_water_ripple_underwater_fog 3
#define k_water_ripple_add 4

// define this in compiler, iterate to compile the full pixl
#define WATER_RIPPLE_SHADER 0

#define k_underwater_murkiness_multiplier 1.44269502
#define k_pi 3.14159274

struct PS_OUTPUT_DEFAULT
{
    float4 ldr;
    float4 hdr;
    float4 unknown;
};

struct RIPPLE_OUTPUT
{
    float4 position : POSITION;
    float4 texcoord : TEXCOORD;
    float2 unknown1 : TEXCOORD1;
    float3 unknown2 : TEXCOORD2;
};

#if WATER_RIPPLE_SHADER == k_water_ripple_update
float4 main() : COLOR
{
    return float4(0, 1, 2, 3);
}
#endif
#if WATER_RIPPLE_SHADER == k_water_ripple_apply

uniform sampler2D tex_ripple_pattern : register(s0);

float4 main(RIPPLE_OUTPUT input) : COLOR
{
    float4 ripple_pattern = tex2D(tex_ripple_pattern, input.texcoord.xy);
    
    float ripple_x = ripple_pattern.r;
    ripple_x -= 0.5f;
    ripple_x *= input.texcoord.w;
    
    if (-0.00999999978 < input.unknown1.x)
    {
        float2 tex = input.texcoord.xy * 2 - 1;
        float d_tex = dot(tex, tex);
        d_tex = rcp(rsqrt(d_tex));
        d_tex = d_tex * -input.unknown1.y + input.unknown1.x;
        d_tex = frac(d_tex * ((1 / k_pi) / 2) + 0.5f);
        d_tex = sin(d_tex * (2 * k_pi) - k_pi);
        ripple_x *= d_tex;
    }
    
    float ripple_y = 0;
    if (0.00999999978 < input.unknown2.x)
    {        
        float2 tex = input.texcoord.xy * 2 - 1;
        float d_tex = dot(tex, tex);
        d_tex = rcp(rsqrt(d_tex));
        d_tex = -d_tex + input.unknown2.y;
        
        float sharpness;
        if (input.unknown2.z - 0.00100000005 >= 0)
            sharpness = 1.0f / input.unknown2.z;
        else 
            sharpness = 1000.0f;

        sharpness = saturate(d_tex * sharpness);
        sharpness *= input.unknown2.x;
        sharpness *= ripple_pattern.a;
        if (d_tex >= 0)
            ripple_y = sharpness;
    }
    
    return float4(ripple_x, ripple_y, 0, 0);
}
#endif
#if WATER_RIPPLE_SHADER == k_water_ripple_slope

uniform sampler2D tex_ripple_buffer_height : register(s1);

float4 main(float2 texcoord : TEXCOORD) : COLOR
{
    float2 height = tex2D(tex_ripple_buffer_height, texcoord).r;
    float height_upper_x = tex2D(tex_ripple_buffer_height, texcoord + float2(0.00124999997, 0)).r;
    float height_upper_y = tex2D(tex_ripple_buffer_height, texcoord + float2(0, 0.00124999997)).r;
    
    float2 height_diff = float2(height_upper_x - height.x, height_upper_y - height.x);
    height_diff = saturate(height_diff * 0.5 + 0.5);
    
    float2 slope_scale = -abs(-0.5 + texcoord) + 0.497000009;
    slope_scale = saturate(slope_scale * 100);
    
    height.x += 1.0f;
    height.x = saturate(height.x * 0.5);
    
    float4 result = float4(height.x, height_diff, height.y) + float4(0.5, 0.5, 0.5, 0);
    return result * min(slope_scale.x, slope_scale.y) - float4(0.5, 0.5, 0.5, 0);
}
#endif

#if WATER_RIPPLE_SHADER == k_water_ripple_underwater_fog

uniform float4 g_exposure : register(c0);
uniform float4x4 k_ps_water_view_xform_inverse : register(c213);
uniform float4 k_ps_water_player_view_constant : register(c218);
uniform float4 k_ps_camera_position : register(c219);
uniform float k_ps_underwater_murkiness : register(c220);
uniform float3 k_ps_underwater_fog_color : register(c221);
uniform sampler2D tex_ldr_buffer : register(s0);
uniform sampler2D tex_depth_buffer : register(s1);

PS_OUTPUT_DEFAULT main(in float4 position : POSITION, in float4 texcoord : TEXCOORD) : COLOR
{
    float4 modulated_tex;
    modulated_tex.xy = texcoord.xy / texcoord.w;
    
    float2 scene_tex = modulated_tex.xy * 0.5f + 0.5f;
    scene_tex.y = 1.0f - scene_tex.y;
    scene_tex = scene_tex * k_ps_water_player_view_constant.zw + k_ps_water_player_view_constant.xy;
    
    float scene_depth = tex2D(tex_depth_buffer, scene_tex).r;
    float3 scene_color = tex2D(tex_ldr_buffer, scene_tex).rgb;
    
    // z = depth, w = 1.0f
    modulated_tex.zw = scene_depth * float2(1, 0) + float2(0, 1);
    
    float4 water_view = mul(modulated_tex, k_ps_water_view_xform_inverse);
    
    water_view.xyz = water_view.xyz * -(1.0f / water_view.w) + k_ps_camera_position.xyz;
    float view_murkiness = rcp(rsqrt(dot(water_view.xyz, water_view.xyz))) * k_ps_underwater_murkiness;
    view_murkiness *= k_underwater_murkiness_multiplier;
    view_murkiness = saturate(1.0f / exp2(view_murkiness));
    
    view_murkiness = -view_murkiness + 1.0f;
    view_murkiness = -view_murkiness + 1.0f;
    view_murkiness *= 0.5f;
    
    float3 fog_color = lerp(scene_color, k_ps_underwater_fog_color, view_murkiness);
    
    fog_color = max(fog_color, 0);
    
    PS_OUTPUT_DEFAULT result;
    
    result.ldr.rgb = fog_color;
    result.hdr.rgb = fog_color / g_exposure.y;
    result.ldr.a = g_exposure.w;
    result.hdr.a = g_exposure.z;
    result.unknown = 0;
    
    return result;
}
#endif

#if WATER_RIPPLE_SHADER == k_water_ripple_add
float4 main() : COLOR
{
    return float4(0, 1, 2, 3);
}
#endif