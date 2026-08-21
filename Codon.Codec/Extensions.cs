// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.Json;
using Codon.Optionals;

namespace Codon.Codec;

public static class Extensions
{
    private static readonly JsonSerializerOptions json_options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    public static Optional<T> ToOptional<T>(this T? nullable) where T : struct
    {
        return nullable.HasValue
            ? Optional.Of<T>(nullable.Value)
            : Optional.Empty<T>();
    }

    public static Optional<T> ToOptional<T>(this T? obj) where T : class
    {
        return obj is not null
            ? Optional.Of<T>(obj)
            : Optional.Empty<T>();
    }

    public static JsonElement ToJson(this string text) => JsonDocument.Parse(text).RootElement;

    public static string ToStringPretty(this JsonElement jsonElement) => JsonSerializer.Serialize(jsonElement, json_options);
}
