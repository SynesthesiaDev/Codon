namespace Codon.Codec.Transcoder;

public interface IVirtualMapBuilder<T>
{
    IVirtualMapBuilder<T> Put(T key, T value);
    IVirtualMapBuilder<T> Put(string key, T value);
    T Build();
}
