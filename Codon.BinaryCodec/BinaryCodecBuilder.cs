// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace Codon.Binary;

public readonly struct BinaryCodecBuilder<T>
{
    public BinaryCodecBuilder<T, P1> Field<P1>(IBinaryCodec<P1> codec, Func<T, P1> getter) where P1 : notnull
        => new(codec, getter);
}

public readonly struct BinaryCodecBuilder<T, P1> where P1 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;

    internal BinaryCodecBuilder(IBinaryCodec<P1> codec, Func<T, P1> getter)
        => (C1, G1) = (codec, getter);

    public BinaryCodecBuilder<T, P1, P2> Field<P2>(IBinaryCodec<P2> codec, Func<T, P2> getter) where P2 : notnull
        => new(C1, G1, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP1<P1, T>(C1, G1, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2> where P1 : notnull where P2 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> codec, Func<T, P2> getter) =>
        (C1, G1, C2, G2) = (c1, g1, codec, getter);

    public BinaryCodecBuilder<T, P1, P2, P3> Field<P3>(IBinaryCodec<P3> codec, Func<T, P3> getter) where P3 : notnull
        => new(C1, G1, C2, G2, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP2<P1, P2, T>(C1, G1, C2, G2, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3> where P1 : notnull where P2 : notnull where P3 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> codec, Func<T, P3> getter) =>
        (C1, G1, C2, G2, C3, G3) = (c1, g1, c2, g2, codec, getter);

    public BinaryCodecBuilder<T, P1, P2, P3, P4> Field<P4>(IBinaryCodec<P4> codec, Func<T, P4> getter) where P4 : notnull
        => new(C1, G1, C2, G2, C3, G3, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP3<P1, P2, P3, T>(C1, G1, C2, G2, C3, G3, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> codec, Func<T, P4> getter) =>
        (C1, G1, C2, G2, C3, G3, C4, G4) = (c1, g1, c2, g2, c3, g3, codec, getter);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5> Field<P5>(IBinaryCodec<P5> codec, Func<T, P5> getter) where P5 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP4<P1, P2, P3, P4, T>(C1, G1, C2, G2, C3, G3, C4, G4, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> codec, Func<T, P5> getter) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5) = (c1, g1, c2, g2, c3, g3, c4, g4, codec, getter);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6> Field<P6>(IBinaryCodec<P6> codec, Func<T, P6> getter) where P6 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP5<P1, P2, P3, P4, P5, T>(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> codec, Func<T, P6> getter) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6) = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, codec, getter);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7> Field<P7>(IBinaryCodec<P7> codec, Func<T, P7> getter) where P7 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP6<P1, P2, P3, P4, P5, P6, T>(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull where P7 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> codec, Func<T, P7> getter) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7) = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, codec, getter);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8> Field<P8>(IBinaryCodec<P8> codec, Func<T, P8> getter) where P8 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP7<P1, P2, P3, P4, P5, P6, P7, T>(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull where P7 : notnull where P8 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> codec, Func<T, P8> getter) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8) = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, codec, getter);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9> Field<P9>(IBinaryCodec<P9> codec, Func<T, P9> getter) where P9 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP8<P1, P2, P3, P4, P5, P6, P7, P8, T>(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly IBinaryCodec<P9> C9; internal readonly Func<T, P9> G9;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> c8, Func<T, P8> g8,
        IBinaryCodec<P9> codec, Func<T, P9> getter) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9) = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, c8, g8, codec, getter);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> Field<P10>(IBinaryCodec<P10> codec, Func<T, P10> getter) where P10 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP9<P1, P2, P3, P4, P5, P6, P7, P8, P9, T>(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
    where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull where P10 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly IBinaryCodec<P9> C9; internal readonly Func<T, P9> G9;
    internal readonly IBinaryCodec<P10> C10; internal readonly Func<T, P10> G10;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> c8, Func<T, P8> g8,
        IBinaryCodec<P9> c9, Func<T, P9> g9,
        IBinaryCodec<P10> c10, Func<T, P10> g10) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10)
        = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, c8, g8, c9, g9, c10, g10);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11> Field<P11>(IBinaryCodec<P11> codec, Func<T, P11> getter) where P11 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP10<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, T>(
            C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
    where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull where P10 : notnull where P11 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly IBinaryCodec<P9> C9; internal readonly Func<T, P9> G9;
    internal readonly IBinaryCodec<P10> C10; internal readonly Func<T, P10> G10;
    internal readonly IBinaryCodec<P11> C11; internal readonly Func<T, P11> G11;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> c8, Func<T, P8> g8,
        IBinaryCodec<P9> c9, Func<T, P9> g9,
        IBinaryCodec<P10> c10, Func<T, P10> g10,
        IBinaryCodec<P11> c11, Func<T, P11> g11) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11)
        = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, c8, g8, c9, g9, c10, g10, c11, g11);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12> Field<P12>(IBinaryCodec<P12> codec, Func<T, P12> getter) where P12 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP11<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, T>(
            C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
    where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull where P10 : notnull
    where P11 : notnull where P12 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly IBinaryCodec<P9> C9; internal readonly Func<T, P9> G9;
    internal readonly IBinaryCodec<P10> C10; internal readonly Func<T, P10> G10;
    internal readonly IBinaryCodec<P11> C11; internal readonly Func<T, P11> G11;
    internal readonly IBinaryCodec<P12> C12; internal readonly Func<T, P12> G12;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> c8, Func<T, P8> g8,
        IBinaryCodec<P9> c9, Func<T, P9> g9,
        IBinaryCodec<P10> c10, Func<T, P10> g10,
        IBinaryCodec<P11> c11, Func<T, P11> g11,
        IBinaryCodec<P12> c12, Func<T, P12> g12) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12)
        = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, c8, g8, c9, g9, c10, g10, c11, g11, c12, g12);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13> Field<P13>(IBinaryCodec<P13> codec, Func<T, P13> getter) where P13 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP12<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, T>(
            C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
    where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull where P10 : notnull
    where P11 : notnull where P12 : notnull where P13 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly IBinaryCodec<P9> C9; internal readonly Func<T, P9> G9;
    internal readonly IBinaryCodec<P10> C10; internal readonly Func<T, P10> G10;
    internal readonly IBinaryCodec<P11> C11; internal readonly Func<T, P11> G11;
    internal readonly IBinaryCodec<P12> C12; internal readonly Func<T, P12> G12;
    internal readonly IBinaryCodec<P13> C13; internal readonly Func<T, P13> G13;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> c8, Func<T, P8> g8,
        IBinaryCodec<P9> c9, Func<T, P9> g9,
        IBinaryCodec<P10> c10, Func<T, P10> g10,
        IBinaryCodec<P11> c11, Func<T, P11> g11,
        IBinaryCodec<P12> c12, Func<T, P12> g12,
        IBinaryCodec<P13> c13, Func<T, P13> g13) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13)
        = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, c8, g8, c9, g9, c10, g10, c11, g11, c12, g12, c13, g13);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14> Field<P14>(IBinaryCodec<P14> codec, Func<T, P14> getter) where P14 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP13<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, T>(
            C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
    where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull where P10 : notnull
    where P11 : notnull where P12 : notnull where P13 : notnull where P14 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly IBinaryCodec<P9> C9; internal readonly Func<T, P9> G9;
    internal readonly IBinaryCodec<P10> C10; internal readonly Func<T, P10> G10;
    internal readonly IBinaryCodec<P11> C11; internal readonly Func<T, P11> G11;
    internal readonly IBinaryCodec<P12> C12; internal readonly Func<T, P12> G12;
    internal readonly IBinaryCodec<P13> C13; internal readonly Func<T, P13> G13;
    internal readonly IBinaryCodec<P14> C14; internal readonly Func<T, P14> G14;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> c8, Func<T, P8> g8,
        IBinaryCodec<P9> c9, Func<T, P9> g9,
        IBinaryCodec<P10> c10, Func<T, P10> g10,
        IBinaryCodec<P11> c11, Func<T, P11> g11,
        IBinaryCodec<P12> c12, Func<T, P12> g12,
        IBinaryCodec<P13> c13, Func<T, P13> g13,
        IBinaryCodec<P14> c14, Func<T, P14> g14) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, C14, G14)
        = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, c8, g8, c9, g9, c10, g10, c11, g11, c12, g12, c13, g13, c14, g14);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15> Field<P15>(IBinaryCodec<P15> codec, Func<T, P15> getter) where P15 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, C14, G14, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP14<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, T>(
            C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, C14, G14, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
    where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull where P10 : notnull
    where P11 : notnull where P12 : notnull where P13 : notnull where P14 : notnull where P15 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly IBinaryCodec<P9> C9; internal readonly Func<T, P9> G9;
    internal readonly IBinaryCodec<P10> C10; internal readonly Func<T, P10> G10;
    internal readonly IBinaryCodec<P11> C11; internal readonly Func<T, P11> G11;
    internal readonly IBinaryCodec<P12> C12; internal readonly Func<T, P12> G12;
    internal readonly IBinaryCodec<P13> C13; internal readonly Func<T, P13> G13;
    internal readonly IBinaryCodec<P14> C14; internal readonly Func<T, P14> G14;
    internal readonly IBinaryCodec<P15> C15; internal readonly Func<T, P15> G15;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> c8, Func<T, P8> g8,
        IBinaryCodec<P9> c9, Func<T, P9> g9,
        IBinaryCodec<P10> c10, Func<T, P10> g10,
        IBinaryCodec<P11> c11, Func<T, P11> g11,
        IBinaryCodec<P12> c12, Func<T, P12> g12,
        IBinaryCodec<P13> c13, Func<T, P13> g13,
        IBinaryCodec<P14> c14, Func<T, P14> g14,
        IBinaryCodec<P15> c15, Func<T, P15> g15) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, C14, G14, C15, G15)
        = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, c8, g8, c9, g9, c10, g10, c11, g11, c12, g12, c13, g13, c14, g14, c15, g15);

    public BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16> Field<P16>(IBinaryCodec<P16> codec, Func<T, P16> getter) where P16 : notnull
        => new(C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, C14, G14, C15, G15, codec, getter);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP15<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, T>(
            C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, C14, G14, C15, G15, factory);
}

public readonly struct BinaryCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
    where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull where P10 : notnull
    where P11 : notnull where P12 : notnull where P13 : notnull where P14 : notnull where P15 : notnull where P16 : notnull
{
    internal readonly IBinaryCodec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly IBinaryCodec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly IBinaryCodec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly IBinaryCodec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly IBinaryCodec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly IBinaryCodec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly IBinaryCodec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly IBinaryCodec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly IBinaryCodec<P9> C9; internal readonly Func<T, P9> G9;
    internal readonly IBinaryCodec<P10> C10; internal readonly Func<T, P10> G10;
    internal readonly IBinaryCodec<P11> C11; internal readonly Func<T, P11> G11;
    internal readonly IBinaryCodec<P12> C12; internal readonly Func<T, P12> G12;
    internal readonly IBinaryCodec<P13> C13; internal readonly Func<T, P13> G13;
    internal readonly IBinaryCodec<P14> C14; internal readonly Func<T, P14> G14;
    internal readonly IBinaryCodec<P15> C15; internal readonly Func<T, P15> G15;
    internal readonly IBinaryCodec<P16> C16; internal readonly Func<T, P16> G16;

    internal BinaryCodecBuilder(
        IBinaryCodec<P1> c1, Func<T, P1> g1,
        IBinaryCodec<P2> c2, Func<T, P2> g2,
        IBinaryCodec<P3> c3, Func<T, P3> g3,
        IBinaryCodec<P4> c4, Func<T, P4> g4,
        IBinaryCodec<P5> c5, Func<T, P5> g5,
        IBinaryCodec<P6> c6, Func<T, P6> g6,
        IBinaryCodec<P7> c7, Func<T, P7> g7,
        IBinaryCodec<P8> c8, Func<T, P8> g8,
        IBinaryCodec<P9> c9, Func<T, P9> g9,
        IBinaryCodec<P10> c10, Func<T, P10> g10,
        IBinaryCodec<P11> c11, Func<T, P11> g11,
        IBinaryCodec<P12> c12, Func<T, P12> g12,
        IBinaryCodec<P13> c13, Func<T, P13> g13,
        IBinaryCodec<P14> c14, Func<T, P14> g14,
        IBinaryCodec<P15> c15, Func<T, P15> g15,
        IBinaryCodec<P16> c16, Func<T, P16> g16) =>
        (C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, C14, G14, C15, G15, C16, G16)
        = (c1, g1, c2, g2, c3, g3, c4, g4, c5, g5, c6, g6, c7, g7, c8, g8, c9, g9, c10, g10, c11, g11, c12, g12, c13, g13, c14, g14, c15, g15, c16, g16);

    public IBinaryCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, T> factory)
        => new BinaryCodecDefinitions.BinaryCodecP16<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, T>(
            C1, G1, C2, G2, C3, G3, C4, G4, C5, G5, C6, G6, C7, G7, C8, G8, C9, G9, C10, G10, C11, G11, C12, G12, C13, G13, C14, G14, C15, G15, C16, G16, factory);
}

