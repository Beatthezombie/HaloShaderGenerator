#line 2 "source\rasterizer\hlsl\blur_11_vertical.hlsl"

#include "global.fx"
#include "hlsl_vertex_types.fx"
#include "utilities.fx"
#include "postprocess.fx"
//@generate screen

LOCAL_SAMPLER_2D(target_sampler, 0);
//float4 kernel[11] : register(c2);		// c2 through c12 are the kernel (r,g,b)

float4 blur_13_v(float2 sample)
{	
	const float2 offset_new[9]=
		{
			{0.5,	-4.0 - 1.0			/	(1.0+17.0)			},					//  -4.1
			{0.5,	-3.0 - 136.0		/	(136.0+680.0)		},					//  
			{0.5,	-2.0 - 2380.0		/	(2380.0+6188.0)		},					//  -2.3
			{0.5,	-1.0 - 12376.0		/	(12376.0+19448.0)	},					//  
			{0.5,	 0.0 - 24310.0		/	(24310.0+24310.0)	},					//	-0.5
			{0.5,	+1.0 - 19448.0		/	(12376.0+19448.0)	},					//  
			{0.5,	+2.0 - 6188.0		/	(2380.0+6188.0)		},					//  +1.3
			{0.5,	+3.0 - 680.0		/	(136.0+680.0)		},					//  
			{0.5,	+4.0 - 17.0			/	(1.0+17.0)			}					//  +3.1
		};
	
    float2 pixel_size_scale = float2(1.0f / 1152, 1.0f / 640) / pixel_size.xy;
	
    float4 color =	(1.0 + 17.0) *			sample2D(target_sampler, sample + offset_new[0] * pixel_size * pixel_size_scale) +
					(136.0 + 680.0) *		sample2D(target_sampler, sample + offset_new[1] * pixel_size * pixel_size_scale) +
					(2380.0 + 6188.0) *		sample2D(target_sampler, sample + offset_new[2] * pixel_size * pixel_size_scale) +
					(12376.0 + 19448.0) *	sample2D(target_sampler, sample + offset_new[3] * pixel_size * pixel_size_scale) +
					(24310.0 + 24310.0) *	sample2D(target_sampler, sample + offset_new[4] * pixel_size * pixel_size_scale) +
					(12376.0 + 19448.0) *	sample2D(target_sampler, sample + offset_new[5] * pixel_size * pixel_size_scale) +
					(2380.0 + 6188.0) *		sample2D(target_sampler, sample + offset_new[6] * pixel_size * pixel_size_scale) +
					(136.0 + 680.0) *		sample2D(target_sampler, sample + offset_new[7] * pixel_size * pixel_size_scale) +
					(1.0 + 17.0) *			sample2D(target_sampler, sample + offset_new[8] * pixel_size * pixel_size_scale);
					
    return color / 131072.0;
}

fast4 default_ps(screen_output IN) : SV_Target
{
	float2 sample= IN.texcoord;

#ifdef APPLY_FIXES
    return blur_13_v(sample);
#endif
/*
	sample.y -= 5.0 * pixel_size.y;		// -5 through +5

	fast3 color= 0.0;
	for (int y= 0; y < 11; y++)
	{
		color += kernel[y].rgb * convert_from_bloom_buffer(sample2D(target_sampler, sample));
		sample.y += pixel_size.y;
	}
*/
	// solution using bilinear filtering:
	// actually this is a 10 wide blur - you get the 11th pixel by offsetting the vertical blur by half a pixel
	//
	// horizontal pass has the effect of shifting the center half a pixel to the left and down
	// vertical pass shifts it half a pixel up and right
	// result is an 11x11 gaussian blur that is perfectly centered
	//
	//   C = center pixel
	//   x = horizontal sample positions
	//   y = vertical sample positions
	//
	//
	//                      .---.---.
	//                      |   |   |
	//                      |---y---|
	//                      |   |   |
	//                      |---|---|
	//                      |   |   |
	//                      |---y---|
	//                      |   |   |
	//                      |---|---|
	//                      |   |   |
	//  .---.---.---.---.---|---y---|---.---.---. . .
	//  |   |   |   |   |   | C |   |   |   |   |   .
	//  '---x---|---x---|---x---|---x---|---x---| . .
	//  |   |   |   |   |   |   |   |   |   |   |   .
	//  '---'---'---'---'---'---y---'---'---'---' . .
	//                      |   |   |
	//                      |---|---|
	//                      |   |   |
	//                      |---y---|
	//                      |   |   |
	//                      '---'---'
	//						`   `   `		<-- virtual pixel you get for 'free' because of the half-pixel shift down in the horizontal pass
	//                      ' - ' - '
	//
	//
	// hard-coded kernel
	//
	//		[1  9]  [36  84]  [126  126]  [84  36]  [9  1]			/ 512
	//
	// Note:  with the half-pixel offset in the other direction, this kernel becomes:
	//
	//		1  10  45  120  210  252  210  120  45  10  1			/ 1024
	
	const float2 offset[5]=
		{
			{0.5,	-4.0 - 1.0 /(1.0+9.0)			},			// -4.1
			{0.5,	-2.0 - 36.0/(36.0+84.0)			},			// -2.3
			{0.5,	 0.0 - 126.0/(126.0+126.0)		},			// -0.5
			{0.5,	+2.0 - 84.0/(84.0+36.0)			},			// +1.3
			{0.5,	+4.0 - 9.0/(1.0+9.0)			}			// +3.1
		};

#ifdef APPLY_FIXES
    float2 pixel_size_scale = float2(1.0f / 1152, 1.0f / 640) / pixel_size.xy;
	
	float4 color=	(1.0   + 9.0)	* sample2D(target_sampler, sample + offset[0] * pixel_size * pixel_size_scale) +
					(36.0  + 84.0)	* sample2D(target_sampler, sample + offset[1] * pixel_size * pixel_size_scale) +
					(126.0 + 126.0)	* sample2D(target_sampler, sample + offset[2] * pixel_size * pixel_size_scale) +
					(84.0  + 36.0)	* sample2D(target_sampler, sample + offset[3] * pixel_size * pixel_size_scale) +
					(1.0   + 9.0)	* sample2D(target_sampler, sample + offset[4] * pixel_size * pixel_size_scale);
#else
    float4 color=	(1.0   + 9.0)	* sample2D(target_sampler, sample + offset[0] * pixel_size) +
					(36.0  + 84.0)	* sample2D(target_sampler, sample + offset[1] * pixel_size) +
					(126.0 + 126.0)	* sample2D(target_sampler, sample + offset[2] * pixel_size) +
					(84.0  + 36.0)	* sample2D(target_sampler, sample + offset[3] * pixel_size) +
					(1.0   + 9.0)	* sample2D(target_sampler, sample + offset[4] * pixel_size);
#endif

	return color / 512.0;
}
