using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Codon.Optionals;
using DotNetty.Buffers;


namespace Codon.Binary;

public static class BinaryCodecs
{
    public class BooleanBinaryCodec : BinaryCodec<bool>
    {
        public void Write(IByteBuffer buffer, bool value)
        {
            buffer.WriteBoolean(value);
        }

        public bool Read(IByteBuffer buffer)
        {
            return buffer.ReadBoolean();
        }
    }

    public class ByteBinaryCodec : BinaryCodec<byte>
    {
        public void Write(IByteBuffer buffer, byte value)
        {
            buffer.WriteByte(value);
        }

        public byte Read(IByteBuffer buffer)
        {
            return buffer.ReadByte();
        }
    }

    public class IntBinaryCodec : BinaryCodec<int>
    {
        public void Write(IByteBuffer buffer, int value)
        {
            buffer.WriteInt(value);
        }

        public int Read(IByteBuffer buffer)
        {
            return buffer.ReadInt();
        }
    }

    public class LongBinaryCodec : BinaryCodec<long>
    {
        public void Write(IByteBuffer buffer, long value)
        {
            buffer.WriteLong(value);
        }

        public long Read(IByteBuffer buffer)
        {
            return buffer.ReadLong();
        }
    }

    public class FloatBinaryCodec : BinaryCodec<float>
    {
        public void Write(IByteBuffer buffer, float value)
        {
            buffer.WriteFloat(value);
        }

        public float Read(IByteBuffer buffer)
        {
            return buffer.ReadFloat();
        }
    }

    public class DoubleBinaryCodec : BinaryCodec<double>
    {
        public void Write(IByteBuffer buffer, double value)
        {
            buffer.WriteDouble(value);
        }

        public double Read(IByteBuffer buffer)
        {
            return buffer.ReadDouble();
        }
    }

    public class VarIntBinaryCodec : BinaryCodec<int>
    {
        private const int segment_bits = 0x7F;
        private const int continue_bit = 0x80;

        public void Write(IByteBuffer buffer, int value)
        {
            var uValue = (uint)value;
            while (true)
            {
                if ((uValue & ~segment_bits) == 0)
                {
                    buffer.WriteByte((byte)uValue);
                    break;
                }

                buffer.WriteByte((byte)((uValue & segment_bits) | continue_bit));

                uValue >>= 7;
            }
        }

        public int Read(IByteBuffer buffer)
        {
            var value = 0;
            var position = 0;

            while (position < 35) // Max 5 bytes
            {
                var currentByte = buffer.ReadByte();
                var segment = currentByte & segment_bits;

                value |= segment << position;

                // finished reading
                if ((currentByte & continue_bit) == 0) return value;

                position += 7;
            }

            throw new InvalidDataException("VarInt is too long");
        }
    }

    public class ByteArrayBinaryCodec(int? maxSize = null) : BinaryCodec<byte[]>
    {
        public void Write(IByteBuffer buffer, byte[] value)
        {
            if (maxSize != null && value.Length > maxSize)
                throw new ArgumentException($"The byte array is longer than maximum allowed ({value.Length} > {maxSize})", nameof(value));

            BinaryCodec.VAR_INT.Write(buffer, value.Length);
            buffer.WriteBytes(value);
        }

        public byte[] Read(IByteBuffer buffer)
        {
            var size = BinaryCodec.VAR_INT.Read(buffer);
            if (size > maxSize)
                throw new InvalidDataException($"The read byte array is longer than maximum allowed (${size} > {maxSize}");

            var destination = new byte[size];
            buffer.ReadBytes(destination);
            return destination;
        }
    }

    public class ByteBufferBinaryCodec(int? maxSize = null) : BinaryCodec<IByteBuffer>
    {
        public void Write(IByteBuffer buffer, IByteBuffer value)
        {
            if (maxSize != null && value.ReadableBytes > maxSize)
                throw new ArgumentException($"The byte buffer is longer than maximum allowed ({value.ReadableBytes} > {maxSize})", nameof(value));

            var array = value.Array;
            BinaryCodec.VAR_INT.Write(buffer, array.Length);
            buffer.WriteBytes(array);
        }

        public IByteBuffer Read(IByteBuffer buffer)
        {
            var size = BinaryCodec.VAR_INT.Read(buffer);
            if (size > maxSize)
                throw new InvalidDataException($"The read byte buffer is longer than maximum allowed (${size} > {maxSize}");

            return buffer.ReadBytes(size);
        }
    }

    public class StringBinaryCodec(int? maxStringLength = null) : BinaryCodec<string>
    {
        public void Write(IByteBuffer buffer, string value)
        {
            if (maxStringLength != null && value.Length > maxStringLength)
                throw new ArgumentException($"String is longer than maximum allowed (${value.Length} > {maxStringLength})", nameof(value));
            var stringBytes = Encoding.UTF8.GetBytes(value);
            BinaryCodec.VAR_INT.Write(buffer, stringBytes.Length);
            buffer.WriteBytes(stringBytes);
        }

        public string Read(IByteBuffer buffer)
        {
            var size = BinaryCodec.VAR_INT.Read(buffer);
            if (size < 0) throw new InvalidDataException("The read string has negative length");
            if (size == 0) return string.Empty;
            if (size > maxStringLength)
                throw new InvalidDataException($"Read string is longer than maximum allowed (${size} > {maxStringLength}");

            var stringBytes = buffer.ReadBytes(size).ToByteArraySafe();
            return Encoding.UTF8.GetString(stringBytes);
        }
    }

    public class RawBytesBinaryCodec : BinaryCodec<byte[]>
    {
        public void Write(IByteBuffer buffer, byte[] value)
        {
            buffer.WriteBytes(value);
        }

        public byte[] Read(IByteBuffer buffer)
        {
            return buffer.ReadBytes(buffer.Array.Length).ToByteArraySafe();
        }
    }

    public class OptionalBinaryCodec<T>(BinaryCodec<T> innerCodec) : BinaryCodec<Optional<T>>
    {
        public void Write(IByteBuffer buffer, Optional<T> value)
        {
            BinaryCodec.BOOLEAN.Write(buffer, value.IsPresent);
            if (value.IsPresent) innerCodec.Write(buffer, value.Value!);
        }

        public Optional<T> Read(IByteBuffer buffer)
        {
            return BinaryCodec.BOOLEAN.Read(buffer) ? Optional.Of(innerCodec.Read(buffer)) : Optional.Empty<T>();
        }
    }

    public class DefaultBinaryCodec<T>(BinaryCodec<T> innerCodec, T defaultValue) : BinaryCodec<T> where T : notnull
    {
        public void Write(IByteBuffer buffer, T? value)
        {
            innerCodec.Write(buffer, value ?? defaultValue);
        }

        public T Read(IByteBuffer buffer)
        {
            return BinaryCodec.BOOLEAN.Read(buffer) ? innerCodec.Read(buffer) : defaultValue;
        }
    }

    public class TransformativeBinaryCodec<T, S>(BinaryCodec<T> innerCodec, Func<S, T> from, Func<T, S> to) : BinaryCodec<S> where S : notnull where T : notnull
    {
        public void Write(IByteBuffer buffer, S value)
        {
            innerCodec.Write(buffer, from.Invoke(value));
        }

        public S Read(IByteBuffer buffer)
        {
            var innerValue = innerCodec.Read(buffer);
            return to.Invoke(innerValue);
        }
    }

    public class DictionaryBinaryCodec<K, V>(BinaryCodec<K> keyCodec, BinaryCodec<V> valueCodec, int? maxSize = null) : BinaryCodec<Dictionary<K, V>> where K : notnull where V : notnull
    {
        public void Write(IByteBuffer buffer, Dictionary<K, V> value)
        {
            if (maxSize != null && value.Count > maxSize)
                throw new ArgumentException($"There are more map entries than maximum allowed ({value.Count} > {maxSize})", nameof(value));

            BinaryCodec.VAR_INT.Write(buffer, value.Count);
            foreach (var keyValuePair in value)
            {
                keyCodec.Write(buffer, keyValuePair.Key);
                valueCodec.Write(buffer, keyValuePair.Value);
            }
        }

        public Dictionary<K, V> Read(IByteBuffer buffer)
        {
            var dict = new Dictionary<K, V>();
            var size = BinaryCodec.VAR_INT.Read(buffer); // how big is my dic(t)...

            if (maxSize != null && size > maxSize)
                throw new InvalidDataException($"The read dictionary has more entries than maximum allowed ({size} > {maxSize})");

            for (var i = 0; i < size; i++)
            {
                var key = keyCodec.Read(buffer);
                var value = valueCodec.Read(buffer);
                dict[key] = value;
            }

            return dict;
        }
    }

    public class ListBinaryCodec<T>(BinaryCodec<T> innerCodec, int? maxSize = null) : BinaryCodec<List<T>>
    {
        public void Write(IByteBuffer buffer, List<T> value)
        {
            if (maxSize != null && value.Count > maxSize)
                throw new ArgumentException($"There are more list entries than maximum allowed ({value.Count} > {maxSize})", nameof(value));

            BinaryCodec.VAR_INT.Write(buffer, value.Count);
            value.ForEach(item => innerCodec.Write(buffer, item));
        }

        public List<T> Read(IByteBuffer buffer)
        {
            var list = new List<T>();
            var size = BinaryCodec.VAR_INT.Read(buffer);

            if (size > maxSize)
                throw new InvalidDataException($"The read list has more entries than maximum allowed ({size} > {maxSize})");

            for (var i = 0; i < size; i++) list.Add(innerCodec.Read(buffer));

            return list;
        }
    }

    public class UnionBinaryCodec<T, K>(BinaryCodec<K> keyCodec, Func<T, K> keyFunc, Func<K, BinaryCodec<T>> serializerFactory) : BinaryCodec<T>
    {
        public void Write(IByteBuffer buffer, T value)
        {
            var key = keyFunc.Invoke(value);
            keyCodec.Write(buffer, key);

            var serializer = serializerFactory.Invoke(key);
            serializer.Write(buffer, value);
        }

        public T Read(IByteBuffer buffer)
        {
            var key = keyCodec.Read(buffer);
            var serializer = serializerFactory.Invoke(key);
            return serializer.Read(buffer);
        }
    }

    public class RecursiveBinaryCodec<T> : BinaryCodec<T> where T : notnull
    {
        private readonly Lazy<BinaryCodec<T>> @delegate;

        public RecursiveBinaryCodec(Func<BinaryCodec<T>, BinaryCodec<T>> self)
        {
            @delegate = new Lazy<BinaryCodec<T>>(() => self.Invoke(this));
        }

        public void Write(IByteBuffer buffer, T value)
        {
            @delegate.Value.Write(buffer, value);
        }

        public T Read(IByteBuffer buffer)
        {
            return @delegate.Value.Read(buffer);
        }
    }

    public class EnumBinaryCodec<E> : BinaryCodec<E> where E : Enum
    {
        private readonly Array entries = Enum.GetValues(typeof(E));

        public void Write(IByteBuffer buffer, E value)
        {
            var ordinal = Array.IndexOf(entries, value);
            BinaryCodec.VAR_INT.Write(buffer, ordinal);
        }

        public E Read(IByteBuffer buffer)
        {
            var ordinal = BinaryCodec.VAR_INT.Read(buffer);
            if (ordinal < 0 || ordinal >= entries.Length) throw new IndexOutOfRangeException($"Ordinal {ordinal} is outside the range [0, {entries.Length - 1}] for enum {typeof(E).Name}");

            return (E)entries.GetValue(ordinal)!;
        }
    }

    public class BinaryCodecEmpty<Result>(
        Func<Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
        {
        }

        public Result Read(IByteBuffer buffer)
        {
            return func.Invoke();
        }
    }

    public class BinaryCodecP1<P1, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        Func<P1, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
        }

        public Result Read(IByteBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            return func.Invoke(result1);
        }
    }

    public class BinaryCodecP2<P1, P2, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        Func<P1, P2, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
        }

        public Result Read(IByteBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            return func.Invoke(result1, result2);
        }
    }

    public class BinaryCodecP3<P1, P2, P3, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        Func<P1, P2, P3, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
        }

        public Result Read(IByteBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            return func.Invoke(result1, result2, result3);
        }
    }

    public class BinaryCodecP4<P1, P2, P3, P4, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        Func<P1, P2, P3, P4, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
        }

        public Result Read(IByteBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            return func.Invoke(result1, result2, result3, result4);
        }
    }

    public class BinaryCodecP5<P1, P2, P3, P4, P5, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        Func<P1, P2, P3, P4, P5, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
        }

        public Result Read(IByteBuffer buffer)
        {
            var result1 = codec1.Read(buffer);
            var result2 = codec2.Read(buffer);
            var result3 = codec3.Read(buffer);
            var result4 = codec4.Read(buffer);
            var result5 = codec5.Read(buffer);
            return func.Invoke(result1, result2, result3, result4, result5);
        }
    }

    public class BinaryCodecP6<P1, P2, P3, P4, P5, P6, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        Func<P1, P2, P3, P4, P5, P6, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
        }

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP7<P1, P2, P3, P4, P5, P6, P7, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        Func<P1, P2, P3, P4, P5, P6, P7, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
        {
            codec1.Write(buffer, getter1.Invoke(value));
            codec2.Write(buffer, getter2.Invoke(value));
            codec3.Write(buffer, getter3.Invoke(value));
            codec4.Write(buffer, getter4.Invoke(value));
            codec5.Write(buffer, getter5.Invoke(value));
            codec6.Write(buffer, getter6.Invoke(value));
            codec7.Write(buffer, getter7.Invoke(value));
        }

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP8<P1, P2, P3, P4, P5, P6, P7, P8, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP9<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        BinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP10<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        BinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        BinaryCodec<P10> codec10,
        Func<Result, P10> getter10,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP11<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        BinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        BinaryCodec<P10> codec10,
        Func<Result, P10> getter10,
        BinaryCodec<P11> codec11,
        Func<Result, P11> getter11,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP12<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        BinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        BinaryCodec<P10> codec10,
        Func<Result, P10> getter10,
        BinaryCodec<P11> codec11,
        Func<Result, P11> getter11,
        BinaryCodec<P12> codec12,
        Func<Result, P12> getter12,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP13<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        BinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        BinaryCodec<P10> codec10,
        Func<Result, P10> getter10,
        BinaryCodec<P11> codec11,
        Func<Result, P11> getter11,
        BinaryCodec<P12> codec12,
        Func<Result, P12> getter12,
        BinaryCodec<P13> codec13,
        Func<Result, P13> getter13,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP14<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        BinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        BinaryCodec<P10> codec10,
        Func<Result, P10> getter10,
        BinaryCodec<P11> codec11,
        Func<Result, P11> getter11,
        BinaryCodec<P12> codec12,
        Func<Result, P12> getter12,
        BinaryCodec<P13> codec13,
        Func<Result, P13> getter13,
        BinaryCodec<P14> codec14,
        Func<Result, P14> getter14,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP15<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        BinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        BinaryCodec<P10> codec10,
        Func<Result, P10> getter10,
        BinaryCodec<P11> codec11,
        Func<Result, P11> getter11,
        BinaryCodec<P12> codec12,
        Func<Result, P12> getter12,
        BinaryCodec<P13> codec13,
        Func<Result, P13> getter13,
        BinaryCodec<P14> codec14,
        Func<Result, P14> getter14,
        BinaryCodec<P15> codec15,
        Func<Result, P15> getter15,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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

    public class BinaryCodecP16<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result>(
        BinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        BinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        BinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        BinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        BinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        BinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        BinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        BinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        BinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        BinaryCodec<P10> codec10,
        Func<Result, P10> getter10,
        BinaryCodec<P11> codec11,
        Func<Result, P11> getter11,
        BinaryCodec<P12> codec12,
        Func<Result, P12> getter12,
        BinaryCodec<P13> codec13,
        Func<Result, P13> getter13,
        BinaryCodec<P14> codec14,
        Func<Result, P14> getter14,
        BinaryCodec<P15> codec15,
        Func<Result, P15> getter15,
        BinaryCodec<P16> codec16,
        Func<Result, P16> getter16,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, Result> func
    ) : BinaryCodec<Result>
    {
        public void Write(IByteBuffer buffer, Result value)
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

        public Result Read(IByteBuffer buffer)
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
