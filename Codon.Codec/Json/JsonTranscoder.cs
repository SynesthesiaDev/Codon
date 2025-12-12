using System.Text.Json;
using System.Text.Json.Nodes;

namespace Codon.Codec.Transcoder.Transcoders;

public class JsonTranscoder : ITranscoder<JsonElement>
{
    private static readonly JsonDocumentOptions DocumentOptions = new() { AllowTrailingCommas = true };
    public static JsonTranscoder Instance = new JsonTranscoder();

    private static JsonElement SerializeValue<T>(T? value)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);

        using var document = JsonDocument.Parse(bytes, DocumentOptions);
        return document.RootElement.Clone();
    }

    private class JsonListBuilder(JsonArray array) : IListBuilder<JsonElement>
    {
        public IListBuilder<JsonElement> Add(JsonElement value)
        {
            array.Add(value);
            return this;
        }

        public JsonElement Build()
        {
            return SerializeValue(array);
        }
    }

    private class JsonVirtualMapBuilder(JsonObject jsonObject) : IVirtualMapBuilder<JsonElement>
    {
        public IVirtualMapBuilder<JsonElement> Put(JsonElement key, JsonElement value)
        {
            Put(key.GetString() ?? throw new InvalidOperationException(), value);
            return this;
        }

        public IVirtualMapBuilder<JsonElement> Put(string key, JsonElement value)
        {
            jsonObject.Add(key ?? throw new InvalidOperationException(), JsonNode.Parse(value.GetRawText()));
            return this;
        }

        public JsonElement Build()
        {
            return SerializeValue(jsonObject);
        }
    }

    private class JsonVirtualMap(JsonElement value) : IVirtualMap<JsonElement>
    {
        private JsonObject? jsonObject => value.ToObject();

        public List<string> GetKeys()
        {
            return jsonObject == null ? throw new InvalidOperationException("JsonElement is not JsonObject") : jsonObject.ToDictionary().Keys.ToList();
        }

        public bool HasValue(string key)
        {
            return jsonObject?.ContainsKey(key) ?? throw new InvalidOperationException("JsonElement is not JsonObject");
        }

        public JsonElement GetValue(string key)
        {
            return jsonObject == null ? throw new InvalidOperationException("JsonElement is not JsonObject") : SerializeValue(jsonObject.ToDictionary()[key]);
        }
    }

    public JsonElement EncodeNull()
    {
        return SerializeValue<object>(null);
    }

    public JsonElement EncodeBool(bool value)
    {
        return SerializeValue(value);
    }

    public bool DecodeBool(JsonElement value)
    {
        return value.GetBoolean();
    }

    public JsonElement EncodeByte(byte value)
    {
        return SerializeValue(value);
    }

    public byte DecodeByte(JsonElement value)
    {
        return value.GetByte();
    }

    public JsonElement EncodeShort(short value)
    {
        return SerializeValue(value);
    }

    public short DecodeShort(JsonElement value)
    {
        return value.GetInt16();
    }

    public JsonElement EncodeInt(int value)
    {
        return SerializeValue(value);
    }

    public int DecodeInt(JsonElement value)
    {
        return value.GetInt32();
    }

    public JsonElement EncodeLong(long value)
    {
        return SerializeValue(value);
    }

    public long DecodeLong(JsonElement value)
    {
        return value.GetInt64();
    }

    public JsonElement EncodeFloat(float value)
    {
        return SerializeValue(value);
    }

    public float DecodeFloat(JsonElement value)
    {
        return value.GetSingle();
    }

    public JsonElement EncodeDouble(double value)
    {
        return SerializeValue(value);
    }

    public double DecodeDouble(JsonElement value)
    {
        return value.GetDouble();
    }

    public JsonElement EncodeString(string value)
    {
        return SerializeValue(value);
    }

    public string DecodeString(JsonElement value)
    {
        return value.GetString() ?? throw new InvalidOperationException();
    }

    public JsonElement EncodeByteArray(byte[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var b in array)
        {
            list.Add(EncodeByte(b));
        }
        return list.Build();
    }

    public byte[] DecodeByteArray(JsonElement value)
    {
        var bytes = new List<byte>();
        DecodeList(value).ForEach(elem => bytes.Add(DecodeByte(elem)));
        return bytes.ToArray();
    }

    public JsonElement EncodeIntArray(int[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var i in array)
        {
            list.Add(EncodeInt(i));
        }
        return list.Build();
    }

    public int[] DecodeIntArray(JsonElement value)
    {
        var ints = new List<int>();
        DecodeList(value).ForEach(elem => ints.Add(DecodeInt(elem)));
        return ints.ToArray();
    }

    public JsonElement EncodeLongArray(long[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var l in array)
        {
            list.Add(EncodeLong(l));
        }
        return list.Build();
    }

    public long[] DecodeLongArray(JsonElement value)
    {
        var longs = new List<long>();
        DecodeList(value).ForEach(elem => longs.Add(DecodeLong(elem)));
        return longs.ToArray();
    }

    public IListBuilder<JsonElement> EncodeList(int size)
    {
        var list = new JsonArray();
        return new JsonListBuilder(list);
    }

    public List<JsonElement> DecodeList(JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("JsonElement is not JsonArray");
        }

        // Return a cloned list of elements preserving their structure (objects, arrays, values)
        return value.EnumerateArray().Select(e => e.Clone()).ToList();
    }

    public IVirtualMapBuilder<JsonElement> EncodeMap()
    {
        var jsonObject = new JsonObject();
        return new JsonVirtualMapBuilder(jsonObject);
    }

    public IVirtualMap<JsonElement> DecodeMap(JsonElement value)
    {
        return new JsonVirtualMap(value);
    }
}