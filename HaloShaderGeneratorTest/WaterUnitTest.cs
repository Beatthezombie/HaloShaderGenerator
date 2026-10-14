using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Water;
using System.Collections.Generic;
using System;
using System.Linq;
using HaloShaderGenerator.TemplateGenerator;

namespace HaloShaderGenerator
{
    public class WaterUnitTest : GenericUnitTest
    {
        public WaterUnitTest(string referencePath) : base(referencePath, new WaterGenerator(), ShaderType.Water) { }

        public override string GenerateExplicitPixelShader(ExplicitShader explicitShader, ShaderStage stage)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateExplicitVertexShader(ExplicitShader explicitShader, ShaderStage stage, VertexType vertexType)
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
