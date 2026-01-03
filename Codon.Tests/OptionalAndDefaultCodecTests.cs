using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Transcoder.Transcoders;
using Codon.Optionals;

namespace Codon.Tests;

public class OptionalAndDefaultCodecTests
{
    private readonly JsonTranscoder _t = JsonTranscoder.Instance;

    [Test]
    public void Optional_Present_RoundTrip()
    {
        var codec = Codecs.Int.Optional();
        var value = Optional.Of(42);
        var encoded = codec.Encode(_t, value);
        var decoded = codec.Decode(_t, encoded);
        Assert.That(decoded.IsPresent, Is.True);
        Assert.That(decoded.Value, Is.EqualTo(42));
    }

    private record OptHolder(int id, Optional<int> oi)
    {
        public static readonly StructCodec<OptHolder> Codec = StructCodec.Of(
            "id", Codecs.Int, h => h.id,
            "oi", Codecs.Int.Optional(), h => h.oi,
            (id, oi) => new OptHolder(id, oi)
        );
    }

    [Test]
    public void Optional_Missing_Field_IsEmpty()
    {
        var json = JsonDocument.Parse("{\"id\": 1}" ).RootElement;
        var decoded = OptHolder.Codec.Decode(_t, json);
        Assert.That(decoded.oi.IsMissing, Is.True);
    }

    private record DefHolder(int id, int x)
    {
        public static readonly StructCodec<DefHolder> Codec = StructCodec.Of(
            "id", Codecs.Int, h => h.id,
            "x", Codecs.Int.Default(5), h => h.x,
            (id, x) => new DefHolder(id, x)
        );
    }

    [Test]
    public void Default_Missing_Field_UsesDefault()
    {
        var json = JsonDocument.Parse("{\"id\": 7}").RootElement;
        var decoded = DefHolder.Codec.Decode(_t, json);
        Assert.That(decoded.x, Is.EqualTo(5));
    }

    [Test]
    public void Default_Present_Field_OverridesDefault()
    {
        var json = JsonDocument.Parse("{\"id\": 9, \"x\":123}").RootElement;
        var decoded = DefHolder.Codec.Decode(_t, json);
        Assert.That(decoded.x, Is.EqualTo(123));
    }
}
