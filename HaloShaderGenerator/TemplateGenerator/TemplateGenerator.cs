using HaloShaderGenerator.Custom;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HaloShaderGenerator.TemplateGenerator
{
    public class TemplateGenerator : GeneratorBaseNew
    {
        private Shared.Blend_Mode GetBlendMode(List<OptionInfo> currentOptions)
        {
            var blendIndex = currentOptions.FindIndex(x => x.Category == "blend_mode");
            if (blendIndex != -1)
                return (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), currentOptions[blendIndex].Option, true);
            return Shared.Blend_Mode.Opaque;
        }

        private Shared.Alpha_Blend_Source GetAlphaBlendSource(List<OptionInfo> currentOptions)
        {
            var atIndex = currentOptions.FindIndex(x => x.Category == "alpha_blend_source");
            if (atIndex != -1)
                return (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), currentOptions[atIndex].Option, true);
            return Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel;
        }

        private Shader.Misc GetMisc(List<OptionInfo> currentOptions)
        {
            var miscIndex = currentOptions.FindIndex(x => x.Category == "misc");
            if (miscIndex != -1)
            {
                if (currentOptions[miscIndex].Option == @"first_person_never_w/rotating_bitmaps")
                    return Shader.Misc.First_Person_Never_With_rotating_Bitmaps;
                return (Shader.Misc)Enum.Parse(typeof(Misc), currentOptions[miscIndex].Option, true);
            }
            return Shader.Misc.First_Person_Never;
        }

        private Shared.Alpha_Test GetAlphaTest(List<OptionInfo> currentOptions)
        {
            var atIndex = currentOptions.FindIndex(x => x.Category == "alpha_test");
            if (atIndex != -1)
                return (Shared.Alpha_Test)Enum.Parse(typeof(Shared.Alpha_Test), currentOptions[atIndex].Option, true);
            return Shared.Alpha_Test.None;
        }

        private static bool SsrEnable(ShaderType shaderType) // todo review
        {
            switch (shaderType)
            {
                case ShaderType.Shader:
                case ShaderType.Halogram:
                case ShaderType.Black:
                case ShaderType.Cortana:
                case ShaderType.Terrain:
                case ShaderType.Foliage:
                case ShaderType.Glass:
                case ShaderType.Water:
                case ShaderType.Custom:
                case ShaderType.Decal:
                case ShaderType.Particle:
                case ShaderType.Beam:
                case ShaderType.Contrail:
                case ShaderType.LightVolume:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetSourceFilename(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.LightVolume:
                    return "light_volume.fx";
                default:
                    return shaderType.ToString().ToLower() + ".fx";
            }
        }

        public static string GetEntryName(ShaderStage entryPoint, bool vs = false)
        {
            string append = vs ? "_vs" : "_ps";
            switch (entryPoint)
            {
                case ShaderStage.Static_Prt_Linear when !vs:
                case ShaderStage.Static_Prt_Quadratic when !vs:
                case ShaderStage.Static_Prt_Ambient when !vs:
                    return "static_prt" + append;
                case ShaderStage.Dynamic_Light_Cinematic:
                    return "dynamic_light_cine" + append;
                default:
                    return entryPoint.ToString().ToLower() + append;
            }
        }

        public static void CreateGlobalMacros(List<D3D.SHADER_MACRO> macros,
            ShaderType shaderType,
            ShaderStage entryPoint,
            Shared.Blend_Mode blendMode,
            Shader.Misc misc,
            Shared.Alpha_Test alphaTest,
            Shared.Alpha_Blend_Source alphaBlendSource,
            bool applyFixes,
            bool vs = false,
            VertexType vertexType = VertexType.World)
        {
            string vertexTypeSnake = string.Concat(vertexType.ToString().Select(
                (x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();

            macros.Add(ShaderGeneratorBase.CreateMacro(vs ? "VERTEX_SHADER" : "PIXEL_SHADER", "1"));
            macros.Add(ShaderGeneratorBase.CreateMacro("entry_point", entryPoint.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateMacro("vertex_type", vertexTypeSnake, "s_", "_vertex"));
            macros.Add(ShaderGeneratorBase.CreateMacro("deform", vertexTypeSnake, "deform_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("SHADER_30", "1"));
            macros.Add(ShaderGeneratorBase.CreateMacro("pc", "1"));
            macros.Add(ShaderGeneratorBase.CreateMacro("DX_VERSION", "9"));
            macros.Add(ShaderGeneratorBase.CreateMacro("disable_register_reorder", "1"));

            // TODO: This should be a 'user' macro
            macros.Add(ShaderGeneratorBase.CreateMacro("BUILD_MS30", "1"));

            if (SsrEnable(shaderType))
            {
                switch (entryPoint)
                {
                    case ShaderStage.Static_Per_Pixel:
                    case ShaderStage.Static_Per_Vertex:
                    case ShaderStage.Static_Sh:
                    case ShaderStage.Static_Prt_Ambient:
                    case ShaderStage.Static_Prt_Linear:
                    case ShaderStage.Static_Prt_Quadratic:
                    case ShaderStage.Dynamic_Light:
                    case ShaderStage.Dynamic_Light_Cinematic:
                    case ShaderStage.Active_Camo:
                    case ShaderStage.Lightmap_Debug_Mode:
                    case ShaderStage.Static_Per_Vertex_Color:
                    case ShaderStage.Z_Only:
                    case ShaderStage.Sfx_Distort:
                    case ShaderStage.Water_Tessellation:
                    case ShaderStage.Water_Shading:
                    case ShaderStage.Default:
                        macros.Add(ShaderGeneratorBase.CreateMacro("SSR_ENABLE", "1"));
                        break;
                }
            }

            if (blendMode != Shared.Blend_Mode.Opaque ||
                (misc != Shader.Misc.First_Person_Never &&
                misc != Shader.Misc.First_Person_Never_With_rotating_Bitmaps))
                macros.Add(ShaderGeneratorBase.CreateMacro("maybe_calc_albedo", "1"));

            if (misc == Shader.Misc.Always_Calc_Albedo)
                macros.Add(ShaderGeneratorBase.CreateMacro("always_calc_albedo", "1"));

            if (alphaTest == Shared.Alpha_Test.From_Albedo_Alpha)
                macros.Add(ShaderGeneratorBase.CreateMacro("ALPHA_TEST_POST_ALBEDO", "1"));

            if (applyFixes)
                macros.Add(ShaderGeneratorBase.CreateMacro("APPLY_FIXES", "1"));

            if (shaderType != ShaderType.Glass)
            {
                // TODO: remove this once alpha_blend_source integrated into all shaders
                string blendSourceMacro = alphaBlendSource.ToString().ToLower().Remove(0, 5); // remove "from_"
                macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", blendSourceMacro));
            }

            //AppendUserMacros(macros); // TODO: replace external references to CreateGlobalMacros
        }

        private static List<OptionInfo> ValidateOptionInfo(List<OptionInfo> options, bool ps)
        {
            List<OptionInfo> newOptions = new List<OptionInfo>();
            foreach (var option in options)
            {
                // do some debug checks here
                if ((option.PsMacro == Shared.StringIdUtil.Empty && option.PsMacroValue != Shared.StringIdUtil.Empty) ||
                    (option.PsMacro != Shared.StringIdUtil.Empty && option.PsMacroValue == Shared.StringIdUtil.Empty))
                    Console.WriteLine($"WARNING: category {option.Category} option not valid ({option.PsMacro}, {option.PsMacroValue})");
                if ((option.VsMacro == Shared.StringIdUtil.Empty && option.VsMacroValue != Shared.StringIdUtil.Empty) ||
                    (option.VsMacro != Shared.StringIdUtil.Empty && option.VsMacroValue == Shared.StringIdUtil.Empty))
                    Console.WriteLine($"WARNING: category {option.Category} option not valid ({option.VsMacro}, {option.VsMacroValue})");

                if (option.PsMacro == Shared.StringIdUtil.Empty && option.VsMacro == Shared.StringIdUtil.Empty)
                    continue; // we can safely skip, no functions to set
                newOptions.Add(option);
            }
            return newOptions;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderType shaderType, ShaderStage entryPoint,
            List<OptionInfo> options, bool applyFixes)
        {
            var currentOptions = ValidateOptionInfo(options, true);

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            CreateGlobalMacros(macros, shaderType, entryPoint, GetBlendMode(currentOptions),
                GetMisc(currentOptions), GetAlphaTest(currentOptions), GetAlphaBlendSource(currentOptions), applyFixes);
            AppendUserMacros(macros); // TODO: move to CreateGlobalMacros

            foreach (var option in currentOptions)
            {
                if (option.PsMacro != Shared.StringIdUtil.Empty)
                    macros.Add(ShaderGeneratorBase.CreateMacro(option.PsMacro, option.PsMacroValue));
                if (option.VsMacro != Shared.StringIdUtil.Empty)
                    macros.Add(ShaderGeneratorBase.CreateMacro(option.VsMacro, option.VsMacroValue));
            }

            string entryName = GetEntryName(entryPoint);
            string filename = GetSourceFilename(shaderType);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(ShaderType shaderType, ShaderStage entryPoint,
            VertexType vertexType, List<OptionInfo> options, bool applyFixes)
        {
            var currentOptions = ValidateOptionInfo(options, false);

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            CreateGlobalMacros(macros, shaderType, entryPoint, GetBlendMode(currentOptions),
                GetMisc(currentOptions), GetAlphaTest(currentOptions), GetAlphaBlendSource(currentOptions), applyFixes, true, vertexType);
            AppendUserMacros(macros); // TODO: move to CreateGlobalMacros

            foreach (var option in currentOptions)
            {
                if (option.PsMacro != Shared.StringIdUtil.Empty)
                    macros.Add(ShaderGeneratorBase.CreateMacro(option.PsMacro, option.PsMacroValue));
                if (option.VsMacro != Shared.StringIdUtil.Empty)
                    macros.Add(ShaderGeneratorBase.CreateMacro(option.VsMacro, option.VsMacroValue));
            }

            string entryName = GetEntryName(entryPoint, true);
            string filename = GetSourceFilename(shaderType);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }
    }
}
