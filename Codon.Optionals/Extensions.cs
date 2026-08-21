// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.CompilerServices;

namespace Codon.Optionals;

public static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? ToNullableStruct<T>(this Optional<T> optional) where T : struct
    {
        return optional.IsPresent ? optional.Value : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? ToNullableClass<T>(this Optional<T> optional) where T : class
    {
        return optional.IsPresent ? optional.Value : null;
    }

}
