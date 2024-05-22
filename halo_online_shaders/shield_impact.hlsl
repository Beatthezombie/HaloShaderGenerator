//#line 2 "source\rasterizer\hlsl\shield_effect.hlsl"

#include "global.fx"
#include "hlsl_constant_mapping.fx"

#include "deform.fx"

#define LDR_ALPHA_ADJUST g_exposure.w
#define HDR_ALPHA_ADJUST g_exposure.b
#define DARK_COLOR_MULTIPLIER g_exposure.g
#include "render_target.fx"
#include "shield_impact_registers.fx"

// noise textures
LOCAL_SAMPLER_2D(shield_impact_noise_texture1, 0);
LOCAL_SAMPLER_2D(shield_impact_noise_texture2, 1);


// Magic line to compile this for various needed vertex types
//@generate rigid
//@generate world
//@generate skinned

struct s_vertex_out
{
	float4 position : SV_Position;
	float4 world_space_pos : TEXCOORD1;
	float4 texcoord : TEXCOORD2;
};

s_vertex_out default_vs(
	in vertex_type vertex_in
	)
{
	s_vertex_out vertex_out;

	float4 local_to_world_transform[3];
	
	deform(vertex_in, local_to_world_transform);
	 	
	vertex_in.position+= vertex_in.normal * extrusion_distance;
	
	vertex_out.world_space_pos= float4(vertex_in.position, 1.0f);	
	vertex_out.position= mul(float4(vertex_in.position, 1.0f), View_Projection);	
	vertex_out.texcoord.xyzw= vertex_in.texcoord.xyxx;
	
	return vertex_out;
}

// things to expose:
// VS
// extrusion amount
// PS
// texture 1 & 2 scroll rate
// shield color, shield hot color
// intensity exponent, bias, and scale



accum_pixel default_ps(s_vertex_out pixel_in)
{
	// Some old grid code
	//float3 grid_test_point= float3(pixel_in.world_space_pos.xyz - k_ps_bound_sphere.xyz);
	
	//float3 xyz_relative_frac= frac(grid_test_point * 20.0f + (plasma_value.xyz - 0.5f));
	//xyz_relative_frac= min((1.0f).xxx - xyz_relative_frac, xyz_relative_frac);
	//float dist_from_grid= min(xyz_relative_frac.x, min(xyz_relative_frac.y, xyz_relative_frac.z));
	//dist_from_grid= 2.0f * (0.5f - dist_from_grid);

	//float mesh_factor= pow(dist_from_grid.xxx, 12.0f);
	
	//xyz_relative= pixel_in.world_space_pos.xyz;
	
	float3 xyz_relative= float3(pixel_in.world_space_pos.xyz - bound_sphere.xyz);
	
	float noise_value1, noise_value2;
	
	float time_parameter= texture_quantities.y * shield_dynamic_quantities.x;
	noise_value1= sample2D(shield_impact_noise_texture1, (pixel_in.texcoord.xy + float2(time_parameter / 12.0f, time_parameter / 13.0f)) * texture_quantities.x);
	noise_value2= sample2D(shield_impact_noise_texture2, (pixel_in.texcoord.xy - float2(time_parameter / 11.0f, time_parameter / 17.0f)) * texture_quantities.x);
	
	float plasma_base= 1.0f-abs(noise_value1 - noise_value2);
	float plasma_value1= max(0, (pow(plasma_base, plasma1_settings.x)-plasma1_settings.z) * plasma1_settings.y);
	float plasma_value2= max(0, (pow(plasma_base, plasma2_settings.x)-plasma2_settings.z) * plasma2_settings.y);

	float non_plasma_value= 1.0f - min(1.0f, (plasma_value1 + plasma_value2));
	float shield_impact_factor= shield_dynamic_quantities.y;
	float overshield_factor= shield_dynamic_quantities.z;
	
	float3 semifinal_shield_impact_color= shield_impact_factor * (plasma_value1 * shield_impact_color1 + plasma_value2 * shield_impact_color2 + non_plasma_value * shield_impact_ambient_color);
	float3 semifinal_overshield_color= overshield_factor * (plasma_value1 * overshield_color1 + plasma_value2 * overshield_color2 + non_plasma_value * overshield_ambient_color);
	
	float4 final_color= float4(semifinal_shield_impact_color + semifinal_overshield_color, 1.0f) * g_exposure.r;
	return convert_to_render_target(final_color, false, false

	// Not actually neccessary, but this particular explicit shader needs a complete rewrite anyhow
	#ifdef SSR_ENABLE
	, 0
    #endif
    );
}