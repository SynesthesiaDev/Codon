namespace Codon.IniTranscoder.Exceptions;

[Serializable]
public class ParsingException(string message) : Exception(message);