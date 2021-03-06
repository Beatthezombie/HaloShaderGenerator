﻿using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Shader
{
    public class ShaderGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Bump_Mapping bump_mapping;
        Alpha_Test alpha_test;
        Specular_Mask specular_mask;
        Material_Model material_model;
        Environment_Mapping environment_mapping;
        Self_Illumination self_illumination;
        Blend_Mode blend_mode;
        Parallax parallax;
        Misc misc;
        Distortion distortion;

        /// <summary>
        /// Generator insantiation for shared shaders. Does not require method options.
        /// </summary>
        public ShaderGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        public ShaderGenerator(Albedo albedo, Bump_Mapping bump_mapping, Alpha_Test alpha_test, Specular_Mask specular_mask, Material_Model material_model,
            Environment_Mapping environment_mapping, Self_Illumination self_illumination, Blend_Mode blend_mode, Parallax parallax, Misc misc, 
            Distortion distortion, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.bump_mapping = bump_mapping;
            this.alpha_test = alpha_test;
            this.specular_mask = specular_mask;
            this.material_model = material_model;
            this.environment_mapping = environment_mapping;
            this.self_illumination = self_illumination;
            this.blend_mode = blend_mode;
            this.parallax = parallax;
            this.misc = misc;
            this.distortion = distortion;
            TemplateGenerationValid = true;
        }

        public ShaderGenerator(byte[] options, bool applyFixes = false)
        {
            this.albedo = (Albedo)options[0];
            this.bump_mapping = (Bump_Mapping)options[1];
            this.alpha_test = (Alpha_Test)options[2];
            this.specular_mask = (Specular_Mask)options[3];
            this.material_model = (Material_Model)options[4];
            this.environment_mapping = (Environment_Mapping)options[5];
            this.self_illumination = (Self_Illumination)options[6];
            this.blend_mode = (Blend_Mode)options[7];
            this.parallax = (Parallax)options[8];
            this.misc = (Misc)options[9];
            this.distortion = (Distortion)options[10];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }


        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Bump_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Alpha_Test>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Specular_Mask>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Material_Model>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Environment_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Self_Illumination>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Parallax>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Misc>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Distortion>());

            macros.Add(ShaderGeneratorBase.CreateMacro("APPLY_HLSL_FIXES", ApplyFixes ? 1 : 0));

            //
            // Convert to shared enum
            //

            var sAlbedo = Enum.Parse(typeof(Shared.Albedo), albedo.ToString());
            var sAlphaTest = Enum.Parse(typeof(Shared.Alpha_Test), alpha_test.ToString());
            var sMaterialModel = Enum.Parse(typeof(Shared.Material_Model), material_model.ToString());
            var sEnvironmentMapping = Enum.Parse(typeof(Shared.Environment_Mapping), environment_mapping.ToString());
            var sSelfIllumination = Enum.Parse(typeof(Shared.Self_Illumination), self_illumination.ToString());
            var sBlendMode = Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            //
            // The following code properly names the macros (like in rmdf)
            //

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", sAlbedo, "calc_albedo_", "_ps"));
            if (albedo == Albedo.Constant_Color)
                macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", sAlbedo, "calc_albedo_", "_vs"));

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", bump_mapping, "calc_bumpmap_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", bump_mapping, "calc_bumpmap_", "_vs"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", sAlphaTest, "calc_alpha_test_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", specular_mask, "calc_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", sSelfIllumination, "calc_self_illumination_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", parallax, "calc_parallax_", "_ps"));

            switch (parallax)
            {
                case Parallax.Simple_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", Parallax.Simple, "calc_parallax_", "_vs"));
                    break;
                default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", parallax, "calc_parallax_", "_vs"));
                    break;
            }

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_material_analytic_specular", sMaterialModel, "calc_material_analytic_specular_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_material_area_specular", sMaterialModel, "calc_material_area_specular_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_lighting_ps", sMaterialModel, "calc_lighting_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_dynamic_lighting_ps", sMaterialModel, "calc_dynamic_lighting_", "_ps"));

            macros.Add(ShaderGeneratorBase.CreateMacro("material_type", sMaterialModel, "material_type_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", sEnvironmentMapping, "envmap_type_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", sBlendMode, "blend_type_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.Shader, "k_shadertype_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("albedo_arg", sAlbedo, "k_albedo_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("material_type_arg", sMaterialModel, "k_material_model_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type_arg", sEnvironmentMapping, "k_environment_mapping_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("self_illumination_arg", sSelfIllumination, "k_self_illumination_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type_arg", sBlendMode, "k_blend_mode_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("misc_arg", misc, "k_misc_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("distortion_arg", distortion, "k_distortion_"));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"pixl_shader.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            Alpha_Test alphaTestOption;

            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Alpha_Test:

                    alphaTestOption = (Alpha_Test)optionIndex;

                    break;
                default:
                    return null;
            }

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Alpha_Test>());

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", alphaTestOption, "calc_alpha_test_", "_ps"));
            
            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"glps_shader.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_VERTEX_SHADER_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<VertexType>());
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(ShaderGeneratorBase.CreateMacro("transform_dominant_light", vertexType, "transform_dominant_light_", ""));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_distortion", vertexType, "calc_distortion_", ""));
            macros.Add(ShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("vertextype", vertexType, "k_vertextype_"));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(@"glvs_shader.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");
            return null;
        }

        public int GetMethodCount()
        {
            return 11;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return 15;
                case ShaderMethods.Bump_Mapping:
                    return 4;
                case ShaderMethods.Alpha_Test:
                    return 2;
                case ShaderMethods.Specular_Mask:
                    return 4;
                case ShaderMethods.Material_Model:
                    return 9;
                case ShaderMethods.Environment_Mapping:
                    return 5;
                case ShaderMethods.Self_Illumination:
                    return 10;
                case ShaderMethods.Blend_Mode:
                    return 6;
                case ShaderMethods.Parallax:
                    return 4;
                case ShaderMethods.Misc:
                    return 2;
                case ShaderMethods.Distortion:
                    return 2;
            }
            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return (int)albedo;
                case ShaderMethods.Bump_Mapping:
                    return (int)bump_mapping;
                case ShaderMethods.Alpha_Test:
                    return (int)alpha_test;
                case ShaderMethods.Specular_Mask:
                    return (int)specular_mask;
                case ShaderMethods.Material_Model:
                    return (int)material_model;
                case ShaderMethods.Environment_Mapping:
                    return (int)environment_mapping;
                case ShaderMethods.Self_Illumination:
                    return (int)self_illumination;
                case ShaderMethods.Blend_Mode:
                    return (int)blend_mode;
                case ShaderMethods.Parallax:
                    return (int)parallax;
                case ShaderMethods.Misc:
                    return (int)misc;
                case ShaderMethods.Distortion:
                    return (int)distortion;
            }
            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Active_Camo:
                case ShaderStage.Sfx_Distort:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Static_Sh:
                case ShaderStage.Shadow_Generate:
                    return true;
                    
                default:
                case ShaderStage.Default:
                case ShaderStage.Z_Only:
                case ShaderStage.Water_Shading:
                case ShaderStage.Water_Tesselation:
                case ShaderStage.Shadow_Apply:
                case ShaderStage.Static_Default:
                    return false;
            }
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            return method_index == 2;
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            return entryPoint == ShaderStage.Shadow_Generate;
        }

        public bool IsSharedPixelShaderWithoutMethod(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            switch (vertexType)
            {
                case VertexType.World:
                case VertexType.Rigid:
                case VertexType.Skinned:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            return true;
        }

        public ShaderParameters GetPixelShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            switch (albedo)
            {
                case Albedo.Default:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
                case Albedo.Two_Detail_Black_Point:
                case Albedo.Two_Detail:
                case Albedo.Detail_Blend:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    break;
                case Albedo.Constant_Color:
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
                case Albedo.Two_Change_Color:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    break;
                case Albedo.Four_Change_Color:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                    result.AddFloat3ColorParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary);
                    break;
                case Albedo.Three_Detail_Blend:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    result.AddSamplerParameter("detail_map3");
                    break;
                case Albedo.Two_Detail_Overlay:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    result.AddSamplerParameter("detail_map_overlay");
                    break;
                case Albedo.Color_Mask:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("color_mask_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    result.AddFloat4ColorParameter("albedo_color2");
                    result.AddFloat4ColorParameter("albedo_color3");
                    result.AddFloat4ColorParameter("neutral_gray");
                    break;
                case Albedo.Two_Change_Color_Anim_Overlay:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat4Parameter("primary_change_color_anim", RenderMethodExtern.object_change_color_primary_anim);
                    result.AddFloat4Parameter("secondary_change_color_anim", RenderMethodExtern.object_change_color_secondary_anim);
                    break;
                case Albedo.Chameleon:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddFloat4ColorParameter("chameleon_color0");
                    result.AddFloat4ColorParameter("chameleon_color1");
                    result.AddFloat4ColorParameter("chameleon_color2");
                    result.AddFloat4ColorParameter("chameleon_color3");
                    result.AddFloatParameter("chameleon_color_offset1");
                    result.AddFloatParameter("chameleon_color_offset2");
                    result.AddFloatParameter("chameleon_fresnel_power");
                    break;
                case Albedo.Two_Change_Color_Chameleon:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat4ColorParameter("chameleon_color0");
                    result.AddFloat4ColorParameter("chameleon_color1");
                    result.AddFloat4ColorParameter("chameleon_color2");
                    result.AddFloat4ColorParameter("chameleon_color3");
                    result.AddFloatParameter("chameleon_color_offset1");
                    result.AddFloatParameter("chameleon_color_offset2");
                    result.AddFloatParameter("chameleon_fresnel_power");
                    break;
                case Albedo.Chameleon_Masked:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("chameleon_mask_map");
                    result.AddFloat4ColorParameter("chameleon_color0");
                    result.AddFloat4ColorParameter("chameleon_color1");
                    result.AddFloat4ColorParameter("chameleon_color2");
                    result.AddFloat4ColorParameter("chameleon_color3");
                    result.AddFloatParameter("chameleon_color_offset1");
                    result.AddFloatParameter("chameleon_color_offset2");
                    result.AddFloatParameter("chameleon_fresnel_power");
                    break;
                case Albedo.Color_Mask_Hard_Light:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("color_mask_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Off:
                    break;
                case Bump_Mapping.Standard:
                    result.AddSamplerParameter("bump_map");
                    break;
                case Bump_Mapping.Detail:
                    result.AddSamplerParameter("bump_map");
                    result.AddSamplerParameter("bump_detail_map");
                    result.AddFloatParameter("bump_detail_coefficient");
                    break;
                case Bump_Mapping.Detail_Masked:
                    result.AddSamplerParameter("bump_map");
                    result.AddSamplerParameter("bump_detail_map");
                    result.AddSamplerParameter("bump_detail_mask_map");
                    result.AddFloatParameter("bump_detail_coefficient");
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    break;
                case Alpha_Test.Simple:
                    result.AddSamplerParameter("alpha_test_map");
                    break;
            }

            switch (specular_mask)
            {
                case Specular_Mask.No_Specular_Mask:
                    break;
                case Specular_Mask.Specular_Mask_From_Diffuse:
                    break;
                case Specular_Mask.Specular_Mask_From_Texture:
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    result.AddSamplerParameter("specular_mask_texture");
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
                case Material_Model.Cook_Torrance:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("use_fresnel_color_environment");
                    result.AddFloat3ColorParameter("fresnel_color_environment");
                    result.AddFloatParameter("fresnel_power");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddFloatParameter("albedo_blend_with_specular_tint");
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddFloatParameter("rim_fresnel_coefficient");
                    result.AddFloat3ColorParameter("rim_fresnel_color");
                    result.AddFloatParameter("rim_fresnel_power");
                    result.AddFloatParameter("rim_fresnel_albedo_blend");
                    break;
                case Material_Model.Two_Lobe_Phong:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("normal_specular_power");
                    result.AddFloat3ColorParameter("normal_specular_tint");
                    result.AddFloatParameter("glancing_specular_power");
                    result.AddFloat3ColorParameter("glancing_specular_tint");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("albedo_specular_tint_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    break;
                case Material_Model.Foliage:
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
                case Material_Model.None:
                    break;

                case Material_Model.Glass:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("fresnel_coefficient");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("fresnel_curve_bias");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
                case Material_Model.Organism:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloat3ColorParameter("diffuse_tint");
                    result.AddFloatParameter("analytical_specular_coefficient");
                    result.AddFloatParameter("area_specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloatParameter("specular_power");
                    result.AddSamplerParameter("specular_map");
                    result.AddFloatParameter("environment_map_coefficient");
                    result.AddFloat3ColorParameter("environment_map_tint");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("rim_coefficient");
                    result.AddFloat3ColorParameter("rim_tint");
                    result.AddFloatParameter("rim_power");
                    result.AddFloatParameter("rim_start");
                    result.AddFloatParameter("rim_maps_transition_ratio");
                    result.AddFloatParameter("ambient_coefficient");
                    result.AddFloat3ColorParameter("ambient_tint");
                    result.AddSamplerParameter("occlusion_parameter_map");

                    result.AddFloatParameter("subsurface_coefficient");
                    result.AddFloat3ColorParameter("subsurface_tint");
                    result.AddFloatParameter("subsurface_propagation_bias");
                    result.AddFloatParameter("subsurface_normal_detail");
                    result.AddSamplerParameter("subsurface_map");

                    result.AddFloatParameter("transparence_coefficient");
                    result.AddFloat3ColorParameter("transparence_tint");
                    result.AddFloatParameter("transparence_normal_bias");
                    result.AddFloatParameter("transparence_normal_detail");
                    result.AddSamplerParameter("transparence_map");

                    result.AddFloat3ColorParameter("final_tint");
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
                case Material_Model.Single_Lobe_Phong:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
                case Material_Model.Car_Paint:
                    throw new System.Exception("Unsupported");

            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    break;
                case Environment_Mapping.Per_Pixel:
                case Environment_Mapping.Custom_Map:
                    result.AddSamplerWithoutXFormParameter("environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloatParameter("env_roughness_scale");
                    break;
                case Environment_Mapping.Dynamic:
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                    result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1);
                    result.AddFloatParameter("env_roughness_scale");
                    break;
                case Environment_Mapping.From_Flat_Texture:
                    result.AddSamplerWithoutXFormParameter("flat_environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloat3Parameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x);
                    result.AddFloat3Parameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y);
                    result.AddFloat3Parameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z);
                    result.AddFloatParameter("hemisphere_percentage");
                    result.AddFloat4Parameter("env_bloom_override");
                    result.AddFloatParameter("env_bloom_override_intensity");
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.Off:
                    break;
                case Self_Illumination.Simple:
                case Self_Illumination.Simple_With_Alpha_Mask:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination._3_Channel_Self_Illum:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("channel_a");
                    result.AddFloat4Parameter("channel_b");
                    result.AddFloat4Parameter("channel_c");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Plasma:
                    result.AddSamplerParameter("noise_map_a");
                    result.AddSamplerParameter("noise_map_b");
                    result.AddFloat4Parameter("color_medium");
                    result.AddFloat4Parameter("color_wide");
                    result.AddFloat4Parameter("color_sharp");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddSamplerParameter("alpha_mask_map");
                    result.AddFloatParameter("thinness_medium");
                    result.AddFloatParameter("thinness_wide");
                    result.AddFloatParameter("thinness_sharp");
                    break;
                case Self_Illumination.From_Diffuse:
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;

                case Self_Illumination.Illum_Detail:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddSamplerParameter("self_illum_detail_map");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;

                case Self_Illumination.Meter:
                    result.AddSamplerParameter("meter_map");
                    result.AddFloat4Parameter("meter_color_off");
                    result.AddFloat4Parameter("meter_color_on");
                    result.AddFloatParameter("meter_value");
                    break;

                case Self_Illumination.Self_Illum_Times_Diffuse:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("primary_change_color_blend");
                    break;

                case Self_Illumination.Simple_Four_Change_Color:
                    result.AddSamplerParameter("self_illum_map");
                    //result.AddFloat4Parameter("self_illum_color"); object_change_color_quaternary
                    result.AddFloatParameter("self_illum_intensity");
                    break;

            }

            switch (parallax)
            {
                case Parallax.Off:
                    break;
                case Parallax.Simple:
                case Parallax.Interpolated:
                    result.AddSamplerParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    break;
                case Parallax.Simple_Detail:
                    result.AddSamplerParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    result.AddSamplerParameter("height_scale_map");
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    break;
                case Distortion.On:
                    result.AddSamplerParameter("distort_map");
                    result.AddFloatParameter("distort_scale");
                    //result.AddBooleanParameter("soft_fresnel_enabled");
                    //result.AddFloatParameter("soft_fresnel_power");
                    //result.AddBooleanParameter("soft_z_enabled");
                    //result.AddFloatParameter("soft_z_range");
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            return new ShaderParameters();
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddSamplerWithoutXFormParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerWithoutXFormParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerWithoutXFormParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture);
            result.AddSamplerWithoutXFormParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1);
            result.AddSamplerWithoutXFormParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat4Parameter("debug_tint", RenderMethodExtern.debug_tint);
            result.AddSamplerWithoutXFormParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerWithoutXFormParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerWithoutXFormParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerWithoutXFormParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = "";
            optionName = "";
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(ShaderMethods));
        }
    }
}
