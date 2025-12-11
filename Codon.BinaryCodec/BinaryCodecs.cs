using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Codon.Buffer;
using Codon.Buffer.Extensions;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

namespace Codon.Binary;

public static class BinaryCodecs
{

    public class BooleanBinaryCodec : BinaryCodec<bool>
    {
        public void Write(BinaryBuffer buffer, bool value)
        {
            buffer.WriteBoolean(value);
        }

        public bool Read(BinaryBuffer buffer)
        {
            return buffer.ReadBoolean();
        }
    }

    public class ByteBinaryCodec : BinaryCodec<byte>
    {
        public void Write(BinaryBuffer buffer, byte value)
        {
            buffer.WriteByte(value);
        }

        public byte Read(BinaryBuffer buffer)
        {
            return buffer.ReadByte();
        }
    }

    public class IntBinaryCodec : BinaryCodec<int>
    {
        public void Write(BinaryBuffer buffer, int value)
        {
            buffer.WriteInt(value);
        }

        public int Read(BinaryBuffer buffer)
        {
            return buffer.ReadInt();
        }
    }

    public class LongBinaryCodec : BinaryCodec<long>
    {
        public void Write(BinaryBuffer buffer, long value)
        {
            buffer.WriteLong(value);
        }

        public long Read(BinaryBuffer buffer)
        {
            return buffer.ReadLong();
        }
    }

    public class FloatBinaryCodec : BinaryCodec<float>
    {
        public void Write(BinaryBuffer buffer, float value)
        {
            buffer.WriteFloat(value);
        }

        public float Read(BinaryBuffer buffer)
        {
            return buffer.ReadFloat();
        }
    }

    public class DoubleBinaryCodec : BinaryCodec<double>
    {
        public void Write(BinaryBuffer buffer, double value)
        {
            buffer.WriteDouble(value);
        }

        public double Read(BinaryBuffer buffer)
        {
            return buffer.ReadDouble();
        }
    }

    public class VarIntBinaryCodec : BinaryCodec<int>
    {
        private const int SEGMENT_BITS = 0x7F;
        private const int CONTINUE_BIT = 0x80;

        public void Write(BinaryBuffer buffer, int value)
        {
            var uValue = (uint)value;
            while (true)
            {
                if ((uValue & ~SEGMENT_BITS) == 0)
                {
                    buffer.WriteByte((byte)uValue);
                    break;
                }

                buffer.WriteByte((byte)((uValue & SEGMENT_BITS) | CONTINUE_BIT));

                uValue >>= 7;
            }
        }

        public int Read(BinaryBuffer buffer)
        {
            var value = 0;
            var position = 0;

            while (position < 35) // Max 5 bytes
            {
                var currentByte = buffer.ReadByte();
                var segment = currentByte & SEGMENT_BITS;

                value |= segment << position;

                // finished reading
                if ((currentByte & CONTINUE_BIT) == 0) return value;

                position += 7;
            }

            throw new InvalidDataException("VarInt is too long");
        }
    }

    public class ByteArrayBinaryCodec : BinaryCodec<byte[]>
    {
        public void Write(BinaryBuffer buffer, byte[] value)
        {
            BinaryCodec.VarInt.Write(buffer, value.Length);
            buffer.WriteBytes(value);
        }

        public byte[] Read(BinaryBuffer buffer)
        {
            var size = BinaryCodec.VarInt.Read(buffer);
            return buffer.ReadBytes(size).ToArray();
        }
    }

    public class BinaryBufferBinaryCodec : BinaryCodec<BinaryBuffer>
    {
        public void Write(BinaryBuffer buffer, BinaryBuffer value)
        {
            var array = value.ToArray();
            BinaryCodec.VarInt.Write(buffer, array.Length);
            buffer.WriteBytes(array);
        }

        public BinaryBuffer Read(BinaryBuffer buffer)
        {
            var size = BinaryCodec.VarInt.Read(buffer);
            return buffer.ReadBytes(size).ToArray().ToBinaryBuffer();
        }
    }

    public class StringBinaryCodec : BinaryCodec<string>
    {
        public void Write(BinaryBuffer buffer, string value)
        {
            var stringBytes = Encoding.UTF8.GetBytes(value);
            BinaryCodec.VarInt.Write(buffer, stringBytes.Length);
            buffer.WriteBytes(stringBytes);
        }

        public string Read(BinaryBuffer buffer)
        {
            var size = BinaryCodec.VarInt.Read(buffer);
            if (size < 0) throw new InvalidDataException("String cannot have negative length");
            var stringBytes = buffer.ReadBytes(size);
            return Encoding.UTF8.GetString(stringBytes);
        }
    }

    public class RawBytesBinaryCodec : BinaryCodec<byte[]>
    {
        public void Write(BinaryBuffer buffer, byte[] value)
        {
            buffer.WriteBytes(value);
        }

        public byte[] Read(BinaryBuffer buffer)
        {
            return buffer.ReadBytes(buffer.Length).ToArray();
        }
    }

    public class OptionalBinaryCodec<T>(BinaryCodec<T> innerCodec) : BinaryCodec<T?> where T : notnull
    {
        public void Write(BinaryBuffer buffer, T? value)
        {
            BinaryCodec.Boolean.Write(buffer, value != null);
            if (value != null) innerCodec.Write(buffer, value);
        }

        public T? Read(BinaryBuffer buffer)
        {
            return BinaryCodec.Boolean.Read(buffer) ? innerCodec.Read(buffer) : default;
        }
    }

    public class DefaultBinaryCodec<T>(BinaryCodec<T> innerCodec, T defaultValue) : BinaryCodec<T> where T : notnull
    {
        public void Write(BinaryBuffer buffer, T? value)
        {
            innerCodec.Write(buffer, value ?? defaultValue);
        }

        public T Read(BinaryBuffer buffer)
        {
            return BinaryCodec.Boolean.Read(buffer) ? innerCodec.Read(buffer) : defaultValue;
        }
    }

    public class TransformativeBinaryCodec<T, S>(BinaryCodec<T> innerCodec, Func<S, T> from, Func<T, S> to) : BinaryCodec<S> where S : notnull where T : notnull
    {
        public void Write(BinaryBuffer buffer, S value)
        {
            innerCodec.Write(buffer, from.Invoke(value));
        }

        public S Read(BinaryBuffer buffer)
        {
            var innerValue = innerCodec.Read(buffer);
            return to.Invoke(innerValue);
        }
    }

    public class DictionaryBinaryCodec<K, V>(BinaryCodec<K> keyCodec, BinaryCodec<V> valueCodec) : BinaryCodec<Dictionary<K, V>> where K : notnull where V : notnull
    {
        public void Write(BinaryBuffer buffer, Dictionary<K, V> value)
        {
            BinaryCodec.VarInt.Write(buffer, value.Count);
            foreach (var keyValuePair in value)
            {
                keyCodec.Write(buffer, keyValuePair.Key);
                valueCodec.Write(buffer, keyValuePair.Value);
            }
        }

        public Dictionary<K, V> Read(BinaryBuffer buffer)
        {
            var dict = new Dictionary<K, V>();
            var size = BinaryCodec.VarInt.Read(buffer); // how big is my dict...

            for (var i = 0; i < size; i++)
            {
                var key = keyCodec.Read(buffer);
                var value = valueCodec.Read(buffer);
                dict[key] = value;
            }

            return dict;
        }
    }

    public class ListBinaryCodec<T>(BinaryCodec<T> innerCodec) : BinaryCodec<List<T>> where T : notnull
    {
        public void Write(BinaryBuffer buffer, List<T> value)
        {
            BinaryCodec.VarInt.Write(buffer, value.Count);
            value.ForEach(item => innerCodec.Write(buffer, item));
        }

        public List<T> Read(BinaryBuffer buffer)
        {
            var list = new List<T>();
            var size = BinaryCodec.VarInt.Read(buffer);

            for (var i = 0; i < size; i++) list.Add(innerCodec.Read(buffer));

            return list;
        }
    }

    public class UnionBinaryCodec<T, K>(BinaryCodec<K> keyCodec, Func<T, K> keyFunc, Func<K, BinaryCodec<T>> serializerFactory) : BinaryCodec<T>
    {
        public void Write(BinaryBuffer buffer, T value)
        {
            var key = keyFunc.Invoke(value);
            keyCodec.Write(buffer, key);

            var serializer = serializerFactory.Invoke(key);
            serializer.Write(buffer, value);
        }

        public T Read(BinaryBuffer buffer)
        {
            var key = keyCodec.Read(buffer);
            var serializer = serializerFactory.Invoke(key);
            return serializer.Read(buffer);
        }
    }

    public class RecursiveBinaryCodec<T> : BinaryCodec<T> where T : notnull
    {
        private readonly Lazy<BinaryCodec<T>> _delegate;

        public RecursiveBinaryCodec(Func<BinaryCodec<T>, BinaryCodec<T>> self)
        {
            _delegate = new Lazy<BinaryCodec<T>>(() => self.Invoke(this));
        }

        public void Write(BinaryBuffer buffer, T value)
        {
            _delegate.Value.Write(buffer, value);
        }

        public T Read(BinaryBuffer buffer)
        {
            return _delegate.Value.Read(buffer);
        }
    }

    public class EnumBinaryCodec<E> : BinaryCodec<E> where E : Enum
    {
        private Array _entries = System.Enum.GetValues(typeof(E));

        public void Write(BinaryBuffer buffer, E value)
        {
            var ordinal = Array.IndexOf(_entries, value);
            BinaryCodec.VarInt.Write(buffer, ordinal);
        }

        public E Read(BinaryBuffer buffer)
        {
            var ordinal = BinaryCodec.VarInt.Read(buffer);
            if (ordinal < 0 || ordinal >= _entries.Length) throw new IndexOutOfRangeException($"Ordinal {ordinal} is outside the range [0, {_entries.Length - 1}] for enum {typeof(E).Name}");

            return (E)_entries.GetValue(ordinal)!;
        }
    }
}