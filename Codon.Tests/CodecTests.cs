using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Json;
using Codon.Optionals;

namespace Codon.Tests;

public class CodecTests
{
    private const string jeson = "{\"name\":\"Synesthesia Dev\",\"age\":20}";

    public record Person(string name, int age, Optional<bool> isAwesome)
    {
        public static readonly StructCodec<Person> Codec = StructCodec.Of
        (
            "name", Codecs.String, p => p.name,
            "age", Codecs.Int, p => p.age,
            "is_awesome", Codecs.Boolean.Optional(), p => p.isAwesome,
            (name, age, someBoolean) => new Person(name, age, someBoolean)
        );
    }

    [Test]
    public void TestCodec()
    {
        var person = new Person("Silly Billy", 18, Optional.Of(true));
        var encoded = Person.Codec.Encode(JsonTranscoder.INSTANCE, person);

        Console.WriteLine(encoded.GetRawText()); // {"name":"Silly Billy","age":18,"is_awesome":true}

        var decoded = Person.Codec.Decode(JsonTranscoder.INSTANCE, encoded);
        Console.WriteLine(decoded); // Person { name = Silly Billy, age = 18, isAwesome = True }

        Assert.That(decoded, Is.EqualTo(person));
    }

    [Test]
    public void TestDecodeFromString()
    {
        var decoded = Person.Codec.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(jeson).RootElement);
        Assert.That(decoded.name, Is.EqualTo("Synesthesia Dev"));
        Assert.That(decoded.age, Is.EqualTo(20));
        Assert.That(Optional.Empty<bool>(), Is.EqualTo(decoded.isAwesome));
    }
}
