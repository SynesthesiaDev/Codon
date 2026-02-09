// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Codon.Codec.Transcoder;

namespace Codon.Codec.Versioned;

public static class SchemaMigrations
{
    public static SchemaMigrations<D> Empty<D>() => new SchemaMigrations<D>.Builder().Build();

    public static SchemaMigrations<D>.Builder Builder<D>() => new();

}

public class SchemaMigrations<D>
{
    public required IReadOnlyDictionary<int, SchemaMigration<D>> Migrations { get; init; }


    public class Builder
    {
        private readonly Dictionary<int, SchemaMigration<D>> migrations = new();

        public Builder Add(int version, Action<ITranscoder<D>, IVirtualMap<D>, IVirtualMapBuilder<D>> applyMethod)
        {
            migrations.Add(version, new SchemaMigration<D>
            {
                Version = version,
                ApplyMethod = applyMethod
            });

            return this;
        }

        public SchemaMigrations<D> Build()
        {
            return new SchemaMigrations<D>
            {
                Migrations = migrations,
            };
        }
    }
}
