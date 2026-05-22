// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Runtime.CompilerServices;
using DotNetty.Buffers;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

namespace Codon.Binary;

public static class BinaryCodecs
{
    public static readonly IBinaryCodec<bool> BOOLEAN = new BinaryCodecDefinitions.BooleanBinaryCodec();

    public static readonly IBinaryCodec<byte> BYTE = new BinaryCodecDefinitions.ByteBinaryCodec();

    public static readonly IBinaryCodec<int> INT = new BinaryCodecDefinitions.IntBinaryCodec();

    public static readonly IBinaryCodec<long> LONG = new BinaryCodecDefinitions.LongBinaryCodec();

    public static readonly IBinaryCodec<double> DOUBLE = new BinaryCodecDefinitions.DoubleBinaryCodec();

    public static readonly IBinaryCodec<float> FLOAT = new BinaryCodecDefinitions.FloatBinaryCodec();

    public static readonly IBinaryCodec<int> VAR_INT = new BinaryCodecDefinitions.VarIntBinaryCodec();

    public static readonly IBinaryCodec<byte[]> BYTE_ARRAY = new BinaryCodecDefinitions.ByteArrayBinaryCodec();

    public static readonly IBinaryCodec<IByteBuffer> BYTE_BUFFER = new BinaryCodecDefinitions.ByteBufferBinaryCodec();

    public static readonly IBinaryCodec<byte[]> RAW_BYTES = new BinaryCodecDefinitions.RawBytesBinaryCodec();

    public static readonly IBinaryCodec<string> STRING = new BinaryCodecDefinitions.StringBinaryCodec();

    public static readonly IBinaryCodec<Guid> GUID = BYTE_ARRAY.Transform(guid => guid.ToByteArray(), bytes => new Guid(bytes));

    public static BinaryCodecDefinitions.RecursiveBinaryCodec<T> Recursive<T>(Func<IBinaryCodec<T>, IBinaryCodec<T>> self) where T : notnull => new(self);

    public static IBinaryCodec<byte[]> ByteArray(int? maxSize = null) => new BinaryCodecDefinitions.ByteArrayBinaryCodec(maxSize);

    public static IBinaryCodec<IByteBuffer> ByteBuffer(int? maxSize = null) => new BinaryCodecDefinitions.ByteBufferBinaryCodec(maxSize);

    public static BinaryCodecDefinitions.EnumBinaryCodec<E> Enum<E>() where E : Enum => new();

    public static IBinaryCodec<Te> Flags<Te>() where Te : struct, Enum => BYTE.Transform(te => Unsafe.As<Te, byte>(ref te), by => Unsafe.As<byte, Te>(ref by));

    public static IBinaryCodec<string> String(int maxLength) => new BinaryCodecDefinitions.StringBinaryCodec(maxLength);

    public static IBinaryCodec<R> Empty<R>(Func<R> func) => new BinaryCodecDefinitions.BinaryCodecEmpty<R>(func);

    public static BinaryCodecBuilder<T> For<T>() => new();
}

public interface IBinaryCodec<T>
{
    void Write(IByteBuffer buffer, T value);
    T Read(IByteBuffer buffer);

    BinaryCodecDefinitions.OptionalBinaryCodec<T> Optional()
    {
        return new BinaryCodecDefinitions.OptionalBinaryCodec<T>(this);
    }

    BinaryCodecDefinitions.DefaultBinaryCodec<T> Default(T defaultValue)
    {
        return new BinaryCodecDefinitions.DefaultBinaryCodec<T>(this, defaultValue);
    }

    BinaryCodecDefinitions.TransformativeBinaryCodec<T, S> Transform<S>(Func<S, T> from, Func<T, S> to)
    {
        return new BinaryCodecDefinitions.TransformativeBinaryCodec<T, S>(this, from, to);
    }

    BinaryCodecDefinitions.DictionaryBinaryCodec<T, V> MapTo<V>(IBinaryCodec<V> valueCodec, int? maxSize = null) where V : notnull
    {
        return new BinaryCodecDefinitions.DictionaryBinaryCodec<T, V>(this, valueCodec, maxSize);
    }

    BinaryCodecDefinitions.ListBinaryCodec<T> List(int? maxSize = null)
    {
        return new BinaryCodecDefinitions.ListBinaryCodec<T>(this, maxSize);
    }

    BinaryCodecDefinitions.UnionBinaryCodec<K, T> Union<K>(Func<T, IBinaryCodec<K>> serializerFactory, Func<K, T> keyFunc) where K : notnull
    {
        return new BinaryCodecDefinitions.UnionBinaryCodec<K, T>(this, keyFunc, serializerFactory);
    }
}
