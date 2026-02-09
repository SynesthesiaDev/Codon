// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace Codon.Codec.Versioned;

public class SchemaMigrationRegistry
{
    public static SchemaMigrationRegistry Builder() => new();


    private readonly Dictionary<Type, object> typeToObject = new();

    public SchemaMigrationRegistry For<D>(Action<SchemaMigrations<D>.Builder> migrations)
    {
        var builder = SchemaMigrations.Builder<D>();
        migrations.Invoke(builder);
        Add(builder.Build());
        return this;
    }

    public void Add<D>(SchemaMigrations<D> migrations)
    {
        typeToObject[typeof(D)] = migrations;
    }

    public SchemaMigrations<D> Get<D>()
    {
        if (typeToObject.TryGetValue(typeof(D), out var boxed) && boxed is SchemaMigrations<D> typed)
            return typed;

        throw new KeyNotFoundException($"Migration for type {typeof(D)} was not found!");
    }
}
