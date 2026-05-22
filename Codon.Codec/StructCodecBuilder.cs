// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace Codon.Codec;

public readonly struct StructCodecBuilder<T>
{
    public StructCodecBuilder<T, P1> Field<P1>(string name, Codec<P1> codec, Func<T, P1> getter) where P1 : notnull
        => new(name, codec, getter);
}

public readonly struct StructCodecBuilder<T, P1> where P1 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;

    internal StructCodecBuilder(string name, Codec<P1> codec, Func<T, P1> getter)
        => (N1, C1, G1) = (name, codec, getter);

    public StructCodecBuilder<T, P1, P2> Field<P2>(string name, Codec<P2> codec, Func<T, P2> getter) where P2 : notnull
        => new(N1, C1, G1, name, codec, getter);

    public StructCodec<T> Build(Func<P1, T> factory)
        => new StructCodec<object>.StructCodec1P<P1, T>(N1, C1, G1, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2> where P1 : notnull where P2 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string name, Codec<P2> codec, Func<T, P2> getter) =>
        (N1, C1, G1, N2, C2, G2) = (n1, c1, g1, name, codec, getter);

    public StructCodecBuilder<T, P1, P2, P3> Field<P3>(string name, Codec<P3> codec, Func<T, P3> getter) where P3 : notnull
        => new(N1, C1, G1, N2, C2, G2, name, codec, getter);

    public StructCodec<T> Build(Func<P1, P2, T> factory)
        => new StructCodec<object>.StructCodec2P<P1, P2, T>(N1, C1, G1, N2, C2, G2, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2, P3> where P1 : notnull where P2 : notnull where P3 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly string N3; internal readonly Codec<P3> C3; internal readonly Func<T, P3> G3;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string n2, Codec<P2> c2, Func<T, P2> g2,
        string name, Codec<P3> codec, Func<T, P3> getter) =>
        (N1, C1, G1, N2, C2, G2, N3, C3, G3) = (n1, c1, g1, n2, c2, g2, name, codec, getter);

    public StructCodecBuilder<T, P1, P2, P3, P4> Field<P4>(string name, Codec<P4> codec, Func<T, P4> getter) where P4 : notnull
        => new(N1, C1, G1, N2, C2, G2, N3, C3, G3, name, codec, getter);

    public StructCodec<T> Build(Func<P1, P2, P3, T> factory)
        => new StructCodec<object>.StructCodec3P<P1, P2, P3, T>(N1, C1, G1, N2, C2, G2, N3, C3, G3, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2, P3, P4>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly string N3; internal readonly Codec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly string N4; internal readonly Codec<P4> C4; internal readonly Func<T, P4> G4;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string n2, Codec<P2> c2, Func<T, P2> g2,
        string n3, Codec<P3> c3, Func<T, P3> g3,
        string name, Codec<P4> codec, Func<T, P4> getter) =>
        (N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4) = (n1, c1, g1, n2, c2, g2, n3, c3, g3, name, codec, getter);

    public StructCodecBuilder<T, P1, P2, P3, P4, P5> Field<P5>(string name, Codec<P5> codec, Func<T, P5> getter) where P5 : notnull
        => new(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, name, codec, getter);

    public StructCodec<T> Build(Func<P1, P2, P3, P4, T> factory)
        => new StructCodec<object>.StructCodec4P<P1, P2, P3, P4, T>(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2, P3, P4, P5>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly string N3; internal readonly Codec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly string N4; internal readonly Codec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly string N5; internal readonly Codec<P5> C5; internal readonly Func<T, P5> G5;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string n2, Codec<P2> c2, Func<T, P2> g2,
        string n3, Codec<P3> c3, Func<T, P3> g3,
        string n4, Codec<P4> c4, Func<T, P4> g4,
        string name, Codec<P5> codec, Func<T, P5> getter) =>
        (N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5) = (n1, c1, g1, n2, c2, g2, n3, c3, g3, n4, c4, g4, name, codec, getter);

    public StructCodecBuilder<T, P1, P2, P3, P4, P5, P6> Field<P6>(string name, Codec<P6> codec, Func<T, P6> getter) where P6 : notnull
        => new(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, name, codec, getter);

    public StructCodec<T> Build(Func<P1, P2, P3, P4, P5, T> factory)
        => new StructCodec<object>.StructCodec5P<P1, P2, P3, P4, P5, T>(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2, P3, P4, P5, P6>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly string N3; internal readonly Codec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly string N4; internal readonly Codec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly string N5; internal readonly Codec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly string N6; internal readonly Codec<P6> C6; internal readonly Func<T, P6> G6;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string n2, Codec<P2> c2, Func<T, P2> g2,
        string n3, Codec<P3> c3, Func<T, P3> g3,
        string n4, Codec<P4> c4, Func<T, P4> g4,
        string n5, Codec<P5> c5, Func<T, P5> g5,
        string name, Codec<P6> codec, Func<T, P6> getter) =>
        (N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6) = (n1, c1, g1, n2, c2, g2, n3, c3, g3, n4, c4, g4, n5, c5, g5, name, codec, getter);

    public StructCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7> Field<P7>(string name, Codec<P7> codec, Func<T, P7> getter) where P7 : notnull
        => new(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, name, codec, getter);

    public StructCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, T> factory)
        => new StructCodec<object>.StructCodec6P<P1, P2, P3, P4, P5, P6, T>(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull where P7 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly string N3; internal readonly Codec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly string N4; internal readonly Codec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly string N5; internal readonly Codec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly string N6; internal readonly Codec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly string N7; internal readonly Codec<P7> C7; internal readonly Func<T, P7> G7;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string n2, Codec<P2> c2, Func<T, P2> g2,
        string n3, Codec<P3> c3, Func<T, P3> g3,
        string n4, Codec<P4> c4, Func<T, P4> g4,
        string n5, Codec<P5> c5, Func<T, P5> g5,
        string n6, Codec<P6> c6, Func<T, P6> g6,
        string name, Codec<P7> codec, Func<T, P7> getter) =>
        (N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7) = (n1, c1, g1, n2, c2, g2, n3, c3, g3, n4, c4, g4, n5, c5, g5, n6, c6, g6, name, codec, getter);

    public StructCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8> Field<P8>(string name, Codec<P8> codec, Func<T, P8> getter) where P8 : notnull
        => new(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, name, codec, getter);

    public StructCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, T> factory)
        => new StructCodec<object>.StructCodec7P<P1, P2, P3, P4, P5, P6, P7, T>(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull where P7 : notnull where P8 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly string N3; internal readonly Codec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly string N4; internal readonly Codec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly string N5; internal readonly Codec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly string N6; internal readonly Codec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly string N7; internal readonly Codec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly string N8; internal readonly Codec<P8> C8; internal readonly Func<T, P8> G8;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string n2, Codec<P2> c2, Func<T, P2> g2,
        string n3, Codec<P3> c3, Func<T, P3> g3,
        string n4, Codec<P4> c4, Func<T, P4> g4,
        string n5, Codec<P5> c5, Func<T, P5> g5,
        string n6, Codec<P6> c6, Func<T, P6> g6,
        string n7, Codec<P7> c7, Func<T, P7> g7,
        string name, Codec<P8> codec, Func<T, P8> getter) =>
        (N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, N8, C8, G8) = (n1, c1, g1, n2, c2, g2, n3, c3, g3, n4, c4, g4, n5, c5, g5, n6, c6, g6, n7, c7, g7, name, codec, getter);

    public StructCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9> Field<P9>(string name, Codec<P9> codec, Func<T, P9> getter) where P9 : notnull
        => new(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, N8, C8, G8, name, codec, getter);

    public StructCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, T> factory)
        => new StructCodec<object>.StructCodec8P<P1, P2, P3, P4, P5, P6, P7, P8, T>(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, N8, C8, G8, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly string N3; internal readonly Codec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly string N4; internal readonly Codec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly string N5; internal readonly Codec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly string N6; internal readonly Codec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly string N7; internal readonly Codec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly string N8; internal readonly Codec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly string N9; internal readonly Codec<P9> C9; internal readonly Func<T, P9> G9;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string n2, Codec<P2> c2, Func<T, P2> g2,
        string n3, Codec<P3> c3, Func<T, P3> g3,
        string n4, Codec<P4> c4, Func<T, P4> g4,
        string n5, Codec<P5> c5, Func<T, P5> g5,
        string n6, Codec<P6> c6, Func<T, P6> g6,
        string n7, Codec<P7> c7, Func<T, P7> g7,
        string n8, Codec<P8> c8, Func<T, P8> g8,
        string name, Codec<P9> codec, Func<T, P9> getter) =>
        (N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, N8, C8, G8, N9, C9, G9) = (n1, c1, g1, n2, c2, g2, n3, c3, g3, n4, c4, g4, n5, c5, g5, n6, c6, g6, n7, c7, g7, n8, c8, g8, name, codec, getter);

    public StructCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> Field<P10>(string name, Codec<P10> codec, Func<T, P10> getter) where P10 : notnull
        => new(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, N8, C8, G8, N9, C9, G9, name, codec, getter);

    public StructCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, T> factory)
        => new StructCodec<object>.StructCodec9P<P1, P2, P3, P4, P5, P6, P7, P8, P9, T>(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, N8, C8, G8, N9, C9, G9, factory);
}

public readonly struct StructCodecBuilder<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>
    where P1 : notnull where P2 : notnull where P3 : notnull where P4 : notnull where P5 : notnull where P6 : notnull where P7 : notnull where P8 : notnull where P9 : notnull where P10 : notnull
{
    internal readonly string N1; internal readonly Codec<P1> C1; internal readonly Func<T, P1> G1;
    internal readonly string N2; internal readonly Codec<P2> C2; internal readonly Func<T, P2> G2;
    internal readonly string N3; internal readonly Codec<P3> C3; internal readonly Func<T, P3> G3;
    internal readonly string N4; internal readonly Codec<P4> C4; internal readonly Func<T, P4> G4;
    internal readonly string N5; internal readonly Codec<P5> C5; internal readonly Func<T, P5> G5;
    internal readonly string N6; internal readonly Codec<P6> C6; internal readonly Func<T, P6> G6;
    internal readonly string N7; internal readonly Codec<P7> C7; internal readonly Func<T, P7> G7;
    internal readonly string N8; internal readonly Codec<P8> C8; internal readonly Func<T, P8> G8;
    internal readonly string N9; internal readonly Codec<P9> C9; internal readonly Func<T, P9> G9;
    internal readonly string N10; internal readonly Codec<P10> C10; internal readonly Func<T, P10> G10;

    internal StructCodecBuilder(
        string n1, Codec<P1> c1, Func<T, P1> g1,
        string n2, Codec<P2> c2, Func<T, P2> g2,
        string n3, Codec<P3> c3, Func<T, P3> g3,
        string n4, Codec<P4> c4, Func<T, P4> g4,
        string n5, Codec<P5> c5, Func<T, P5> g5,
        string n6, Codec<P6> c6, Func<T, P6> g6,
        string n7, Codec<P7> c7, Func<T, P7> g7,
        string n8, Codec<P8> c8, Func<T, P8> g8,
        string n9, Codec<P9> c9, Func<T, P9> g9,
        string name, Codec<P10> codec, Func<T, P10> getter) =>
        (N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, N8, C8, G8, N9, C9, G9, N10, C10, G10) = (n1, c1, g1, n2, c2, g2, n3, c3, g3, n4, c4, g4, n5, c5, g5, n6, c6, g6, n7, c7, g7, n8, c8, g8, n9, c9, g9, name, codec, getter);

    // No .Field() method here since we are capping it at 10 flat properties.
    public StructCodec<T> Build(Func<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, T> factory)
        => new StructCodec<object>.StructCodec10P<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, T>(N1, C1, G1, N2, C2, G2, N3, C3, G3, N4, C4, G4, N5, C5, G5, N6, C6, G6, N7, C7, G7, N8, C8, G8, N9, C9, G9, N10, C10, G10, factory);
}
