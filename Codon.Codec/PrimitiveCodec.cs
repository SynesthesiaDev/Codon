using Codon.Codec.Transcoder;

namespace Codon.Codec;

public class PrimitiveCodec<A>(Func<dynamic, A, dynamic> encoder, Func<dynamic, dynamic, A> decoder) : Codec<A> where A : notnull
{
    public override D Encode<D>(ITranscoder<D> transcoder, A value)
    {
        return (D)encoder.Invoke((dynamic)transcoder, value);
    }

    public override A Decode<D>(ITranscoder<D> transcoder, D value)
    {
        return decoder.Invoke((dynamic)transcoder, value);
    }
}
