using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Json;
using Codon.Optionals;

namespace Codon.Tests;

public class CodecTests
{
    private const string jeson = "{\"name\":\"Synesthesia Dev\",\"age\":20}";

    public record Person(string Name, int Age, Optional<bool> IsAwesome)
    {
        public static readonly Codec<Person> CODEC = StructCodec.Of
        (
            "name", Codecs.STRING, p => p.Name,
            "age", Codecs.INT, p => p.Age,
            "is_awesome", Codecs.BOOLEAN.Optional(), p => p.IsAwesome,
            (name, age, someBoolean) => new Person(name, age, someBoolean)
        );
    }

    public record Car(string Model, List<Person> Passengers, Optional<Person> Driver)
    {
        public static readonly StructCodec<Car> CODEC = StructCodec.Of
        (
            "model", Codecs.STRING, c => c.Model,
            "passengers", Person.CODEC.List(), c => c.Passengers,
            "driver", Person.CODEC.Optional(), c => c.Driver,
            (model, passengers, driver) => new Car(model, passengers, driver)
        );
    }

    [Test]
    public void TestCodec()
    {
        var person = new Person("Silly Billy", 18, Optional.Of(true));
        var encoded = Person.CODEC.Encode(JsonTranscoder.INSTANCE, person);

        Console.WriteLine(encoded.GetRawText()); // {"name":"Silly Billy","age":18,"is_awesome":true}

        var decoded = Person.CODEC.Decode(JsonTranscoder.INSTANCE, encoded);
        Console.WriteLine(decoded); // Person { name = Silly Billy, age = 18, isAwesome = True }

        Assert.That(decoded, Is.EqualTo(person));
    }

    [Test]
    public void TestDecodeFromString()
    {
        var decoded = Person.CODEC.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(jeson).RootElement);
        Assert.That(decoded.Name, Is.EqualTo("Synesthesia Dev"));
        Assert.That(decoded.Age, Is.EqualTo(20));
        Assert.That(Optional.Empty<bool>(), Is.EqualTo(decoded.IsAwesome));
    }
}
