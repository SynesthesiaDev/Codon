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
        public static readonly Codec<Person> Codec = StructCodec.Of
        (
            "name", Codecs.STRING, p => p.name,
            "age", Codecs.INT, p => p.age,
            "is_awesome", Codecs.BOOLEAN.Optional(), p => p.isAwesome,
            (name, age, someBoolean) => new Person(name, age, someBoolean)
        );
    }

    public record Car(string Model, List<Person> Passengers, Optional<Person> Driver)
    {
        public static readonly StructCodec<Car> CODEC = StructCodec.Of
        (
            "model", Codecs.STRING, c => c.Model,
            "passengers", Person.Codec.List(), c => c.Passengers,
            "driver", Person.Codec.Optional(), c => c.Driver,
            (model, passengers, driver) => new Car(model, passengers, driver)
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
