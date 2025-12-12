using System;
using Codon.Buffer;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

namespace Codon.Binary;

public static class BinaryCodec
{
    public static readonly IBinaryCodec<bool> Boolean = new BinaryCodecs.BooleanBinaryCodec();

    public static readonly IBinaryCodec<byte> Byte = new BinaryCodecs.ByteBinaryCodec();

    public static readonly IBinaryCodec<int> Int = new BinaryCodecs.IntBinaryCodec();

    public static readonly IBinaryCodec<long> Long = new BinaryCodecs.LongBinaryCodec();

    public static readonly IBinaryCodec<double> Double = new BinaryCodecs.DoubleBinaryCodec();

    public static readonly IBinaryCodec<float> Float = new BinaryCodecs.FloatBinaryCodec();

    public static readonly IBinaryCodec<int> VarInt = new BinaryCodecs.VarIntBinaryCodec();

    public static readonly IBinaryCodec<byte[]> ByteArray = new BinaryCodecs.ByteArrayBinaryCodec();

    public static readonly IBinaryCodec<BinaryBuffer> BinaryBuffer = new BinaryCodecs.BinaryBufferBinaryCodec();

    public static readonly IBinaryCodec<byte[]> RawBytes = new BinaryCodecs.RawBytesBinaryCodec();

    public static readonly IBinaryCodec<string> String = new BinaryCodecs.StringBinaryCodec();

    public static BinaryCodecs.RecursiveBinaryCodec<T> Recursive<T>(Func<IBinaryCodec<T>, IBinaryCodec<T>> self) where T : notnull
    {
        return new BinaryCodecs.RecursiveBinaryCodec<T>(self);
    }

    public static BinaryCodecs.EnumBinaryCodec<E> Enum<E>() where E : Enum
    {
        return new BinaryCodecs.EnumBinaryCodec<E>();
    }

    public static IBinaryCodec<R> Of<P1, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        Func<P1, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP1<P1, R>(codec1, getter1, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        Func<P1, P2, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP2<P1, P2, R>(codec1, getter1, codec2, getter2, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        Func<P1, P2, P3, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP3<P1, P2, P3, R>(codec1, getter1, codec2, getter2, codec3, getter3, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        Func<P1, P2, P3, P4, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP4<P1, P2, P3, P4, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        Func<P1, P2, P3, P4, P5, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP5<P1, P2, P3, P4, P5, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        Func<P1, P2, P3, P4, P5, P6, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP6<P1, P2, P3, P4, P5, P6, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        Func<P1, P2, P3, P4, P5, P6, P7, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP7<P1, P2, P3, P4, P5, P6, P7, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP8<P1, P2, P3, P4, P5, P6, P7, P8, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        IBinaryCodec<P9> codec9, Func<R, P9> getter9,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP9<P1, P2, P3, P4, P5, P6, P7, P8, P9, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, codec9, getter9, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        IBinaryCodec<P9> codec9, Func<R, P9> getter9,
        IBinaryCodec<P10> codec10, Func<R, P10> getter10,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP10<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, codec9, getter9, codec10, getter10, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        IBinaryCodec<P9> codec9, Func<R, P9> getter9,
        IBinaryCodec<P10> codec10, Func<R, P10> getter10,
        IBinaryCodec<P11> codec11, Func<R, P11> getter11,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP11<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, codec9, getter9, codec10, getter10, codec11, getter11, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        IBinaryCodec<P9> codec9, Func<R, P9> getter9,
        IBinaryCodec<P10> codec10, Func<R, P10> getter10,
        IBinaryCodec<P11> codec11, Func<R, P11> getter11,
        IBinaryCodec<P12> codec12, Func<R, P12> getter12,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP12<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, codec9, getter9, codec10, getter10, codec11, getter11, codec12, getter12, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        IBinaryCodec<P9> codec9, Func<R, P9> getter9,
        IBinaryCodec<P10> codec10, Func<R, P10> getter10,
        IBinaryCodec<P11> codec11, Func<R, P11> getter11,
        IBinaryCodec<P12> codec12, Func<R, P12> getter12,
        IBinaryCodec<P13> codec13, Func<R, P13> getter13,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP13<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, codec9, getter9, codec10, getter10, codec11, getter11, codec12, getter12, codec13, getter13, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        IBinaryCodec<P9> codec9, Func<R, P9> getter9,
        IBinaryCodec<P10> codec10, Func<R, P10> getter10,
        IBinaryCodec<P11> codec11, Func<R, P11> getter11,
        IBinaryCodec<P12> codec12, Func<R, P12> getter12,
        IBinaryCodec<P13> codec13, Func<R, P13> getter13,
        IBinaryCodec<P14> codec14, Func<R, P14> getter14,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP14<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, codec9, getter9, codec10, getter10, codec11, getter11, codec12, getter12, codec13, getter13, codec14, getter14, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        IBinaryCodec<P9> codec9, Func<R, P9> getter9,
        IBinaryCodec<P10> codec10, Func<R, P10> getter10,
        IBinaryCodec<P11> codec11, Func<R, P11> getter11,
        IBinaryCodec<P12> codec12, Func<R, P12> getter12,
        IBinaryCodec<P13> codec13, Func<R, P13> getter13,
        IBinaryCodec<P14> codec14, Func<R, P14> getter14,
        IBinaryCodec<P15> codec15, Func<R, P15> getter15,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP15<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, codec9, getter9, codec10, getter10, codec11, getter11, codec12, getter12, codec13, getter13, codec14, getter14, codec15, getter15, func);
    }

    public static IBinaryCodec<R> Of<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, R>
    (
        IBinaryCodec<P1> codec1, Func<R, P1> getter1,
        IBinaryCodec<P2> codec2, Func<R, P2> getter2,
        IBinaryCodec<P3> codec3, Func<R, P3> getter3,
        IBinaryCodec<P4> codec4, Func<R, P4> getter4,
        IBinaryCodec<P5> codec5, Func<R, P5> getter5,
        IBinaryCodec<P6> codec6, Func<R, P6> getter6,
        IBinaryCodec<P7> codec7, Func<R, P7> getter7,
        IBinaryCodec<P8> codec8, Func<R, P8> getter8,
        IBinaryCodec<P9> codec9, Func<R, P9> getter9,
        IBinaryCodec<P10> codec10, Func<R, P10> getter10,
        IBinaryCodec<P11> codec11, Func<R, P11> getter11,
        IBinaryCodec<P12> codec12, Func<R, P12> getter12,
        IBinaryCodec<P13> codec13, Func<R, P13> getter13,
        IBinaryCodec<P14> codec14, Func<R, P14> getter14,
        IBinaryCodec<P15> codec15, Func<R, P15> getter15,
        IBinaryCodec<P16> codec16, Func<R, P16> getter16,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, R> func
    )
    {
        return new BinaryCodecs.BinaryCodecP16<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, R>(codec1, getter1, codec2, getter2, codec3, getter3, codec4, getter4, codec5, getter5, codec6, getter6, codec7, getter7, codec8, getter8, codec9, getter9, codec10, getter10, codec11, getter11, codec12, getter12, codec13, getter13, codec14, getter14, codec15, getter15, codec16, getter16, func);
    }
}

public interface IBinaryCodec<T>
{
    public void Write(BinaryBuffer buffer, T value);
    public T Read(BinaryBuffer buffer);

    public BinaryCodecs.OptionalBinaryCodec<T> Optional()
    {
        return new BinaryCodecs.OptionalBinaryCodec<T>(this);
    }

    public BinaryCodecs.DefaultBinaryCodec<T> Default(T defaultValue)
    {
        return new BinaryCodecs.DefaultBinaryCodec<T>(this, defaultValue);
    }

    public BinaryCodecs.TransformativeBinaryCodec<T, S> Transform<S>(Func<S, T> from, Func<T, S> to)
    {
        return new BinaryCodecs.TransformativeBinaryCodec<T, S>(this, from, to);
    }

    public BinaryCodecs.DictionaryBinaryCodec<T, V> MapTo<V>(IBinaryCodec<V> valueCodec) where V : notnull
    {
        return new BinaryCodecs.DictionaryBinaryCodec<T, V>(this, valueCodec);
    }

    public BinaryCodecs.ListBinaryCodec<T> List()
    {
        return new BinaryCodecs.ListBinaryCodec<T>(this);
    }

    public BinaryCodecs.UnionBinaryCodec<K, T> Union<K>(Func<T, IBinaryCodec<K>> serializerFactory, Func<K, T> keyFunc) where K : notnull
    {
        return new BinaryCodecs.UnionBinaryCodec<K, T>(this, keyFunc, serializerFactory);
    }
}