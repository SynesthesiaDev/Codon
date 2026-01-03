using Codon.IniTranscoder.Exceptions;
using SynesthesiaUtil.Extensions;

namespace Codon.IniTranscoder.Elements;

public class IniValue(string? value) : IIniElement
{
    public static IniValue Null => new(null);

    public string? Value => value;

    public override string ToString()
    {
        return Value ?? "null";
    }

    public T? GetOrNull<T>()
    {
        if (Value == null) return default;

        return typeof(T) switch
        {
            var t when t == typeof(bool) => (T)(object)Value.ToBoolean(),
            var t when t == typeof(string) => (T)(object)Value,
            var t when t == typeof(double) => (T)(object)Value.ToDouble(),
            var t when t == typeof(int) => (T)(object)Value.ToInt(),
            var t when t == typeof(float) => (T)(object)Value.ToFloat(),
            var t when t == typeof(short) => (T)(object)Value.ToShort(),
            var t when t == typeof(long) => (T)(object)Convert.ToInt64(Value),
            var t when t == typeof(byte) => (T)(object)Value.ToByte(),

            _ => default
        };
    }

    public T GetOrThrow<T>()
    {
        return GetOrNull<T>() ?? throw new ParsingException("Value is null or has invalid type");
    }
}