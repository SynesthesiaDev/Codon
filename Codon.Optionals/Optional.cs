namespace Codon.Optionals;

public class Optional<T>(bool isPresent, T? value)
{
    public T? Value => IsPresent ? value : default;

    public readonly bool IsPresent = isPresent;
    public bool IsMissing => !IsPresent;

    public Optional() : this(false, default)
    {
    }

    public Optional(T? value) : this(value != null, value)
    {
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
    public static Optional<T> Empty<T>() => new(false, default);
    public static Optional<T?> Of<T>(T? value) => new(value);
    public static Optional<T> From<T>(T value) => new(true, value);

}
