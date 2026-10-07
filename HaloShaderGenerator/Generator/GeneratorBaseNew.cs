using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloShaderGenerator.DirectX;

namespace HaloShaderGenerator.Generator
{
    public class GeneratorBaseNew
    {
        public bool IsMs30 = false;
        private List<D3D.SHADER_MACRO> UserMacros = null;

        public List<D3D.SHADER_MACRO> GetUserMacros() => UserMacros;
        public void SetUserMacros(List<D3D.SHADER_MACRO> macros) => UserMacros = macros;
        public void UnsetUserMacros() => UserMacros = null;
        public void ClearUserMacros() => UserMacros?.Clear();

        public bool AppendUserMacros(List<D3D.SHADER_MACRO> macros)
        {
            if (UserMacros != null)
            {
                macros.AddRange(UserMacros);
                return true;
            }
            return false;
        }
    }
}
