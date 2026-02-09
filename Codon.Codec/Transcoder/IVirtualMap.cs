namespace Codon.Codec.Transcoder;

public interface IVirtualMap<T>
{
    List<string> GetKeys();
    bool HasValue(string key);
    T GetValue(string key);

    int Count => GetKeys().Count;
    bool IsEmpty => GetKeys().Count == 0;
}
