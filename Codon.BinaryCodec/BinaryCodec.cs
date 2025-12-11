using System;
using Codon.Buffer;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

namespace Codon.Binary
{
    public static class BinaryCodec
    {
        public static readonly BinaryCodecs.BooleanBinaryCodec Boolean = new();

        public static readonly BinaryCodecs.ByteBinaryCodec Byte = new();

        public static readonly BinaryCodecs.IntBinaryCodec Int = new();

        public static readonly BinaryCodecs.LongBinaryCodec Long = new();

        public static readonly BinaryCodecs.DoubleBinaryCodec Double = new();

        public static readonly BinaryCodecs.FloatBinaryCodec Float = new();

        public static readonly BinaryCodecs.VarIntBinaryCodec VarInt = new();

        public static readonly BinaryCodecs.ByteArrayBinaryCodec ByteArray = new();

        public static readonly BinaryCodecs.BinaryBufferBinaryCodec BinaryBuffer = new();

        public static readonly BinaryCodecs.RawBytesBinaryCodec RawBytes = new();

        public static readonly BinaryCodecs.StringBinaryCodec String = new();

        public static BinaryCodecs.RecursiveBinaryCodec<T> Recursive<T>(Func<BinaryCodec<T>, BinaryCodec<T>> self) where T : notnull
        {
            return new BinaryCodecs.RecursiveBinaryCodec<T>(self);
        }

        public static BinaryCodecs.EnumBinaryCodec<E> Enum<E>() where E : Enum
        {
            return new BinaryCodecs.EnumBinaryCodec<E>();
        }
    }

    public interface BinaryCodec<T>
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

        public BinaryCodecs.DictionaryBinaryCodec<T, V> MapTo<V>(BinaryCodec<V> valueCodec) where V : notnull
        {
            return new BinaryCodecs.DictionaryBinaryCodec<T, V>(this, valueCodec);
        }

        public BinaryCodecs.ListBinaryCodec<T> List()
        {
            return new BinaryCodecs.ListBinaryCodec<T>(this);
        }

        public BinaryCodecs.UnionBinaryCodec<K, T> Union<K>(Func<T, BinaryCodec<K>> serializerFactory, Func<K, T> keyFunc) where K : notnull
        {
            return new BinaryCodecs.UnionBinaryCodec<K, T>(this, keyFunc, serializerFactory);
        }
    }
}