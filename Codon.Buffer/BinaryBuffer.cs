using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;

namespace Codon.Buffer
{
    public class BinaryBuffer : IDisposable
    {
        private List<byte> _data = [];

        public int ReaderIndex { get; set; }

        public int WriterIndex { get; set; }

        public int Length => _data.Count;

        public static BinaryBuffer FromArray(byte[] array)
        {
            var buffer = new BinaryBuffer();
            buffer.WriteBytes(array);
            return buffer;
        }

        public static BinaryBuffer FromReadOnlySequence(ReadOnlySequence<byte> readOnlySequence)
        {
            return FromArray(readOnlySequence.ToArray());
        }

        public bool CanRead(int count)
        {
            if (count < 0) return false;
            return Length >= count;
        }

        public void SkipBytes(int count)
        {
            if (count <= 0) return;
            ReaderIndex += count;
        }

        public void WriteBytes(ReadOnlySpan<byte> value)
        {
            foreach (var b in value) _data.Add(b);
            WriterIndex += value.Length;
        }

        public void WriteBytes(byte[] value)
        {
            WriteBytes(value.AsSpan());
        }

        public void WriteByte(byte value)
        {
            _data.Add(value);
            WriterIndex += sizeof(byte);
        }

        public void WriteInt(int value)
        {
            var span = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(span, value);
            WriteBytes(span);
        }

        public void WriteLong(long value)
        {
            var span = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(span, value);
            WriteBytes(span);
        }

        public void WriteDouble(double value)
        {
            var span = new byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleBigEndian(span, value);
            WriteBytes(span);
        }

        public void WriteFloat(float value)
        {
            var span = new byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(span, value);
            WriteBytes(span);
        }

        public void WriteBoolean(bool value)
        {
            WriteByte(value ? (byte)1 : (byte)0);
        }

        public Span<byte> ReadBytes(int count)
        {
            var arr = new List<byte>();
            for (var i = 0; i < count; i++)
            {
                var a = ReadByte();
                arr.Add(a);
            }

            return arr.ToArray();
        }

        public int ReadInt()
        {
            var bytes = ReadBytes(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(bytes);
        }

        public byte ReadByte()
        {
            return ReaderIndex >= _data.Count ? throw new EndOfStreamException("No data is available.") : _data[ReaderIndex++];
        }

        public bool ReadBoolean()
        {
            return ReadByte() != 0;
        }

        public double ReadDouble()
        {
            var bytes = ReadBytes(sizeof(double));
            return BinaryPrimitives.ReadDoubleBigEndian(bytes);
        }

        public long ReadLong()
        {
            var bytes = ReadBytes(sizeof(long));
            return BinaryPrimitives.ReadInt64BigEndian(bytes);
        }

        public float ReadFloat()
        {
            var bytes = ReadBytes(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(bytes);
        }

        public byte[] ToArray()
        {
            return _data.ToArray();
        }

        public void Clear()
        {
            _data.Clear();
            WriterIndex = 0;
            ReaderIndex = 0;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}