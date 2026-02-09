using Codon.Codec.Transcoder;

namespace Codon.Codec;

public class PrimitiveCodec<A>(Func<dynamic, A, dynamic> encoder, Func<dynamic, dynamic, A> decoder) : ICodec<A>
{
    public D Encode<D>(ITranscoder<D> transcoder, A value)
    {
        return (D)encoder.Invoke((dynamic)transcoder, value);
    }

    public A Decode<D>(ITranscoder<D> transcoder, D value)
    {
        return decoder.Invoke((dynamic)transcoder, value);
    }
}
