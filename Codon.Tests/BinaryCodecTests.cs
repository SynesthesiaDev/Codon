using Codon.Buffer;
using Codon.Binary;
using Codon.Codec;

namespace Codon.Tests;

public class BinaryCodecTests
{
    private static T RoundTrip<T>(IBinaryCodec<T> codec, T value)
    {
        var buf = new BinaryBuffer();
        codec.Write(buf, value!);
        return codec.Read(buf);
    }

    [Test]
    public void PrimitiveCodecs_RoundTrip()
    {
        Assert.That(RoundTrip(BinaryCodec.Boolean, true), Is.True);
        Assert.That(RoundTrip(BinaryCodec.Boolean, false), Is.False);

        Assert.That(RoundTrip(BinaryCodec.Byte, (byte)0), Is.EqualTo((byte)0));
        Assert.That(RoundTrip(BinaryCodec.Byte, (byte)255), Is.EqualTo((byte)255));

        Assert.That(RoundTrip(BinaryCodec.Int, 0), Is.EqualTo(0));
        Assert.That(RoundTrip(BinaryCodec.Int, 123456789), Is.EqualTo(123456789));
        Assert.That(RoundTrip(BinaryCodec.Int, -123456789), Is.EqualTo(-123456789));

        Assert.That(RoundTrip(BinaryCodec.Long, 0L), Is.EqualTo(0L));
        Assert.That(RoundTrip(BinaryCodec.Long, long.MaxValue), Is.EqualTo(long.MaxValue));
        Assert.That(RoundTrip(BinaryCodec.Long, long.MinValue), Is.EqualTo(long.MinValue));

        Assert.That(RoundTrip(BinaryCodec.Float, 123.456f), Is.EqualTo(123.456f));
        Assert.That(RoundTrip(BinaryCodec.Double, -123.456789), Is.EqualTo(-123.456789));
    }

    [Test]
    public void VarInt_RoundTrip_WithEdgeCases()
    {
        var values = new[] { 0, 1, 2, 127, 128, 129, 16384, int.MaxValue, -1, -2, int.MinValue };
        foreach (var v in values)
        {
            var buf = new BinaryBuffer();
            BinaryCodec.VarInt.Write(buf, v);
            var read = BinaryCodec.VarInt.Read(buf);
            Assert.That(read, Is.EqualTo(v), $"VarInt roundtrip failed for {v}");
        }
    }

    [Test]
    public void ByteArray_And_RawBytes_RoundTrip()
    {
        var empty = Array.Empty<byte>();
        Assert.That(RoundTrip(BinaryCodec.ByteArray, empty), Is.EqualTo(empty));

        var data = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
        Assert.That(RoundTrip(BinaryCodec.ByteArray, data), Is.EqualTo(data));

        var buf = new BinaryBuffer();
        BinaryCodec.RawBytes.Write(buf, data);
        var read = BinaryCodec.RawBytes.Read(buf);
        Assert.That(read, Is.EqualTo(data));
    }

    [Test]
    public void BinaryBuffer_Codec_RoundTrip()
    {
        var original = new BinaryBuffer();
        original.WriteBytes("hello"u8.ToArray());
        original.WriteInt(42);

        var buf = new BinaryBuffer();
        BinaryCodec.BinaryBuffer.Write(buf, original);
        var round = BinaryCodec.BinaryBuffer.Read(buf);

        Assert.That(round.ToArray(), Is.EqualTo(original.ToArray()));
    }

    [Test]
    public void String_Codec_RoundTrip()
    {
        Assert.That(RoundTrip(BinaryCodec.String, string.Empty), Is.EqualTo(string.Empty));
        Assert.That(RoundTrip(BinaryCodec.String, "ascii"), Is.EqualTo("ascii"));
        var unicode = "你好世界 👋🌍";
        Assert.That(RoundTrip(BinaryCodec.String, unicode), Is.EqualTo(unicode));
    }

    [Test]
    public void Optional_Codec_WritesPresenceAndValue()
    {
        var optionalInt = BinaryCodec.Int.Optional();

        var some = Optional.Of(123);
        var bufSome = new BinaryBuffer();
        optionalInt.Write(bufSome, some);
        var readSome = optionalInt.Read(bufSome);
        Assert.That(readSome.IsPresent);
        Assert.AreEqual(readSome.Value, some.Value);

        var none = Optional.Empty<int>();
        var bufNone = new BinaryBuffer();
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
        var codec = BinaryCodec.Enum<TestEnum>();
        Assert.That(RoundTrip(codec, TestEnum.A), Is.EqualTo(TestEnum.A));
        Assert.That(RoundTrip(codec, TestEnum.B), Is.EqualTo(TestEnum.B));
        Assert.That(RoundTrip(codec, TestEnum.C), Is.EqualTo(TestEnum.C));
    }

    [Test]
    public void Transformative_Codec_RoundTrip()
    {
        var codec = BinaryCodec.Int.Transform(
            from: s => s.Length,
            to: n => new string('x', n)
        );

        var s = "hello";
        var buf = new BinaryBuffer();
        codec.Write(buf, s);
        var read = codec.Read(buf);
        Assert.That(read, Is.EqualTo(new string('x', s.Length)));
    }

    [Test]
    public void List_And_Dictionary_Codecs_RoundTrip()
    {
        BinaryCodecs.ListBinaryCodec<string> listCodec = BinaryCodec.String.List();

        var list = new List<string> { "a", "b", "c" };
        Assert.That(RoundTrip(listCodec, list), Is.EqualTo(list));

        var mapCodec = BinaryCodec.Int.MapTo(BinaryCodec.String);
        var dict = new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" } };
        var round = RoundTrip(mapCodec, dict);
        Assert.That(round, Has.Count.EqualTo(3));
        foreach (var kv in dict) Assert.That(round[kv.Key], Is.EqualTo(kv.Value));
    }

    [Test]
    public void Union_Codec_RoundTrip_WithInt_KeyByte()
    {
        var union = new BinaryCodecs.UnionBinaryCodec<int, byte>(
            keyCodec: BinaryCodec.Byte,
            keyFunc: v => (byte)(v % 2),
            serializerFactory: _ => BinaryCodec.Int
        );

        var round = RoundTrip(union, 7);
        Assert.That(round, Is.EqualTo(7));
    }

    public record Person(string Name, int Age, bool Active);

    [Test]
    public void Composite_Codec_P3_RoundTrip()
    {
        var personCodec = BinaryCodec.Of(
            BinaryCodec.String, p => p.Name,
            BinaryCodec.Int, p => p.Age,
            BinaryCodec.Boolean, p => p.Active,
            (name, age, active) => new Person(name, age, active)
        );

        var p = new Person("Alice", 30, true);
        Assert.That(RoundTrip(personCodec, p), Is.EqualTo(p));
    }

    public record Node(string Name, List<Node> Children)
    {
        public static readonly IBinaryCodec<Node> Codec = BinaryCodec.Recursive<Node>(self =>
            BinaryCodec.Of(
                BinaryCodec.String, n => n.Name,
                self.List(), n => n.Children,
                (name, children) => new Node(name, children)
            )
        );
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

        var round = RoundTrip(Node.Codec, tree);
        Assert.That(round.Name, Is.EqualTo("root"));
        Assert.That(round.Children, Has.Count.EqualTo(2));
        Assert.That(round.Children[0].Name, Is.EqualTo("child1"));
        Assert.That(round.Children[0].Children, Has.Count.EqualTo(0));
        Assert.That(round.Children[1].Name, Is.EqualTo("child2"));
        Assert.That(round.Children[1].Children, Has.Count.EqualTo(1));
        Assert.That(round.Children[1].Children[0].Name, Is.EqualTo("grandchild1"));
    }
}