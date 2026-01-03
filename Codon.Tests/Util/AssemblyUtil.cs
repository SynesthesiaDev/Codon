using System.Reflection;
using System.Text;

namespace Codon.Tests.Util;

public static class AssemblyUtil
{
    public static string GetTextResource(Assembly assembly, string resourcePath)
    {
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourcePath}' does not exist!");

        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: false);
        return reader.ReadToEnd();
    }
}