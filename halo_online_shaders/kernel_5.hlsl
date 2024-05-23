#line 2 "source\rasterizer\hlsl\kernel_5.hlsl"

#include "global.fx"
#include "hlsl_vertex_types.fx"
#include "utilities.fx"
#include "postprocess.fx"
#include "kernel_5_registers.fx"
//@generate screen

LOCAL_SAMPLER_2D(target_sampler, 0);

float4 default_ps(screen_output IN) : SV_Target
{
	float2 sample= IN.texcoord;
	
    float2 pixel_size_scale = float2(1.0f / 1152, 1.0f / 640) / pixel_size.xy;

#ifdef APPLY_FIXES
	float4 color=	kernel[0].z * sample2D(target_sampler, sample + kernel[0].xy * pixel_size.xy * pixel_size_scale) +
					kernel[1].z * sample2D(target_sampler, sample + kernel[1].xy * pixel_size.xy * pixel_size_scale) +
					kernel[2].z * sample2D(target_sampler, sample + kernel[2].xy * pixel_size.xy * pixel_size_scale) +
					kernel[3].z * sample2D(target_sampler, sample + kernel[3].xy * pixel_size.xy * pixel_size_scale) +
					kernel[4].z * sample2D(target_sampler, sample + kernel[4].xy * pixel_size.xy * pixel_size_scale);
#else
   float4 color=	kernel[0].z * sample2D(target_sampler, sample + kernel[0].xy * pixel_size.xy) +
					kernel[1].z * sample2D(target_sampler, sample + kernel[1].xy * pixel_size.xy) +
					kernel[2].z * sample2D(target_sampler, sample + kernel[2].xy * pixel_size.xy) +
					kernel[3].z * sample2D(target_sampler, sample + kernel[3].xy * pixel_size.xy) +
					kernel[4].z * sample2D(target_sampler, sample + kernel[4].xy * pixel_size.xy);
#endif

	return color * scale;
}
