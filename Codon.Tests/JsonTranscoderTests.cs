using System.Text.Json;
using Codon.Codec.Json;

namespace Codon.Tests;

public class JsonTranscoderTests
{
    private JsonTranscoder t => new();

    [Test]
    public void EncodeNull_ProducesNullJson()
    {
        var e = t.EncodeNull();
        Assert.That(e.ValueKind, Is.EqualTo(JsonValueKind.Null));
    }

    [Test]
    public void Primitives_RoundTrip()
    {
        Assert.That(t.DecodeBool(t.EncodeBool(true)), Is.True);
        Assert.That(t.DecodeBool(t.EncodeBool(false)), Is.False);

        Assert.That(t.DecodeByte(t.EncodeByte(200)), Is.EqualTo((byte)200));
        Assert.That(t.DecodeShort(t.EncodeShort(-12345)), Is.EqualTo((short)-12345));
        Assert.That(t.DecodeInt(t.EncodeInt(int.MaxValue)), Is.EqualTo(int.MaxValue));
        Assert.That(t.DecodeInt(t.EncodeInt(int.MinValue)), Is.EqualTo(int.MinValue));
        Assert.That(t.DecodeLong(t.EncodeLong(long.MinValue)), Is.EqualTo(long.MinValue));
        Assert.That(t.DecodeLong(t.EncodeLong(long.MaxValue)), Is.EqualTo(long.MaxValue));

        Assert.That(t.DecodeFloat(t.EncodeFloat(3.14f)), Is.EqualTo(3.14f).Within(1e-6));
        Assert.That(t.DecodeDouble(t.EncodeDouble(-1.2345)), Is.EqualTo(-1.2345).Within(1e-12));

        const string s = "hello \"json\" 😊";
        Assert.That(t.DecodeString(t.EncodeString(s)), Is.EqualTo(s));
    }

    [Test]
    public void List_EncodeDecode_RoundTrip()
    {
        var builder = t.EncodeList(3);
        builder.Add(t.EncodeInt(1)).Add(t.EncodeInt(2)).Add(t.EncodeInt(3));
        var encoded = builder.Build();
        Assert.That(encoded.ValueKind, Is.EqualTo(JsonValueKind.Array));

        var list = t.DecodeList(encoded);
        CollectionAssert.AreEqual(new[] {1,2,3}, list.Select(t.DecodeInt));
    }

    [Test]
    public void Map_EncodeDecode_Basics()
    {
        var mapEncoded = t.EncodeMap()
            .Put("a", t.EncodeInt(1))
            .Put("b", t.EncodeString("x"))
            .Build();

        Assert.That(mapEncoded.ValueKind, Is.EqualTo(JsonValueKind.Object));

        var map = t.DecodeMap(mapEncoded);
        var keys = map.GetKeys();
        CollectionAssert.AreEquivalent(new[] {"a", "b"}, keys);
        Assert.That(map.HasValue("a"), Is.True);
        Assert.That(map.HasValue("b"), Is.True);
        Assert.That(t.DecodeInt(map.GetValue("a")), Is.EqualTo(1));
        Assert.That(t.DecodeString(map.GetValue("b")), Is.EqualTo("x"));
    }

    [Test]
    public void Map_Put_WithElementKey_UsesStringValue()
    {
        var keyElement = t.EncodeString("k");
        var encoded = t.EncodeMap().Put(keyElement, t.EncodeBool(true)).Build();
        var map = t.DecodeMap(encoded);
        Assert.That(map.HasValue("k"), Is.True);
        Assert.That(t.DecodeBool(map.GetValue("k")), Is.True);
    }

    [Test]
    public void DecodeList_OnNonArray_Throws()
    {
        var notArray = t.EncodeInt(5);
        Assert.Throws<InvalidOperationException>(() => t.DecodeList(notArray));
    }

    [Test]
    public void DecodeMap_View_OnNonObject_Throws()
    {
        var notObject = t.EncodeInt(5);
        var view = t.DecodeMap(notObject);
        Assert.Throws<InvalidOperationException>(() => view.GetKeys());
        Assert.Throws<InvalidOperationException>(() => view.HasValue("x"));
        Assert.Throws<InvalidOperationException>(() => view.GetValue("x"));
    }

    [Test]
    public void DecodeList_ElementsRemainJsonElements()
    {
        var arr = t.EncodeList(2).Add(t.EncodeString("a")).Add(t.EncodeInt(2)).Build();
        var list = t.DecodeList(arr);
        Assert.That(list[0].ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(list[1].ValueKind, Is.EqualTo(JsonValueKind.Number));
    }
}
