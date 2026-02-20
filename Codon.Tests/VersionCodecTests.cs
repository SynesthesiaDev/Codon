using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Json;
using Codon.Codec.Versioned;
using Codon.Optionals;

namespace Codon.Tests;

public class VersionCodecTests
{
    private const string jeson = "{\"display_name\":\"Synesthesia Dev\", \"is_awesome\":true}";
    private const string json_newer = "{\"_schemaVersion\": 3, \"name\":\"Synesthesia Dev\", \"age\": 99, \"is_awesome\":true}";

    public record Person(string Name, int Age, Optional<bool> IsAwesome)
    {
        public static readonly StructCodec<Person> CODEC = StructCodec.Of
        (
            "name", Codecs.STRING, p => p.Name,
            "age", Codecs.INT, p => p.Age,
            "is_awesome", Codecs.BOOLEAN.Optional(), p => p.IsAwesome,
            (name, age, someBoolean) => new Person(name, age, someBoolean)
        );

        // schema version 0 -> 1: added "age" field
        // schema version 1 -> 2: renamed "display_name" to just "name"

        public static readonly VersionedStructCodec<Person> VERSIONED_CODEC = new VersionedStructCodec<Person>()
        {
            CurrentSchemaVersion = 2,
            InnerCodec = Person.CODEC,
            SchemaMigrationRegistry = SchemaMigrationRegistry.Builder()
                .For<JsonElement>(migrations =>
                {
                    // migration to version 1: ensure "age" exists
                    migrations.Add(1, (transcoder, _, output) => output.Put("age", transcoder.EncodeInt(0)));

                    // migration to version 2: copy "display_name" -> "name"
                    migrations.Add(2, (transcoder, input, output) =>
                    {
                        var name = transcoder.DecodeString(input.GetValue("display_name"));
                        output.Put("name", transcoder.EncodeString(name));
                    });
                })
        };
    }

    private static JsonElement parseJson(string json) => JsonDocument.Parse(json).RootElement;

    [Test]
    public void Decode_V0_NoSchemaVersionField_AppliesAllMigrations_AndDecodes()
    {

        // no _schemaVersion, also uses "display_name" (pre-v2)
        var v0 = parseJson("{\"display_name\":\"Synesthesia Dev\", \"is_awesome\":true}");

        var decoded = Person.VERSIONED_CODEC.Decode(JsonTranscoder.INSTANCE, v0);

        Assert.That(decoded.Name, Is.EqualTo("Synesthesia Dev"));
        Assert.That(decoded.Age, Is.EqualTo(0), "v0->v1 migration should default age to 0");
        Assert.That(decoded.IsAwesome.IsPresent, Is.True);
        Assert.That(decoded.IsAwesome.Value, Is.True);
    }

    [Test]
    public void Decode_V1_WithSchemaVersion_AppliesRemainingMigrationsOnly()
    {
        // v1 has _schemaVersion=1 and already has age, but still uses display_name
        var v1 = parseJson("{\"_schemaVersion\":1, \"display_name\":\"Ada\", \"age\":42, \"is_awesome\":false}");

        var decoded = Person.VERSIONED_CODEC.Decode(JsonTranscoder.INSTANCE, v1);

        Assert.That(decoded.Name, Is.EqualTo("Ada"));
        Assert.That(decoded.Age, Is.EqualTo(42), "Age should not be overwritten by v0->v1 migration when already v1");
        Assert.That(decoded.IsAwesome.IsPresent, Is.True);
        Assert.That(decoded.IsAwesome.Value, Is.False);
    }

    [Test]
    public void Decode_AlreadyCurrentSchema_DoesNotRequireMigrations_AndIgnoresUnknownFields()
    {
        // name + age and includes extra field that should be ignored by StructCodec
        var v2 = parseJson("{\"_schemaVersion\":2, \"name\":\"Grace\", \"age\":99, \"is_awesome\":true, \"extra\":\"ignored\"}");

        var decoded = Person.VERSIONED_CODEC.Decode(JsonTranscoder.INSTANCE, v2);

        Assert.That(decoded.Name, Is.EqualTo("Grace"));
        Assert.That(decoded.Age, Is.EqualTo(99));
        Assert.That(decoded.IsAwesome.IsPresent, Is.True);
        Assert.That(decoded.IsAwesome.Value, Is.True);
    }

    [Test]
    public void Decode_MissingRequiredFieldDuringMigration_Throws()
    {
        // v1 -> v2 migration expects display_name. If it's missing, migration should fail.
        var v1MissingDisplayName = parseJson("{\"_schemaVersion\":1, \"age\":5, \"is_awesome\":true}");

        Assert.Throws<KeyNotFoundException>(() => Person.VERSIONED_CODEC.Decode(JsonTranscoder.INSTANCE, v1MissingDisplayName));
    }

    [Test]
    public void Decode_WhenMigrationIsMissing_ThrowsKeyNotFoundException()
    {
        var versioned = new VersionedStructCodec<Person>
        {
            CurrentSchemaVersion = 2,
            InnerCodec = Person.CODEC,
            SchemaMigrationRegistry = SchemaMigrationRegistry.Builder()
                .For<JsonElement>(migrations =>
                {
                    // Intentionally omit migration to version 2
                    migrations.Add(1, (transcoder, _, output) => output.Put("age", transcoder.EncodeInt(0)));
                })
        };

        var v0 = parseJson("{\"display_name\":\"X\", \"is_awesome\":true}");

        Assert.Throws<KeyNotFoundException>(() => versioned.Decode(JsonTranscoder.INSTANCE, v0));
    }

    [Test]
    public void Encode_AlwaysAddsSchemaVersion_AndRoundTripsWithDecode()
    {
        var original = new Person("Synesthesia Dev", 123, Optional.Of(true));

        var encoded = Person.VERSIONED_CODEC.Encode(JsonTranscoder.INSTANCE, original);

        var map = JsonTranscoder.INSTANCE.DecodeMap(encoded);
        Assert.That(map.HasValue("_schemaVersion"), Is.True);
        Assert.That(JsonTranscoder.INSTANCE.DecodeInt(map.GetValue("_schemaVersion")), Is.EqualTo(2));

        var decoded = Person.VERSIONED_CODEC.Decode(JsonTranscoder.INSTANCE, encoded);
        Assert.That(decoded, Is.EqualTo(original));
    }

    [Test]
    public void RoundTrip()
    {
        var decoded = Person.VERSIONED_CODEC.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(jeson).RootElement);
        Assert.That(decoded.Name, Is.EqualTo("Synesthesia Dev"));
        Assert.That(decoded.Age, Is.EqualTo(0));

        var encoded = Person.VERSIONED_CODEC.Encode(JsonTranscoder.INSTANCE, decoded);
        var map = JsonTranscoder.INSTANCE.DecodeMap(encoded);
        Assert.That(JsonTranscoder.INSTANCE.DecodeInt(map.GetValue("_schemaVersion")), Is.EqualTo(2));
        Assert.That(JsonTranscoder.INSTANCE.DecodeString(map.GetValue("name")), Is.EqualTo("Synesthesia Dev"));
    }

    [Test]
    public void Decode_FutureSchemaVersion()
    {
        Assert.Throws<InvalidOperationException>(() => Person.VERSIONED_CODEC.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(json_newer).RootElement));
    }
}
