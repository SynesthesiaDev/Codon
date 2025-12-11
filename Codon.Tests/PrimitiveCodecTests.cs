using Codon.Codec;
using Codon.Codec.Transcoder.Transcoders;

namespace Codon.Tests;

public class PrimitiveCodecTests
{
    private readonly JsonTranscoder _t = JsonTranscoder.Instance;

    private static void RoundTrip<T>(ICodec<T> codec, T value)
    {
        var encoded = codec.Encode(JsonTranscoder.Instance, value);
        var decoded = codec.Decode(JsonTranscoder.Instance, encoded);
        Assert.That(decoded, Is.EqualTo(value));
    }

    [Test]
    public void Bool_RoundTrip() => RoundTrip(Codecs.Boolean, true);

    [Test]
    public void Byte_RoundTrip() => RoundTrip(Codecs.Byte, (byte)123);

    [Test]
    public void Short_RoundTrip() => RoundTrip(Codecs.Short, (short)-456);

    [Test]
    public void Int_RoundTrip() => RoundTrip(Codecs.Int, 123456);

    [Test]
    public void Long_RoundTrip() => RoundTrip(Codecs.Long, 1234567890123L);

    [Test]
    public void Float_RoundTrip() => RoundTrip(Codecs.Float, 123.5f);

    [Test]
    public void Double_RoundTrip() => RoundTrip(Codecs.Double, -9876.4321);

    [Test]
    public void String_RoundTrip() => RoundTrip(Codecs.String, "hello world");

    [Test]
    public void ByteArray_RoundTrip()
    {
        var value = new byte[] { 0, 1, 2, 255 };
        RoundTrip(Codecs.ByteArray, value);
    }

    [Test]
    public void IntArray_RoundTrip()
    {
        var value = new[] { -1, 0, 1, int.MaxValue };
        RoundTrip(Codecs.IntArray, value);
    }

    [Test]
    public void LongArray_RoundTrip()
    {
        var value = new[] { -1L, 0L, 1L, long.MaxValue };
        RoundTrip(Codecs.LongArray, value);
    }
}
