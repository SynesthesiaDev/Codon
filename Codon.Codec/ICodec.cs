using Codon.Codec.Transcoder;
using Codon.Optionals;

namespace Codon.Codec;

public interface ICodec<T> where T : notnull
{
    D Encode<D>(ITranscoder<D> transcoder, T value);
    T Decode<D>(ITranscoder<D> transcoder, D value);

    ICodec<Optional<T>> Optional()
    {
        return new Codecs.OptionalCodec<T>(this);
    }

    ICodec<T> Default(T value)
    {
        return new Codecs.DefaultCodec<T>(this, value);
    }

    ICodec<List<T>> List()
    {
        return new Codecs.ListCodec<T>(this);
    }

    ICodec<Dictionary<T, V>> MapTo<V>(ICodec<V> valueCodec) where V : notnull
    {
        return new Codecs.MapCodec<T, V>(this, valueCodec);
    }

    ICodec<T> ForwardRef()
    {
        return new Codecs.ForwardRefCodec<T>(() => this);
    }

    ICodec<S> Transform<S>(Func<T, S> to, Func<S, T> from) where S : notnull
    {
        return new Codecs.TransformativeCodec<T, S>(this, to, from);
    }

    StructCodec<R> Union<R>(string keyField, Func<T, StructCodec<R>> serializers, Func<R, T> keyFunc) where R : notnull
    {
        return new Codecs.UnionCodec<T, R>(keyField, this, serializers, keyFunc);
    }
}
