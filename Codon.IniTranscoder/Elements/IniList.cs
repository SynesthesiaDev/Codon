using System.Collections;
using Codon.IniTranscoder.Util;
using SynesthesiaUtil.Extensions;

namespace Codon.IniTranscoder.Elements;

public class IniList : IIniElement, IEnumerable<IniValue>
{
    private List<IniValue> _values = [];

    public int Count => _values.Count;

    public int Capacity => _values.Capacity;

    public void Add(IniValue value) => _values.Add(value);

    public void Add(string value) => _values.Add(new IniValue(value));

    public void Remove(IniValue value) => _values.Remove(value);

    public IniValue Get(int index) => _values[0];

    public IniValue? GetOrNull(int index) => _values.GetOrNull(index);

    public IEnumerator<IniValue> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

    public List<IniValue> ToList() => _values.ToList();

    public List<IIniElement> ToListRaw() => (_values.ToArray() as IIniElement[]).ToList();

    public override string ToString()
    {
        var stringBuilder = "[";

        var index = 0;
        foreach (var value in _values)
        {
            var isLast = index == Count - 1;
            var append = isLast ? string.Empty : ", ";

            stringBuilder += $"{value}{append}";
            index++;
        }

        stringBuilder += "]";
        return stringBuilder;
    }

    public static IniList Parse(string text)
    {
        var cleaned = text.RemovePrefix("[").RemoveSuffix("]");
        var split = cleaned.Split(", ");
        var list = new IniList();
        foreach (var item in split)
        {
            var value = item == "null" ? new IniValue(null) : new IniValue(item);
            list.Add(value);
        }

        return list;
    }
}