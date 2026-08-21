// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rider.Plugins.CodonPlugin;

public static class CodedBuilder
{
public static string For(string csharpType, bool binary, bool isEnum = false)
    {
        var trimmed = csharpType.Trim();
        var nullable = trimmed.EndsWith('?');
        var baseType = nullable ? trimmed.Substring(0, trimmed.Length - 1) : trimmed;
        var prefix = binary ? "BinaryCodecs" : "Codecs";

        string field;

        if (isEnum)
        {
            field = binary ? $"BinaryCodecs.Enum<{baseType}>()" : $"Codecs.Enum<{baseType}>()";
        }
        else if (baseType.EndsWith("[]"))
        {
            var elementType = baseType.Substring(0, baseType.Length - 2);
            field = elementType switch
            {
                "byte" => $"{prefix}.BYTE_ARRAY",
                "int" => $"{prefix}.INT_ARRAY",
                "long" => $"{prefix}.LONG_ARRAY",
                _ => $"{For(elementType, binary, isEnum)}.List()"
            };
        }
        else if (TryParseGeneric(baseType, "List", out var listArg) || TryParseGeneric(baseType, "IList", out listArg))
        {
            field = $"{For(listArg, binary, isEnum)}.List()";
        }
        else if (TryParseGenericPair(baseType, out var keyArg, out var valArg))
        {
            field = $"{For(keyArg, binary)}.MapTo({For(valArg, binary)})";
        }
        else
        {
            field = baseType switch
            {
                "bool" => $"{prefix}.BOOLEAN",
                "byte" => $"{prefix}.BYTE",
                "short" => $"{prefix}.SHORT",
                "int" => $"{prefix}.INT",
                "uint" when binary => $"{prefix}.UINT",
                "long" => $"{prefix}.LONG",
                "float" => $"{prefix}.FLOAT",
                "double" => $"{prefix}.DOUBLE",
                "string" => $"{prefix}.STRING",
                "Guid" => $"{prefix}.GUID",
                "IByteBuffer" when binary => $"{prefix}.BYTE_BUFFER",
                _ => binary ? $"{baseType}.BINARY_CODEC" : $"{baseType}.CODEC"
            };
        }

        return nullable ? $"{field}.Optional()" : field;
    }

    public static string BuildCodecField(string className, bool binary, IReadOnlyList<(string Name, string Type, bool Nullable, bool IsEnum)> members)
    {
        var fieldName = binary ? "BINARY_CODEC" : "CODEC";
        var codecType = binary ? "IBinaryCodec" : "Codec";
        var builderType = binary ? "BinaryCodecs" : "StructCodec";

        var sb = new StringBuilder();
        sb.AppendLine($"public static readonly {codecType}<{className}> {fieldName} = {builderType}.For<{className}>()");

        foreach (var m in members)
        {
            var codec = For(m.Type, binary, m.IsEnum);
            var accessor = m.Nullable ? $"c => Optional.Of(c.{m.Name})" : $"c => c.{m.Name}";

            var nameArg = binary ? "" : $"\"{m.Name}\", ";
            sb.AppendLine($"    .Field({nameArg}{codec}, {accessor})");
        }

        var ctorParams = string.Join(", ", members.Select(m => m.Name.ToLowerInvariant()));
        var ctorArgs = string.Join(", ", members.Select(m =>
        {
            var mname = m.Name.ToLowerInvariant();
            var isStruct = m.IsEnum || isKnownStructType(m.Type);

            if (!m.Nullable)
                return mname;


            return isStruct ? $"{mname}.ToNullableStruct()" : $"{mname}.ToNullableClass()";
        }));
        sb.AppendLine($"    .Build(({ctorParams}) =>");
        sb.AppendLine($"        new {className}({ctorArgs}));");

        return sb.ToString();
    }

    public static bool TryParseGeneric(string type, string genericName, out string arg)
    {
        arg = null;
        var prefix = $"{genericName}<";
        if (!type.StartsWith(prefix) || !type.EndsWith(">"))
        {
            return false;
        }

        arg = type.Substring(prefix.Length, type.Length - prefix.Length - 1).Trim();
        return true;
    }

    public static bool TryParseGenericPair(string type, out string key, out string value)
    {
        key = null;
        value = null;

        foreach (var name in new[] { "Dictionary", "IDictionary" })
        {
            var prefix = $"{name}<";
            if (!type.StartsWith(prefix) || !type.EndsWith(">"))
            {
                continue;
            }

            var inner = type.Substring(prefix.Length, type.Length - prefix.Length - 1);
            var depth = 0;
            var splitAt = -1;
            for (var i = 0; i < inner.Length; i++)
            {
                if (inner[i] == '<')
                {
                    depth++;
                }
                else if (inner[i] == '>')
                {
                    depth--;
                }
                else if (inner[i] == ',' && depth == 0)
                {
                    splitAt = i;
                    break;
                }
            }

            if (splitAt < 0)
            {
                continue;
            }

            key = inner.Substring(0, splitAt).Trim();
            value = inner.Substring(splitAt + 1).Trim();
            return true;
        }

        return false;
    }

    private static bool isKnownStructType(string typeName)
    {
        var baseType = typeName.TrimEnd('?').Trim();

        return baseType switch
        {
            "bool" => true,
            "byte" => true,
            "sbyte" => true,
            "char" => true,
            "decimal" => true,
            "double" => true,
            "float" => true,
            "int" => true,
            "uint" => true,
            "long" => true,
            "ulong" => true,
            "short" => true,
            "ushort" => true,
            "Guid" => true,
            "DateTime" => true,
            "TimeSpan" => true,
            "DateTimeOffset" => true,
            _ => false
        };
    }
}
