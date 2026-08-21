namespace Codon.Optionals;

public class Optional<T>
{
    public T? Value { get; set; }
    public bool IsPresent { get; set; }
    public bool IsMissing => !IsPresent;

    public Optional()
    {
        IsPresent = false;
        Value = default;
    }

    public Optional(object? value)
    {
        if (value is null)
        {
            IsPresent = false;
            Value = default;
        }
        else
        {
            IsPresent = true;
            Value = (T)value;
        }
    }

    public T GetOrElse(T defaultValue) => IsPresent ? Value! : defaultValue;

    public override string ToString() => IsMissing ? "null" : Value!.ToString()!;

    public bool Equals(Optional<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (IsMissing && other.IsMissing) return true;
        if (IsMissing || other.IsMissing) return false;

        return EqualityComparer<T>.Default.Equals(Value!, other.Value!);
    }

    public override bool Equals(object? obj) => obj is Optional<T> other && Equals(other);

    public override int GetHashCode() => IsMissing ? 0 : EqualityComparer<T>.Default.GetHashCode(Value!);

}

public static class Optional
{
    public static Optional<T> Empty<T>() => new();

    public static Optional<T> Of<T>(object? value) => new(value);
}
