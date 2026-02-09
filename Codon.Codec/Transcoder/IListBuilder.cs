namespace Codon.Codec.Transcoder;

public interface IListBuilder<T>
{
    IListBuilder<T> Add(T value);
    T Build();
}
