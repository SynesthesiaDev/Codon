
using System.Buffers;
using Codon.Buffer;

namespace Codon.Tests;

public class BinaryBufferTests
{
    [Test]
    public void WriteRead_Byte()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteByte(0xAB);

        Assert.That(buffer.ReadByte(), Is.EqualTo(0xAB));
    }

    [Test]
    public void WriteRead_Boolean()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBoolean(true);
        buffer.WriteBoolean(false);

        Assert.That(buffer.ReadBoolean(), Is.True);
        Assert.That(buffer.ReadBoolean(), Is.False);
    }

    [Test]
    public void WriteRead_Int()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteInt(0);
        buffer.WriteInt(123456789);
        buffer.WriteInt(-123456789);
        buffer.WriteInt(int.MinValue);
        buffer.WriteInt(int.MaxValue);

        Assert.That(buffer.ReadInt(), Is.EqualTo(0));
        Assert.That(buffer.ReadInt(), Is.EqualTo(123456789));
        Assert.That(buffer.ReadInt(), Is.EqualTo(-123456789));
        Assert.That(buffer.ReadInt(), Is.EqualTo(int.MinValue));
        Assert.That(buffer.ReadInt(), Is.EqualTo(int.MaxValue));
    }

    [Test]
    public void WriteRead_Long()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteLong(0L);
        buffer.WriteLong(1234567890123456789L);
        buffer.WriteLong(-1234567890123456789L);
        buffer.WriteLong(long.MinValue);
        buffer.WriteLong(long.MaxValue);

        Assert.That(buffer.ReadLong(), Is.EqualTo(0L));
        Assert.That(buffer.ReadLong(), Is.EqualTo(1234567890123456789L));
        Assert.That(buffer.ReadLong(), Is.EqualTo(-1234567890123456789L));
        Assert.That(buffer.ReadLong(), Is.EqualTo(long.MinValue));
        Assert.That(buffer.ReadLong(), Is.EqualTo(long.MaxValue));
    }

    [Test]
    public void WriteRead_Float()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteFloat(0f);
        buffer.WriteFloat(1.5f);
        buffer.WriteFloat(-2.25f);
        buffer.WriteFloat(float.PositiveInfinity);
        buffer.WriteFloat(float.NegativeInfinity);
        buffer.WriteFloat(float.NaN);

        Assert.That(buffer.ReadFloat(), Is.EqualTo(0f));
        Assert.That(buffer.ReadFloat(), Is.EqualTo(1.5f));
        Assert.That(buffer.ReadFloat(), Is.EqualTo(-2.25f));
        Assert.That(float.IsPositiveInfinity(buffer.ReadFloat()), Is.True);
        Assert.That(float.IsNegativeInfinity(buffer.ReadFloat()), Is.True);
        Assert.That(float.IsNaN(buffer.ReadFloat()), Is.True);
    }

    [Test]
    public void WriteRead_Double()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteDouble(0d);
        buffer.WriteDouble(1.5d);
        buffer.WriteDouble(-2.25d);
        buffer.WriteDouble(double.PositiveInfinity);
        buffer.WriteDouble(double.NegativeInfinity);
        buffer.WriteDouble(double.NaN);

        Assert.That(buffer.ReadDouble(), Is.EqualTo(0d));
        Assert.That(buffer.ReadDouble(), Is.EqualTo(1.5d));
        Assert.That(buffer.ReadDouble(), Is.EqualTo(-2.25d));
        Assert.That(double.IsPositiveInfinity(buffer.ReadDouble()), Is.True);
        Assert.That(double.IsNegativeInfinity(buffer.ReadDouble()), Is.True);
        Assert.That(double.IsNaN(buffer.ReadDouble()), Is.True);
    }

    [Test]
    public void Endianness_IsBigEndian_ForInt()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteInt(0x01020304);

        var bytes = buffer.ToArray();
        Assert.That(bytes, Is.EqualTo(new byte[] { 0x01, 0x02, 0x03, 0x04 }));
    }

    [Test]
    public void SkipBytes_Skips_Correctly()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 1, 2, 3, 4 });

        buffer.SkipBytes(2);
        Assert.That(buffer.ReadByte(), Is.EqualTo(3));
        Assert.That(buffer.ReadByte(), Is.EqualTo(4));
    }

    [Test]
    public void ReadBytes_Throws_On_Insufficient_Data()
    {
        var buffer = new BinaryBuffer();
        Assert.Throws<EndOfStreamException>(() => buffer.ReadBytes(1));

        buffer = new BinaryBuffer();
        buffer.WriteByte(0xFF);
        Assert.Throws<EndOfStreamException>(() => buffer.ReadBytes(2));
    }

    [Test]
    public void FromArray_And_FromReadOnlySequence()
    {
        var raw = new byte[] { 10, 20, 30 };
        var buf1 = BinaryBuffer.FromArray(raw);
        Assert.That(buf1.ReadByte(), Is.EqualTo(10));
        Assert.That(buf1.ReadByte(), Is.EqualTo(20));
        Assert.That(buf1.ReadByte(), Is.EqualTo(30));

        var seq = new ReadOnlySequence<byte>(new byte[] { 5, 6 });
        var buf2 = BinaryBuffer.FromReadOnlySequence(seq);
        Assert.That(buf2.ReadByte(), Is.EqualTo(5));
        Assert.That(buf2.ReadByte(), Is.EqualTo(6));
    }

    [Test]
    public void Clear_Removes_All_Readable_Data()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 1, 2, 3 });

        Assert.That(buffer.Length, Is.EqualTo(3));
        buffer.Clear();
        Assert.That(buffer.Length, Is.EqualTo(0));
        Assert.That(buffer.ToArray(), Is.Empty);
    }

    [Test]
    public void WriterIndex_And_Length_Track_Writes()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteByte(0xAA);
        buffer.WriteInt(0x01020304);
        buffer.WriteBoolean(true);

        // 1 + 4 + 1 = 6
        Assert.That(buffer.WriterIndex, Is.EqualTo(6));
        Assert.That(buffer.Length, Is.EqualTo(6));
        Assert.That(buffer.ToArray().Length, Is.EqualTo(6));
    }

    [Test]
    public void ReaderIndex_Progresses_With_Read_And_Skip()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 1, 2, 3, 4, 5 });

        Assert.That(buffer.ReaderIndex, Is.EqualTo(0));
        Assert.That(buffer.ReadByte(), Is.EqualTo(1));
        Assert.That(buffer.ReaderIndex, Is.EqualTo(1));

        buffer.SkipBytes(2); // skip 2 and 3
        Assert.That(buffer.ReaderIndex, Is.EqualTo(3));
        Assert.That(buffer.ReadByte(), Is.EqualTo(4));
        Assert.That(buffer.ReaderIndex, Is.EqualTo(4));
    }

    [Test]
    public void SkipBytes_Zero_And_Negative_Do_Nothing()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 10, 20, 30 });

        buffer.SkipBytes(0);
        Assert.That(buffer.ReaderIndex, Is.EqualTo(0));

        buffer.SkipBytes(-5);
        Assert.That(buffer.ReaderIndex, Is.EqualTo(0));
    }

    [Test]
    public void SkipBytes_Beyond_Length_Then_Read_Throws()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 1, 2 });

        buffer.SkipBytes(10); // move reader past end
        Assert.Throws<EndOfStreamException>(() => buffer.ReadByte());
    }

    [Test]
    public void Boolean_Encoding_Is_One_And_Zero()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBoolean(true);
        buffer.WriteBoolean(false);

        Assert.That(buffer.ToArray(), Is.EqualTo(new byte[] { 1, 0 }));
    }

    [Test]
    public void Endianness_IsBigEndian_ForLong_ToArray()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteLong(0x0102030405060708);

        var bytes = buffer.ToArray();
        Assert.That(bytes, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }));
    }

    [Test]
    public void ReadByte_Throws_On_Empty_And_After_Consumption()
    {
        var empty = new BinaryBuffer();
        Assert.Throws<EndOfStreamException>(() => empty.ReadByte());

        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 42 });
        Assert.That(buffer.ReadByte(), Is.EqualTo(42));
        Assert.Throws<EndOfStreamException>(() => buffer.ReadByte());
    }

    [Test]
    public void FromArray_Is_Copy_Semantics()
    {
        var src = new byte[] { 7, 8, 9 };
        var buf = BinaryBuffer.FromArray(src);

        src[0] = 99;

        Assert.That(buf.ToArray(), Is.EqualTo(new byte[] { 7, 8, 9 }));
        Assert.That(buf.ReadByte(), Is.EqualTo(7));
    }

    [Test]
    public void Reads_Do_Not_Change_Length_Or_Written_Data()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 1, 2, 3, 4 });

        var beforeLen = buffer.Length;
        var beforeArr = buffer.ToArray();

        buffer.ReadByte();
        buffer.SkipBytes(2);

        Assert.That(buffer.Length, Is.EqualTo(beforeLen));
        Assert.That(buffer.ToArray(), Is.EqualTo(beforeArr));
        Assert.That(buffer.ReaderIndex, Is.EqualTo(3));
    }

    [Test]
    public void Clear_And_Dispose_Reset_Indices()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 1, 2, 3 });
        buffer.ReadByte(); // advance reader
        Assert.That(buffer.ReaderIndex, Is.EqualTo(1));
        Assert.That(buffer.WriterIndex, Is.EqualTo(3));

        buffer.Clear();
        Assert.That(buffer.ReaderIndex, Is.EqualTo(0));
        Assert.That(buffer.WriterIndex, Is.EqualTo(0));
        Assert.That(buffer.Length, Is.EqualTo(0));

        buffer.WriteBytes(new byte[] { 5, 6 });
        buffer.ReadByte();
        Assert.That(buffer.ReaderIndex, Is.EqualTo(1));

        buffer.Dispose();
        Assert.That(buffer.ReaderIndex, Is.EqualTo(0));
        Assert.That(buffer.WriterIndex, Is.EqualTo(0));
        Assert.That(buffer.Length, Is.EqualTo(0));
    }

    [Test]
    public void CanRead_Negative_IsFalse_And_Bounds_Check()
    {
        var buffer = new BinaryBuffer();
        buffer.WriteBytes(new byte[] { 1, 2, 3 });

        Assert.That(buffer.CanRead(-1), Is.False);
        Assert.That(buffer.CanRead(3), Is.True);
        Assert.That(buffer.CanRead(4), Is.False);
    }

    [Test]
    public void FromReadOnlySequence_Creates_Equivalent_Buffer()
    {
        var seq = new ReadOnlySequence<byte>([11, 22, 33, 44]);
        var buf = BinaryBuffer.FromReadOnlySequence(seq);

        Assert.That(buf.ToArray(), Is.EqualTo(new byte[] { 11, 22, 33, 44 }));
        Assert.That(buf.ReadInt(), Is.EqualTo(0x0B16212C)); // 11,22,33,44 big-endian
    }
}