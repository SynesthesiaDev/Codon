using Codon.Codec.Transcoder;

namespace Codon.Codec;

public interface ICodec<T>
{
    public D Encode<D>(ITranscoder<D> transcoder, T value);
    public T Decode<D>(ITranscoder<D> transcoder, D value);

    public Codecs.OptionalCodec<T> Optional()
    {
        return new Codecs.OptionalCodec<T>(this);
    }

    public Codecs.DefaultCodec<T> Default(T value)
    {
        return new Codecs.DefaultCodec<T>(this, value);
    }

    public Codecs.ListCodec<T> List()
    {
        return new Codecs.ListCodec<T>(this);
    }

    public Codecs.MapCodec<T, V> MapTo<V>(ICodec<V> valueCodec)
    {
        return new Codecs.MapCodec<T, V>(this, valueCodec);
    }

    public Codecs.ForwardRefCodec<T> ForwardRef()
    {
        return new Codecs.ForwardRefCodec<T>(() => this);
    }

    public Codecs.TransformativeCodec<T, S> Transform<S>(Func<T, S> to, Func<S, T> from)
    {
        return new Codecs.TransformativeCodec<T, S>(this, to, from);
    }

    public Codecs.UnionCodec<T, R> Union<R>(string keyField, Func<T, StructCodec<R>> serializers, Func<R, T> keyFunc)
    {
        return new Codecs.UnionCodec<T, R>(keyField, this, serializers, keyFunc);
    }
}