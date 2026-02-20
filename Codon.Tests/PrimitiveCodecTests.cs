using Codon.Codec;
using Codon.Codec.Json;

namespace Codon.Tests;

public class PrimitiveCodecTests
{
    private readonly JsonTranscoder _t = JsonTranscoder.INSTANCE;

    private static void RoundTrip<T>(Codec<T> codec, T value)
    {
        var encoded = codec.Encode(JsonTranscoder.INSTANCE, value);
        var decoded = codec.Decode(JsonTranscoder.INSTANCE, encoded);
        Assert.That(decoded, Is.EqualTo(value));
    }

    [Test]
    public void Bool_RoundTrip() => RoundTrip(Codecs.BOOLEAN, true);

    [Test]
    public void Byte_RoundTrip() => RoundTrip(Codecs.BYTE, (byte)123);

    [Test]
    public void Short_RoundTrip() => RoundTrip(Codecs.SHORT, (short)-456);

    [Test]
    public void Int_RoundTrip() => RoundTrip(Codecs.INT, 123456);

    [Test]
    public void Long_RoundTrip() => RoundTrip(Codecs.LONG, 1234567890123L);

    [Test]
    public void Float_RoundTrip() => RoundTrip(Codecs.FLOAT, 123.5f);

    [Test]
    public void Double_RoundTrip() => RoundTrip(Codecs.DOUBLE, -9876.4321);

    [Test]
    public void String_RoundTrip() => RoundTrip(Codecs.STRING, "hello world");

    [Test]
    public void ByteArray_RoundTrip()
    {
        var value = new byte[] { 0, 1, 2, 255 };
        RoundTrip(Codecs.BYTE_ARRAY, value);
    }

    [Test]
    public void IntArray_RoundTrip()
    {
        var value = new[] { -1, 0, 1, int.MaxValue };
        RoundTrip(Codecs.INT_ARRAY, value);
    }

    [Test]
    public void LongArray_RoundTrip()
    {
        var value = new[] { -1L, 0L, 1L, long.MaxValue };
        RoundTrip(Codecs.LONG_ARRAY, value);
    }
}
