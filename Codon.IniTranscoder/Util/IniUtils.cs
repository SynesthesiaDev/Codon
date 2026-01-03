using Codon.Codec;

namespace Codon.IniTranscoder.Util;

public static class IniUtils
{
    public static readonly char[] COMMENT_SYMBOLS = [';', '#'];

    public static string RemoveComments(string text)
    {
        var array = text.ToCharArray();
        var stringBuffer = string.Empty;

        foreach (var character in array)
        {
            if (COMMENT_SYMBOLS.Contains(character)) break;
            stringBuffer += character;
        }

        return stringBuffer;
    }

    public static T? GetOrNull<T>(this List<T> list, int index)
    {
        return (index >= 0 && index < list.Count) ? list[index] : default;
    }

    public static ICodec<T> WithSection<T>(this ICodec<T> codec, string sectionName) where T : notnull
    {
        return new PrimitiveCodec<T>(
            (transcoder, value) =>
            {
                var namedTranscoder = transcoder.Named(sectionName);
                return codec.Encode(namedTranscoder, value);
            },
            (transcoder, value) => codec.Decode(transcoder, value)
        );
    }
}
