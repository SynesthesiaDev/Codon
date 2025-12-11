using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Transcoder.Transcoders;
using Codon.Codec.Transcoder;

namespace Codon.Tests;

public class ListMapEnumAndMoreCodecTests
{
    private readonly JsonTranscoder _t = JsonTranscoder.Instance;

    [Test]
    public void List_Primitive_RoundTrip()
    {
        var codec = Codecs.Int.List();
        var list = new List<int> { 1, 2, 3, 4 };
        var encoded = codec.Encode(_t, list);
        var decoded = codec.Decode(_t, encoded);
        Assert.That(decoded, Is.EqualTo(list));
    }

    [Test]
    public void List_Empty_RoundTrip()
    {
        var codec = Codecs.String.List();
        var list = new List<string>();
        var encoded = codec.Encode(_t, list);
        var decoded = codec.Decode(_t, encoded);
        Assert.That(decoded, Is.Empty);
    }

    [Test]
    public void Map_Primitive_RoundTrip()
    {
        var codec = Codecs.String.MapTo(Codecs.Int);
        var map = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
        var encoded = codec.Encode(_t, map);
        var decoded = codec.Decode(_t, encoded);
        Assert.That(decoded, Is.EqualTo(map));
    }

    [Test]
    public void Map_Empty_RoundTrip()
    {
        var codec = Codecs.String.MapTo(Codecs.String);
        var map = new Dictionary<string, string>();
        var encoded = codec.Encode(_t, map);
        var decoded = codec.Decode(_t, encoded);
        Assert.That(decoded, Is.Empty);
    }

    private enum Color { Red, Green, Blue }

    [Test]
    public void Enum_RoundTrip()
    {
        var codec = Codecs.Enum<Color>();
        var encoded = codec.Encode(_t, Color.Green);
        var decoded = codec.Decode(_t, encoded);
        Assert.That(decoded, Is.EqualTo(Color.Green));
    }

    [Test]
    public void TransformativeCodec_RoundTrip()
    {
        // Store an int via string inner codec using transform int<->string
        var intAsStringCodec = Codecs.String.Transform<int>(
            to: s => int.Parse(s),
            from: i => i.ToString()
        );
        var value = 12345;
        var encoded = intAsStringCodec.Encode(_t, value);
        var decoded = intAsStringCodec.Decode(_t, encoded);
        Assert.That(decoded, Is.EqualTo(value));
    }

    private abstract record Shape;
    private record Rect(int w, int h) : Shape;

    private static readonly StructCodec<Rect> RectCodec = StructCodec.Of(
        "w", Codecs.Int, r => r.w,
        "h", Codecs.Int, r => r.h,
        (w, h) => new Rect(w, h)
    );

    private enum Kind { Rect }

    private class UpcastStructCodec<R, V> : StructCodec<R>
    {
        private readonly StructCodec<V> _inner;
        private readonly Func<R, V> _down;
        private readonly Func<V, R> _up;

        public UpcastStructCodec(StructCodec<V> inner, Func<R, V> down, Func<V, R> up)
        {
            _inner = inner;
            _down = down;
            _up = up;
        }

        public override T EncodeToMap<T>(ITranscoder<T> transcoder, R value, IVirtualMapBuilder<T> mapBuilder)
        {
            return _inner.EncodeToMap<T>(transcoder, _down(value), mapBuilder);
        }

        public override R DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var v = _inner.DecodeFromMap<T>(transcoder, map);
            return _up(v);
        }
    }

    private static readonly StructCodec<Shape> ShapeCodec = ((ICodec<Kind>)Codecs.Enum<Kind>()).Union<Shape>(
        keyField: "kind",
        serializers: kind => kind switch
        {
            Kind.Rect => new UpcastStructCodec<Shape, Rect>(RectCodec, r => (Rect)r, v => v),
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
        var encoded = ShapeCodec.Encode(_t, shape);
        var map = _t.DecodeMap(encoded);
        Assert.That(map.HasValue("kind"), Is.True);
        Assert.That(_t.DecodeString(map.GetValue("kind")), Is.EqualTo("Rect"));
        var decoded = ShapeCodec.Decode(_t, encoded);
        Assert.That(decoded, Is.EqualTo(shape));
    }

    [Test]
    public void ForwardRefCodec_Delegates()
    {
        var inner = Codecs.Int;
        var forward = inner.ForwardRef();
        var val = 77;
        var enc = forward.Encode(_t, val);
        var dec = forward.Decode(_t, enc);
        Assert.That(dec, Is.EqualTo(val));
    }

    private record Nested(string name, List<int> nums, Dictionary<string, int> map);

    private static readonly StructCodec<Nested> NestedCodec = StructCodec.Of(
        "name", Codecs.String, n => n.name,
        "nums", Codecs.Int.List(), n => n.nums,
        "map", Codecs.String.MapTo(Codecs.Int), n => n.map,
        (name, nums, map) => new Nested(name, nums, map)
    );

    [Test]
    public void Nested_StructCodec_RoundTrip()
    {
        var n = new Nested(
            "hi",
            new List<int> { 1, 2, 3 },
            new Dictionary<string, int> { { "a", 1 }, { "b", 2 } }
        );
        var enc = NestedCodec.Encode(_t, n);
        var dec = NestedCodec.Decode(_t, enc);
        Assert.That(dec.name, Is.EqualTo(n.name));
        CollectionAssert.AreEqual(n.nums, dec.nums);
        Assert.That(dec.map.Count, Is.EqualTo(n.map.Count));
        foreach (var kv in n.map)
        {
            Assert.That(dec.map.ContainsKey(kv.Key), Is.True);
            Assert.That(dec.map[kv.Key], Is.EqualTo(kv.Value));
        }
    }
}
