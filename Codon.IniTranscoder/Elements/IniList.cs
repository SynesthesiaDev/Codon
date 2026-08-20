using System.Collections;
using Codon.IniTranscoder.Util;
using Synesthesia.Utils.Extensions;

namespace Codon.IniTranscoder.Elements;

public class IniList : IIniElement, IEnumerable<IniValue>
{
    private readonly List<IniValue> values = [];

    public int Count => values.Count;

    public int Capacity => values.Capacity;

    public void Add(IniValue value) => values.Add(value);

    public void Add(string value) => values.Add(new IniValue(value));

    public void Remove(IniValue value) => values.Remove(value);

    public IniValue Get(int index) => values[0];

    public IniValue? GetOrNull(int index) => values.GetOrNull(index);

    public IEnumerator<IniValue> GetEnumerator() => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();

    public List<IniValue> ToList() => values.ToList();

    public List<IIniElement> ToListRaw() => (values.ToArray() as IIniElement[]).ToList();

    public override string ToString()
    {
        var stringBuilder = "[";

        var index = 0;
        foreach (var value in values)
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
