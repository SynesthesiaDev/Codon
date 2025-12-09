namespace Codon.Codec.Transcoder;

public interface Transcoder<T>
{
    public T EncodeNull();

    public T EncodeBool(bool value);
    public bool DecodeBool(T value);

    public T EncodeByte(byte value);
    public byte DecodeByte(T value);

    public T EncodeShort(short value);
    public short DecodeShort(T value);

    public T EncodeInt(int value);
    public int DecodeInt(T value);

    public T EncodeLong(long value);
    public long DecodeLong(T value);

    public T EncodeFloat(float value);
    public float DecodeFloat(T value);

    public T EncodeDouble(double value);
    public double DecodeDouble(T value);

    public T EncodeString(string value);
    public string DecodeString(T value);

    public IListBuilder<T> EncodeList(int size);
    public List<T> DecodeList(T value);

    public IVirtualMapBuilder<T> EncodeMap();
    public IVirtualMap<T> DecodeMap();

    public T EmptyMap() => EncodeMap().Build();

    public T EncodeByteArray(byte[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var b in array)
        {
            list.Add(EncodeByte(b));
        }

        return list.Build();
    }

    public byte[] DecodeByteArray(T value)
    {
        var list = new List<byte>();
        DecodeList(value).ForEach(b => list.Add(DecodeByte(b)));
        return list.ToArray();
    }

    public T EncodeIntArray(int[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var i in array)
        {
            list.Add(EncodeInt(i));
        }

        return list.Build();
    }

    public int[] DecodeIntArray(T value)
    {
        var list = new List<int>();
        DecodeList(value).ForEach(b => list.Add(DecodeInt(b)));
        return list.ToArray();
    }

    public T EncodeLongArray(long[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var l in array)
        {
            list.Add(EncodeLong(l));
        }

        return list.Build();
    }

    public long[] DecodeLongArray(T value)
    {
        var list = new List<long>();
        DecodeList(value).ForEach(b => list.Add(DecodeLong(b)));
        return list.ToArray();
    }
}