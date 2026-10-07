using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.TemplateGenerator;
using System.Text.RegularExpressions;

namespace HaloShaderGenerator
{
    public class ShaderUnitTest : GenericUnitTest
    {
        public ShaderUnitTest(string referencePath) : base(referencePath, new ShaderGenerator(), ShaderType.Shader) { }

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
            var gen = new ShaderGenerator();
            var bytecode = gen.GenerateVertexShader(vertex, stage).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateSharedVertexShader(VertexType vertex, ShaderStage stage)
        {
            var gen = new ShaderGenerator();
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
    
    public abstract class GenericUnitTest
    {
        private static bool IgnoreD3DX = true;
        private static string ReferencePath;
        private IShaderGenerator ReferenceGenerator;
        private static string ShaderTypeName;
        internal static ShaderType Type;

        public GenericUnitTest(string referencePath, IShaderGenerator referenceGenerator, ShaderType shaderType)
        {
            ReferencePath = referencePath;
            ReferenceGenerator = referenceGenerator;
            ShaderTypeName = Regex.Replace(shaderType.ToString(), @"(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", "_").ToLowerInvariant();
            Type = shaderType;
        }

        public static string BuildShaderName(List<int> methods)
        {
            string result = "";
            foreach (var m in methods)
            {
                result += $"_{m}";
            }
            return result;
        }

        public static string BuildPixelShaderEntryPointName(ShaderStage stage)
        {
            return $"{stage.ToString().ToLower()}.pixel_shader";
        }

        public static List<List<int>> GetAllTestPixelShaders()
        {
            var pixelShaderPath = Path.Combine(ReferencePath, $"{ShaderTypeName}_templates");
            List<string> availableShaders = Directory.GetDirectories(pixelShaderPath).ToList();
            List<List<int>> availableShaderMethods = new List<List<int>>();
            foreach (var shader in availableShaders)
            {
                var folder = shader.Split('\\').Last();
                var methods = folder.Remove(0, 1).Split('_');
                List<int> methodIndices = new List<int>();
                foreach (var method in methods)
                    methodIndices.Add(Int32.Parse(method));
                availableShaderMethods.Add(methodIndices);
            }
            return availableShaderMethods;
        }

        public static string GetTestSharedVertexShader(VertexType vertex, ShaderStage stage)
        {
            var vertexShaderPath = Path.Combine(ReferencePath, $"{ShaderTypeName}_shared_vertex_shaders");
            vertexShaderPath = Path.Combine(vertexShaderPath, $"{vertex.ToString().ToLower()}");
            vertexShaderPath = Path.Combine(vertexShaderPath, $"{stage.ToString().ToLower()}.shared_vertex_shader");
            return vertexShaderPath;
        }

        public static string GetTestSharedPixelShader(ShaderStage stage, int methodIndex = -1, int optionIndex = -1)
        {
            var vertexShaderPath = Path.Combine(ReferencePath, $"{ShaderTypeName}_shared_pixel_shaders");
            var filename = $"{stage.ToString().ToLower()}";
            if(methodIndex != -1 && optionIndex != -1)
            {
                filename += $"_{methodIndex}_{optionIndex}";
            }

            vertexShaderPath = Path.Combine(vertexShaderPath, $"{filename}.shared_pixel_shader");

            return vertexShaderPath;
        }

        public static void DisplayPixelShaderTestResults(bool success, string shaderName, ShaderStage stage, bool usesD3DX)
        {
            if (IgnoreD3DX && usesD3DX)
                return;

            string stageFixedLength = stage.ToString().ToLower().PadRight(24);

            if (!success)
                QueuedConsole.QueueMessage($"{shaderName.PadRight(32)}{stageFixedLength}\tnot identical to reference" + (usesD3DX ? " USES D3DX." : ""), ConsoleColor.Red);
            else
                QueuedConsole.QueueMessage($"{shaderName.PadRight(32)}{stageFixedLength}\tidentical to reference", ConsoleColor.Green);
        }

        public static void DisplayVertexShaderTestResults(bool success, VertexType vertex, ShaderStage stage, bool usesD3DX)
        {
            if (IgnoreD3DX && usesD3DX)
                return;

            string stageFixedLength = stage.ToString().ToLower().PadRight(24);

            if (!success)
                QueuedConsole.QueueMessage($"{stageFixedLength}\tvertex type {vertex.ToString().ToLower().PadRight(24)}\tnot identical to reference" + (usesD3DX ? " USES D3DX." : ""), ConsoleColor.Red);
            else
                QueuedConsole.QueueMessage($"{stageFixedLength}\tvertex type {vertex.ToString().ToLower().PadRight(24)}\tidentical to reference", ConsoleColor.Green);
        }

        public static void DisplaySharedPixelShaderTestResults(bool success, int methodIndex, int optionIndex, ShaderStage stage, bool usesD3DX)
        {
            if (IgnoreD3DX && usesD3DX)
                return;

            string stageFixedLength = (stage.ToString().ToLower() + $"_{methodIndex}_{optionIndex}").PadRight(24);

            if (!success)
                QueuedConsole.QueueMessage($"{stageFixedLength}\tnot identical to reference" + (usesD3DX ? " USES D3DX." : ""), ConsoleColor.Red);
            else
                QueuedConsole.QueueMessage($"{stageFixedLength}\tidentical to reference", ConsoleColor.Green);
        }

        public static bool CompareShaders(string generatedDissassembly, string filePath, string version, out bool usesD3DX)
        {
            string fileGuid = Guid.NewGuid().ToString("N") + ".shader";

            var generatedShaderFile = new FileInfo(fileGuid);
            using (var genStream = generatedShaderFile.Create())
            using (StreamWriter writer = new StreamWriter(genStream))
            {
                writer.WriteLine(generatedDissassembly);
            }
            var referenceDissasembly = File.ReadAllText(filePath);
            generatedDissassembly = File.ReadAllText(fileGuid);

            bool equal = string.Equals(generatedDissassembly, referenceDissasembly);
            generatedShaderFile.Delete();

            if (!equal)
            {
                equal = ReorderConstantsAndTest(generatedDissassembly, referenceDissasembly, version);
            }

            usesD3DX = referenceDissasembly.Contains("Generated by Microsoft (R) D3DX9 Shader Compiler");

            return equal;
        }

        public struct RegisterConstant
        {
            public string Name;
            public string Register; // eg. s2 for sampler 2

            public static bool operator==(RegisterConstant a, RegisterConstant b)
            {
                return a.Name == b.Name;
            }
            public static bool operator!=(RegisterConstant a, RegisterConstant b)
            {
                return a.Name != b.Name;
            }
            public override bool Equals(object a)
            {
                return typeof(RegisterConstant) == a.GetType() && this.Name == ((RegisterConstant)a).Name;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }

        public struct DisassemblyConstants
        {
            public string Name;
            public int Index;
            public string X;
            public string Y;
            public string Z;
            public string W;

            public DisassemblyConstants(string name, int index, string x, string y, string z, string w)
            {
                Name = name;
                Index = index;
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
        }

        public static Dictionary<string, DisassemblyConstants> GetConstants(string data, string version)
        {
            var startIndex = data.IndexOf(version) + 7;
            var trimmedString = data.Substring(startIndex);
            var endIndex = trimmedString.IndexOf("dcl_") - 5;
            if (endIndex < 0)
                return null;
            var constantsBlock = trimmedString.Substring(0, endIndex);
            constantsBlock = constantsBlock.Replace("    def ", "");
            List<string> registerConstants = constantsBlock.Split('\n').ToList();
            Dictionary<string, DisassemblyConstants> constantsMapping = new Dictionary<string, DisassemblyConstants>();
            for(int i = 0; i < registerConstants.Count; i++)
            {
                var register = registerConstants[i];
                var regConstants = register.Split(',').ToList();
                if (regConstants.Count != 4) // must be 4, otherwise not a constant
                    continue;
                var name = regConstants[0];
                regConstants.RemoveAt(0);
                constantsMapping[name] = new DisassemblyConstants(name, i, regConstants[0], regConstants[1], regConstants[2], regConstants[3]);
            }

            return constantsMapping;
        }

        // samplers only for now
        public static List<RegisterConstant> GetRegisters(string data)
        {
            List<RegisterConstant> result = new List<RegisterConstant>();
            List<string> dataLined = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int titleLineIndex = dataLined.IndexOf("// Registers:");
            int registerCount = titleLineIndex - 7;
            int regOffset = dataLined[titleLineIndex + 2].IndexOf("Reg");

            for (int i = titleLineIndex + 4; i < titleLineIndex + 4 + registerCount; i++)
            {
                string name = dataLined[i].Remove(0, 5).Split(' ')[0];
                string register = dataLined[i].Remove(0, regOffset).Split(' ')[0];
                RegisterConstant constant = new RegisterConstant { Name = name, Register = register };
                if (register.StartsWith("s"))
                    result.Add(constant);
            }

            return result;
        }

        public static string RebuildConstants(DisassemblyConstants constant)
        {
            return $"{constant.X}, {constant.Y}, {constant.Z}, {constant.W}";
        }

        public static string ReplaceConstants(DisassemblyConstants genConstants, DisassemblyConstants refConstants, string genData)
        {
            genData = genData.Replace($"{genConstants.Name} ", $"shadergenprefix{refConstants.Name}shadergensuffix ");
            genData = genData.Replace($"{genConstants.Name}.", $"shadergenprefix{refConstants.Name}shadergensuffix.");
            genData = genData.Replace($"{genConstants.Name}\n", $"shadergenprefix{refConstants.Name}shadergensuffix\n");
            return genData;
        }

        public static bool ReorderConstantsAndTest(string genData, string refData, string version)
        {
            var refConstants = GetConstants(refData, version);
            var genConstants = GetConstants(genData, version);

            if (refConstants == null || genConstants == null)
                return false;

            if (refConstants.Count != genConstants.Count)
                return false;

            Dictionary<DisassemblyConstants, DisassemblyConstants> swaps = new Dictionary<DisassemblyConstants, DisassemblyConstants>();

            foreach(var refConstantName in refConstants.Keys)
            {
                if (!genConstants.ContainsKey(refConstantName))
                    return false;
                var refConstant = refConstants[refConstantName];
                var genConstant = genConstants[refConstantName];
                var refConstantString = RebuildConstants(refConstant);
                if (RebuildConstants(genConstant).Equals(refConstantString))
                    continue;

                // different constants, replace lines properly
                foreach(var genConstantName in genConstants.Keys)
                {
                    // add per component here if needed
                    if (RebuildConstants(genConstants[genConstantName]).Equals(refConstantString))
                    {
                        // found matching line
                        genData = ReplaceConstants(genConstants[genConstantName], refConstant, genData);
                        //swaps[genConstants[genConstantName]] = refConstant;
                        break;
                    }
                }
            }

            genData = genData.Replace("shadergenprefix", "");
            genData = genData.Replace("shadergensuffix", "");
            var startIndex = genData.IndexOf("ps_3_0") + 7;
            var sourceConstantBlock = genData.Substring(startIndex);
            var endIndex = sourceConstantBlock.IndexOf("dcl_") - 5;
            sourceConstantBlock = sourceConstantBlock.Substring(0, endIndex);

            startIndex = refData.IndexOf("ps_3_0") + 7;
            var destConstantBlock = refData.Substring(startIndex);
            endIndex = destConstantBlock.IndexOf("dcl_") - 5;
            destConstantBlock = destConstantBlock.Substring(0, endIndex);

            genData = genData.Replace(sourceConstantBlock, destConstantBlock);

            // samplers
            var refSamplers = GetRegisters(refData);
            var genSamplers = GetRegisters(genData);

            Dictionary<string, string> replacements = new Dictionary<string, string>();

            // replace with generic first so we don't override
            foreach (var refSampler in refSamplers)
            {
                int index = genSamplers.IndexOf(refSampler);
                if (index == -1)
                    return false;

                string samplerIndex = genSamplers[index].Register.Remove(0, 1);
                string rep = $"SAMPLER{samplerIndex}";

                genData = genData.Replace(genSamplers[index].Register, rep);
                replacements.Add(rep, refSampler.Register);
            }
            foreach (var rep in replacements)
            {
                // hack for columns
                if (rep.Key.Remove(0, 6).Length < rep.Value.Length)
                {
                    int index = genData.IndexOf(rep.Key);

                    genData = genData.Remove(index + rep.Key.Length, 1);
                }
                else if (rep.Key.Remove(0, 6).Length > rep.Value.Length)
                {
                    int index = genData.IndexOf(rep.Key);

                    genData = genData.Insert(index + rep.Key.Length, " ");
                }

                genData = genData.Replace(rep.Key, rep.Value);
            }

            return string.Equals(genData, refData);
        }


        public List<ShaderStage> GetAllShaderStages()
        {
            var stages = new List<ShaderStage>();
            foreach (var stage in Enum.GetValues(typeof(ShaderStage)))
                stages.Add((ShaderStage)stage);
            return stages;
        }

        public List<VertexType> GetAllVertexFormats()
        {
            var vertices = new List<VertexType>();
            foreach (var vertex in Enum.GetValues(typeof(VertexType)))
                vertices.Add((VertexType)vertex);
            return vertices;
        }

        public bool TestExplicitPixelShader(ExplicitShader explicitShader)
        {
            bool success = true;

            var entries = Generic.GenericShaderStage.GetExplicitEntryPoints(explicitShader);

            foreach (var entry in entries)
            {
                string filePath = Path.Combine(Path.Combine(ReferencePath, explicitShader.ToString()), $"0_{entry.ToString().ToLower()}.pixel_shader");
                var file = new FileInfo(filePath);

                if (file.Exists == false)
                {
                    Console.WriteLine($"No reference shader for {explicitShader}");
                    success = false;
                    continue;
                }

                var disassembly = GenerateExplicitPixelShader(explicitShader, entry);
                bool equal = CompareShaders(disassembly, filePath, "ps_3_0", out bool usesD3DX);
                success &= equal;
                DisplayPixelShaderTestResults(equal, explicitShader.ToString(), entry, usesD3DX);

                if (!equal)
                {
                    string filename = $"generated_{Application.ExplicitShader}_{entry.ToString().ToLower()}.pixel_shader";
                    Application.WriteShaderFile(filename, disassembly);
                }
            }

            return success;
        }

        public bool TestChudPixelShader(ChudShader chudShader)
        {
            bool success = true;

            var entries = Generic.GenericShaderStage.GetChudEntryPoints(chudShader);

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                string filePath = Path.Combine(Path.Combine(ReferencePath, chudShader.ToString()), $"{i}_{entry.ToString().ToLower()}.pixel_shader");
                var file = new FileInfo(filePath);

                if (file.Exists == false)
                {
                    Console.WriteLine($"No reference shader for {chudShader}");
                    success = false;
                    continue;
                }

                var disassembly = GenerateChudPixelShader(chudShader, entry);
                bool equal = CompareShaders(disassembly, filePath, "ps_3_0", out bool usesD3DX);
                success &= equal;
                DisplayPixelShaderTestResults(equal, chudShader.ToString(), entry, usesD3DX);

                if (!equal)
                {
                    string filename = $"generated_{Application.ChudShader}_{entry}.pixel_shader";
                    Application.WriteShaderFile(filename, disassembly);
                }
            }

            return success;
        }

        public bool TestAllExplicitPixelShaders()
        {
            bool success = true;

            List<Task<bool>> testTasks = new List<Task<bool>>();

            foreach (ExplicitShader explicitShader in Enum.GetValues(typeof(ExplicitShader)))
            {
                Task<bool> testTask = Task.Run(() => { return TestExplicitPixelShader(explicitShader); });
                testTasks.Add(testTask);
            }

            Task.WaitAll(testTasks.ToArray());

            success = !testTasks.Any(p => p.Result != true);

            if (success)
                Console.WriteLine("All unit tests passed sucessfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        public bool TestAllChudPixelShaders()
        {
            bool success = true;

            List<Task<bool>> testTasks = new List<Task<bool>>();

            foreach (ChudShader chudShader in Enum.GetValues(typeof(ChudShader)))
            {
                if (chudShader == ChudShader.chud_unknown)
                    continue; // no reference for now (need to create own)

                Task<bool> testTask = Task.Run(() => { return TestChudPixelShader(chudShader); });
                testTasks.Add(testTask);
            }

            Task.WaitAll(testTasks.ToArray());

            success = !testTasks.Any(p => p.Result != true);

            if (success)
                Console.WriteLine("All unit tests passed sucessfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        public bool TestExplicitVertexShader(ExplicitShader explicitShader)
        {
            bool success = true;
            var generator = new ExplicitGenerator();

            var entries = generator.ScrapeEntryPoints(explicitShader);

            foreach (var entry in entries)
            {
                string filePath = Path.Combine(Path.Combine(ReferencePath, explicitShader.ToString()), $"world\\0_{entry}.vertex_shader");
                var file = new FileInfo(filePath);

                if (file.Exists == false)
                {
                    Console.WriteLine($"No reference shader for {explicitShader}");
                    success = false;
                    continue;
                }

                var vertexTypes = generator.ScrapeVertexTypes(explicitShader);

                foreach (var vertexType in vertexTypes)
                {
                    var disassembly = GenerateExplicitVertexShader(explicitShader, entry, vertexType);
                    bool equal = CompareShaders(disassembly, filePath, "vs_3_0", out bool usesD3DX);
                    success &= equal;
                    DisplayPixelShaderTestResults(equal, explicitShader.ToString() + $" {vertexType}", entry, usesD3DX);

                    if (!equal)
                    {
                        string filename = $"generated_{Application.ExplicitShader}_{entry}_{vertexType}.vertex_shader";
                        Application.WriteShaderFile(filename, disassembly);
                    }
                }
            }

            return success;
        }

        public bool TestChudVertexShader(ChudShader chudShader)
        {
            bool success = true;

            var vertexTypes = Generic.GenericShaderStage.GetChudVertexTypes(chudShader);
            var entries = Generic.GenericShaderStage.GetChudEntryPoints(chudShader);

            foreach (var vertexType in vertexTypes)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    string filePath = Path.Combine(Path.Combine(ReferencePath, chudShader.ToString()), $"world\\{i}_{entry}.vertex_shader");
                    var file = new FileInfo(filePath);

                    if (file.Exists == false)
                    {
                        Console.WriteLine($"No reference shader for {chudShader}");
                        success = false;
                        continue;
                    }

                    var disassembly = GenerateChudVertexShader(chudShader, entry, vertexType);
                    bool equal = CompareShaders(disassembly, filePath, "vs_3_0", out bool usesD3DX);
                    success &= equal;
                    DisplayVertexShaderTestResults(equal, vertexType, entry, usesD3DX);

                    if (!equal)
                    {
                        string filename = $"generated_{Application.ChudShader}_{vertexType}_{entry}.vertex_shader";
                        Application.WriteShaderFile(filename, disassembly);
                    }
                }
            }

            return success;
        }

        public bool TestAllExplicitVertexShaders()
        {
            bool success = true;

            List<Task<bool>> testTasks = new List<Task<bool>>();

            foreach (ExplicitShader explicitShader in Enum.GetValues(typeof(ExplicitShader)))
            {
                Task<bool> testTask = Task.Run(() => { return TestExplicitVertexShader(explicitShader); });
                testTasks.Add(testTask);
            }

            Task.WaitAll(testTasks.ToArray());

            success = !testTasks.Any(p => p.Result != true);

            if (success)
                Console.WriteLine("All unit tests passed successfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        public bool TestAllChudVertexShaders()
        {
            bool success = true;

            List<Task<bool>> testTasks = new List<Task<bool>>();

            foreach (ChudShader chudShader in Enum.GetValues(typeof(ChudShader)))
            {
                if (chudShader == ChudShader.chud_unknown)
                    continue; // no reference for now (need to create own)

                Task<bool> testTask = Task.Run(() => { return TestChudVertexShader(chudShader); });
                testTasks.Add(testTask);
            }

            Task.WaitAll(testTasks.ToArray());

            success = !testTasks.Any(p => p.Result != true);

            if (success)
                Console.WriteLine("All unit tests passed successfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        private bool TestPixelShaderTaskBody(List<int> testShader, List<ShaderStage> stageOverrides, List<List<int>> methodOverrides)
        {
            bool success = true;

            // if we've added new options, old shader lists won't have them. use this to generate
            List<int> generatorList = new List<int>();

            for (int i = 0; i < StaticOptionInfo.GetCategoryCount(Type); i++)
            {
                if (i < testShader.Count)
                    generatorList.Add(testShader[i]);
                else
                    generatorList.Add(0);
            }

            List<ShaderStage> stages;
            if (stageOverrides != null && stageOverrides.Count > 0)
                stages = stageOverrides;
            else
                stages = GetAllShaderStages();

            foreach (var stage in stages)
            {
                if (ReferenceGenerator.IsEntryPointSupported(stage) && !ReferenceGenerator.IsPixelShaderShared(stage))
                {
                    if (methodOverrides != null && methodOverrides.Count == ReferenceGenerator.GetMethodCount())
                    {
                        bool validOptions = true;
                        for (int i = 0; i < ReferenceGenerator.GetMethodCount(); i++)
                        {
                            var optionOverrides = methodOverrides[i];
                            if (optionOverrides != null && optionOverrides.Count > 0)
                                validOptions &= optionOverrides.Contains(testShader[i]);
                        }


                        if (!validOptions)
                            continue;
                    }

                    string filePath = Path.Combine(Path.Combine(ReferencePath, $"{ShaderTypeName.ToLower()}_templates"), BuildShaderName(testShader));
                    filePath = Path.Combine(filePath, BuildPixelShaderEntryPointName(stage));
                    var file = new FileInfo(filePath);

                    if (file.Exists == false)
                    {
                        Console.WriteLine($"No reference shader for {BuildShaderName(testShader)} at {stage.ToString().ToLower()}");
                        GeneratePixelShader(stage, generatorList);
                        DisplayPixelShaderTestResults(true, BuildShaderName(generatorList), stage, false);
                        success = false;
                        continue;
                    }

                    var disassembly = GeneratePixelShader(stage, generatorList);
                    bool equal = CompareShaders(disassembly, filePath, "ps_3_0", out bool usesD3DX);
                    success &= (equal || usesD3DX);
                    DisplayPixelShaderTestResults(equal, BuildShaderName(generatorList), stage, usesD3DX);

                    if (Application.OutputAll && !equal)
                    {
                        string filename = $"generated_{stage.ToString().ToLower()}{BuildShaderName(generatorList)}.pixl";
                        Application.WriteShaderFile(filename, disassembly);
                    }
                }
            }

            return success;
        }

        public bool TestAllPixelShaders(List<List<int>> shaderOverrides, List<ShaderStage> stageOverrides, List<List<int>> methodOverrides)
        {
            List<List<int>> shaders;
            if (shaderOverrides != null && shaderOverrides.Count > 0)
                shaders = shaderOverrides;
            else
                shaders = GetAllTestPixelShaders();

            List<Task<bool>> testTasks = new List<Task<bool>>();

            foreach (var testShader in shaders)
            {
                if (HalogramIsMs25(testShader))
                    continue;

                Task<bool> testTask = Task.Run(() => { return TestPixelShaderTaskBody(testShader, stageOverrides, methodOverrides); });
                testTasks.Add(testTask);
            }

            Task.WaitAll(testTasks.ToArray());

            bool success = !testTasks.Any(p => p.Result != true);

            if (success)
                Console.WriteLine("All unit tests passed successfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        private bool TestSharedPixelShaderTaskBody(ShaderStage stage)
        {
            bool success = true;
            if (ReferenceGenerator.IsEntryPointSupported(stage) && ReferenceGenerator.IsPixelShaderShared(stage))
            {
                if (ReferenceGenerator.IsSharedPixelShaderUsingMethods(stage))
                {
                    for (int i = 0; i < ReferenceGenerator.GetMethodCount(); i++)
                    {
                        for (int j = 0; j < ReferenceGenerator.GetMethodOptionCount(i); j++)
                        {
                            if (ReferenceGenerator.IsMethodSharedInEntryPoint(stage, i))
                            {
                                string filePath = GetTestSharedPixelShader(stage, i, j);
                                var file = new FileInfo(filePath);

                                if (file.Exists == false)
                                {
                                    Console.WriteLine($"No reference shader for {stage}_{i}_{j} at {stage.ToString().ToLower()}");
                                    success = false;
                                    continue;
                                }
                                bool equal = CompareShaders(GenerateSharedPixelShader(stage, i, j), filePath, "ps_3_0", out bool usesD3DX);
                                success &= equal;
                                DisplaySharedPixelShaderTestResults(equal, i, j, stage, usesD3DX);
                            }
                        }

                    }
                }
                else
                {
                    string filePath = GetTestSharedPixelShader(stage, -1, -1);
                    var file = new FileInfo(filePath);

                    if (file.Exists == false)
                    {
                        Console.WriteLine($"No reference shader for {stage} at {stage.ToString().ToLower()}");
                        success = false;
                        return success;
                    }
                    bool equal = CompareShaders(GenerateSharedPixelShader(stage, -1, -1), filePath, "ps_3_0", out bool usesD3DX);
                    success &= equal;
                    DisplaySharedPixelShaderTestResults(equal, -1, -1, stage, usesD3DX);
                }
            }

            return success;
        }

        public bool TestAllSharedPixelShaders(List<ShaderStage> stageOverrides)
        {
            List<ShaderStage> stages;
            if (stageOverrides != null && stageOverrides.Count > 0)
                stages = stageOverrides;
            else
                stages = GetAllShaderStages();

            List<Task<bool>> testTasks = new List<Task<bool>>();

            foreach (var stage in stages)
            {
                Task<bool> testTask = Task.Run(() => { return TestSharedPixelShaderTaskBody(stage); });
                testTasks.Add(testTask);
            }

            Task.WaitAll(testTasks.ToArray());
            bool success = !testTasks.Any(p => p.Result != true);

            if (success)
                Console.WriteLine("All unit tests passed successfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        private bool TestShaderVertexShaderTaskBody(VertexType vertex, List<ShaderStage> stages)
        {
            bool success = true;
            if (!ReferenceGenerator.IsVertexFormatSupported(vertex))
                return false;

            foreach (var stage in stages)
            {
                if (ReferenceGenerator.IsEntryPointSupported(stage) && ReferenceGenerator.IsVertexShaderShared(stage))
                {
                    string filePath = GetTestSharedVertexShader(vertex, stage);
                    var file = new FileInfo(filePath);

                    if (file.Exists == false)
                    {
                        Console.WriteLine($"No reference shader for {stage.ToString().ToLower()} vertex format {vertex.ToString().ToLower()}");
                        success = false;
                        continue;
                    }

                    bool equal = CompareShaders(GenerateSharedVertexShader(vertex, stage), filePath, "vs_3_0", out bool usesD3DX);
                    success &= equal;
                    DisplayVertexShaderTestResults(equal, vertex, stage, usesD3DX);
                }
            }

            return success;
        }

        public bool TestAllSharedVertexShaders(List<VertexType> vertexOverrides, List<ShaderStage> stageOverrides)
        {
            bool success = true;

            List<VertexType> vertices;
            if (vertexOverrides != null && vertexOverrides.Count > 0)
                vertices = vertexOverrides;
            else
                vertices = GetAllVertexFormats();

            List<ShaderStage> stages;
            if (stageOverrides != null && stageOverrides.Count > 0)
                stages = stageOverrides;
            else
                stages = GetAllShaderStages();

            List<Task<bool>> testTasks = new List<Task<bool>>();

            foreach (var vertex in vertices)
            {
                Task<bool> testTask = Task.Run(() => { return TestShaderVertexShaderTaskBody(vertex, stages); });
                testTasks.Add(testTask);
            }

            Task.WaitAll(testTasks.ToArray());
            success = !testTasks.Any(p => p.Result != true);

            if (success)
                Console.WriteLine("All unit tests passed successfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        public abstract string GeneratePixelShader(ShaderStage stage, List<int> shaderOptions);

        public abstract string GenerateSharedPixelShader(ShaderStage stage, int methodIndex, int optionIndex);

        public abstract string GenerateVertexShader(VertexType vertex, ShaderStage stage);

        public abstract string GenerateSharedVertexShader(VertexType vertex, ShaderStage stage);

        public abstract string GenerateExplicitPixelShader(ExplicitShader explicitShader, ShaderStage entry);

        public abstract string GenerateExplicitVertexShader(ExplicitShader explicitShader, ShaderStage entry, VertexType vertexType);

        public abstract string GenerateChudPixelShader(ChudShader chudShader, ShaderStage entry);

        public abstract string GenerateChudVertexShader(ChudShader chudShader, ShaderStage entry, VertexType vertexType);

        private bool HalogramIsMs25(List<int> testShader)
        {
            if (ReferenceGenerator.GetType() == typeof(Halogram.HalogramGenerator))
            {
                foreach (var ms25H in Application.HalogramMS25)
                {
                    bool isEqual = true;
                    for (int i = 0; i < testShader.Count; i++)
                    {
                        if (testShader[i] != ms25H[i])
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    if (isEqual)
                        return true;
                }
            }

            return false;
        }
    }
}
