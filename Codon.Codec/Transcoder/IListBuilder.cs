namespace Codon.Codec.Transcoder;

public interface IListBuilder<T>
{
    public IListBuilder<T> Add(T value);
    public T Build();
}