using Codon.Codec;
using Codon.Codec.Json;
using Codon.Codec.Transcoder;

namespace Codon.Tests;

public class ListMapEnumAndMoreCodecTests
{
    private readonly JsonTranscoder t = JsonTranscoder.INSTANCE;

    [Test]
    public void List_Primitive_RoundTrip()
    {
        var codec = Codecs.INT.List();
        var list = new List<int> { 1, 2, 3, 4 };
        var encoded = codec.Encode(t, list);
        var decoded = codec.Decode(t, encoded);
        Assert.That(decoded, Is.EqualTo(list));
    }

    [Test]
    public void List_Empty_RoundTrip()
    {
        var codec = Codecs.STRING.List();
        var list = new List<string>();
        var encoded = codec.Encode(t, list);
        var decoded = codec.Decode(t, encoded);
        Assert.That(decoded, Is.Empty);
    }

    [Test]
    public void Map_Primitive_RoundTrip()
    {
        var codec = Codecs.STRING.MapTo(Codecs.INT);
        var map = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
        var encoded = codec.Encode(t, map);
        var decoded = codec.Decode(t, encoded);
        Assert.That(decoded, Is.EqualTo(map));
    }

    [Test]
    public void Map_Empty_RoundTrip()
    {
        var codec = Codecs.STRING.MapTo(Codecs.STRING);
        var map = new Dictionary<string, string>();
        var encoded = codec.Encode(t, map);
        var decoded = codec.Decode(t, encoded);
        Assert.That(decoded, Is.Empty);
    }

    private enum Color { Red, Green, Blue }

    [Test]
    public void Enum_RoundTrip()
    {
        var codec = Codecs.Enum<Color>();
        var encoded = codec.Encode(t, Color.Green);
        var decoded = codec.Decode(t, encoded);
        Assert.That(decoded, Is.EqualTo(Color.Green));
    }

    [Test]
    public void TransformativeCodec_RoundTrip()
    {
        // Store an int via string inner codec using transform int<->string
        var intAsStringCodec = Codecs.STRING.Transform<int>(
            to: s => int.Parse(s),
            from: i => i.ToString()
        );
        var value = 12345;
        var encoded = intAsStringCodec.Encode(t, value);
        var decoded = intAsStringCodec.Decode(t, encoded);
        Assert.That(decoded, Is.EqualTo(value));
    }

    private abstract record Shape;

    private record Rect(int W, int H) : Shape;

    private static readonly StructCodec<Rect> rect_codec = StructCodec.For<Rect>()
        .Field("w", Codecs.INT, r => r.W)
        .Field("h", Codecs.INT, r => r.H)
        .Build((w, h) => new Rect(w, h));

    private enum Kind { Rect }

    private class UpcastStructCodec<R, V> : StructCodec<R>
    {
        private readonly StructCodec<V> inner;
        private readonly Func<R, V> down;
        private readonly Func<V, R> up;

        public UpcastStructCodec(StructCodec<V> inner, Func<R, V> down, Func<V, R> up)
        {
            this.inner = inner;
            this.down = down;
            this.up = up;
        }

        public override T EncodeToMap<T>(ITranscoder<T> transcoder, R value, IVirtualMapBuilder<T> mapBuilder)
        {
            return inner.EncodeToMap<T>(transcoder, down(value), mapBuilder);
        }

        public override R DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var v = inner.DecodeFromMap<T>(transcoder, map);
            return up(v);
        }
    }

    private static readonly StructCodec<Shape> shape_codec = Codecs.Enum<Kind>().Union<Shape>(
        keyField: "kind",
        serializers: kind => kind switch
        {
            Kind.Rect => new UpcastStructCodec<Shape, Rect>(rect_codec, r => (Rect)r, v => v),
            _ => throw new InvalidOperationException()
        },
        keyFunc: shape => shape switch
        {
            Rect => Kind.Rect,
            _ => throw new InvalidOperationException()
        }
    );

    [Test]
    public void UnionCodec_EncodeAddsDiscriminator_AndRoundTrip_Works()
    {
        Shape shape = new Rect(3, 4);
        var encoded = shape_codec.Encode(t, shape);
        var map = t.DecodeMap(encoded);
        Assert.That(map.HasValue("kind"), Is.True);
        Assert.That(t.DecodeString(map.GetValue("kind")), Is.EqualTo("Rect"));
        var decoded = shape_codec.Decode(t, encoded);
        Assert.That(decoded, Is.EqualTo(shape));
    }

    [Test]
    public void ForwardRefCodec_Delegates()
    {
        var inner = Codecs.INT;
        var forward = inner.ForwardRef();
        var val = 77;
        var enc = forward.Encode(t, val);
        var dec = forward.Decode(t, enc);
        Assert.That(dec, Is.EqualTo(val));
    }

    private record Nested(string Name, List<int> Nums, Dictionary<string, int> Map);

    private static readonly StructCodec<Nested> nested_codec = StructCodec.For<Nested>()
        .Field("name", Codecs.STRING, n => n.Name)
        .Field("nums", Codecs.INT.List(), n => n.Nums)
        .Field("map", Codecs.STRING.MapTo(Codecs.INT), n => n.Map)
        .Build((name, nums, map) => new Nested(name, nums, map));

    [Test]
    public void Nested_StructCodec_RoundTrip()
    {
        var n = new Nested(
            "hi",
            new List<int> { 1, 2, 3 },
            new Dictionary<string, int> { { "a", 1 }, { "b", 2 } }
        );
        var enc = nested_codec.Encode(t, n);
        var dec = nested_codec.Decode(t, enc);
        Assert.That(dec.Name, Is.EqualTo(n.Name));
        CollectionAssert.AreEqual(n.Nums, dec.Nums);
        Assert.That(dec.Map.Count, Is.EqualTo(n.Map.Count));
        foreach (var kv in n.Map)
        {
            Assert.That(dec.Map.ContainsKey(kv.Key), Is.True);
            Assert.That(dec.Map[kv.Key], Is.EqualTo(kv.Value));
        }
    }
}
