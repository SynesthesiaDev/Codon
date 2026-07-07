using Codon.Codec.Transcoder;

namespace Codon.Codec.Versioned;


public sealed class VersionedStructCodec<R> : Codec<R> where R : notnull
{
    private const string schema_version_key = "_schemaVersion";

    public required int CurrentSchemaVersion { get; init; }

    public required StructCodec<R> InnerCodec { get; init; }

    public required SchemaMigrationRegistry SchemaMigrationRegistry { get; init; }

    // Apply any data migrations before actually decoding
    public override R Decode<D>(ITranscoder<D> transcoder, D value)
    {
        var map = transcoder.DecodeMap(value);
        var schemaVersion = map.HasValue(schema_version_key) ? transcoder.DecodeInt(map.GetValue(schema_version_key)) : 0;

        if (schemaVersion > CurrentSchemaVersion) throw new InvalidOperationException($"Value has higher schema version ({schemaVersion}) than currently known ({CurrentSchemaVersion})");

        var migrations = SchemaMigrationRegistry.Get<D>();

        while (schemaVersion < CurrentSchemaVersion)
        {
            schemaVersion++;

            if (!migrations.Migrations.TryGetValue(schemaVersion, out var migration))
                throw new KeyNotFoundException($"Missing migration for schema version {schemaVersion}.");

            var output = transcoder.EncodeMap();

            foreach (var key in map.GetKeys())
                output.Put(key, map.GetValue(key));

            migration.ApplyMethod.Invoke(transcoder, map, output);

            var migrated = output.Build();
            map = transcoder.DecodeMap(migrated);
        }

        return InnerCodec.DecodeFromMap(transcoder, map);
    }

    public override D Encode<D>(ITranscoder<D> transcoder, R value)
    {
        var mapBuilder = transcoder.EncodeMap();
        mapBuilder.Put(schema_version_key, transcoder.EncodeInt(CurrentSchemaVersion));

        return InnerCodec.EncodeToMap(transcoder, value, mapBuilder);
    }
}
