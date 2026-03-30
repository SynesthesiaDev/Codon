using Codon.IniTranscoder.Elements;
using Codon.IniTranscoder.Exceptions;

namespace Codon.Tests;

public class IniTranscoderTests
{
    private IniTranscoder.IniTranscoder t => IniTranscoder.IniTranscoder.INSTANCE;

    [Test]
    public void EncodeNull_ProducesNullValue()
    {
        var e = t.EncodeNull();
        Assert.That(e, Is.InstanceOf<IniValue>());
        Assert.That(((IniValue)e).Value, Is.Null);
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

        const string s = "hello ini 😊";
        Assert.That(t.DecodeString(t.EncodeString(s)), Is.EqualTo(s));
    }

    [Test]
    public void List_EncodeDecode_RoundTrip()
    {
        var builder = t.EncodeList(3);
        builder.Add(t.EncodeInt(1)).Add(t.EncodeInt(2)).Add(t.EncodeInt(3));
        var encoded = builder.Build();
        Assert.That(encoded, Is.InstanceOf<IniList>());

        var list = t.DecodeList(encoded);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list.Select(t.DecodeInt));
    }

    [Test]
    public void Map_EncodeDecode_Basics()
    {
        var mapEncoded = t.EncodeMap()
            .Put("a", t.EncodeInt(1))
            .Put("b", t.EncodeString("x"))
            .Build();

        Assert.That(mapEncoded, Is.InstanceOf<IniSection>());

        var map = t.DecodeMap(mapEncoded);
        var keys = map.GetKeys();
        CollectionAssert.AreEquivalent(new[] { "a", "b" }, keys);
        Assert.That(map.HasValue("a"), Is.True);
        Assert.That(map.HasValue("b"), Is.True);
        Assert.That(t.DecodeInt(map.GetValue("a")), Is.EqualTo(1));
        Assert.That(t.DecodeString(map.GetValue("b")), Is.EqualTo("x"));
    }

    [Test]
    public void Map_WithSectionName()
    {
        var mapEncoded = t.Named("MySection").EncodeMap()
            .Put("key", t.EncodeString("value"))
            .Build();

        Assert.That(mapEncoded, Is.InstanceOf<IniSection>());
        var section = (IniSection)mapEncoded;
        Assert.That(section.Name, Is.EqualTo("MySection"));
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
    public void DecodeList_OnNonList_Throws()
    {
        var notList = t.EncodeInt(5);
        Assert.Throws<ParsingException>(() => t.DecodeList(notList));
    }

    [Test]
    public void DecodeMap_OnNonSection_Throws()
    {
        var notSection = t.EncodeInt(5);
        Assert.Throws<ParsingException>(() => t.DecodeMap(notSection));
    }

    [Test]
    public void NestedStructures_RoundTrip()
    {
        // List of Lists
        var innerList1 = t.EncodeList(2).Add(t.EncodeInt(1)).Add(t.EncodeInt(2)).Build();
        var innerList2 = t.EncodeList(2).Add(t.EncodeInt(3)).Add(t.EncodeInt(4)).Build();
        var outerList = t.EncodeList(2).Add(innerList1).Add(innerList2).Build();

        var decodedOuter = t.DecodeList(outerList);
        Assert.That(decodedOuter.Count, Is.EqualTo(2));
        
        var decodedInner1 = t.DecodeList(decodedOuter[0]);
        CollectionAssert.AreEqual(new[] { 1, 2 }, decodedInner1.Select(t.DecodeInt));

        var decodedInner2 = t.DecodeList(decodedOuter[1]);
        CollectionAssert.AreEqual(new[] { 3, 4 }, decodedInner2.Select(t.DecodeInt));

        // Map containing List
        var mapEncoded = t.EncodeMap()
            .Put("list", innerList1)
            .Build();
        
        var decodedMap = t.DecodeMap(mapEncoded);
        Assert.That(decodedMap.HasValue("list"), Is.True);
        var decodedListFromMap = t.DecodeList(decodedMap.GetValue("list"));
        CollectionAssert.AreEqual(new[] { 1, 2 }, decodedListFromMap.Select(t.DecodeInt));
    }
}
