using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Codon.Buffer;
using Codon.Buffer.Extensions;
using Codon.Codec;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

namespace Codon.Binary;

public static class BinaryCodecs
{
    public class BooleanBinaryCodec : IBinaryCodec<bool>
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

    public class ByteBinaryCodec : IBinaryCodec<byte>
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

    public class IntBinaryCodec : IBinaryCodec<int>
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

    public class LongBinaryCodec : IBinaryCodec<long>
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

    public class FloatBinaryCodec : IBinaryCodec<float>
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

    public class DoubleBinaryCodec : IBinaryCodec<double>
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

    public class VarIntBinaryCodec : IBinaryCodec<int>
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

    public class ByteArrayBinaryCodec : IBinaryCodec<byte[]>
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

    public class BinaryBufferBinaryCodec : IBinaryCodec<BinaryBuffer>
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

    public class StringBinaryCodec : IBinaryCodec<string>
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

    public class RawBytesBinaryCodec : IBinaryCodec<byte[]>
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

    public class OptionalBinaryCodec<T>(IBinaryCodec<T> innerCodec) : IBinaryCodec<Optional<T>>
    {
        public void Write(BinaryBuffer buffer, Optional<T> value)
        {
            BinaryCodec.Boolean.Write(buffer, value.IsPresent);
            if (value.IsPresent) innerCodec.Write(buffer, value.Value!);
        }

        public Optional<T> Read(BinaryBuffer buffer)
        {
            return BinaryCodec.Boolean.Read(buffer) ? Optional.Of(innerCodec.Read(buffer)) : Optional.Empty<T>();
        }
    }

    public class DefaultBinaryCodec<T>(IBinaryCodec<T> innerCodec, T defaultValue) : IBinaryCodec<T> where T : notnull
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

    public class TransformativeBinaryCodec<T, S>(IBinaryCodec<T> innerCodec, Func<S, T> from, Func<T, S> to) : IBinaryCodec<S> where S : notnull where T : notnull
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

    public class DictionaryBinaryCodec<K, V>(IBinaryCodec<K> keyCodec, IBinaryCodec<V> valueCodec) : IBinaryCodec<Dictionary<K, V>> where K : notnull where V : notnull
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

    public class ListBinaryCodec<T>(IBinaryCodec<T> innerCodec) : IBinaryCodec<List<T>>
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

    public class UnionBinaryCodec<T, K>(IBinaryCodec<K> keyCodec, Func<T, K> keyFunc, Func<K, IBinaryCodec<T>> serializerFactory) : IBinaryCodec<T>
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

    public class RecursiveBinaryCodec<T> : IBinaryCodec<T> where T : notnull
    {
        private readonly Lazy<IBinaryCodec<T>> _delegate;

        public RecursiveBinaryCodec(Func<IBinaryCodec<T>, IBinaryCodec<T>> self)
        {
            _delegate = new Lazy<IBinaryCodec<T>>(() => self.Invoke(this));
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

    public class EnumBinaryCodec<E> : IBinaryCodec<E> where E : Enum
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

    public class BinaryCodecP1<P1, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        Func<P1, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            return func.Invoke(result1);
        }
    }

    public class BinaryCodecP2<P1, P2, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        Func<P1, P2, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            return func.Invoke(result1, result2);
        }
    }

    public class BinaryCodecP3<P1, P2, P3, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        Func<P1, P2, P3, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            return func.Invoke(result1, result2, result3);
        }
    }

    public class BinaryCodecP4<P1, P2, P3, P4, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        Func<P1, P2, P3, P4, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            return func.Invoke(result1, result2, result3, result4);
        }
    }

    public class BinaryCodecP5<P1, P2, P3, P4, P5, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        Func<P1, P2, P3, P4, P5, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5);
        }
    }

    public class BinaryCodecP6<P1, P2, P3, P4, P5, P6, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        Func<P1, P2, P3, P4, P5, P6, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6);
        }
    }

    public class BinaryCodecP7<P1, P2, P3, P4, P5, P6, P7, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        Func<P1, P2, P3, P4, P5, P6, P7, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7);
        }
    }

    public class BinaryCodecP8<P1, P2, P3, P4, P5, P6, P7, P8, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8);
        }
    }

    public class BinaryCodecP9<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9, Func<Result, P9> getter9,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
            codec9.Write(buffer, getter9.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            var result9 = codec9.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9);
        }
    }

    public class BinaryCodecP10<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9, Func<Result, P9> getter9,
        IBinaryCodec<P10> codec10, Func<Result, P10> getter10,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
            codec9.Write(buffer, getter9.Invoke(value));
            codec10.Write(buffer, getter10.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            var result9 = codec9.Read(buffer);
            var result10 = codec10.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10);
        }
    }

    public class BinaryCodecP11<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9, Func<Result, P9> getter9,
        IBinaryCodec<P10> codec10, Func<Result, P10> getter10,
        IBinaryCodec<P11> codec11, Func<Result, P11> getter11,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
            codec9.Write(buffer, getter9.Invoke(value));
            codec10.Write(buffer, getter10.Invoke(value));
            codec11.Write(buffer, getter11.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            var result9 = codec9.Read(buffer);
            var result10 = codec10.Read(buffer);
            var result11 = codec11.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11);
        }
    }

    public class BinaryCodecP12<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9, Func<Result, P9> getter9,
        IBinaryCodec<P10> codec10, Func<Result, P10> getter10,
        IBinaryCodec<P11> codec11, Func<Result, P11> getter11,
        IBinaryCodec<P12> codec12, Func<Result, P12> getter12,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
            codec9.Write(buffer, getter9.Invoke(value));
            codec10.Write(buffer, getter10.Invoke(value));
            codec11.Write(buffer, getter11.Invoke(value));
            codec12.Write(buffer, getter12.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            var result9 = codec9.Read(buffer);
            var result10 = codec10.Read(buffer);
            var result11 = codec11.Read(buffer);
            var result12 = codec12.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12);
        }
    }

    public class BinaryCodecP13<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9, Func<Result, P9> getter9,
        IBinaryCodec<P10> codec10, Func<Result, P10> getter10,
        IBinaryCodec<P11> codec11, Func<Result, P11> getter11,
        IBinaryCodec<P12> codec12, Func<Result, P12> getter12,
        IBinaryCodec<P13> codec13, Func<Result, P13> getter13,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
            codec9.Write(buffer, getter9.Invoke(value));
            codec10.Write(buffer, getter10.Invoke(value));
            codec11.Write(buffer, getter11.Invoke(value));
            codec12.Write(buffer, getter12.Invoke(value));
            codec13.Write(buffer, getter13.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            var result9 = codec9.Read(buffer);
            var result10 = codec10.Read(buffer);
            var result11 = codec11.Read(buffer);
            var result12 = codec12.Read(buffer);
            var result13 = codec13.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12, result13);
        }
    }

    public class BinaryCodecP14<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9, Func<Result, P9> getter9,
        IBinaryCodec<P10> codec10, Func<Result, P10> getter10,
        IBinaryCodec<P11> codec11, Func<Result, P11> getter11,
        IBinaryCodec<P12> codec12, Func<Result, P12> getter12,
        IBinaryCodec<P13> codec13, Func<Result, P13> getter13,
        IBinaryCodec<P14> codec14, Func<Result, P14> getter14,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
            codec9.Write(buffer, getter9.Invoke(value));
            codec10.Write(buffer, getter10.Invoke(value));
            codec11.Write(buffer, getter11.Invoke(value));
            codec12.Write(buffer, getter12.Invoke(value));
            codec13.Write(buffer, getter13.Invoke(value));
            codec14.Write(buffer, getter14.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            var result9 = codec9.Read(buffer);
            var result10 = codec10.Read(buffer);
            var result11 = codec11.Read(buffer);
            var result12 = codec12.Read(buffer);
            var result13 = codec13.Read(buffer);
            var result14 = codec14.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12, result13, result14);
        }
    }

    public class BinaryCodecP15<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9, Func<Result, P9> getter9,
        IBinaryCodec<P10> codec10, Func<Result, P10> getter10,
        IBinaryCodec<P11> codec11, Func<Result, P11> getter11,
        IBinaryCodec<P12> codec12, Func<Result, P12> getter12,
        IBinaryCodec<P13> codec13, Func<Result, P13> getter13,
        IBinaryCodec<P14> codec14, Func<Result, P14> getter14,
        IBinaryCodec<P15> codec15, Func<Result, P15> getter15,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
            codec9.Write(buffer, getter9.Invoke(value));
            codec10.Write(buffer, getter10.Invoke(value));
            codec11.Write(buffer, getter11.Invoke(value));
            codec12.Write(buffer, getter12.Invoke(value));
            codec13.Write(buffer, getter13.Invoke(value));
            codec14.Write(buffer, getter14.Invoke(value));
            codec15.Write(buffer, getter15.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            var result9 = codec9.Read(buffer);
            var result10 = codec10.Read(buffer);
            var result11 = codec11.Read(buffer);
            var result12 = codec12.Read(buffer);
            var result13 = codec13.Read(buffer);
            var result14 = codec14.Read(buffer);
            var result15 = codec15.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12, result13, result14, result15);
        }
    }

    public class BinaryCodecP16<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result>
    (
        IBinaryCodec<P1> codec1, Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2, Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3, Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4, Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5, Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6, Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7, Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8, Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9, Func<Result, P9> getter9,
        IBinaryCodec<P10> codec10, Func<Result, P10> getter10,
        IBinaryCodec<P11> codec11, Func<Result, P11> getter11,
        IBinaryCodec<P12> codec12, Func<Result, P12> getter12,
        IBinaryCodec<P13> codec13, Func<Result, P13> getter13,
        IBinaryCodec<P14> codec14, Func<Result, P14> getter14,
        IBinaryCodec<P15> codec15, Func<Result, P15> getter15,
        IBinaryCodec<P16> codec16, Func<Result, P16> getter16,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result> func
    ) : IBinaryCodec<Result>
    {
        public void Write(BinaryBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
            codec8.Write(buffer, getter8.Invoke(value));
            codec9.Write(buffer, getter9.Invoke(value));
            codec10.Write(buffer, getter10.Invoke(value));
            codec11.Write(buffer, getter11.Invoke(value));
            codec12.Write(buffer, getter12.Invoke(value));
            codec13.Write(buffer, getter13.Invoke(value));
            codec14.Write(buffer, getter14.Invoke(value));
            codec15.Write(buffer, getter15.Invoke(value));
            codec16.Write(buffer, getter16.Invoke(value));
        }

        public Result Read(BinaryBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            var result6 = codec6.Read(buffer);
            var result7 = codec7.Read(buffer);
            var result8 = codec8.Read(buffer);
            var result9 = codec9.Read(buffer);
            var result10 = codec10.Read(buffer);
            var result11 = codec11.Read(buffer);
            var result12 = codec12.Read(buffer);
            var result13 = codec13.Read(buffer);
            var result14 = codec14.Read(buffer);
            var result15 = codec15.Read(buffer);
            var result16 = codec16.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5, result6, result7, result8, result9, result10, result11, result12, result13, result14, result15, result16);
        }
    }
}