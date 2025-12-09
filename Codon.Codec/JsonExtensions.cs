using System.Text.Json;
using System.Text.Json.Nodes;

namespace Codon.Codec;

public static class JsonExtensions
{
    public static JsonObject? ToObject(this JsonElement element)
    {
        var json = element.GetRawText();
        return JsonNode.Parse(json)?.AsObject();
    }

    public static JsonArray? ToArray(this JsonElement element)
    {
        var json = element.GetRawText();
        return JsonNode.Parse(json)?.AsArray();
    }
}