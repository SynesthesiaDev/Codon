using Codon.Codec;
using Codon.Codec.Json;

namespace Codon.Tests;

public class PrimitiveCodecTests
{
    private readonly JsonTranscoder t = JsonTranscoder.INSTANCE;

    private static void roundTrip<T>(Codec<T> codec, T value)
    {
        var encoded = codec.Encode(JsonTranscoder.INSTANCE, value);
        var decoded = codec.Decode(JsonTranscoder.INSTANCE, encoded);
        Assert.That(decoded, Is.EqualTo(value));
    }

    [Test]
    public void Bool_RoundTrip() => roundTrip(Codecs.BOOLEAN, true);

    [Test]
    public void Byte_RoundTrip() => roundTrip(Codecs.BYTE, (byte)123);

    [Test]
    public void Short_RoundTrip() => roundTrip(Codecs.SHORT, (short)-456);

    [Test]
    public void Int_RoundTrip() => roundTrip(Codecs.INT, 123456);

    [Test]
    public void Long_RoundTrip() => roundTrip(Codecs.LONG, 1234567890123L);

    [Test]
    public void Float_RoundTrip() => roundTrip(Codecs.FLOAT, 123.5f);

    [Test]
    public void Double_RoundTrip() => roundTrip(Codecs.DOUBLE, -9876.4321);

    [Test]
    public void String_RoundTrip() => roundTrip(Codecs.STRING, "hello world");

    [Test]
    public void ByteArray_RoundTrip()
    {
        var value = new byte[] { 0, 1, 2, 255 };
        roundTrip(Codecs.BYTE_ARRAY, value);
    }

    [Test]
    public void IntArray_RoundTrip()
    {
        var value = new[] { -1, 0, 1, int.MaxValue };
        roundTrip(Codecs.INT_ARRAY, value);
    }

    [Test]
    public void LongArray_RoundTrip()
    {
        var value = new[] { -1L, 0L, 1L, long.MaxValue };
        roundTrip(Codecs.LONG_ARRAY, value);
    }
}
