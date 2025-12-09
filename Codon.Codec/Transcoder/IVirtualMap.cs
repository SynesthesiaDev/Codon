namespace Codon.Codec.Transcoder;

public interface IVirtualMap<T>
{
    public List<string> GetKeys();
    public bool HasValue(string key);
    public T GetValue(string key);

    public int Count => GetKeys().Count;
    public bool IsEmpty => GetKeys().Count == 0;
}