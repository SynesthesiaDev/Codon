using System.Linq.Expressions;
using Codon.Codec.Transcoder;

namespace Codon.Codec;

public static class StructCodec
{
    public static StructCodec<Result> Of<P1, P2, Result>(
        string name1, ICodec<P1> codec1, Expression<Func<Result, P1>> getter1,
        string name2, ICodec<P2> codec2, Expression<Func<Result, P2>> getter2,
        Expression<Func<P1, P2, Result>> func
    )
    {
        return new StructCodec<object>.StructCodec2P<P1, P2, Result>(name1, codec1, getter1, name2, codec2, getter2, func);
    }
}

public abstract class StructCodec<R> : ICodec<R>
{

    public const string Inline = "_inline_";

    public abstract T EncodeToMap<T>(ITranscoder<T> transcoder, R value, IVirtualMapBuilder<T> mapBuilder);

    public abstract R DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map);

    public D Encode<D>(ITranscoder<D> transcoder, R value)
    {
        return EncodeToMap(transcoder, value, transcoder.EncodeMap());
    }

    public R Decode<D>(ITranscoder<D> transcoder, D value)
    {
        return DecodeFromMap(transcoder, transcoder.DecodeMap(value));
    }

    private static D Put<D, T>(ITranscoder<D> transcoder, ICodec<T> codec, IVirtualMapBuilder<D> mapBuilder, string key, T value)
    {
        if (key == Inline)
        {
            var encodeCodec = codec switch
            {
                Codecs.DefaultCodec<T> defaultCodec => defaultCodec.Inner,
                Codecs.OptionalCodec<T> optionalCodec => optionalCodec.Inner,
                //Todo Recursive
                _ => codec
            };

            return encodeCodec is not StructCodec<T> structCodec ? throw new InvalidOperationException($"Provided coded for inline {key} is not StructCodec") : structCodec.EncodeToMap(transcoder, value, mapBuilder);
        }

        var result = codec.Encode(transcoder, value);
        mapBuilder.Put(key, result);
        return result;
    }

    public static T Get<D, T>(ITranscoder<D> transcoder, ICodec<T> codec, string key, IVirtualMap<D> map)
    {
        if (key == Inline)
        {
            var decodeCodec = codec switch
            {
                Codecs.OptionalCodec<T> optionalCodec => optionalCodec.Inner,
                Codecs.DefaultCodec<T> defaultCodec => defaultCodec.Inner,
                //Todo Recursive
                _ => codec
            };

            if (decodeCodec is not StructCodec<T> structCodec)
            {
                throw new InvalidOperationException($"Provided coded for inline {key} is not StructCodec");
            }

            try
            {
                // The C# equivalent of runCatching { ... } is a standard try block
                var result = structCodec.DecodeFromMap(transcoder, map);
                return result;
            }
            catch
            {
                switch (codec)
                {
                    case Codecs.DefaultCodec<T> defaultCodec:
                        return defaultCodec.Default;
                    case Codecs.OptionalCodec<T> optionalCodec:
                        return default(T);
                    default:
                        throw;
                }
            }
        }

        return (codec switch
        {
            Codecs.DefaultCodec<T> defaultValue when !map.HasValue(key) => defaultValue.Default,
            Codecs.OptionalCodec<T> when !map.HasValue(key) => default,
            _ => codec.Decode(transcoder, map.GetValue(key))
        })!;
    }

    public class StructCodec0P<F>(Func<F> func) : StructCodec<F>
    {
        public override T1 EncodeToMap<T1>(ITranscoder<T1> transcoder, F value, IVirtualMapBuilder<T1> mapBuilder)
        {
            return transcoder.EmptyMap();
        }

        public override F DecodeFromMap<T1>(ITranscoder<T1> transcoder, IVirtualMap<T1> map)
        {
            return func.Invoke();
        }
    }

    public class StructCodec1P<P1, Result>(string name1, ICodec<P1> codec1, Expression<Func<Result, P1>> getter1, Expression<Func<P1, Result>> func) : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Compile().Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            return func.Compile().Invoke(result1);
        }
    }

    public class StructCodec2P<P1, P2, Result>
    (
        string name1, ICodec<P1> codec1, Expression<Func<Result, P1>> getter1,
        string name2, ICodec<P2> codec2, Expression<Func<Result, P2>> getter2,
        Expression<Func<P1, P2, Result>> func
    )
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Compile().Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Compile().Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            return func.Compile().Invoke(result1, result2);
        }
    }
}