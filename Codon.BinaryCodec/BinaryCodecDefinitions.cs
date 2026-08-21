// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Codon.Optionals;
using DotNetty.Buffers;

namespace Codon.Binary;

public static class BinaryCodecDefinitions
{
    public class CustomBinaryCodec<T>(Action<IByteBuffer, T> encode, Func<IByteBuffer, T> decode) : IBinaryCodec<T>
    {
        public void Write(IByteBuffer buffer, T value) => encode.Invoke(buffer, value);
        public T Read(IByteBuffer buffer) => decode.Invoke(buffer);
    }

    public class ShortBinaryCodec : IBinaryCodec<short>
    {
        public void Write(IByteBuffer buffer, short value)
        {
            buffer.WriteShort(value);
        }

        public short Read(IByteBuffer buffer)
        {
            return buffer.ReadShort();
        }
    }

    public class UIntBinaryCodec : IBinaryCodec<uint>
    {
        public void Write(IByteBuffer buffer, uint value)
        {
            buffer.WriteUnsignedInt(value);
        }

        public uint Read(IByteBuffer buffer)
        {
            return buffer.ReadUnsignedInt();
        }
    }

    public class BooleanBinaryCodec : IBinaryCodec<bool>
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

    public class ByteBinaryCodec : IBinaryCodec<byte>
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

    public class IntBinaryCodec : IBinaryCodec<int>
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

    public class LongBinaryCodec : IBinaryCodec<long>
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

    public class FloatBinaryCodec : IBinaryCodec<float>
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

    public class DoubleBinaryCodec : IBinaryCodec<double>
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

    public class VarIntBinaryCodec : IBinaryCodec<int>
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

    public class ByteArrayBinaryCodec(int? maxSize = null) : IBinaryCodec<byte[]>
    {
        public void Write(IByteBuffer buffer, byte[] value)
        {
            if (maxSize != null && value.Length > maxSize)
                throw new ArgumentException($"The byte array is longer than maximum allowed ({value.Length} > {maxSize})", nameof(value));

            BinaryCodecs.VAR_INT.Write(buffer, value.Length);
            buffer.WriteBytes(value);
        }

        public byte[] Read(IByteBuffer buffer)
        {
            var size = BinaryCodecs.VAR_INT.Read(buffer);
            return size > maxSize ? throw new InvalidDataException($"The read byte array is longer than maximum allowed (${size} > {maxSize}") : buffer.ToByteArraySafe(size);
        }
    }

    public class ByteBufferBinaryCodec(int? maxSize = null) : IBinaryCodec<IByteBuffer>
    {
        public void Write(IByteBuffer buffer, IByteBuffer value)
        {
            var size = value.ReadableBytes;

            if (maxSize is not null && size > maxSize.Value)
            {
                throw new ArgumentException(
                    $"The byte buffer is longer than maximum allowed ({size} > {maxSize.Value})",
                    nameof(value));
            }

            BinaryCodecs.VAR_INT.Write(buffer, size);
            buffer.WriteBytes(value, value.ReaderIndex, size);
        }

        public IByteBuffer Read(IByteBuffer buffer)
        {
            var size = BinaryCodecs.VAR_INT.Read(buffer);

            if (size < 0)
                throw new InvalidDataException($"Size cannot be negative.");


            if (size > maxSize)
                throw new InvalidDataException($"The read byte buffer is longer than maximum allowed (${size} > {maxSize}");

            return buffer.ReadBytes(size);
        }
    }

    public class StringBinaryCodec(int? maxStringLength = null) : IBinaryCodec<string>
    {
        public void Write(IByteBuffer buffer, string value)
        {
            var stringBytes = Encoding.UTF8.GetBytes(value);

            if (maxStringLength is not null && stringBytes.Length > maxStringLength.Value)
                throw new ArgumentException($"String is longer than maximum allowed ({stringBytes.Length} > {maxStringLength.Value})", nameof(value));

            BinaryCodecs.VAR_INT.Write(buffer, stringBytes.Length);
            buffer.WriteBytes(stringBytes);
        }

        public string Read(IByteBuffer buffer)
        {
            var size = BinaryCodecs.VAR_INT.Read(buffer);

            if (size < 0)
                throw new InvalidDataException($"The read string has negative length: {size}");

            if (maxStringLength is not null && size > maxStringLength.Value)
                throw new InvalidDataException($"Read string is longer than maximum allowed ({size} > {maxStringLength.Value})");

            var stringBytes = buffer.ToByteArraySafe(size);
            return Encoding.UTF8.GetString(stringBytes);
        }
    }

    public class RawBytesBinaryCodec : IBinaryCodec<byte[]>
    {
        public void Write(IByteBuffer buffer, byte[] value)
        {
            buffer.WriteBytes(value);
        }

        public byte[] Read(IByteBuffer buffer)
        {
            return buffer.ToByteArraySafe();
        }
    }

    public class OptionalBinaryCodec<T>(IBinaryCodec<T> innerCodec) : IBinaryCodec<Optional<T>>
    {
        public void Write(IByteBuffer buffer, Optional<T> value)
        {
            BinaryCodecs.BOOLEAN.Write(buffer, value.IsPresent);
            if (value.IsPresent) innerCodec.Write(buffer, value.Value!);
        }

        public Optional<T> Read(IByteBuffer buffer)
        {
            return BinaryCodecs.BOOLEAN.Read(buffer) ? Optional.Of<T>(innerCodec.Read(buffer)) : Optional.Empty<T>();
        }
    }

    public class DefaultBinaryCodec<T>(IBinaryCodec<T> innerCodec, T defaultValue) : IBinaryCodec<T> where T : notnull
    {
        public void Write(IByteBuffer buffer, T? value)
        {
            BinaryCodecs.BOOLEAN.Write(buffer, value != null);
            innerCodec.Write(buffer, value ?? defaultValue);
        }

        public T Read(IByteBuffer buffer)
        {
            return BinaryCodecs.BOOLEAN.Read(buffer) ? innerCodec.Read(buffer) : defaultValue;
        }
    }

    public class TransformativeBinaryCodec<T, S>(IBinaryCodec<T> innerCodec, Func<S, T> from, Func<T, S> to) : IBinaryCodec<S> where S : notnull where T : notnull
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

    public class DictionaryBinaryCodec<K, V>(IBinaryCodec<K> keyCodec, IBinaryCodec<V> valueCodec, int? maxSize = null) : IBinaryCodec<Dictionary<K, V>> where K : notnull where V : notnull
    {
        public void Write(IByteBuffer buffer, Dictionary<K, V> value)
        {

            if (maxSize != null && value.Count > maxSize)
                throw new ArgumentException($"There are more map entries than maximum allowed ({value.Count} > {maxSize})", nameof(value));

            BinaryCodecs.VAR_INT.Write(buffer, value.Count);
            foreach (var keyValuePair in value)
            {
                keyCodec.Write(buffer, keyValuePair.Key);
                valueCodec.Write(buffer, keyValuePair.Value);
            }
        }

        public Dictionary<K, V> Read(IByteBuffer buffer)
        {
            var dict = new Dictionary<K, V>();
            var size = BinaryCodecs.VAR_INT.Read(buffer); // how big is my dic(t)...

            if (size < 0)
                throw new InvalidDataException($"The read dictionary has negative entry count: {size}");

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

    public class ListBinaryCodec<T>(IBinaryCodec<T> innerCodec, int? maxSize = null) : IBinaryCodec<List<T>>
    {
        public void Write(IByteBuffer buffer, List<T> value)
        {
            if (maxSize != null && value.Count > maxSize)
                throw new ArgumentException($"There are more list entries than maximum allowed ({value.Count} > {maxSize})", nameof(value));

            BinaryCodecs.VAR_INT.Write(buffer, value.Count);
            value.ForEach(item => innerCodec.Write(buffer, item));
        }

        public List<T> Read(IByteBuffer buffer)
        {
            var list = new List<T>();
            var size = BinaryCodecs.VAR_INT.Read(buffer);

            if (size < 0)
                throw new InvalidDataException($"The read list has negative entry count: {size}");

            if (size > maxSize)
                throw new InvalidDataException($"The read list has more entries than maximum allowed ({size} > {maxSize})");

            for (var i = 0; i < size; i++) list.Add(innerCodec.Read(buffer));

            return list;
        }
    }

    public class UnionBinaryCodec<T, K>(IBinaryCodec<K> keyCodec, Func<T, K> keyFunc, Func<K, IBinaryCodec<T>> serializerFactory) : IBinaryCodec<T>
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

    public class RecursiveBinaryCodec<T> : IBinaryCodec<T> where T : notnull
    {
        private readonly Lazy<IBinaryCodec<T>> @delegate;

        public RecursiveBinaryCodec(Func<IBinaryCodec<T>, IBinaryCodec<T>> self)
        {
            @delegate = new Lazy<IBinaryCodec<T>>(() => self.Invoke(this));
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

    public class EnumBinaryCodec<E> : IBinaryCodec<E> where E : Enum
    {
        private readonly Array entries = Enum.GetValues(typeof(E));

        public void Write(IByteBuffer buffer, E value)
        {
            var ordinal = Array.IndexOf(entries, value);
            if (ordinal < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"Value is not defined for enum {typeof(E).Name}.");
            }

            BinaryCodecs.VAR_INT.Write(buffer, ordinal);
        }

        public E Read(IByteBuffer buffer)
        {
            var ordinal = BinaryCodecs.VAR_INT.Read(buffer);
            if (ordinal < 0 || ordinal >= entries.Length) throw new IndexOutOfRangeException($"Ordinal {ordinal} is outside the range [0, {entries.Length - 1}] for enum {typeof(E).Name}");

            return (E)entries.GetValue(ordinal)!;
        }
    }

    public class BinaryCodecEmpty<Result>(
        Func<Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        Func<P1, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        Func<P1, P2, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        Func<P1, P2, P3, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        Func<P1, P2, P3, P4, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        Func<P1, P2, P3, P4, P5, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        Func<P1, P2, P3, P4, P5, P6, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        Func<P1, P2, P3, P4, P5, P6, P7, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, Result> func
    ) : IBinaryCodec<Result>
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
        IBinaryCodec<P1> codec1,
        Func<Result, P1> getter1,
        IBinaryCodec<P2> codec2,
        Func<Result, P2> getter2,
        IBinaryCodec<P3> codec3,
        Func<Result, P3> getter3,
        IBinaryCodec<P4> codec4,
        Func<Result, P4> getter4,
        IBinaryCodec<P5> codec5,
        Func<Result, P5> getter5,
        IBinaryCodec<P6> codec6,
        Func<Result, P6> getter6,
        IBinaryCodec<P7> codec7,
        Func<Result, P7> getter7,
        IBinaryCodec<P8> codec8,
        Func<Result, P8> getter8,
        IBinaryCodec<P9> codec9,
        Func<Result, P9> getter9,
        IBinaryCodec<P10> codec10,
        Func<Result, P10> getter10,
        Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, Result> func
    ) : IBinaryCodec<Result>
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
}
