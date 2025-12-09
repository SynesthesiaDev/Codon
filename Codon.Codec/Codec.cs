using Codon.Codec.Transcoder;

namespace Codon.Codec;

public interface ICodec<T>
{
    public D Encode<D>(ITranscoder<D> transcoder, T value);
    public T Decode<D>(ITranscoder<D> transcoder, D value);


}