using Codon.Codec;
using Codon.Codec.Transcoder.Transcoders;

namespace Codon.Tests;

public class CodecTests
{
    public record Person(string name, int age)
    {
        public static readonly StructCodec<Person> Codec = StructCodec.Of
        (
            "name", Codecs.String, p => p.name,
            "age", Codecs.Int, p => p.age,
            (name, age) => new Person(name, age)
        );
    }

    [Test]
    public void TestCodec()
    {
        var person = new Person("Silly Billy", 18);
        var encoded = Person.Codec.Encode(JsonTranscoder.Instance, person);
        Console.WriteLine(encoded.GetRawText());
        var decoded = Person.Codec.Decode(JsonTranscoder.Instance, encoded);

        Assert.AreEqual(person, decoded);
    }
}