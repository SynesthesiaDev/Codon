using System.Text.Json;
using Codon.Codec.Json;
using Codon.Codec.Transcoder;

namespace Codon.Tests;

public class TranscoderArrayHelpersTests
{
    private readonly ITranscoder<JsonElement> t = new JsonTranscoder();

    [Test]
    public void EncodeDecode_ByteArray_RoundTrip()
    {
        var input = new byte[] { 0, 1, 2, 3, 254, 255 };
        var encoded = t.EncodeByteArray(input);
        Assert.That(encoded.ValueKind, Is.EqualTo(JsonValueKind.Array));
        var decoded = t.DecodeByteArray(encoded);
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void EncodeDecode_IntArray_RoundTrip()
    {
        var input = new[] { -123, 0, 1, 2, int.MaxValue, int.MinValue };
        var encoded = t.EncodeIntArray(input);
        Assert.That(encoded.ValueKind, Is.EqualTo(JsonValueKind.Array));
        var decoded = t.DecodeIntArray(encoded);
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void EncodeDecode_LongArray_RoundTrip()
    {
        var input = new[] { -123L, 0L, 1L, 2L, long.MaxValue, long.MinValue };
        var encoded = t.EncodeLongArray(input);
        Assert.That(encoded.ValueKind, Is.EqualTo(JsonValueKind.Array));
        var decoded = t.DecodeLongArray(encoded);
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void EmptyMap_ReturnsEmptyJsonObject()
    {
        var empty = t.EmptyMap();
        Assert.That(empty.ValueKind, Is.EqualTo(JsonValueKind.Object));
        Assert.That(empty.EnumerateObject().Count(), Is.EqualTo(0));
    }
}
