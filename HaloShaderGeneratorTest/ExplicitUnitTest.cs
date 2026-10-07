using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shader;
using System.Collections.Generic;

namespace HaloShaderGenerator
{
    public class ExplicitUnitTest : GenericUnitTest
    {
        public ExplicitUnitTest(string referencePath) : base(referencePath, null, ShaderType.Shader) { }

        public override string GeneratePixelShader(ShaderStage stage, List<int> shaderOptions)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateSharedPixelShader(ShaderStage stage, int methodIndex, int optionIndex)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateVertexShader(VertexType vertex, ShaderStage stage)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateSharedVertexShader(VertexType vertex, ShaderStage stage)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateExplicitPixelShader(ExplicitShader explicitShader, ShaderStage entry)
        {
            var generator = new ExplicitGenerator();
            var bytecode = generator.GeneratePixelShader(explicitShader, entry, false).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateExplicitVertexShader(ExplicitShader explicitShader, ShaderStage entry, VertexType vertexType)
        {
            var generator = new ExplicitGenerator();
            var bytecode = generator.GenerateVertexShader(explicitShader, entry, vertexType, false).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateChudPixelShader(ChudShader chudShader, ShaderStage entry)
        {
            var bytecode = GenericPixelShaderGenerator.GeneratePixelShader(chudShader.ToString(), entry.ToString().ToLower(), true).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateChudVertexShader(ChudShader chudShader, ShaderStage entry, VertexType vertexType)
        {
            var bytecode = GenericVertexShaderGenerator.GenerateVertexShader(chudShader.ToString(), entry.ToString().ToLower(), vertexType).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }
    }
}
