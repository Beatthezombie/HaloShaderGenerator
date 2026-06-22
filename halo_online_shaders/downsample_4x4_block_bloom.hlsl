#line 2 "source\rasterizer\hlsl\downsample_4x4_block_bloom.hlsl"


#include "global.fx"
#include "hlsl_vertex_types.fx"
#include "utilities.fx"
#include "postprocess.fx"
#include "downsample_registers.fx"
//@generate screen


LOCAL_SAMPLER_2D(dark_source_sampler, 0);

#ifdef pc
float xenon_gamma_adjust(float x)
{
	float gamma;
	if (x < (64.0f / 1023.0f))
	{
		gamma = x * (1023.0f / 255.0f);
	} else
	if (x < (128.0f / 1023.0f))
	{
		gamma = (x * (341.0f / 170.0f)) + (32.0f / 255.0f);
	} else
	if (x < (256.0f / 1023.0f))
	{
		gamma = (x * (341.0f / 340.0f)) + (64.0f / 255.0f);
	} else
	{
		gamma = (x * (341.0f / 680.0f)) + (128.0f / 255.0f);
	}
	return gamma*gamma;
}

float3 xenon_gamma_adjust(float3 rgb)
{
	return float3(
		xenon_gamma_adjust(rgb.r),
		xenon_gamma_adjust(rgb.g),
		xenon_gamma_adjust(rgb.b));
}
#endif

float4 default_ps(screen_output IN) : SV_Target
{
#ifdef pc
	float3 color= 0.00000001f;						// hack to keep divide by zero from happening on the nVidia cards
#else
	float3 color= 0.0f;
#endif

	float4 sample= tex2D_offset(dark_source_sampler, IN.texcoord, -1, -1);
	color += sample.rgb * sample.rgb;
	sample= tex2D_offset(dark_source_sampler, IN.texcoord, +1, -1);
	color += sample.rgb * sample.rgb;
	sample= tex2D_offset(dark_source_sampler, IN.texcoord, -1, +1);
	color += sample.rgb * sample.rgb;
	sample= tex2D_offset(dark_source_sampler, IN.texcoord, +1, +1);
	color += sample.rgb * sample.rgb;
	color = xenon_gamma_adjust(color / 4.0f) * DARK_COLOR_MULTIPLIER;

	// calculate 'intensity'		(max or dot product?)
	float intensity= dot(color.rgb, intensity_vector.rgb);					// max(max(color.r, color.g), color.b);
	
	// calculate bloom curve intensity
	float bloom_intensity= max(intensity*scale.y, intensity-scale.x);		// ###ctchou $PERF could compute both parameters with a single mad followed by max
	
	// calculate bloom color
	float3 bloom_color= color * (bloom_intensity / intensity);
    
	return float4(bloom_color.rgb, intensity);
}
