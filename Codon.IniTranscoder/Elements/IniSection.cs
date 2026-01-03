using Codon.IniTranscoder.Exceptions;
using Codon.IniTranscoder.Util;
using SynesthesiaUtil.Extensions;

namespace Codon.IniTranscoder.Elements;

public class IniSection(string? name, Dictionary<string, IniValue> values) : IIniElement
{
    public readonly string? Name = name;

    public Dictionary<string, IniValue> Values => values;

    public void Add(string name, IniValue value)
    {
        values.Add(name, value);
    }

    public void Remove(string name)
    {
        values.Remove(name);
    }

    public IniValue? GetOrNull(string name)
    {
        return values.GetValueOrDefault(name);
    }

    public IniValue GetOrThrow(string name)
    {
        return GetOrNull(name) ?? throw new ParsingException($"No such value with key {name} in IniSection");
    }

    public static IniSection Parse(string text)
    {
        string? name = null;
        var values = new Dictionary<string, IniValue>();
        var lines = text.Trim().Split("\n");

        var lineCount = 0;
        foreach (var line in lines)
        {
            if (line.IsEmpty()) continue; // skip empty lines
            if (line.StartsWith(';') || line.StartsWith('#')) continue; // comment

            // section name
            if (lineCount == 0 && text.StartsWith('['))
            {
                var nameSplit = text.Split("]")[0];
                name = nameSplit.RemovePrefix("[").RemoveSuffix("]").Trim();
            }

            else
            {
                var split = line.Split("=").ToList();

                var key = IniUtils.RemoveComments(split[0].Trim());

                var nullableValue = split.GetOrNull(1);
                if (nullableValue != null && nullableValue.IsEmpty()) nullableValue = null;

                var cleaned = nullableValue?.Trim();

                if (!string.IsNullOrEmpty(cleaned))
                {
                    cleaned = IniUtils.RemoveComments(cleaned).Trim();

                    if ((cleaned.StartsWith('"') && cleaned.EndsWith('"')) ||
                        (cleaned.StartsWith('\'') && cleaned.EndsWith('\'')))
                    {
                        cleaned = cleaned[1..^1];
                    }
                }

                values.Add(key, new IniValue(cleaned?.Trim()));
            }

            lineCount++;
        }

        return new IniSection(name, values);
    }

    public override string ToString()
    {
        var stringBuilder = string.Empty;
        if (Name != null) stringBuilder += $"[{Name}]\n";

        foreach (var (key, value) in Values)
        {
            stringBuilder += $"{key} = {value}\n";
        }

        return stringBuilder.Trim();
    }
}