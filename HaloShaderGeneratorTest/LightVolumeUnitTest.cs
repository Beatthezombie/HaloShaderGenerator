using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.LightVolume;
using System.Collections.Generic;
using HaloShaderGenerator.TemplateGenerator;

namespace HaloShaderGenerator
{
    class LightVolumeUnitTest : GenericUnitTest
    {
        public LightVolumeUnitTest(string referencePath) : base(referencePath, new LightVolumeGenerator(), ShaderType.LightVolume) { }

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
