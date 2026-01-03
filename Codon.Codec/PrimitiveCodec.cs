using Codon.Codec.Transcoder;

namespace Codon.Codec;

public class PrimitiveCodec<A>(Func<dynamic, A, dynamic> Encoder, Func<dynamic, dynamic, A> Decoder) : ICodec<A>
{
    public D Encode<D>(ITranscoder<D> transcoder, A value)
    {
        return (D)Encoder.Invoke((dynamic)transcoder, value);
    }

    public A Decode<D>(ITranscoder<D> transcoder, D value)
    {
        return Decoder.Invoke((dynamic)transcoder, value);
    }
}
