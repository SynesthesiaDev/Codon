using Codon.Codec.Transcoder;

namespace Codon.Codec.Versioned;

public class SchemaMigration<D>
{
    public required int Version { get; init; }
    public required Action<ITranscoder<D>, IVirtualMap<D>, IVirtualMapBuilder<D>> ApplyMethod { get; init; }
}
