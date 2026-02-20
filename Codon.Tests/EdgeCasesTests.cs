// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Codon.Codec;
using Codon.IniTranscoder.Elements;

namespace Codon.Tests;

public class EdgeCasesTests
{
    private readonly Codecs.OptionalCodec<string> optionalString = new(Codecs.STRING);

    [Test]
    public void Test()
    {
        var decoded = optionalString.Decode(IniTranscoder.IniTranscoder.INSTANCE, IniValue.Null);
        var decodedStringifiedNull = optionalString.Decode(IniTranscoder.IniTranscoder.INSTANCE, new IniValue("null"));

        Assert.That(decoded.IsMissing, Is.True);
        Assert.That(decodedStringifiedNull.IsMissing, Is.True);
    }
}
