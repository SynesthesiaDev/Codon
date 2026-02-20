using Codon.Codec.Transcoder;
using Codon.Optionals;

namespace Codon.Codec;

public abstract class Codec<T> where T : notnull
{
    public abstract D Encode<D>(ITranscoder<D> transcoder, T value);
    public abstract T Decode<D>(ITranscoder<D> transcoder, D value);

    public Codec<Optional<T>> Optional()
    {
        return new Codecs.OptionalCodec<T>(this);
    }

    public Codec<T> Default(T value)
    {
        return new Codecs.DefaultCodec<T>(this, value);
    }

    public Codec<List<T>> List()
    {
        return new Codecs.ListCodec<T>(this);
    }

    public Codec<Dictionary<T, V>> MapTo<V>(Codec<V> valueCodec) where V : notnull
    {
        return new Codecs.MapCodec<T, V>(this, valueCodec);
    }

    public Codec<T> ForwardRef()
    {
        return new Codecs.ForwardRefCodec<T>(() => this);
    }

    public Codec<S> Transform<S>(Func<T, S> to, Func<S, T> from) where S : notnull
    {
        return new Codecs.TransformativeCodec<T, S>(this, to, from);
    }

    public StructCodec<R> Union<R>(string keyField, Func<T, StructCodec<R>> serializers, Func<R, T> keyFunc) where R : notnull
    {
        return new Codecs.UnionCodec<T, R>(keyField, this, serializers, keyFunc);
    }
}
