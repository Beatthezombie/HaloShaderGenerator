using HaloShaderGenerator.Globals;
using HaloShaderGenerator.TemplateGenerator;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator
{
    public static class StaticOptionInfo
    {
        private static FrozenDictionary<string, FrozenSet<OptionInfo>> GetInfoDictionary(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Shader:
                    return ShaderOptionInfo;
                case ShaderType.Beam:
                    return BeamOptionInfo;
                case ShaderType.Contrail:
                    return ContrailOptionInfo;
                case ShaderType.Decal:
                    return DecalOptionInfo;
                case ShaderType.Halogram:
                    return HalogramOptionInfo;
                case ShaderType.LightVolume:
                    return LightVolumeOptionInfo;
                case ShaderType.Particle:
                    return ParticleOptionInfo;
                case ShaderType.Terrain:
                    return TerrainOptionInfo;
                //case ShaderType.Cortana:
                //    return CortanaOptionInfo;
                case ShaderType.Water:
                    return WaterOptionInfo;
                //case ShaderType.Black:
                //    return BlackOptionInfo;
                case ShaderType.Screen:
                    return ScreenOptionInfo;
                case ShaderType.Custom:
                    return CustomOptionInfo;
                case ShaderType.Foliage:
                    return FoliageOptionInfo;
                case ShaderType.Zonly:
                    return ZonlyOptionInfo;
                //case ShaderType.Glass:
                //    return OptionInfo;

                default:
                    throw new Exception("unsupported");
            }
        }

        public static int GetCategoryCount(ShaderType shaderType)
        {
            return GetInfoDictionary(shaderType).Keys.Length;
        }

        public static int GetCategoryOptionCount(ShaderType shaderType, int categoryIndex)
        {
            return GetInfoDictionary(shaderType).ElementAt(categoryIndex).Value.Count;
        }

        public static List<OptionInfo> OptionIndicesToOptionInfo(ShaderType shaderType, List<int> options)
        {
            List<OptionInfo> optionInfos = new List<OptionInfo>();

            for (int i = 0; i < options.Count; i++)
            {
                optionInfos.Add(GetOptionInfo(shaderType, i, options[i]));
            }

            optionInfos.AddRange(GetAutoMacroDefinitions(shaderType));

            return optionInfos;
        }

        public static OptionInfo GetOptionInfo(ShaderType shaderType, int categoryIndex, int optionIndex)
        {
            if (categoryIndex != -1 && optionIndex != -1)
            {
                switch (shaderType)
                {
                    case ShaderType.Shader:
                        return ShaderOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Beam:
                        return BeamOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Contrail:
                        return ContrailOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Decal:
                        return DecalOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Halogram:
                        return HalogramOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.LightVolume:
                        return LightVolumeOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Particle:
                        return ParticleOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Terrain:
                        return TerrainOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    //case ShaderType.Cortana:
                    //    return CortanaOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Water:
                        return WaterOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    //case ShaderType.Black:
                    //    return BlackOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Screen:
                        return ScreenOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Custom:
                        return CustomOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Foliage:
                        return FoliageOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    case ShaderType.Zonly:
                        return ZonlyOptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                    //case ShaderType.Glass:
                    //    return OptionInfo.ElementAt(categoryIndex).Value.ElementAt(optionIndex);
                }
            }

            throw new Exception("Unsupported");
        }

        public static List<OptionInfo> GetAutoMacroDefinitions(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Beam:
                    return [.. BeamAutoMacros];
                case ShaderType.Contrail:
                    return [.. ContrailAutoMacros];
                case ShaderType.Decal:
                    return [.. DecalAutoMacros];
                case ShaderType.LightVolume:
                    return [.. LightVolumeAutoMacros];
                case ShaderType.Particle:
                    return [.. ParticleAutoMacros];
                case ShaderType.Water:
                    return [.. WaterAutoMacros];
                case ShaderType.Zonly:
                    return [.. ZonlyAutoMacros];
                default:
                    return [];
            }
        }

        private readonly static FrozenSet<OptionInfo> ParticleAutoMacros = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo_option_diffuse_only", "", "0", ""),
                new("albedo", "diffuse_plus_billboard_alpha", "category_albedo_option_diffuse_plus_billboard_alpha", "", "1", ""),
                new("albedo", "palettized", "category_albedo_option_palettized", "", "2", ""),
                new("albedo", "palettized_plus_billboard_alpha", "category_albedo_option_palettized_plus_billboard_alpha", "", "3", ""),
                new("albedo", "diffuse_plus_sprite_alpha", "category_albedo_option_diffuse_plus_sprite_alpha", "", "4", ""),
                new("albedo", "palettized_plus_sprite_alpha", "category_albedo_option_palettized_plus_sprite_alpha", "", "5", ""),
                new("blend_mode", "opaque", "category_blend_mode_option_opaque", "", "0", ""),
                new("blend_mode", "additive", "category_blend_mode_option_additive", "", "1", ""),
                new("blend_mode", "multiply", "category_blend_mode_option_multiply", "", "2", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode_option_alpha_blend", "", "3", ""),
                new("blend_mode", "double_multiply", "category_blend_mode_option_double_multiply", "", "4", ""),
                new("blend_mode", "maximum", "category_blend_mode_option_maximum", "", "5", ""),
                new("blend_mode", "multiply_add", "category_blend_mode_option_multiply_add", "", "6", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode_option_add_src_times_dstalpha", "", "7", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode_option_add_src_times_srcalpha", "", "8", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode_option_inv_alpha_blend", "", "9", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode_option_pre_multiplied_alpha", "", "10", ""),
                new("specialized_rendering", "none", "category_specialized_rendering_option_none", "", "0", ""),
                new("specialized_rendering", "distortion", "category_specialized_rendering_option_distortion", "", "1", ""),
                new("specialized_rendering", "distortion_expensive", "category_specialized_rendering_option_distortion_expensive", "", "2", ""),
                new("specialized_rendering", "distortion_diffuse", "category_specialized_rendering_option_distortion_diffuse", "", "3", ""),
                new("specialized_rendering", "distortion_expensive_diffuse", "category_specialized_rendering_option_distortion_expensive_diffuse", "", "4", ""),
                new("lighting", "none", "category_lighting_option_none", "", "0", ""),
                new("lighting", "per_pixel_ravi_order_3", "category_lighting_option_per_pixel_ravi_order_3", "", "1", ""),
                new("lighting", "per_vertex_ravi_order_0", "category_lighting_option_per_vertex_ravi_order_0", "", "2", ""),
                new("render_targets", "ldr_and_hdr", "category_render_targets_option_ldr_and_hdr", "", "0", ""),
                new("render_targets", "ldr_only", "category_render_targets_option_ldr_only", "", "1", ""),
                new("depth_fade", "off", "category_depth_fade_option_off", "", "0", ""),
                new("depth_fade", "on", "category_depth_fade_option_on", "", "1", ""),
                new("black_point", "off", "category_black_point_option_off", "", "0", ""),
                new("black_point", "on", "category_black_point_option_on", "", "1", ""),
                new("fog", "off", "category_fog_option_off", "", "0", ""),
                new("fog", "on", "category_fog_option_on", "", "1", ""),
                new("frame_blend", "off", "category_frame_blend_option_off", "", "0", ""),
                new("frame_blend", "on", "category_frame_blend_option_on", "", "1", ""),
                new("self_illumination", "none", "category_self_illumination_option_none", "", "0", ""),
                new("self_illumination", "constant_color", "category_self_illumination_option_constant_color", "", "1", ""),
                }.ToFrozenSet();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> ParticleOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo", "", "category_albedo_option_diffuse_only", ""),
                new("albedo", "diffuse_plus_billboard_alpha", "category_albedo", "", "category_albedo_option_diffuse_plus_billboard_alpha", ""),
                new("albedo", "palettized", "category_albedo", "", "category_albedo_option_palettized", ""),
                new("albedo", "palettized_plus_billboard_alpha", "category_albedo", "", "category_albedo_option_palettized_plus_billboard_alpha", ""),
                new("albedo", "diffuse_plus_sprite_alpha", "category_albedo", "", "category_albedo_option_diffuse_plus_sprite_alpha", ""),
                new("albedo", "palettized_plus_sprite_alpha", "category_albedo", "", "category_albedo_option_palettized_plus_sprite_alpha", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "category_blend_mode", "", "category_blend_mode_option_opaque", ""),
                new("blend_mode", "additive", "category_blend_mode", "", "category_blend_mode_option_additive", ""),
                new("blend_mode", "multiply", "category_blend_mode", "", "category_blend_mode_option_multiply", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode", "", "category_blend_mode_option_alpha_blend", ""),
                new("blend_mode", "double_multiply", "category_blend_mode", "", "category_blend_mode_option_double_multiply", ""),
                new("blend_mode", "maximum", "category_blend_mode", "", "category_blend_mode_option_maximum", ""),
                new("blend_mode", "multiply_add", "category_blend_mode", "", "category_blend_mode_option_multiply_add", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_dstalpha", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_srcalpha", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode", "", "category_blend_mode_option_inv_alpha_blend", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode", "", "category_blend_mode_option_pre_multiplied_alpha", ""),}.ToFrozenSet(),
            ["specialized_rendering"] = new OptionInfo[] {
                new("specialized_rendering", "none", "category_specialized_rendering", "", "category_specialized_rendering_option_none", ""),
                new("specialized_rendering", "distortion", "category_specialized_rendering", "", "category_specialized_rendering_option_distortion", ""),
                new("specialized_rendering", "distortion_expensive", "category_specialized_rendering", "", "category_specialized_rendering_option_distortion_expensive", ""),
                new("specialized_rendering", "distortion_diffuse", "category_specialized_rendering", "", "category_specialized_rendering_option_distortion_diffuse", ""),
                new("specialized_rendering", "distortion_expensive_diffuse", "category_specialized_rendering", "", "category_specialized_rendering_option_distortion_expensive_diffuse", ""),}.ToFrozenSet(),
            ["lighting"] = new OptionInfo[] {
                new("lighting", "none", "category_lighting", "", "category_lighting_option_none", ""),
                new("lighting", "per_pixel_ravi_order_3", "category_lighting", "", "category_lighting_option_per_pixel_ravi_order_3", ""),
                new("lighting", "per_vertex_ravi_order_0", "category_lighting", "", "category_lighting_option_per_vertex_ravi_order_0", ""),}.ToFrozenSet(),
            ["render_targets"] = new OptionInfo[] {
                new("render_targets", "ldr_and_hdr", "category_render_targets", "", "category_render_targets_option_ldr_and_hdr", ""),
                new("render_targets", "ldr_only", "category_render_targets", "", "category_render_targets_option_ldr_only", ""),}.ToFrozenSet(),
            ["depth_fade"] = new OptionInfo[] {
                new("depth_fade", "off", "category_depth_fade", "", "category_depth_fade_option_off", ""),
                new("depth_fade", "on", "category_depth_fade", "", "category_depth_fade_option_on", ""),}.ToFrozenSet(),
            ["black_point"] = new OptionInfo[] {
                new("black_point", "off", "category_black_point", "", "category_black_point_option_off", ""),
                new("black_point", "on", "category_black_point", "", "category_black_point_option_on", ""),}.ToFrozenSet(),
            ["fog"] = new OptionInfo[] {
                new("fog", "off", "category_fog", "", "category_fog_option_off", ""),
                new("fog", "on", "category_fog", "", "category_fog_option_on", ""),}.ToFrozenSet(),
            ["frame_blend"] = new OptionInfo[] {
                new("frame_blend", "off", "category_frame_blend", "", "category_frame_blend_option_off", ""),
                new("frame_blend", "on", "category_frame_blend", "", "category_frame_blend_option_on", ""),}.ToFrozenSet(),
            ["self_illumination"] = new OptionInfo[] {
                new("self_illumination", "none", "category_self_illumination", "", "category_self_illumination_option_none", ""),
                new("self_illumination", "constant_color", "category_self_illumination", "", "category_self_illumination_option_constant_color", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> ShaderOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "default", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_default_ps", "calc_albedo_default_vs"),
                new("albedo", "detail_blend", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_detail_blend_ps", "calc_albedo_default_vs"),
                new("albedo", "constant_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_constant_color_ps", "calc_albedo_constant_color_vs"),
                new("albedo", "two_change_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_change_color_ps", "calc_albedo_default_vs"),
                new("albedo", "four_change_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_four_change_color_ps", "calc_albedo_default_vs"),
                new("albedo", "three_detail_blend", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_three_detail_blend_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail_overlay", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_overlay_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_ps", "calc_albedo_default_vs"),
                new("albedo", "color_mask", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_color_mask_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail_black_point", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_black_point_ps", "calc_albedo_default_vs"),
                new("albedo", "two_change_color_anim_overlay", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_change_color_anim_ps", "calc_albedo_default_vs"),
                new("albedo", "chameleon", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_chameleon_ps", "calc_albedo_default_vs"),
                new("albedo", "two_change_color_chameleon", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_change_color_chameleon_ps", "calc_albedo_default_vs"),
                new("albedo", "chameleon_masked", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_chameleon_masked_ps", "calc_albedo_default_vs"),
                new("albedo", "color_mask_hard_light", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_color_mask_hard_light_ps", "calc_albedo_default_vs"),
                new("albedo", "two_change_color_tex_overlay", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_change_color_tex_overlay_ps", "calc_albedo_default_vs"),
                new("albedo", "chameleon_albedo_masked", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_chameleon_albedo_masked_ps", "calc_albedo_default_vs"),}.ToFrozenSet(),
            ["bump_mapping"] = new OptionInfo[] {
                new("bump_mapping", "off", "calc_bumpmap_ps", "calc_bumpmap_vs", "calc_bumpmap_off_ps", "calc_bumpmap_off_vs"),
                new("bump_mapping", "standard", "calc_bumpmap_ps", "calc_bumpmap_vs", "calc_bumpmap_default_ps", "calc_bumpmap_default_vs"),
                new("bump_mapping", "detail", "calc_bumpmap_ps", "calc_bumpmap_vs", "calc_bumpmap_detail_ps", "calc_bumpmap_detail_vs"),
                new("bump_mapping", "detail_masked", "calc_bumpmap_ps", "calc_bumpmap_vs", "calc_bumpmap_detail_masked_ps", "calc_bumpmap_detail_vs"),
                new("bump_mapping", "detail_plus_detail_masked", "calc_bumpmap_ps", "calc_bumpmap_vs", "calc_bumpmap_detail_plus_detail_masked_ps", "calc_bumpmap_detail_vs"),}.ToFrozenSet(),
            ["alpha_test"] = new OptionInfo[] {
                new("alpha_test", "none", "calc_alpha_test_ps", "", "calc_alpha_test_off_ps", ""),
                new("alpha_test", "simple", "calc_alpha_test_ps", "", "calc_alpha_test_on_ps", ""),}.ToFrozenSet(),
            ["specular_mask"] = new OptionInfo[] {
                new("specular_mask", "no_specular_mask", "calc_specular_mask_ps", "", "calc_specular_mask_no_specular_mask_ps", ""),
                new("specular_mask", "specular_mask_from_diffuse", "calc_specular_mask_ps", "", "calc_specular_mask_from_diffuse_ps", ""),
                new("specular_mask", "specular_mask_from_texture", "calc_specular_mask_ps", "", "calc_specular_mask_texture_ps", ""),
                new("specular_mask", "specular_mask_from_color_texture", "calc_specular_mask_ps", "", "calc_specular_mask_color_texture_ps", ""),}.ToFrozenSet(),
            ["material_model"] = new OptionInfo[] {
                new("material_model", "diffuse_only", "material_type", "", "diffuse_only", ""),
                new("material_model", "cook_torrance", "material_type", "", "cook_torrance", ""),
                new("material_model", "two_lobe_phong", "material_type", "", "two_lobe_phong", ""),
                new("material_model", "foliage", "material_type", "", "foliage", ""),
                new("material_model", "none", "material_type", "", "none", ""),
                new("material_model", "glass", "material_type", "", "glass", ""),
                new("material_model", "organism", "material_type", "", "organism", ""),
                new("material_model", "single_lobe_phong", "material_type", "", "single_lobe_phong", ""),
                new("material_model", "car_paint", "material_type", "", "car_paint", ""),}.ToFrozenSet(),
            ["environment_mapping"] = new OptionInfo[] {
                new("environment_mapping", "none", "envmap_type", "", "none", ""),
                new("environment_mapping", "per_pixel", "envmap_type", "", "per_pixel", ""),
                new("environment_mapping", "dynamic", "envmap_type", "", "dynamic", ""),
                new("environment_mapping", "from_flat_texture", "envmap_type", "", "from_flat_texture", ""),
                new("environment_mapping", "custom_map", "envmap_type", "", "custom_map", ""),}.ToFrozenSet(),
            ["self_illumination"] = new OptionInfo[] {
                new("self_illumination", "off", "calc_self_illumination_ps", "", "calc_self_illumination_none_ps", ""),
                new("self_illumination", "simple", "calc_self_illumination_ps", "", "calc_self_illumination_simple_ps", ""),
                new("self_illumination", "3_channel_self_illum", "calc_self_illumination_ps", "", "calc_self_illumination_three_channel_ps", ""),
                new("self_illumination", "plasma", "calc_self_illumination_ps", "", "calc_self_illumination_plasma_ps", ""),
                new("self_illumination", "from_diffuse", "calc_self_illumination_ps", "", "calc_self_illumination_from_albedo_ps", ""),
                new("self_illumination", "illum_detail", "calc_self_illumination_ps", "", "calc_self_illumination_detail_ps", ""),
                new("self_illumination", "meter", "calc_self_illumination_ps", "", "calc_self_illumination_meter_ps", ""),
                new("self_illumination", "self_illum_times_diffuse", "calc_self_illumination_ps", "", "calc_self_illumination_times_diffuse_ps", ""),
                new("self_illumination", "simple_with_alpha_mask", "calc_self_illumination_ps", "", "calc_self_illumination_simple_with_alpha_mask_ps", ""),
                new("self_illumination", "simple_four_change_color", "calc_self_illumination_ps", "", "calc_self_illumination_simple_ps", ""),
                new("self_illumination", "illum_detail_world_space_four_cc", "calc_self_illumination_ps", "", "calc_self_illumination_detail_world_space_ps", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "blend_type", "", "opaque", ""),
                new("blend_mode", "additive", "blend_type", "", "additive", ""),
                new("blend_mode", "multiply", "blend_type", "", "multiply", ""),
                new("blend_mode", "alpha_blend", "blend_type", "", "alpha_blend", ""),
                new("blend_mode", "double_multiply", "blend_type", "", "double_multiply", ""),
                new("blend_mode", "pre_multiplied_alpha", "blend_type", "", "pre_multiplied_alpha", ""),}.ToFrozenSet(),
            ["parallax"] = new OptionInfo[] {
                new("parallax", "off", "calc_parallax_ps", "calc_parallax_vs", "calc_parallax_off_ps", "calc_parallax_off_vs"),
                new("parallax", "simple", "calc_parallax_ps", "calc_parallax_vs", "calc_parallax_simple_ps", "calc_parallax_simple_vs"),
                new("parallax", "interpolated", "calc_parallax_ps", "calc_parallax_vs", "calc_parallax_interpolated_ps", "calc_parallax_interpolated_vs"),
                new("parallax", "simple_detail", "calc_parallax_ps", "calc_parallax_vs", "calc_parallax_simple_detail_ps", "calc_parallax_simple_vs"),}.ToFrozenSet(),
            ["misc"] = new OptionInfo[] {
                new("misc", "first_person_never", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_sometimes", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_always", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_never_w/rotating_bitmaps", "bitmap_rotation", "", "1", ""),}.ToFrozenSet(),
            ["distortion"] = new OptionInfo[] {
                new("distortion", "off", "distort_proc_ps", "distort_proc_vs", "distort_off_ps", "distort_nocolor_vs"),
                new("distortion", "on", "distort_proc_ps", "distort_proc_vs", "distort_on_ps", "distort_nocolor_vs"),}.ToFrozenSet(),
            ["soft_fade"] = new OptionInfo[] {
                new("soft_fade", "off", "apply_soft_fade", "", "apply_soft_fade_off", ""),
                new("soft_fade", "on", "apply_soft_fade", "", "apply_soft_fade_on", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenSet<OptionInfo> DecalAutoMacros = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo_option_diffuse_only", "", "0", ""),
                new("albedo", "palettized", "category_albedo_option_palettized", "", "1", ""),
                new("albedo", "palettized_plus_alpha", "category_albedo_option_palettized_plus_alpha", "", "2", ""),
                new("albedo", "diffuse_plus_alpha", "category_albedo_option_diffuse_plus_alpha", "", "3", ""),
                new("albedo", "emblem_change_color", "category_albedo_option_emblem_change_color", "", "4", ""),
                new("albedo", "change_color", "category_albedo_option_change_color", "", "5", ""),
                new("albedo", "diffuse_plus_alpha_mask", "category_albedo_option_diffuse_plus_alpha_mask", "", "6", ""),
                new("albedo", "palettized_plus_alpha_mask", "category_albedo_option_palettized_plus_alpha_mask", "", "7", ""),
                new("albedo", "vector_alpha", "category_albedo_option_vector_alpha", "", "8", ""),
                new("albedo", "vector_alpha_drop_shadow", "category_albedo_option_vector_alpha_drop_shadow", "", "9", ""),
                new("blend_mode", "opaque", "category_blend_mode_option_opaque", "", "0", ""),
                new("blend_mode", "additive", "category_blend_mode_option_additive", "", "1", ""),
                new("blend_mode", "multiply", "category_blend_mode_option_multiply", "", "2", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode_option_alpha_blend", "", "3", ""),
                new("blend_mode", "double_multiply", "category_blend_mode_option_double_multiply", "", "4", ""),
                new("blend_mode", "maximum", "category_blend_mode_option_maximum", "", "5", ""),
                new("blend_mode", "multiply_add", "category_blend_mode_option_multiply_add", "", "6", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode_option_add_src_times_dstalpha", "", "7", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode_option_add_src_times_srcalpha", "", "8", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode_option_inv_alpha_blend", "", "9", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode_option_pre_multiplied_alpha", "", "10", ""),
                new("render_pass", "pre_lighting", "category_render_pass_option_pre_lighting", "", "0", ""),
                new("render_pass", "post_lighting", "category_render_pass_option_post_lighting", "", "1", ""),
                new("specular", "leave", "category_specular_option_leave", "", "0", ""),
                new("specular", "modulate", "category_specular_option_modulate", "", "1", ""),
                new("bump_mapping", "leave", "category_bump_mapping_option_leave", "", "0", ""),
                new("bump_mapping", "standard", "category_bump_mapping_option_standard", "", "1", ""),
                new("bump_mapping", "standard_mask", "category_bump_mapping_option_standard_mask", "", "2", ""),
                new("tinting", "none", "category_tinting_option_none", "", "0", ""),
                new("tinting", "unmodulated", "category_tinting_option_unmodulated", "", "1", ""),
                new("tinting", "partially_modulated", "category_tinting_option_partially_modulated", "", "2", ""),
                new("tinting", "fully_modulated", "category_tinting_option_fully_modulated", "", "3", ""),
                }.ToFrozenSet();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> DecalOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo", "", "category_albedo_option_diffuse_only", ""),
                new("albedo", "palettized", "category_albedo", "", "category_albedo_option_palettized", ""),
                new("albedo", "palettized_plus_alpha", "category_albedo", "", "category_albedo_option_palettized_plus_alpha", ""),
                new("albedo", "diffuse_plus_alpha", "category_albedo", "", "category_albedo_option_diffuse_plus_alpha", ""),
                new("albedo", "emblem_change_color", "category_albedo", "", "category_albedo_option_emblem_change_color", ""),
                new("albedo", "change_color", "category_albedo", "", "category_albedo_option_change_color", ""),
                new("albedo", "diffuse_plus_alpha_mask", "category_albedo", "", "category_albedo_option_diffuse_plus_alpha_mask", ""),
                new("albedo", "palettized_plus_alpha_mask", "category_albedo", "", "category_albedo_option_palettized_plus_alpha_mask", ""),
                new("albedo", "vector_alpha", "category_albedo", "", "category_albedo_option_vector_alpha", ""),
                new("albedo", "vector_alpha_drop_shadow", "category_albedo", "", "category_albedo_option_vector_alpha_drop_shadow", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "category_blend_mode", "", "category_blend_mode_option_opaque", ""),
                new("blend_mode", "additive", "category_blend_mode", "", "category_blend_mode_option_additive", ""),
                new("blend_mode", "multiply", "category_blend_mode", "", "category_blend_mode_option_multiply", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode", "", "category_blend_mode_option_alpha_blend", ""),
                new("blend_mode", "double_multiply", "category_blend_mode", "", "category_blend_mode_option_double_multiply", ""),
                new("blend_mode", "maximum", "category_blend_mode", "", "category_blend_mode_option_maximum", ""),
                new("blend_mode", "multiply_add", "category_blend_mode", "", "category_blend_mode_option_multiply_add", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_dstalpha", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_srcalpha", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode", "", "category_blend_mode_option_inv_alpha_blend", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode", "", "category_blend_mode_option_pre_multiplied_alpha", ""),}.ToFrozenSet(),
            ["render_pass"] = new OptionInfo[] {
                new("render_pass", "pre_lighting", "category_render_pass", "", "category_render_pass_option_pre_lighting", ""),
                new("render_pass", "post_lighting", "category_render_pass", "", "category_render_pass_option_post_lighting", ""),}.ToFrozenSet(),
            ["specular"] = new OptionInfo[] {
                new("specular", "leave", "category_specular", "", "category_specular_option_leave", ""),
                new("specular", "modulate", "category_specular", "", "category_specular_option_modulate", ""),}.ToFrozenSet(),
            ["bump_mapping"] = new OptionInfo[] {
                new("bump_mapping", "leave", "category_bump_mapping", "", "category_bump_mapping_option_leave", ""),
                new("bump_mapping", "standard", "category_bump_mapping", "", "category_bump_mapping_option_standard", ""),
                new("bump_mapping", "standard_mask", "category_bump_mapping", "", "category_bump_mapping_option_standard_mask", ""),}.ToFrozenSet(),
            ["tinting"] = new OptionInfo[] {
                new("tinting", "none", "category_tinting", "", "category_tinting_option_none", ""),
                new("tinting", "unmodulated", "category_tinting", "", "category_tinting_option_unmodulated", ""),
                new("tinting", "partially_modulated", "category_tinting", "", "category_tinting_option_partially_modulated", ""),
                new("tinting", "fully_modulated", "category_tinting", "", "category_tinting_option_fully_modulated", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenSet<OptionInfo> ContrailAutoMacros = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo_option_diffuse_only", "", "0", ""),
                new("albedo", "palettized", "category_albedo_option_palettized", "", "1", ""),
                new("albedo", "palettized_plus_alpha", "category_albedo_option_palettized_plus_alpha", "", "2", ""),
                new("blend_mode", "opaque", "category_blend_mode_option_opaque", "", "0", ""),
                new("blend_mode", "additive", "category_blend_mode_option_additive", "", "1", ""),
                new("blend_mode", "multiply", "category_blend_mode_option_multiply", "", "2", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode_option_alpha_blend", "", "3", ""),
                new("blend_mode", "double_multiply", "category_blend_mode_option_double_multiply", "", "4", ""),
                new("blend_mode", "maximum", "category_blend_mode_option_maximum", "", "5", ""),
                new("blend_mode", "multiply_add", "category_blend_mode_option_multiply_add", "", "6", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode_option_add_src_times_dstalpha", "", "7", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode_option_add_src_times_srcalpha", "", "8", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode_option_inv_alpha_blend", "", "9", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode_option_pre_multiplied_alpha", "", "10", ""),
                new("black_point", "off", "category_black_point_option_off", "", "0", ""),
                new("black_point", "on", "category_black_point_option_on", "", "1", ""),
                new("fog", "off", "category_fog_option_off", "", "0", ""),
                new("fog", "on", "category_fog_option_on", "", "1", ""),
                }.ToFrozenSet();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> ContrailOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo", "", "category_albedo_option_diffuse_only", ""),
                new("albedo", "palettized", "category_albedo", "", "category_albedo_option_palettized", ""),
                new("albedo", "palettized_plus_alpha", "category_albedo", "", "category_albedo_option_palettized_plus_alpha", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "category_blend_mode", "", "category_blend_mode_option_opaque", ""),
                new("blend_mode", "additive", "category_blend_mode", "", "category_blend_mode_option_additive", ""),
                new("blend_mode", "multiply", "category_blend_mode", "", "category_blend_mode_option_multiply", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode", "", "category_blend_mode_option_alpha_blend", ""),
                new("blend_mode", "double_multiply", "category_blend_mode", "", "category_blend_mode_option_double_multiply", ""),
                new("blend_mode", "maximum", "category_blend_mode", "", "category_blend_mode_option_maximum", ""),
                new("blend_mode", "multiply_add", "category_blend_mode", "", "category_blend_mode_option_multiply_add", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_dstalpha", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_srcalpha", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode", "", "category_blend_mode_option_inv_alpha_blend", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode", "", "category_blend_mode_option_pre_multiplied_alpha", ""),}.ToFrozenSet(),
            ["black_point"] = new OptionInfo[] {
                new("black_point", "off", "category_black_point", "", "category_black_point_option_off", ""),
                new("black_point", "on", "category_black_point", "", "category_black_point_option_on", ""),}.ToFrozenSet(),
            ["fog"] = new OptionInfo[] {
                new("fog", "off", "category_fog", "", "category_fog_option_off", ""),
                new("fog", "on", "category_fog", "", "category_fog_option_on", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenSet<OptionInfo> LightVolumeAutoMacros = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo_option_diffuse_only", "", "0", ""),
                new("blend_mode", "opaque", "category_blend_mode_option_opaque", "", "0", ""),
                new("blend_mode", "additive", "category_blend_mode_option_additive", "", "1", ""),
                new("blend_mode", "multiply", "category_blend_mode_option_multiply", "", "2", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode_option_alpha_blend", "", "3", ""),
                new("blend_mode", "double_multiply", "category_blend_mode_option_double_multiply", "", "4", ""),
                new("blend_mode", "maximum", "category_blend_mode_option_maximum", "", "5", ""),
                new("blend_mode", "multiply_add", "category_blend_mode_option_multiply_add", "", "6", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode_option_add_src_times_dstalpha", "", "7", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode_option_add_src_times_srcalpha", "", "8", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode_option_inv_alpha_blend", "", "9", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode_option_pre_multiplied_alpha", "", "10", ""),
                new("fog", "off", "category_fog_option_off", "", "0", ""),
                new("fog", "on", "category_fog_option_on", "", "1", ""),
                }.ToFrozenSet();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> LightVolumeOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo", "", "category_albedo_option_diffuse_only", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "category_blend_mode", "", "category_blend_mode_option_opaque", ""),
                new("blend_mode", "additive", "category_blend_mode", "", "category_blend_mode_option_additive", ""),
                new("blend_mode", "multiply", "category_blend_mode", "", "category_blend_mode_option_multiply", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode", "", "category_blend_mode_option_alpha_blend", ""),
                new("blend_mode", "double_multiply", "category_blend_mode", "", "category_blend_mode_option_double_multiply", ""),
                new("blend_mode", "maximum", "category_blend_mode", "", "category_blend_mode_option_maximum", ""),
                new("blend_mode", "multiply_add", "category_blend_mode", "", "category_blend_mode_option_multiply_add", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_dstalpha", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_srcalpha", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode", "", "category_blend_mode_option_inv_alpha_blend", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode", "", "category_blend_mode_option_pre_multiplied_alpha", ""),}.ToFrozenSet(),
            ["fog"] = new OptionInfo[] {
                new("fog", "off", "category_fog", "", "category_fog_option_off", ""),
                new("fog", "on", "category_fog", "", "category_fog_option_on", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> HalogramOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "default", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_default_ps", "calc_albedo_default_vs"),
                new("albedo", "detail_blend", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_detail_blend_ps", "calc_albedo_default_vs"),
                new("albedo", "constant_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_constant_color_ps", "calc_albedo_constant_color_vs"),
                new("albedo", "two_change_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_change_color_ps", "calc_albedo_default_vs"),
                new("albedo", "four_change_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_four_change_color_ps", "calc_albedo_default_vs"),
                new("albedo", "three_detail_blend", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_three_detail_blend_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail_overlay", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_overlay_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_ps", "calc_albedo_default_vs"),
                new("albedo", "color_mask", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_color_mask_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail_black_point", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_black_point_ps", "calc_albedo_default_vs"),}.ToFrozenSet(),
            ["self_illumination"] = new OptionInfo[] {
                new("self_illumination", "off", "calc_self_illumination_ps", "", "calc_self_illumination_none_ps", ""),
                new("self_illumination", "simple", "calc_self_illumination_ps", "", "calc_self_illumination_simple_ps", ""),
                new("self_illumination", "3_channel_self_illum", "calc_self_illumination_ps", "", "calc_self_illumination_three_channel_ps", ""),
                new("self_illumination", "plasma", "calc_self_illumination_ps", "", "calc_self_illumination_plasma_ps", ""),
                new("self_illumination", "from_diffuse", "calc_self_illumination_ps", "", "calc_self_illumination_from_albedo_ps", ""),
                new("self_illumination", "illum_detail", "calc_self_illumination_ps", "", "calc_self_illumination_detail_ps", ""),
                new("self_illumination", "meter", "calc_self_illumination_ps", "", "calc_self_illumination_meter_ps", ""),
                new("self_illumination", "self_illum_times_diffuse", "calc_self_illumination_ps", "", "calc_self_illumination_times_diffuse_ps", ""),
                new("self_illumination", "multilayer_additive", "calc_self_illumination_ps", "", "calc_self_illumination_multilayer_ps", ""),
                new("self_illumination", "ml_add_four_change_color", "calc_self_illumination_ps", "", "calc_self_illumination_multilayer_ps", ""),
                new("self_illumination", "ml_add_five_change_color", "calc_self_illumination_ps", "", "calc_self_illumination_multilayer_ps", ""),
                new("self_illumination", "scope_blur", "calc_self_illumination_ps", "", "calc_self_illumination_scope_blur_ps", ""),
                new("self_illumination", "plasma_wide_and_sharp_five_change_color", "calc_self_illumination_ps", "", "calc_self_illumination_plasma_wide_and_sharp_five_change_color_ps", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "blend_type", "", "opaque", ""),
                new("blend_mode", "additive", "blend_type", "", "additive", ""),
                new("blend_mode", "multiply", "blend_type", "", "multiply", ""),
                new("blend_mode", "alpha_blend", "blend_type", "", "alpha_blend", ""),
                new("blend_mode", "double_multiply", "blend_type", "", "double_multiply", ""),}.ToFrozenSet(),
            ["misc"] = new OptionInfo[] {
                new("misc", "first_person_never", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_sometimes", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_always", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_never_w/rotating_bitmaps", "bitmap_rotation", "", "1", ""),}.ToFrozenSet(),
            ["warp"] = new OptionInfo[] {
                new("warp", "none", "calc_parallax_ps", "", "calc_parallax_off_ps", ""),
                new("warp", "from_texture", "calc_parallax_ps", "", "calc_warp_from_texture_ps", ""),
                new("warp", "parallax_simple", "calc_parallax_ps", "", "calc_parallax_simple_ps", ""),}.ToFrozenSet(),
            ["overlay"] = new OptionInfo[] {
                new("overlay", "none", "calc_overlay_ps", "", "calc_overlay_none_ps", ""),
                new("overlay", "additive", "calc_overlay_ps", "", "calc_overlay_additive_ps", ""),
                new("overlay", "additive_detail", "calc_overlay_ps", "", "calc_overlay_additive_detail_ps", ""),
                new("overlay", "multiply", "calc_overlay_ps", "", "calc_overlay_multiply_ps", ""),
                new("overlay", "multiply_and_additive_detail", "calc_overlay_ps", "", "calc_overlay_multiply_and_additive_detail_ps", ""),}.ToFrozenSet(),
            ["edge_fade"] = new OptionInfo[] {
                new("edge_fade", "none", "calc_edge_fade_ps", "", "calc_edge_fade_none_ps", ""),
                new("edge_fade", "simple", "calc_edge_fade_ps", "", "calc_edge_fade_simple_ps", ""),}.ToFrozenSet(),
            ["distortion"] = new OptionInfo[] {
                new("distortion", "off", "distort_proc_ps", "distort_proc_vs", "distort_off_ps", "distort_nocolor_vs"),
                new("distortion", "on", "distort_proc_ps", "distort_proc_vs", "distort_on_ps", "distort_nocolor_vs"),}.ToFrozenSet(),
            ["soft_fade"] = new OptionInfo[] {
                new("soft_fade", "off", "apply_soft_fade", "", "apply_soft_fade_off", ""),
                new("soft_fade", "on", "apply_soft_fade", "", "apply_soft_fade_on", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> ScreenOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["warp"] = new OptionInfo[] {
                new("warp", "none", "warp_type", "", "none", ""),
                new("warp", "pixel_space", "warp_type", "", "pixel_space", ""),
                new("warp", "screen_space", "warp_type", "", "screen_space", ""),}.ToFrozenSet(),
            ["base"] = new OptionInfo[] {
                new("base", "single_screen_space", "base_type", "", "single_screen_space", ""),
                new("base", "single_pixel_space", "base_type", "", "single_pixel_space", ""),}.ToFrozenSet(),
            ["overlay_a"] = new OptionInfo[] {
                new("overlay_a", "none", "overlay_a_type", "", "none", ""),
                new("overlay_a", "tint_add_color", "overlay_a_type", "", "tint_add_color", ""),
                new("overlay_a", "detail_screen_space", "overlay_a_type", "", "detail_screen_space", ""),
                new("overlay_a", "detail_pixel_space", "overlay_a_type", "", "detail_pixel_space", ""),
                new("overlay_a", "detail_masked_screen_space", "overlay_a_type", "", "detail_masked_screen_space", ""),}.ToFrozenSet(),
            ["overlay_b"] = new OptionInfo[] {
                new("overlay_b", "none", "overlay_b_type", "", "none", ""),
                new("overlay_b", "tint_add_color", "overlay_b_type", "", "tint_add_color", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "blend_type", "", "opaque", ""),
                new("blend_mode", "additive", "blend_type", "", "additive", ""),
                new("blend_mode", "multiply", "blend_type", "", "multiply", ""),
                new("blend_mode", "alpha_blend", "blend_type", "", "alpha_blend", ""),
                new("blend_mode", "double_multiply", "blend_type", "", "double_multiply", ""),
                new("blend_mode", "pre_multiplied_alpha", "blend_type", "", "pre_multiplied_alpha", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenSet<OptionInfo> BeamAutoMacros = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo_option_diffuse_only", "", "0", ""),
                new("albedo", "palettized", "category_albedo_option_palettized", "", "1", ""),
                new("albedo", "palettized_plus_alpha", "category_albedo_option_palettized_plus_alpha", "", "2", ""),
                new("blend_mode", "opaque", "category_blend_mode_option_opaque", "", "0", ""),
                new("blend_mode", "additive", "category_blend_mode_option_additive", "", "1", ""),
                new("blend_mode", "multiply", "category_blend_mode_option_multiply", "", "2", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode_option_alpha_blend", "", "3", ""),
                new("blend_mode", "double_multiply", "category_blend_mode_option_double_multiply", "", "4", ""),
                new("blend_mode", "maximum", "category_blend_mode_option_maximum", "", "5", ""),
                new("blend_mode", "multiply_add", "category_blend_mode_option_multiply_add", "", "6", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode_option_add_src_times_dstalpha", "", "7", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode_option_add_src_times_srcalpha", "", "8", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode_option_inv_alpha_blend", "", "9", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode_option_pre_multiplied_alpha", "", "10", ""),
                new("black_point", "off", "category_black_point_option_off", "", "0", ""),
                new("black_point", "on", "category_black_point_option_on", "", "1", ""),
                new("fog", "off", "category_fog_option_off", "", "0", ""),
                new("fog", "on", "category_fog_option_on", "", "1", ""),
                }.ToFrozenSet();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> BeamOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "diffuse_only", "category_albedo", "", "category_albedo_option_diffuse_only", ""),
                new("albedo", "palettized", "category_albedo", "", "category_albedo_option_palettized", ""),
                new("albedo", "palettized_plus_alpha", "category_albedo", "", "category_albedo_option_palettized_plus_alpha", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "category_blend_mode", "", "category_blend_mode_option_opaque", ""),
                new("blend_mode", "additive", "category_blend_mode", "", "category_blend_mode_option_additive", ""),
                new("blend_mode", "multiply", "category_blend_mode", "", "category_blend_mode_option_multiply", ""),
                new("blend_mode", "alpha_blend", "category_blend_mode", "", "category_blend_mode_option_alpha_blend", ""),
                new("blend_mode", "double_multiply", "category_blend_mode", "", "category_blend_mode_option_double_multiply", ""),
                new("blend_mode", "maximum", "category_blend_mode", "", "category_blend_mode_option_maximum", ""),
                new("blend_mode", "multiply_add", "category_blend_mode", "", "category_blend_mode_option_multiply_add", ""),
                new("blend_mode", "add_src_times_dstalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_dstalpha", ""),
                new("blend_mode", "add_src_times_srcalpha", "category_blend_mode", "", "category_blend_mode_option_add_src_times_srcalpha", ""),
                new("blend_mode", "inv_alpha_blend", "category_blend_mode", "", "category_blend_mode_option_inv_alpha_blend", ""),
                new("blend_mode", "pre_multiplied_alpha", "category_blend_mode", "", "category_blend_mode_option_pre_multiplied_alpha", ""),}.ToFrozenSet(),
            ["black_point"] = new OptionInfo[] {
                new("black_point", "off", "category_black_point", "", "category_black_point_option_off", ""),
                new("black_point", "on", "category_black_point", "", "category_black_point_option_on", ""),}.ToFrozenSet(),
            ["fog"] = new OptionInfo[] {
                new("fog", "off", "category_fog", "", "category_fog_option_off", ""),
                new("fog", "on", "category_fog", "", "category_fog_option_on", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> TerrainOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["blending"] = new OptionInfo[] {
                new("blending", "morph", "blend_type", "", "morph", ""),
                new("blending", "dynamic_morph", "blend_type", "", "dynamic", ""),}.ToFrozenSet(),
            ["environment_map"] = new OptionInfo[] {
                new("environment_map", "none", "envmap_type", "", "none", ""),
                new("environment_map", "per_pixel", "envmap_type", "", "per_pixel", ""),
                new("environment_map", "dynamic", "envmap_type", "", "dynamic", ""),}.ToFrozenSet(),
            ["material_0"] = new OptionInfo[] {
                new("material_0", "diffuse_only", "material_0_type", "", "diffuse_only", ""),
                new("material_0", "diffuse_plus_specular", "material_0_type", "", "diffuse_plus_specular", ""),
                new("material_0", "off", "material_0_type", "", "off", ""),
                new("material_0", "diffuse_only_plus_self_illum", "material_0_type", "", "diffuse_only_plus_self_illum", ""),
                new("material_0", "diffuse_plus_specular_plus_self_illum", "material_0_type", "", "diffuse_plus_specular_plus_self_illum", ""),}.ToFrozenSet(),
            ["material_1"] = new OptionInfo[] {
                new("material_1", "diffuse_only", "material_1_type", "", "diffuse_only", ""),
                new("material_1", "diffuse_plus_specular", "material_1_type", "", "diffuse_plus_specular", ""),
                new("material_1", "off", "material_1_type", "", "off", ""),
                new("material_1", "diffuse_only_plus_self_illum", "material_1_type", "", "diffuse_only_plus_self_illum", ""),
                new("material_1", "diffuse_plus_specular_plus_self_illum", "material_1_type", "", "diffuse_plus_specular_plus_self_illum", ""),}.ToFrozenSet(),
            ["material_2"] = new OptionInfo[] {
                new("material_2", "diffuse_only", "material_2_type", "", "diffuse_only", ""),
                new("material_2", "diffuse_plus_specular", "material_2_type", "", "diffuse_plus_specular", ""),
                new("material_2", "off", "material_2_type", "", "off", ""),
                new("material_2", "diffuse_only_plus_self_illum", "material_2_type", "", "diffuse_only_plus_self_illum", ""),
                new("material_2", "diffuse_plus_specular_plus_self_illum", "material_2_type", "", "diffuse_plus_specular_plus_self_illum", ""),}.ToFrozenSet(),
            ["material_3"] = new OptionInfo[] {
                new("material_3", "off", "material_3_type", "", "off", ""),
                new("material_3", "diffuse_only_(four_material_shaders_disable_detail_bump)", "material_3_type", "", "diffuse_only", ""),
                new("material_3", "diffuse_plus_specular_(four_material_shaders_disable_detail_bump)", "material_3_type", "", "diffuse_plus_specular", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> FoliageOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "default", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_default_ps", "calc_albedo_default_vs"),}.ToFrozenSet(),
            ["alpha_test"] = new OptionInfo[] {
                new("alpha_test", "none", "calc_alpha_test_ps", "", "calc_alpha_test_off_ps", ""),
                new("alpha_test", "simple", "calc_alpha_test_ps", "", "calc_alpha_test_on_ps", ""),}.ToFrozenSet(),
            ["material_model"] = new OptionInfo[] {
                new("material_model", "default", "calculate_material", "", "calculate_material_default", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> CustomOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["albedo"] = new OptionInfo[] {
                new("albedo", "default", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_default_ps", "calc_albedo_default_vs"),
                new("albedo", "detail_blend", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_detail_blend_ps", "calc_albedo_default_vs"),
                new("albedo", "constant_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_constant_color_ps", "calc_albedo_constant_color_vs"),
                new("albedo", "two_change_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_change_color_ps", "calc_albedo_default_vs"),
                new("albedo", "four_change_color", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_four_change_color_ps", "calc_albedo_default_vs"),
                new("albedo", "three_detail_blend", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_three_detail_blend_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail_overlay", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_overlay_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_ps", "calc_albedo_default_vs"),
                new("albedo", "color_mask", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_color_mask_ps", "calc_albedo_default_vs"),
                new("albedo", "two_detail_black_point", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_two_detail_black_point_ps", "calc_albedo_default_vs"),
                new("albedo", "waterfall", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_waterfall_ps", "calc_albedo_default_vs"),
                new("albedo", "multiply_map", "calc_albedo_ps", "calc_albedo_vs", "calc_albedo_multiply_map_ps", "calc_albedo_default_vs"),}.ToFrozenSet(),
            ["bump_mapping"] = new OptionInfo[] {
                new("bump_mapping", "off", "calc_bumpmap_ps", "calc_bumpmap_vs", "calc_bumpmap_off_ps", "calc_bumpmap_off_vs"),
                new("bump_mapping", "standard", "calc_bumpmap_ps", "calc_bumpmap_vs", "calc_bumpmap_default_ps", "calc_bumpmap_default_vs"),
                new("bump_mapping", "detail", "calc_bumpmap_ps", "calc_bumpmap_vs", "calc_bumpmap_detail_ps", "calc_bumpmap_detail_vs"),}.ToFrozenSet(),
            ["alpha_test"] = new OptionInfo[] {
                new("alpha_test", "none", "calc_alpha_test_ps", "", "calc_alpha_test_off_ps", ""),
                new("alpha_test", "simple", "calc_alpha_test_ps", "", "calc_alpha_test_on_ps", ""),
                new("alpha_test", "multiply_map", "calc_alpha_test_ps", "", "calc_alpha_test_multiply_map_ps", ""),}.ToFrozenSet(),
            ["specular_mask"] = new OptionInfo[] {
                new("specular_mask", "no_specular_mask", "calc_specular_mask_ps", "", "calc_specular_mask_no_specular_mask_ps", ""),
                new("specular_mask", "specular_mask_from_diffuse", "calc_specular_mask_ps", "", "calc_specular_mask_from_diffuse_ps", ""),
                new("specular_mask", "specular_mask_from_texture", "calc_specular_mask_ps", "", "calc_specular_mask_texture_ps", ""),}.ToFrozenSet(),
            ["material_model"] = new OptionInfo[] {
                new("material_model", "diffuse_only", "material_type", "", "diffuse_only", ""),
                new("material_model", "two_lobe_phong", "material_type", "", "two_lobe_phong", ""),
                new("material_model", "foliage", "material_type", "", "foliage", ""),
                new("material_model", "none", "material_type", "", "none", ""),}.ToFrozenSet(),
            ["environment_mapping"] = new OptionInfo[] {
                new("environment_mapping", "none", "envmap_type", "", "none", ""),
                new("environment_mapping", "per_pixel", "envmap_type", "", "per_pixel", ""),
                new("environment_mapping", "dynamic", "envmap_type", "", "dynamic", ""),
                new("environment_mapping", "from_flat_texture", "envmap_type", "", "from_flat_texture", ""),}.ToFrozenSet(),
            ["self_illumination"] = new OptionInfo[] {
                new("self_illumination", "off", "calc_self_illumination_ps", "", "calc_self_illumination_none_ps", ""),
                new("self_illumination", "simple", "calc_self_illumination_ps", "", "calc_self_illumination_simple_ps", ""),
                new("self_illumination", "3_channel_self_illum", "calc_self_illumination_ps", "", "calc_self_illumination_three_channel_ps", ""),
                new("self_illumination", "plasma", "calc_self_illumination_ps", "", "calc_self_illumination_plasma_ps", ""),
                new("self_illumination", "from_diffuse", "calc_self_illumination_ps", "", "calc_self_illumination_from_albedo_ps", ""),
                new("self_illumination", "illum_detail", "calc_self_illumination_ps", "", "calc_self_illumination_detail_ps", ""),
                new("self_illumination", "meter", "calc_self_illumination_ps", "", "calc_self_illumination_meter_ps", ""),
                new("self_illumination", "self_illum_times_diffuse", "calc_self_illumination_ps", "", "calc_self_illumination_times_diffuse_ps", ""),}.ToFrozenSet(),
            ["blend_mode"] = new OptionInfo[] {
                new("blend_mode", "opaque", "blend_type", "", "opaque", ""),
                new("blend_mode", "additive", "blend_type", "", "additive", ""),
                new("blend_mode", "multiply", "blend_type", "", "multiply", ""),
                new("blend_mode", "alpha_blend", "blend_type", "", "alpha_blend", ""),
                new("blend_mode", "double_multiply", "blend_type", "", "double_multiply", ""),}.ToFrozenSet(),
            ["parallax"] = new OptionInfo[] {
                new("parallax", "off", "calc_parallax_ps", "calc_parallax_vs", "calc_parallax_off_ps", "calc_parallax_off_vs"),
                new("parallax", "simple", "calc_parallax_ps", "calc_parallax_vs", "calc_parallax_simple_ps", "calc_parallax_simple_vs"),
                new("parallax", "interpolated", "calc_parallax_ps", "calc_parallax_vs", "calc_parallax_interpolated_ps", "calc_parallax_interpolated_vs"),
                new("parallax", "simple_detail", "calc_parallax_ps", "calc_parallax_vs", "calc_parallax_simple_detail_ps", "calc_parallax_simple_vs"),}.ToFrozenSet(),
            ["misc"] = new OptionInfo[] {
                new("misc", "first_person_never", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_sometimes", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_always", "bitmap_rotation", "", "0", ""),
                new("misc", "first_person_never_w/rotating_bitmaps", "bitmap_rotation", "", "1", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenSet<OptionInfo> WaterAutoMacros = new OptionInfo[] {
                new("waveshape", "default", "category_waveshape_option_default", "", "0", ""),
                new("waveshape", "none", "category_waveshape_option_none", "", "1", ""),
                new("waveshape", "bump", "category_waveshape_option_bump", "", "2", ""),
                new("watercolor", "pure", "category_watercolor_option_pure", "", "0", ""),
                new("watercolor", "texture", "category_watercolor_option_texture", "", "1", ""),
                new("reflection", "none", "category_reflection_option_none", "", "0", ""),
                new("reflection", "static", "category_reflection_option_static", "", "1", ""),
                new("reflection", "dynamic", "category_reflection_option_dynamic", "", "2", ""),
                new("refraction", "none", "category_refraction_option_none", "", "0", ""),
                new("refraction", "dynamic", "category_refraction_option_dynamic", "", "1", ""),
                new("bankalpha", "none", "category_bankalpha_option_none", "", "0", ""),
                new("bankalpha", "depth", "category_bankalpha_option_depth", "", "1", ""),
                new("bankalpha", "paint", "category_bankalpha_option_paint", "", "2", ""),
                new("appearance", "default", "category_appearance_option_default", "", "0", ""),
                new("global_shape", "none", "category_global_shape_option_none", "", "0", ""),
                new("global_shape", "paint", "category_global_shape_option_paint", "", "1", ""),
                new("global_shape", "depth", "category_global_shape_option_depth", "", "2", ""),
                new("foam", "none", "category_foam_option_none", "", "0", ""),
                new("foam", "auto", "category_foam_option_auto", "", "1", ""),
                new("foam", "paint", "category_foam_option_paint", "", "2", ""),
                new("foam", "both", "category_foam_option_both", "", "3", ""),
                }.ToFrozenSet();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> WaterOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["waveshape"] = new OptionInfo[] {
                new("waveshape", "default", "category_waveshape", "", "category_waveshape_option_default", ""),
                new("waveshape", "none", "category_waveshape", "", "category_waveshape_option_none", ""),
                new("waveshape", "bump", "category_waveshape", "", "category_waveshape_option_bump", ""),}.ToFrozenSet(),
            ["watercolor"] = new OptionInfo[] {
                new("watercolor", "pure", "category_watercolor", "", "category_watercolor_option_pure", ""),
                new("watercolor", "texture", "category_watercolor", "", "category_watercolor_option_texture", ""),}.ToFrozenSet(),
            ["reflection"] = new OptionInfo[] {
                new("reflection", "none", "category_reflection", "", "category_reflection_option_none", ""),
                new("reflection", "static", "category_reflection", "", "category_reflection_option_static", ""),
                new("reflection", "dynamic", "category_reflection", "", "category_reflection_option_dynamic", ""),}.ToFrozenSet(),
            ["refraction"] = new OptionInfo[] {
                new("refraction", "none", "category_refraction", "", "category_refraction_option_none", ""),
                new("refraction", "dynamic", "category_refraction", "", "category_refraction_option_dynamic", ""),}.ToFrozenSet(),
            ["bankalpha"] = new OptionInfo[] {
                new("bankalpha", "none", "category_bankalpha", "", "category_bankalpha_option_none", ""),
                new("bankalpha", "depth", "category_bankalpha", "", "category_bankalpha_option_depth", ""),
                new("bankalpha", "paint", "category_bankalpha", "", "category_bankalpha_option_paint", ""),}.ToFrozenSet(),
            ["appearance"] = new OptionInfo[] {
                new("appearance", "default", "category_appearance", "", "category_appearance_option_default", ""),}.ToFrozenSet(),
            ["global_shape"] = new OptionInfo[] {
                new("global_shape", "none", "category_global_shape", "", "category_global_shape_option_none", ""),
                new("global_shape", "paint", "category_global_shape", "", "category_global_shape_option_paint", ""),
                new("global_shape", "depth", "category_global_shape", "", "category_global_shape_option_depth", ""),}.ToFrozenSet(),
            ["foam"] = new OptionInfo[] {
                new("foam", "none", "category_foam", "", "category_foam_option_none", ""),
                new("foam", "auto", "category_foam", "", "category_foam_option_auto", ""),
                new("foam", "paint", "category_foam", "", "category_foam_option_paint", ""),
                new("foam", "both", "category_foam", "", "category_foam_option_both", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
        private readonly static FrozenSet<OptionInfo> ZonlyAutoMacros = new OptionInfo[] {
                new("test", "default", "category_test_option_default", "", "0", ""),
                }.ToFrozenSet();
        private readonly static FrozenDictionary<string, FrozenSet<OptionInfo>> ZonlyOptionInfo = new Dictionary<string, FrozenSet<OptionInfo>>
        {
            ["test"] = new OptionInfo[] {
                new("test", "default", "category_test", "", "category_test_option_default", ""),}.ToFrozenSet(),
        }.ToFrozenDictionary();
    }
}
