using Codon.IniTranscoder.Elements;
using Codon.IniTranscoder.Exceptions;

namespace Codon.Tests;

public class IniTranscoderTests
{
    private IniTranscoder.IniTranscoder T => IniTranscoder.IniTranscoder.Instance;

    [Test]
    public void EncodeNull_ProducesNullValue()
    {
        var e = T.EncodeNull();
        Assert.That(e, Is.InstanceOf<IniValue>());
        Assert.That(((IniValue)e).Value, Is.Null);
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

        const string s = "hello ini 😊";
        Assert.That(T.DecodeString(T.EncodeString(s)), Is.EqualTo(s));
    }

    [Test]
    public void List_EncodeDecode_RoundTrip()
    {
        var builder = T.EncodeList(3);
        builder.Add(T.EncodeInt(1)).Add(T.EncodeInt(2)).Add(T.EncodeInt(3));
        var encoded = builder.Build();
        Assert.That(encoded, Is.InstanceOf<IniList>());

        var list = T.DecodeList(encoded);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list.Select(T.DecodeInt));
    }

    [Test]
    public void Map_EncodeDecode_Basics()
    {
        var mapEncoded = T.EncodeMap()
            .Put("a", T.EncodeInt(1))
            .Put("b", T.EncodeString("x"))
            .Build();

        Assert.That(mapEncoded, Is.InstanceOf<IniSection>());

        var map = T.DecodeMap(mapEncoded);
        var keys = map.GetKeys();
        CollectionAssert.AreEquivalent(new[] { "a", "b" }, keys);
        Assert.That(map.HasValue("a"), Is.True);
        Assert.That(map.HasValue("b"), Is.True);
        Assert.That(T.DecodeInt(map.GetValue("a")), Is.EqualTo(1));
        Assert.That(T.DecodeString(map.GetValue("b")), Is.EqualTo("x"));
    }

    [Test]
    public void Map_WithSectionName()
    {
        var mapEncoded = T.Named("MySection").EncodeMap()
            .Put("key", T.EncodeString("value"))
            .Build();

        Assert.That(mapEncoded, Is.InstanceOf<IniSection>());
        var section = (IniSection)mapEncoded;
        Assert.That(section.Name, Is.EqualTo("MySection"));
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
    public void DecodeList_OnNonList_Throws()
    {
        var notList = T.EncodeInt(5);
        Assert.Throws<ParsingException>(() => T.DecodeList(notList));
    }

    [Test]
    public void DecodeMap_OnNonSection_Throws()
    {
        var notSection = T.EncodeInt(5);
        Assert.Throws<ParsingException>(() => T.DecodeMap(notSection));
    }

    [Test]
    public void NestedStructures_RoundTrip()
    {
        // List of Lists
        var innerList1 = T.EncodeList(2).Add(T.EncodeInt(1)).Add(T.EncodeInt(2)).Build();
        var innerList2 = T.EncodeList(2).Add(T.EncodeInt(3)).Add(T.EncodeInt(4)).Build();
        var outerList = T.EncodeList(2).Add(innerList1).Add(innerList2).Build();

        var decodedOuter = T.DecodeList(outerList);
        Assert.That(decodedOuter.Count, Is.EqualTo(2));
        
        var decodedInner1 = T.DecodeList(decodedOuter[0]);
        CollectionAssert.AreEqual(new[] { 1, 2 }, decodedInner1.Select(T.DecodeInt));

        var decodedInner2 = T.DecodeList(decodedOuter[1]);
        CollectionAssert.AreEqual(new[] { 3, 4 }, decodedInner2.Select(T.DecodeInt));

        // Map containing List
        var mapEncoded = T.EncodeMap()
            .Put("list", innerList1)
            .Build();
        
        var decodedMap = T.DecodeMap(mapEncoded);
        Assert.That(decodedMap.HasValue("list"), Is.True);
        var decodedListFromMap = T.DecodeList(decodedMap.GetValue("list"));
        CollectionAssert.AreEqual(new[] { 1, 2 }, decodedListFromMap.Select(T.DecodeInt));
    }
}
