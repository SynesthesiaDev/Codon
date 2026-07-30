using Codon.Codec;
using Codon.Codec.Json;
using Codon.Optionals;

namespace Codon.Tests;

public class OptionalAndDefaultCodecTests
{
    private readonly JsonTranscoder t = JsonTranscoder.INSTANCE;

    [Test]
    public void Optional_Present_RoundTrip()
    {
        var codec = Codecs.INT.Optional();
        var value = Optional.Of(42);
        var encoded = codec.Encode(t, value);
        var decoded = codec.Decode(t, encoded);
        Assert.That(decoded.IsPresent, Is.True);
        Assert.That(decoded.Value, Is.EqualTo(42));
    }

    private record OptHolder(int Id, Optional<int> Oi)
    {
        public static readonly StructCodec<OptHolder> CODEC = StructCodec.For<OptHolder>()
            .Field("id", Codecs.INT, h => h.Id)
            .Field("oi", Codecs.INT.Optional(), h => h.Oi)
            .Build((id, oi) => new OptHolder(id, oi));
    }

    [Test]
    public void Optional_Missing_Field_IsEmpty()
    {
        var json = "{\"id\": 1}".ToJson();
        var decoded = OptHolder.CODEC.Decode(t, json);
        Assert.That(decoded.Oi.IsMissing, Is.True);
    }

    private record DefHolder(int Id, int X)
    {
        public static readonly StructCodec<DefHolder> CODEC = StructCodec.For<DefHolder>()
            .Field("id", Codecs.INT, h => h.Id)
            .Field("x", Codecs.INT.Default(5), h => h.X)
            .Build((id, x) => new DefHolder(id, x));
    }

    [Test]
    public void Default_Missing_Field_UsesDefault()
    {
        var json = "{\"id\": 7}".ToJson();
        var decoded = DefHolder.CODEC.Decode(t, json);
        Assert.That(decoded.X, Is.EqualTo(5));
    }

    [Test]
    public void Default_Present_Field_OverridesDefault()
    {
        var json = "{\"id\": 9, \"x\":123}".ToJson();
        var decoded = DefHolder.CODEC.Decode(t, json);
        Assert.That(decoded.X, Is.EqualTo(123));
    }
}
