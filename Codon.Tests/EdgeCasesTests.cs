// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Codon.Codec;
using Codon.IniTranscoder.Elements;
using Codon.Optionals;
using SynesthesiaDev.Synx.Codon;

namespace Codon.Tests;

public class EdgeCasesTests
{
    private readonly Codecs.OptionalCodec<string> optionalString = new(Codecs.STRING);
    private record TestingClass(string TestString, TestEnummm? Enuming, Thing? Thing)
    {
        public static readonly StructCodec<TestingClass> CODEC = StructCodec.For<TestingClass>()
            .Field("TestString", Codecs.STRING, t => t.TestString)
            .Field("Enuming", Codecs.Enum<TestEnummm>().Optional(), t => t.Enuming.ToOptional())
            .Field("Thing", Thing.CODEC.Optional(), t => t.Thing.ToOptional())
            .Build((s, e, t) => new TestingClass(s ,e.ToNullableStruct(), t.ToNullableClass()));
    }

    public record Thing(string Name)
    {
        public static readonly Codec<Thing> CODEC = StructCodec.For<Thing>()
            .Field("Name", Codecs.STRING, t => t.Name)
            .Build(n => new Thing(n));
    }

    private enum TestEnummm
    {
        First,
        Second,
        Third
    }

    [Test]
    public void Test()
    {
        var decoded = optionalString.Decode(IniTranscoder.IniTranscoder.INSTANCE, IniValue.Null);
        var decodedStringifiedNull = optionalString.Decode(IniTranscoder.IniTranscoder.INSTANCE, new IniValue("null"));

        Assert.That(decoded.IsMissing, Is.True);
        Assert.That(decodedStringifiedNull.IsMissing, Is.True);
    }

    [Test]
    public void TestEnum()
    {
        var encoded = TestingClass.CODEC.Encode(SynxTranscoder.INSTANCE, new TestingClass("yo", null, null));
        var decoded = TestingClass.CODEC.Decode(SynxTranscoder.INSTANCE, encoded);

        Assert.That(decoded.Enuming, Is.Null);
        Assert.That(decoded.Thing, Is.Null);
    }
}
