// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Codon.Codec;
using Codon.Optionals;

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
}
