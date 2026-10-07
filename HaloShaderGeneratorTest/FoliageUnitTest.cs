using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Foliage;
using System.Collections.Generic;
using HaloShaderGenerator.TemplateGenerator;

namespace HaloShaderGenerator
{
    public class FoliageUnitTest : GenericUnitTest
    {
        public FoliageUnitTest(string referencePath) : base(referencePath, new FoliageGenerator(), ShaderType.Foliage) { }

        public override string GeneratePixelShader(ShaderStage stage, List<int> shaderOptions)
        {
            var gen = new TemplateGenerator.TemplateGenerator();
            var bytecode = gen.GeneratePixelShader(Type, stage, StaticOptionInfo.OptionIndicesToOptionInfo(Type, shaderOptions), false).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateSharedPixelShader(ShaderStage stage, int methodIndex, int optionIndex)
        {
            var gen = new TemplateGenerator.TemplateGenerator();
            var bytecode = gen.GeneratePixelShader(Type, stage, new List<OptionInfo>() { StaticOptionInfo.GetOptionInfo(Type, methodIndex, optionIndex) }, false).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateVertexShader(VertexType vertex, ShaderStage stage)
        {
            var gen = new FoliageGenerator();
            var bytecode = gen.GenerateVertexShader(vertex, stage).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateSharedVertexShader(VertexType vertex, ShaderStage stage)
        {
            var gen = new FoliageGenerator();
            var bytecode = gen.GenerateSharedVertexShader(vertex, stage).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateExplicitPixelShader(ExplicitShader explicitShader, ShaderStage entry)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateExplicitVertexShader(ExplicitShader explicitShader, ShaderStage entry, VertexType vertexType)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateChudPixelShader(ChudShader chudShader, ShaderStage entry)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateChudVertexShader(ChudShader chudShader, ShaderStage entry, VertexType vertexType)
        {
            throw new System.NotImplementedException();
        }
    }
}
