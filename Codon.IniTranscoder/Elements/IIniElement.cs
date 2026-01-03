using Codon.IniTranscoder.Exceptions;

namespace Codon.IniTranscoder.Elements;

public interface IIniElement
{
    public IniValue GetAsValueOrThrow()
    {
        if (this is IniValue) return (IniValue)this;
        throw new ParsingException("IniElement is not IniValue");
    }

    public IniSection GetAsSectionOrThrow()
    {
        if (this is IniSection) return (IniSection)this;
        throw new ParsingException("IniElement is not IniSection");
    }

    public IniList GetAsListOrThrow()
    {
        if (this is IniList) return (IniList)this;
        throw new ParsingException("IniElement is not IniList");
    }

}