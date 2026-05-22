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
        public static readonly StructCodec<Person> CODEC = StructCodec.For<Person>()
            .Field("name", Codecs.STRING, p => p.Name)
            .Field("age", Codecs.INT, p => p.Age)
            .Field("is_awesome", Codecs.BOOLEAN.Optional(), p => p.IsAwesome)
            .Build((name, age, isAwesome) => new Person(name, age, isAwesome));
    }

    public record Car(string Model, List<Person> Passengers, Optional<Person> Driver)
    {
        public static readonly StructCodec<Car> CODEC = StructCodec.For<Car>()
            .Field("model", Codecs.STRING, c => c.Model)
            .Field("passengers", Person.CODEC.List(), c => c.Passengers)
            .Field("driver", Person.CODEC.Optional(), c => c.Driver)
            .Build((model, passengers, driver) => new Car(model, passengers, driver));
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
