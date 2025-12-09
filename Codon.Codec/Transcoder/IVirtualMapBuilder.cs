namespace Codon.Codec.Transcoder;

public interface IVirtualMapBuilder<T>
{
    public IVirtualMapBuilder<T> Put(T key, T value);
    public IVirtualMapBuilder<T> Put(string key, T value);
    public T Build();
}