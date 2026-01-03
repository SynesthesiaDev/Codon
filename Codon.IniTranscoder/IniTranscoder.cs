using Codon.Codec.Transcoder;
using Codon.IniTranscoder.Elements;
using Codon.IniTranscoder.Exceptions;

namespace Codon.IniTranscoder;

public class IniTranscoder : ITranscoder<IIniElement>
{
    public static readonly IniTranscoder Instance = new();

    private string? _nextSectionName;

    public IniTranscoder Named(string name)
    {
        _nextSectionName = name;
        return this;
    }

    private class IniVirtualMapBuilder(IniSection section) : IVirtualMapBuilder<IIniElement>
    {
        public IVirtualMapBuilder<IIniElement> Put(IIniElement key, IIniElement value)
        {
            return Put(key.GetAsValueOrThrow().GetOrThrow<string>(), value);
        }

        public IVirtualMapBuilder<IIniElement> Put(string key, IIniElement value)
        {
            if (value is IniValue iniValue)
            {
                section.Add(key, iniValue);
            }
            else
            {
                section.Add(key, new IniValue(value.ToString()));
            }

            return this;
        }

        public IIniElement Build()
        {
            return section;
        }
    }

    private class IniVirtualMap(IniSection section) : IVirtualMap<IIniElement>
    {
        public List<string> GetKeys()
        {
            return section.Values.Keys.ToList();
        }

        public bool HasValue(string key)
        {
            return section.Values.ContainsKey(key);
        }

        public IIniElement GetValue(string key)
        {
            return section.GetOrThrow(key);
        }
    }

    private class IniListBuilder(IniList list) : IListBuilder<IIniElement>
    {
        public IListBuilder<IIniElement> Add(IIniElement value)
        {
            if (value is IniValue iniValue)
            {
                list.Add(iniValue);
            }
            else
            {
                list.Add(new IniValue(value.ToString()));
            }

            return this;
        }

        public IIniElement Build()
        {
            return list;
        }
    }

    public IIniElement EncodeNull()
    {
        return IniValue.Null;
    }

    public IIniElement EncodeBool(bool value)
    {
        return new IniValue(value.ToString());
    }

    public bool DecodeBool(IIniElement value)
    {
        return value.GetAsValueOrThrow().GetOrThrow<bool>();
    }

    public IIniElement EncodeByte(byte value)
    {
        return new IniValue(value.ToString());
    }

    public byte DecodeByte(IIniElement value)
    {
        return value.GetAsValueOrThrow().GetOrThrow<byte>();
    }

    public IIniElement EncodeShort(short value)
    {
        return new IniValue(value.ToString());
    }

    public short DecodeShort(IIniElement value)
    {
        return value.GetAsValueOrThrow().GetOrThrow<short>();
    }

    public IIniElement EncodeInt(int value)
    {
        return new IniValue(value.ToString());
    }

    public int DecodeInt(IIniElement value)
    {
        return value.GetAsValueOrThrow().GetOrThrow<int>();
    }

    public IIniElement EncodeLong(long value)
    {
        return new IniValue(value.ToString());
    }

    public long DecodeLong(IIniElement value)
    {
        return value.GetAsValueOrThrow().GetOrThrow<long>();
    }

    public IIniElement EncodeFloat(float value)
    {
        return new IniValue(value.ToString());
    }

    public float DecodeFloat(IIniElement value)
    {
        return value.GetAsValueOrThrow().GetOrThrow<float>();
    }

    public IIniElement EncodeDouble(double value)
    {
        return new IniValue(value.ToString());
    }

    public double DecodeDouble(IIniElement value)
    {
        return value.GetAsValueOrThrow().GetOrThrow<double>();
    }

    public IIniElement EncodeString(string value)
    {
        return new IniValue(value);
    }

    public string DecodeString(IIniElement value)
    {
        return value.GetAsValueOrThrow().GetOrThrow<string>();
    }

    public IListBuilder<IIniElement> EncodeList(int size)
    {
        var list = new IniList();
        return new IniListBuilder(list);
    }

    public List<IIniElement> DecodeList(IIniElement value)
    {
        if (value is IniList list) return list.ToListRaw();
        if (value is IniValue iniValue && iniValue.Value != null)
        {
            var text = iniValue.Value;
            if (text.StartsWith('[') && text.EndsWith(']'))
            {
                return IniList.Parse(text).ToListRaw();
            }
        }
        throw new ParsingException("value is not IniList or stringified IniList");
    }

    public IVirtualMapBuilder<IIniElement> EncodeMap()
    {
        var section = new IniSection(_nextSectionName, []);
        _nextSectionName = null;
        return new IniVirtualMapBuilder(section);
    }

    public IVirtualMap<IIniElement> DecodeMap(IIniElement value)
    {
        return new IniVirtualMap(value.GetAsSectionOrThrow());
    }
}