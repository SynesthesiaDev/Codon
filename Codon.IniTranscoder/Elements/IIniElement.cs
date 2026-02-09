using Codon.IniTranscoder.Exceptions;

namespace Codon.IniTranscoder.Elements;

public interface IIniElement
{
    IniValue GetAsValueOrThrow()
    {
        if (this is IniValue) return (IniValue)this;
        throw new ParsingException("IniElement is not IniValue");
    }

    IniSection GetAsSectionOrThrow()
    {
        if (this is IniSection) return (IniSection)this;
        throw new ParsingException("IniElement is not IniSection");
    }

    IniList GetAsListOrThrow()
    {
        if (this is IniList) return (IniList)this;
        throw new ParsingException("IniElement is not IniList");
    }

}
