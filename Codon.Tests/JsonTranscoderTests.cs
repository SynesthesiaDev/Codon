using System.Text.Json;
using System.Text.Json.Nodes;
using Codon.Codec.Transcoder;
using Codon.Codec.Transcoder.Transcoders;

namespace Codon.Tests;

public class JsonTranscoderTests
{
    private JsonTranscoder T => new();

    [Test]
    public void EncodeNull_ProducesNullJson()
    {
        var e = T.EncodeNull();
        Assert.That(e.ValueKind, Is.EqualTo(JsonValueKind.Null));
    }

    [Test]
    public void Primitives_RoundTrip()
    {
        Assert.That(T.DecodeBool(T.EncodeBool(true)), Is.True);
        Assert.That(T.DecodeBool(T.EncodeBool(false)), Is.False);

        Assert.That(T.DecodeByte(T.EncodeByte(200)), Is.EqualTo((byte)200));
        Assert.That(T.DecodeShort(T.EncodeShort(-12345)), Is.EqualTo((short)-12345));
        Assert.That(T.DecodeInt(T.EncodeInt(int.MaxValue)), Is.EqualTo(int.MaxValue));
        Assert.That(T.DecodeInt(T.EncodeInt(int.MinValue)), Is.EqualTo(int.MinValue));
        Assert.That(T.DecodeLong(T.EncodeLong(long.MinValue)), Is.EqualTo(long.MinValue));
        Assert.That(T.DecodeLong(T.EncodeLong(long.MaxValue)), Is.EqualTo(long.MaxValue));

        Assert.That(T.DecodeFloat(T.EncodeFloat(3.14f)), Is.EqualTo(3.14f).Within(1e-6));
        Assert.That(T.DecodeDouble(T.EncodeDouble(-1.2345)), Is.EqualTo(-1.2345).Within(1e-12));

        const string s = "hello \"json\" 😊";
        Assert.That(T.DecodeString(T.EncodeString(s)), Is.EqualTo(s));
    }

    [Test]
    public void List_EncodeDecode_RoundTrip()
    {
        var builder = T.EncodeList(3);
        builder.Add(T.EncodeInt(1)).Add(T.EncodeInt(2)).Add(T.EncodeInt(3));
        var encoded = builder.Build();
        Assert.That(encoded.ValueKind, Is.EqualTo(JsonValueKind.Array));

        var list = T.DecodeList(encoded);
        CollectionAssert.AreEqual(new[] {1,2,3}, list.Select(T.DecodeInt));
    }

    [Test]
    public void Map_EncodeDecode_Basics()
    {
        var mapEncoded = T.EncodeMap()
            .Put("a", T.EncodeInt(1))
            .Put("b", T.EncodeString("x"))
            .Build();

        Assert.That(mapEncoded.ValueKind, Is.EqualTo(JsonValueKind.Object));

        var map = T.DecodeMap(mapEncoded);
        var keys = map.GetKeys();
        CollectionAssert.AreEquivalent(new[] {"a", "b"}, keys);
        Assert.That(map.HasValue("a"), Is.True);
        Assert.That(map.HasValue("b"), Is.True);
        Assert.That(T.DecodeInt(map.GetValue("a")), Is.EqualTo(1));
        Assert.That(T.DecodeString(map.GetValue("b")), Is.EqualTo("x"));
    }

    [Test]
    public void Map_Put_WithElementKey_UsesStringValue()
    {
        var keyElement = T.EncodeString("k");
        var encoded = T.EncodeMap().Put(keyElement, T.EncodeBool(true)).Build();
        var map = T.DecodeMap(encoded);
        Assert.That(map.HasValue("k"), Is.True);
        Assert.That(T.DecodeBool(map.GetValue("k")), Is.True);
    }

    [Test]
    public void DecodeList_OnNonArray_Throws()
    {
        var notArray = T.EncodeInt(5);
        Assert.Throws<InvalidOperationException>(() => T.DecodeList(notArray));
    }

    [Test]
    public void DecodeMap_View_OnNonObject_Throws()
    {
        var notObject = T.EncodeInt(5);
        var view = T.DecodeMap(notObject);
        Assert.Throws<InvalidOperationException>(() => view.GetKeys());
        Assert.Throws<InvalidOperationException>(() => view.HasValue("x"));
        Assert.Throws<InvalidOperationException>(() => view.GetValue("x"));
    }

    [Test]
    public void DecodeList_ElementsRemainJsonElements()
    {
        var arr = T.EncodeList(2).Add(T.EncodeString("a")).Add(T.EncodeInt(2)).Build();
        var list = T.DecodeList(arr);
        Assert.That(list[0].ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(list[1].ValueKind, Is.EqualTo(JsonValueKind.Number));
    }
}
