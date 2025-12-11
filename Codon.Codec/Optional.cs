namespace Codon.Codec;

public class Optional<T>
{
    public readonly T? Value;

    public readonly bool IsPresent;
    public bool IsMissing => !IsPresent;

    public Optional()
    {
        IsPresent = false;
        Value = default;
    }

    public Optional(T? value)
    {
        IsPresent = value != null;
        Value = value;
    }

    public T GetOrElse(T defaultValue)
    {
        return IsPresent ? Value! : defaultValue;
    }

    public override string ToString()
    {
        return IsMissing ? "null" : Value!.ToString()!;
    }

    public bool Equals(Optional<T>? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        if (IsMissing && other.IsMissing) return true;

        if (IsMissing || other.IsMissing) return false;

        return EqualityComparer<T>.Default.Equals(Value!, other.Value!);
    }

    public override bool Equals(object? obj)
    {
        return obj is Optional<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return IsMissing ? 0 : EqualityComparer<T>.Default.GetHashCode(Value!);
    }
}

public static class Optional
{
    public static Optional<T> Empty<T>() => new();
    public static Optional<T> Of<T>(T value) => new(value);
}