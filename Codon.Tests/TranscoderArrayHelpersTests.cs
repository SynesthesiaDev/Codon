using System.Text.Json;
using Codon.Codec.Transcoder;
using Codon.Codec.Transcoder.Transcoders;

namespace Codon.Tests;

public class TranscoderArrayHelpersTests
{
    private readonly ITranscoder<JsonElement> _t = new JsonTranscoder();

    [Test]
    public void EncodeDecode_ByteArray_RoundTrip()
    {
        var input = new byte[] { 0, 1, 2, 3, 254, 255 };
        var encoded = _t.EncodeByteArray(input);
        Assert.That(encoded.ValueKind, Is.EqualTo(JsonValueKind.Array));
        var decoded = _t.DecodeByteArray(encoded);
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void EncodeDecode_IntArray_RoundTrip()
    {
        var input = new[] { -123, 0, 1, 2, int.MaxValue, int.MinValue };
        var encoded = _t.EncodeIntArray(input);
        Assert.That(encoded.ValueKind, Is.EqualTo(JsonValueKind.Array));
        var decoded = _t.DecodeIntArray(encoded);
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void EncodeDecode_LongArray_RoundTrip()
    {
        var input = new[] { -123L, 0L, 1L, 2L, long.MaxValue, long.MinValue };
        var encoded = _t.EncodeLongArray(input);
        Assert.That(encoded.ValueKind, Is.EqualTo(JsonValueKind.Array));
        var decoded = _t.DecodeLongArray(encoded);
        Assert.That(decoded, Is.EqualTo(input));
    }

    [Test]
    public void EmptyMap_ReturnsEmptyJsonObject()
    {
        var empty = _t.EmptyMap();
        Assert.That(empty.ValueKind, Is.EqualTo(JsonValueKind.Object));
        Assert.That(empty.EnumerateObject().Count(), Is.EqualTo(0));
    }
}
