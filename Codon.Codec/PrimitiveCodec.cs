using Codon.Codec.Transcoder;

namespace Codon.Codec;

// Uses dynamic dispatch to invoke the appropriate methods on the provided transcoder
// without forcing an unsafe cast between different generic ITranscoder<T> instantiations.
public class PrimitiveCodec<A>(Func<dynamic, A, dynamic> Encoder, Func<dynamic, dynamic, A> Decoder) : ICodec<A>
{
    public D Encode<D>(ITranscoder<D> transcoder, A value)
    {
        // Use dynamic to call the correctly-typed EncodeX method on the transcoder
        return (D)Encoder.Invoke((dynamic)transcoder, value);
    }

    public A Decode<D>(ITranscoder<D> transcoder, D value)
    {
        // Use dynamic to call the correctly-typed DecodeX method on the transcoder
        return Decoder.Invoke((dynamic)transcoder, value);
    }
}