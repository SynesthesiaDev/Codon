using System.Linq.Expressions;
using Codon.Codec.Transcoder;

namespace Codon.Codec;

public static class StructCodec
{
    public static StructCodec<Result> Of<P1, P2, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        Func<P1, P2, Result> func
    )
    {
        return new StructCodec<object>.StructCodec2P<P1, P2, Result>(name1, codec1, getter1, name2, codec2, getter2, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        Func<P1, P2, P3, Result> func
    )
    {
        return new StructCodec<object>.StructCodec3P<P1, P2, P3, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        Func<P1, P2, P3, P4, Result> func
    )
    {
        return new StructCodec<object>.StructCodec4P<P1, P2, P3, P4, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        Func<P1, P2, P3, P4, P5, Result> func
    )
    {
        return new StructCodec<object>.StructCodec5P<P1, P2, P3, P4, P5, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        Func<P1, P2, P3, P4, P5, P6, Result> func
    )
    {
        return new StructCodec<object>.StructCodec6P<P1, P2, P3, P4, P5, P6, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        Func<P1, P2, P3, P4, P5, P6, P7, Result> func
    )
    {
        return new StructCodec<object>.StructCodec7P<P1, P2, P3, P4, P5, P6, P7, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, Result> func
    )
    {
        return new StructCodec<object>.StructCodec8P<P1, P2, P3, P4, P5, P6, P7, P8, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result> func
    )
    {
        return new StructCodec<object>.StructCodec9P<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, name9, codec9, getter9, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result> func
    )
    {
        return new StructCodec<object>.StructCodec10P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, name9, codec9, getter9, name10, codec10, getter10, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result> func
    )
    {
        return new StructCodec<object>.StructCodec11P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, name9, codec9, getter9, name10, codec10, getter10, name11, codec11, getter11, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result> func
    )
    {
        return new StructCodec<object>.StructCodec12P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, name9, codec9, getter9, name10, codec10, getter10, name11, codec11, getter11, name12, codec12, getter12, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        string name13, ICodec<P13> codec13, Func<Result, P13> getter13,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result> func
    )
    {
        return new StructCodec<object>.StructCodec13P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, name9, codec9, getter9, name10, codec10, getter10, name11, codec11, getter11, name12, codec12, getter12, name13, codec13, getter13, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        string name13, ICodec<P13> codec13, Func<Result, P13> getter13,
        string name14, ICodec<P14> codec14, Func<Result, P14> getter14,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result> func
    )
    {
        return new StructCodec<object>.StructCodec14P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, name9, codec9, getter9, name10, codec10, getter10, name11, codec11, getter11, name12, codec12, getter12, name13, codec13, getter13, name14, codec14, getter14, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        string name13, ICodec<P13> codec13, Func<Result, P13> getter13,
        string name14, ICodec<P14> codec14, Func<Result, P14> getter14,
        string name15, ICodec<P15> codec15, Func<Result, P15> getter15,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result> func
    )
    {
        return new StructCodec<object>.StructCodec15P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, name9, codec9, getter9, name10, codec10, getter10, name11, codec11, getter11, name12, codec12, getter12, name13, codec13, getter13, name14, codec14, getter14, name15, codec15, getter15, func);
    }

    public static StructCodec<Result> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        string name13, ICodec<P13> codec13, Func<Result, P13> getter13,
        string name14, ICodec<P14> codec14, Func<Result, P14> getter14,
        string name15, ICodec<P15> codec15, Func<Result, P15> getter15,
        string name16, ICodec<P16> codec16, Func<Result, P16> getter16,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result> func
    )
    {
        return new StructCodec<object>.StructCodec16P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result>(name1, codec1, getter1, name2, codec2, getter2, name3, codec3, getter3, name4, codec4, getter4, name5, codec5, getter5, name6, codec6, getter6, name7, codec7, getter7, name8, codec8, getter8, name9, codec9, getter9, name10, codec10, getter10, name11, codec11, getter11, name12, codec12, getter12, name13, codec13, getter13, name14, codec14, getter14, name15, codec15, getter15, name16, codec16, getter16, func);
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
                var result = structCodec.DecodeFromMap(transcoder, map);
                return result;
            }
            catch
            {
                switch (codec)
                {
                    case Codecs.DefaultCodec<T> defaultCodec:
                        return defaultCodec.Default;
                    default:
                    {
                        var type = codec.GetType();
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Codecs.OptionalCodec<>))
                        {
                            var innerType = type.GetGenericArguments()[0];
                            var optionalType = typeof(Optional<>).MakeGenericType(innerType);
                            var empty = Activator.CreateInstance(optionalType)!; // Optional<TInner>() => missing
                            return (T)empty;
                        }

                        throw;
                    }
                }
            }
        }

        return (codec switch
        {
            Codecs.DefaultCodec<T> defaultValue when !map.HasValue(key) => defaultValue.Default,
            _ when !map.HasValue(key) && codec.GetType().IsGenericType && codec.GetType().GetGenericTypeDefinition() == typeof(Codecs.OptionalCodec<>)
                => (T)Activator.CreateInstance(typeof(Optional<>).MakeGenericType(codec.GetType().GetGenericArguments()[0]))!,
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

    public class StructCodec1P<P1, Result>(
        string name1,
        ICodec<P1> codec1,
        Func<Result, P1> getter1,
        Func<P1, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            return func.Invoke(result1);
        }
    }

    public class StructCodec2P<P1, P2, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        Func<P1, P2, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            return func.Invoke(result1, result2);
        }
    }

    public class StructCodec3P<P1, P2, P3, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        Func<P1, P2, P3, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            return func.Invoke(result1, result2, result3);
        }
    }

    public class StructCodec4P<P1, P2, P3, P4, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        Func<P1, P2, P3, P4, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            return func.Invoke(result1, result2, result3, result4);
        }
    }

    public class StructCodec5P<P1, P2, P3, P4, P5, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        Func<P1, P2, P3, P4, P5, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            return func.Invoke(result1, result2, result3, result4, result5);
        }
    }

    public class StructCodec6P<P1, P2, P3, P4, P5, P6, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        Func<P1, P2, P3, P4, P5, P6, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6);
        }
    }

    public class StructCodec7P<P1, P2, P3, P4, P5, P6, P7, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        Func<P1, P2, P3, P4, P5, P6, P7, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7);
        }
    }

    public class StructCodec8P<P1, P2, P3, P4, P5, P6, P7, P8, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8);
        }
    }

    public class StructCodec9P<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            Put(transcoder, codec9, mapBuilder, name9, getter9.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            var result9 = Get(transcoder, codec9, name9, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9);
        }
    }

    public class StructCodec10P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            Put(transcoder, codec9, mapBuilder, name9, getter9.Invoke(value));
            Put(transcoder, codec10, mapBuilder, name10, getter10.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            var result9 = Get(transcoder, codec9, name9, map);
            var result10 = Get(transcoder, codec10, name10, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10);
        }
    }

    public class StructCodec11P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            Put(transcoder, codec9, mapBuilder, name9, getter9.Invoke(value));
            Put(transcoder, codec10, mapBuilder, name10, getter10.Invoke(value));
            Put(transcoder, codec11, mapBuilder, name11, getter11.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            var result9 = Get(transcoder, codec9, name9, map);
            var result10 = Get(transcoder, codec10, name10, map);
            var result11 = Get(transcoder, codec11, name11, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11);
        }
    }

    public class StructCodec12P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            Put(transcoder, codec9, mapBuilder, name9, getter9.Invoke(value));
            Put(transcoder, codec10, mapBuilder, name10, getter10.Invoke(value));
            Put(transcoder, codec11, mapBuilder, name11, getter11.Invoke(value));
            Put(transcoder, codec12, mapBuilder, name12, getter12.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            var result9 = Get(transcoder, codec9, name9, map);
            var result10 = Get(transcoder, codec10, name10, map);
            var result11 = Get(transcoder, codec11, name11, map);
            var result12 = Get(transcoder, codec12, name12, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12);
        }
    }

    public class StructCodec13P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        string name13, ICodec<P13> codec13, Func<Result, P13> getter13,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            Put(transcoder, codec9, mapBuilder, name9, getter9.Invoke(value));
            Put(transcoder, codec10, mapBuilder, name10, getter10.Invoke(value));
            Put(transcoder, codec11, mapBuilder, name11, getter11.Invoke(value));
            Put(transcoder, codec12, mapBuilder, name12, getter12.Invoke(value));
            Put(transcoder, codec13, mapBuilder, name13, getter13.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            var result9 = Get(transcoder, codec9, name9, map);
            var result10 = Get(transcoder, codec10, name10, map);
            var result11 = Get(transcoder, codec11, name11, map);
            var result12 = Get(transcoder, codec12, name12, map);
            var result13 = Get(transcoder, codec13, name13, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12, result13);
        }
    }

    public class StructCodec14P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        string name13, ICodec<P13> codec13, Func<Result, P13> getter13,
        string name14, ICodec<P14> codec14, Func<Result, P14> getter14,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            Put(transcoder, codec9, mapBuilder, name9, getter9.Invoke(value));
            Put(transcoder, codec10, mapBuilder, name10, getter10.Invoke(value));
            Put(transcoder, codec11, mapBuilder, name11, getter11.Invoke(value));
            Put(transcoder, codec12, mapBuilder, name12, getter12.Invoke(value));
            Put(transcoder, codec13, mapBuilder, name13, getter13.Invoke(value));
            Put(transcoder, codec14, mapBuilder, name14, getter14.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            var result9 = Get(transcoder, codec9, name9, map);
            var result10 = Get(transcoder, codec10, name10, map);
            var result11 = Get(transcoder, codec11, name11, map);
            var result12 = Get(transcoder, codec12, name12, map);
            var result13 = Get(transcoder, codec13, name13, map);
            var result14 = Get(transcoder, codec14, name14, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12, result13, result14);
        }
    }

    public class StructCodec15P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        string name13, ICodec<P13> codec13, Func<Result, P13> getter13,
        string name14, ICodec<P14> codec14, Func<Result, P14> getter14,
        string name15, ICodec<P15> codec15, Func<Result, P15> getter15,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            Put(transcoder, codec9, mapBuilder, name9, getter9.Invoke(value));
            Put(transcoder, codec10, mapBuilder, name10, getter10.Invoke(value));
            Put(transcoder, codec11, mapBuilder, name11, getter11.Invoke(value));
            Put(transcoder, codec12, mapBuilder, name12, getter12.Invoke(value));
            Put(transcoder, codec13, mapBuilder, name13, getter13.Invoke(value));
            Put(transcoder, codec14, mapBuilder, name14, getter14.Invoke(value));
            Put(transcoder, codec15, mapBuilder, name15, getter15.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            var result9 = Get(transcoder, codec9, name9, map);
            var result10 = Get(transcoder, codec10, name10, map);
            var result11 = Get(transcoder, codec11, name11, map);
            var result12 = Get(transcoder, codec12, name12, map);
            var result13 = Get(transcoder, codec13, name13, map);
            var result14 = Get(transcoder, codec14, name14, map);
            var result15 = Get(transcoder, codec15, name15, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12, result13, result14, result15);
        }
    }

    public class StructCodec16P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result>(
        string name1, ICodec<P1> codec1, Func<Result, P1> getter1,
        string name2, ICodec<P2> codec2, Func<Result, P2> getter2,
        string name3, ICodec<P3> codec3, Func<Result, P3> getter3,
        string name4, ICodec<P4> codec4, Func<Result, P4> getter4,
        string name5, ICodec<P5> codec5, Func<Result, P5> getter5,
        string name6, ICodec<P6> codec6, Func<Result, P6> getter6,
        string name7, ICodec<P7> codec7, Func<Result, P7> getter7,
        string name8, ICodec<P8> codec8, Func<Result, P8> getter8,
        string name9, ICodec<P9> codec9, Func<Result, P9> getter9,
        string name10, ICodec<P10> codec10, Func<Result, P10> getter10,
        string name11, ICodec<P11> codec11, Func<Result, P11> getter11,
        string name12, ICodec<P12> codec12, Func<Result, P12> getter12,
        string name13, ICodec<P13> codec13, Func<Result, P13> getter13,
        string name14, ICodec<P14> codec14, Func<Result, P14> getter14,
        string name15, ICodec<P15> codec15, Func<Result, P15> getter15,
        string name16, ICodec<P16> codec16, Func<Result, P16> getter16,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result> func)
        : StructCodec<Result>
    {
        public override T EncodeToMap<T>(ITranscoder<T> transcoder, Result value, IVirtualMapBuilder<T> mapBuilder)
        {
            Put(transcoder, codec1, mapBuilder, name1, getter1.Invoke(value));
            Put(transcoder, codec2, mapBuilder, name2, getter2.Invoke(value));
            Put(transcoder, codec3, mapBuilder, name3, getter3.Invoke(value));
            Put(transcoder, codec4, mapBuilder, name4, getter4.Invoke(value));
            Put(transcoder, codec5, mapBuilder, name5, getter5.Invoke(value));
            Put(transcoder, codec6, mapBuilder, name6, getter6.Invoke(value));
            Put(transcoder, codec7, mapBuilder, name7, getter7.Invoke(value));
            Put(transcoder, codec8, mapBuilder, name8, getter8.Invoke(value));
            Put(transcoder, codec9, mapBuilder, name9, getter9.Invoke(value));
            Put(transcoder, codec10, mapBuilder, name10, getter10.Invoke(value));
            Put(transcoder, codec11, mapBuilder, name11, getter11.Invoke(value));
            Put(transcoder, codec12, mapBuilder, name12, getter12.Invoke(value));
            Put(transcoder, codec13, mapBuilder, name13, getter13.Invoke(value));
            Put(transcoder, codec14, mapBuilder, name14, getter14.Invoke(value));
            Put(transcoder, codec15, mapBuilder, name15, getter15.Invoke(value));
            Put(transcoder, codec16, mapBuilder, name16, getter16.Invoke(value));
            return mapBuilder.Build();
        }

        public override Result DecodeFromMap<T>(ITranscoder<T> transcoder, IVirtualMap<T> map)
        {
            var result1 = Get(transcoder, codec1, name1, map);
            var result2 = Get(transcoder, codec2, name2, map);
            var result3 = Get(transcoder, codec3, name3, map);
            var result4 = Get(transcoder, codec4, name4, map);
            var result5 = Get(transcoder, codec5, name5, map);
            var result6 = Get(transcoder, codec6, name6, map);
            var result7 = Get(transcoder, codec7, name7, map);
            var result8 = Get(transcoder, codec8, name8, map);
            var result9 = Get(transcoder, codec9, name9, map);
            var result10 = Get(transcoder, codec10, name10, map);
            var result11 = Get(transcoder, codec11, name11, map);
            var result12 = Get(transcoder, codec12, name12, map);
            var result13 = Get(transcoder, codec13, name13, map);
            var result14 = Get(transcoder, codec14, name14, map);
            var result15 = Get(transcoder, codec15, name15, map);
            var result16 = Get(transcoder, codec16, name16, map);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12, result13, result14, result15, result16);
        }
    }
}