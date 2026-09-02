namespace Codon.Codec.Transcoder;

public interface ITranscoder<T>
{
    T EncodeNull();

    T EncodeBool(bool value);
    bool DecodeBool(T value);

    T EncodeByte(byte value);
    byte DecodeByte(T value);

    T EncodeShort(short value);
    short DecodeShort(T value);

    T EncodeInt(int value);
    int DecodeInt(T value);

    T EncodeLong(long value);
    long DecodeLong(T value);

    T EncodeFloat(float value);
    float DecodeFloat(T value);

    T EncodeDouble(double value);
    double DecodeDouble(T value);

    T EncodeString(string value);
    string DecodeString(T value);

    IListBuilder<T> EncodeList(int size);
    List<T> DecodeList(T value);

    IVirtualMapBuilder<T> EncodeMap();
    IVirtualMap<T> DecodeMap(T value);

    T EmptyMap() => EncodeMap().Build();

    T EncodeByteArray(byte[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var b in array)
        {
            list.Add(EncodeByte(b));
        }

        return list.Build();
    }

    byte[] DecodeByteArray(T value)
    {
        var list = new List<byte>();
        DecodeList(value).ForEach(b => list.Add(DecodeByte(b)));
        return list.ToArray();
    }

    T EncodeIntArray(int[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var i in array)
        {
            list.Add(EncodeInt(i));
        }

        return list.Build();
    }

    int[] DecodeIntArray(T value)
    {
        var list = new List<int>();
        DecodeList(value).ForEach(b => list.Add(DecodeInt(b)));
        return list.ToArray();
    }

    T EncodeLongArray(long[] array)
    {
        var list = EncodeList(array.Length);
        foreach (var l in array)
        {
            list.Add(EncodeLong(l));
        }

        return list.Build();
    }

    long[] DecodeLongArray(T value)
    {
        var list = new List<long>();
        DecodeList(value).ForEach(b => list.Add(DecodeLong(b)));
        return list.ToArray();
    }

    T DecodeObjectFromString(string body);
    string EncodeObjectToString(T value);
}
