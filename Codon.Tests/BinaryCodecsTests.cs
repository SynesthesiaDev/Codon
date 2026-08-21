using Codon.Binary;
using Codon.Optionals;
using DotNetty.Buffers;

namespace Codon.Tests;

public class BinaryCodecsTests
{
    private static T roundTrip<T>(IBinaryCodec<T> codec, T value)
    {
        var buf = Unpooled.Buffer();
        codec.Write(buf, value!);
        return codec.Read(buf);
    }

    private record VeryEmptyClass
    {
        public static readonly IBinaryCodec<VeryEmptyClass> CODEC = BinaryCodecs.Empty(() => new VeryEmptyClass());
    }

    [Test]
    public void TestEmptyCodec()
    {
        var emptyClass = new VeryEmptyClass();
        var buf = Unpooled.Buffer();
        VeryEmptyClass.CODEC.Write(buf, emptyClass);
        var read = VeryEmptyClass.CODEC.Read(buf);
        Assert.That(read, Is.TypeOf<VeryEmptyClass>());
    }

    [Test]
    public void PrimitiveCodecs_RoundTrip()
    {
        Assert.That(roundTrip(BinaryCodecs.BOOLEAN, true), Is.True);
        Assert.That(roundTrip(BinaryCodecs.BOOLEAN, false), Is.False);

        Assert.That(roundTrip(BinaryCodecs.BYTE, (byte)0), Is.EqualTo((byte)0));
        Assert.That(roundTrip(BinaryCodecs.BYTE, (byte)255), Is.EqualTo((byte)255));

        Assert.That(roundTrip(BinaryCodecs.INT, 0), Is.EqualTo(0));
        Assert.That(roundTrip(BinaryCodecs.INT, 123456789), Is.EqualTo(123456789));
        Assert.That(roundTrip(BinaryCodecs.INT, -123456789), Is.EqualTo(-123456789));

        Assert.That(roundTrip(BinaryCodecs.LONG, 0L), Is.EqualTo(0L));
        Assert.That(roundTrip(BinaryCodecs.LONG, long.MaxValue), Is.EqualTo(long.MaxValue));
        Assert.That(roundTrip(BinaryCodecs.LONG, long.MinValue), Is.EqualTo(long.MinValue));

        Assert.That(roundTrip(BinaryCodecs.FLOAT, 123.456f), Is.EqualTo(123.456f));
        Assert.That(roundTrip(BinaryCodecs.DOUBLE, -123.456789), Is.EqualTo(-123.456789));
    }

    [Test]
    public void VarInt_RoundTrip_WithEdgeCases()
    {
        var values = new[] { 0, 1, 2, 127, 128, 129, 16384, int.MaxValue, -1, -2, int.MinValue };
        foreach (var v in values)
        {
            var buf = Unpooled.Buffer();
            BinaryCodecs.VAR_INT.Write(buf, v);
            var read = BinaryCodecs.VAR_INT.Read(buf);
            Assert.That(read, Is.EqualTo(v), $"VarInt roundtrip failed for {v}");
        }
    }

    [Test]
    public void Guid_RoundTrip()
    {
        var guid = Guid.NewGuid();
        var buf = Unpooled.Buffer();
        BinaryCodecs.GUID.Write(buf, guid);
        var read = BinaryCodecs.GUID.Read(buf);
        Assert.That(read, Is.EqualTo(guid));
    }

    [Test]
    public void Flags_RoundTrip()
    {
        const FlagsTest flags = FlagsTest.SoCool & FlagsTest.Gay;
        var buf = Unpooled.Buffer();
        BinaryCodecs.Flags<FlagsTest>().Write(buf, flags);
        var read = BinaryCodecs.Flags<FlagsTest>().Read(buf);
        Assert.That(read, Is.EqualTo(flags));
    }

    [Flags]
    enum FlagsTest
    {
        Testing = 0,
        Woah = 1,
        SoCool = 2,
        Flags = 4,
        Im = 8,
        So = 16,
        Gay = 32
    }

    [Test]
    public void ByteArray_And_RawBytes_RoundTrip()
    {
        var empty = Array.Empty<byte>();
        Assert.That(roundTrip(BinaryCodecs.BYTE_ARRAY, empty), Is.EqualTo(empty));

        var data = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
        Assert.That(roundTrip(BinaryCodecs.BYTE_ARRAY, data), Is.EqualTo(data));

        var buf = Unpooled.Buffer();
        BinaryCodecs.RAW_BYTES.Write(buf, data);
        var read = BinaryCodecs.RAW_BYTES.Read(buf);
        Assert.That(read, Is.EqualTo(data));
    }

    [Test]
    public void BinaryBuffer_Codec_RoundTrip()
    {
        var original = Unpooled.Buffer();
        original.WriteBytes("hello"u8.ToArray());
        original.WriteInt(42);

        var buf = Unpooled.Buffer();
        BinaryCodecs.BYTE_BUFFER.Write(buf, original);
        var round = BinaryCodecs.BYTE_BUFFER.Read(buf);

        Assert.That(round.ReadableBytes, Is.EqualTo(original.ReadableBytes));

        Assert.That(round.ReadByte(), Is.EqualTo((byte)'h'));
        Assert.That(round.ReadByte(), Is.EqualTo((byte)'e'));
        Assert.That(round.ReadByte(), Is.EqualTo((byte)'l'));
        Assert.That(round.ReadByte(), Is.EqualTo((byte)'l'));
        Assert.That(round.ReadByte(), Is.EqualTo((byte)'o'));
        Assert.That(round.ReadInt(), Is.EqualTo(42));
    }

    [Test]
    public void String_Codec_RoundTrip()
    {
        Assert.That(roundTrip(BinaryCodecs.STRING, string.Empty), Is.EqualTo(string.Empty));
        Assert.That(roundTrip(BinaryCodecs.STRING, "ascii"), Is.EqualTo("ascii"));
        var unicode = "你好世界 👋🌍";
        Assert.That(roundTrip(BinaryCodecs.STRING, unicode), Is.EqualTo(unicode));
    }

    [Test]
    public void Optional_Codec_WritesPresenceAndValue()
    {
        var optionalInt = BinaryCodecs.INT.Optional();

        var some = Optional.Of<int>(123);
        var bufSome = Unpooled.Buffer();
        optionalInt.Write(bufSome, some);
        var readSome = optionalInt.Read(bufSome);
        Assert.That(readSome.IsPresent);
        Assert.That(some.Value, Is.EqualTo(readSome.Value));

        var none = Optional.Empty<int>();
        var bufNone = Unpooled.Buffer();
        optionalInt.Write(bufNone, none);
        var readNone = optionalInt.Read(bufNone);
        Assert.That(readNone.IsMissing);
    }

    private enum TestEnum
    {
        A,
        B,
        C
    }

    [Test]
    public void Enum_Codec_UsesOrdinal()
    {
        var codec = BinaryCodecs.Enum<TestEnum>();
        Assert.That(roundTrip(codec, TestEnum.A), Is.EqualTo(TestEnum.A));
        Assert.That(roundTrip(codec, TestEnum.B), Is.EqualTo(TestEnum.B));
        Assert.That(roundTrip(codec, TestEnum.C), Is.EqualTo(TestEnum.C));
    }

    [Test]
    public void Transformative_Codec_RoundTrip()
    {
        var codec = BinaryCodecs.INT.Transform(
            from: s => s.Length,
            to: n => new string('x', n)
        );

        var s = "hello";
        var buf = Unpooled.Buffer();
        codec.Write(buf, s);
        var read = codec.Read(buf);
        Assert.That(read, Is.EqualTo(new string('x', s.Length)));
    }

    [Test]
    public void List_And_Dictionary_Codecs_RoundTrip()
    {
        var listCodec = BinaryCodecs.STRING.List();

        var list = new List<string> { "a", "b", "c" };
        Assert.That(roundTrip(listCodec, list), Is.EqualTo(list));

        var mapCodec = BinaryCodecs.INT.MapTo(BinaryCodecs.STRING);
        var dict = new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" } };
        var round = roundTrip(mapCodec, dict);
        Assert.That(round, Has.Count.EqualTo(3));
        foreach (var kv in dict) Assert.That(round[kv.Key], Is.EqualTo(kv.Value));
    }

    [Test]
    public void Union_Codec_RoundTrip_WithInt_KeyByte()
    {
        var union = new BinaryCodecDefinitions.UnionBinaryCodec<int, byte>(
            keyCodec: BinaryCodecs.BYTE,
            keyFunc: v => (byte)(v % 2),
            serializerFactory: _ => BinaryCodecs.INT
        );

        var round = roundTrip(union, 7);
        Assert.That(round, Is.EqualTo(7));
    }

    public record Person(string Name, int Age, bool Active);

    [Test]
    public void Composite_Codec_P3_RoundTrip()
    {
        var personCodec = BinaryCodecs.For<Person>()
            .Field(BinaryCodecs.STRING, p => p.Name)
            .Field(BinaryCodecs.INT, p => p.Age)
            .Field(BinaryCodecs.BOOLEAN, p => p.Active)
            .Build((name, age, active) => new Person(name, age, active));

        var p = new Person("Alice", 30, true);
        Assert.That(roundTrip(personCodec, p), Is.EqualTo(p));
    }

    public record Node(string Name, List<Node> Children)
    {
        public static readonly IBinaryCodec<Node> CODEC = BinaryCodecs.Recursive<Node>(self =>
            BinaryCodecs.For<Node>()
                .Field(BinaryCodecs.STRING, n => n.Name)
                .Field(self.List(), n => n.Children)
                .Build((name, children) => new Node(name, children)));
    }

    [Test]
    public void Recursive_Codec_RoundTrip()
    {
        var tree = new Node(
            "root",
            [
                new Node("child1", []),
                new Node("child2", [new Node("grandchild1", [])])
            ]
        );

        var round = roundTrip(Node.CODEC, tree);
        Assert.That(round.Name, Is.EqualTo("root"));
        Assert.That(round.Children, Has.Count.EqualTo(2));
        Assert.That(round.Children[0].Name, Is.EqualTo("child1"));
        Assert.That(round.Children[0].Children, Has.Count.EqualTo(0));
        Assert.That(round.Children[1].Name, Is.EqualTo("child2"));
        Assert.That(round.Children[1].Children, Has.Count.EqualTo(1));
        Assert.That(round.Children[1].Children[0].Name, Is.EqualTo("grandchild1"));
    }
}
