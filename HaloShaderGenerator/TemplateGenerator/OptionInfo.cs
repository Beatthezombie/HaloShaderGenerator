
namespace HaloShaderGenerator.TemplateGenerator
{
    public readonly record struct OptionInfo(
        string Category,
        string Option,
        string PsMacro,
        string VsMacro,
        string PsMacroValue,
        string VsMacroValue);
}
