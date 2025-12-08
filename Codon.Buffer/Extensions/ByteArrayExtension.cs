namespace Codon.Buffer.Extensions;

public static class ByteArrayExtension
{
    public static BinaryBuffer ToBinaryBuffer(this byte[] array)
    {
        return BinaryBuffer.FromArray(array);
    }
}