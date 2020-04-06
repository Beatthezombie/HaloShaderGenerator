﻿#include "registers\vertex_shader.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\transform_math.hlsli"
#include "helpers\math.hlsli"
#include "helpers\atmosphere.hlsli"

// TODO: figure out dual quaternion skinned vertices

VS_OUTPUT_ALBEDO entry_albedo_rigid(VS_INPUT_RIGID_VERTEX_ALBEDO input)
{
    VS_OUTPUT_ALBEDO output;

	float3x3 node_transformation = float3x3(nodes[0].xyz, nodes[1].xyz, nodes[2].xyz);
	float4x4 v_node_transformation = float4x4(nodes[0], nodes[1], nodes[2], float4(0,0,0,0));
	output.texcoord.xy = calculate_texcoord(input.texcoord);
	output.normal.xyz = transform_vector(input.normal.xyz, node_transformation);
	output.binormal.xyz = transform_vector(transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz), node_transformation);
	output.tangent.xyz = transform_vector(input.tangent.xyz, node_transformation);
	float4 vertex_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	vertex_position.xyz = mul(v_node_transformation, vertex_position.xyzw).xyz;
	float4 screen_position = calculate_screenspace_position(vertex_position);
	output.camera_dir = camera_position - vertex_position.xyz;
	output.position = screen_position.xyzw;
	output.normal.w = screen_position.w;	
	return output;
}

VS_OUTPUT_ALBEDO entry_albedo_skinned(VS_INPUT_SKINNED_VERTEX_ALBEDO input)
{
	VS_OUTPUT_ALBEDO output;

	output.texcoord.xy = calculate_texcoord(input.texcoord);
	int4 indices = int4(3 * floor(input.node_indices)); // offset into the matrix by 3
	float4 weights = input.node_weights * (1.0 / dot(input.node_weights, 1)); // make sure weights sum to 1
	// compute transformation matrix for weighted vertices
	float4 basis1 = weights.x * nodes[indices.x + 0] + weights.y * nodes[indices.y + 0] + weights.z * nodes[indices.z + 0] + weights.w * nodes[indices.w + 0];
	float4 basis2 = weights.x * nodes[indices.x + 1] + weights.y * nodes[indices.y + 1] + weights.z * nodes[indices.z + 1] + weights.w * nodes[indices.w + 1];
	float4 basis3 = weights.x * nodes[indices.x + 2] + weights.y * nodes[indices.y + 2] + weights.z * nodes[indices.z + 2] + weights.w * nodes[indices.w + 2];
	
	float3x3 node_transformation = float3x3(basis1.xyz, basis2.xyz, basis3.xyz);
	float4x4 v_node_transformation = float4x4(basis1, basis2, basis3, float4(0, 0, 0, 0));

	output.normal.xyz = transform_vector(input.normal.xyz, node_transformation);
	output.binormal.xyz = transform_vector(input.binormal.xyz, node_transformation);
	output.tangent.xyz = transform_vector(input.tangent.xyz, node_transformation);
	float4 vertex_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	vertex_position.xyz = mul(v_node_transformation, vertex_position.xyzw).xyz;
	float4 screen_position = calculate_screenspace_position(vertex_position);
	output.camera_dir = camera_position - vertex_position.xyz;
	output.position = screen_position.xyzw;
	output.normal.w = screen_position.w;
	return output;
}

VS_OUTPUT_ALBEDO entry_albedo_world(VS_INPUT_WORLD_VERTEX_ALBEDO input)
{
	VS_OUTPUT_ALBEDO output;
	output.binormal.xyz = transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz);
	float4 vertex_position = float4(input.position.xyz, 1.0);
	float4 screen_position = calculate_screenspace_position(vertex_position);
	output.camera_dir = camera_position - vertex_position.xyz;
	output.position = screen_position.xyzw;
	output.texcoord.xy = input.texcoord.xy;
	output.normal.w = screen_position.w;
	output.normal.xyz = input.normal.xyz;
	output.tangent.xyz = input.tangent.xyz;
	return output;
}

VS_OUTPUT_STATIC_PTR_AMBIENT entry_static_prt_ambient_rigid(VS_INPUT_RIGID_VERTEX_AMBIENT_PRT input)
{
	VS_OUTPUT_STATIC_PTR_AMBIENT output;
	float3x3 node_transformation = float3x3(nodes[0].xyz, nodes[1].xyz, nodes[2].xyz);
	float4x4 v_node_transformation = float4x4(nodes[0], nodes[1], nodes[2], float4(0, 0, 0, 0));
	output.texcoord.xy = calculate_texcoord(input.texcoord);
	output.normal.xyz = transform_vector(input.normal.xyz, node_transformation);
	output.binormal.xyz = transform_vector(transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz), node_transformation);
	output.tangent.xyz = transform_vector(input.tangent.xyz, node_transformation);
	float4 vertex_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	vertex_position.xyz = mul(v_node_transformation, vertex_position.xyzw).xyz;
	float3 camera_dir = camera_position - vertex_position.xyz;
	output.camera_dir = camera_dir;
	
	float3 extinction_factor, sky_radiance;
	calculate_atmosphere_radiance(vertex_position, camera_dir, extinction_factor, sky_radiance);
	output.extinction_factor.rgb = extinction_factor;
	output.sky_radiance.rgb = sky_radiance;
	float4 screen_position = calculate_screenspace_position(vertex_position);
	output.position = screen_position.xyzw;
	output.normal.w = screen_position.w;

	float tempx = dot(v_lighting_constant_0, 0.333333333);
	float tempy = tempx * input.coefficient.x;
	tempx = tempx * 0.282094806;
	output.TexCoord7.x = max(tempy, EPSILON) / max(tempx, EPSILON);
	output.TexCoord7.w = min(tempy, dot(output.normal.xyz, normalize(v_lighting_constant_3.rgb + v_lighting_constant_1.rgb + v_lighting_constant_2.rgb)));
	output.TexCoord7.y = tempy;
	output.TexCoord7.z = input.coefficient.x * 3.54490733;

	return output;
}