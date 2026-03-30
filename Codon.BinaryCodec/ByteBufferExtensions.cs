// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using DotNetty.Buffers;

namespace Codon.Binary;

public static class ByteBufferExtensions
{
    public static byte[] ToByteArraySafe(this IByteBuffer buffer, int size)
    {
        var destination = new byte[size];
        buffer.ReadBytes(destination);
        return destination;
    }

    public static byte[] ToByteArraySafe(this IByteBuffer buffer)
    {
        int size = Math.Max(0, buffer.ReadableBytes);
        return buffer.ToByteArraySafe(size);
    }

}
