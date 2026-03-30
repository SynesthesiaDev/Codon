using Codon.Codec;
using Codon.IniTranscoder.Elements;
using Codon.IniTranscoder.Util;
using SynesthesiaUtil.Extensions;

namespace Codon.Tests;

public class IniParserTests
{
    private static readonly string section_test_text = AssemblyInfo.TestAssembly.GetTextResource("Codon.Tests.Resources.section_test.ini");
    private static readonly string section_test_without_header_text = AssemblyInfo.TestAssembly.GetTextResource("Codon.Tests.Resources.section_test_without_header.ini");

    public record User(string Name, int Age, bool IsTester, List<string> Properties)
    {
        public static readonly Codec<User> CODEC = StructCodec.Of
        (
            "name", Codecs.STRING, u => u.Name,
            "age", Codecs.INT, u => u.Age,
            "is_tester", Codecs.BOOLEAN, u => u.IsTester,
            "properties", Codecs.STRING.List(), u => u.Properties,
            (name, age, isTester, properties) => new User(name, age, isTester, properties)
        ).WithSection("User");
    }

    [Test]
    public void TestTranscoder()
    {
        var user = new User("Synesthesia", 20, false, ["is_trans", "is_pan"]);
        var ini = User.CODEC.Encode(IniTranscoder.IniTranscoder.INSTANCE, user);
        Console.WriteLine(ini);
    }

    [Test]
    public void TestListToString()
    {
        var list = new IniList
        {
            "test",
            "wow",
            "null",
            "aaaaaaa"
        };

        var stringified = list.ToString();
        Console.WriteLine(stringified);

        var newList = IniList.Parse(stringified);
        Console.WriteLine(newList.ToString());
        Assert.That(newList.ToString(), Is.EqualTo(stringified));
    }

    [Test]
    public void TestSectionToString()
    {
        var section = IniSection.Parse(section_test_text);
        var stringified = section.ToString();

        var newSection = IniSection.Parse(stringified);
        Console.WriteLine(stringified);
        Assert.That(newSection.ToString(), Is.EqualTo(stringified));
    }

    [Test]
    public void TestParseSection()
    {
        var section = IniSection.Parse(section_test_text);
        foreach (var iniKeyValuePair in section.Values)
        {
            Console.WriteLine(iniKeyValuePair);
        }

        Assert.That(section.Name, Is.EqualTo("section name Woaaah"));
        Assert.That(section.Values.Count, Is.EqualTo(5));

        assertEquals(section.Values["boolianing"], "True");
        assertEquals(section.Values["doubling"], "0.4");
        assertEquals(section.Values["floating"], "6.7f");
        assertEquals(section.Values["strining"], "ayoo");
        assertEquals(section.Values["strining2"], "ayoo");
    }

    [Test]
    public void TestParseSectionWithoutHeader()
    {
        var section = IniSection.Parse(section_test_without_header_text);
        foreach (var iniKeyValuePair in section.Values)
        {
            Console.WriteLine(iniKeyValuePair);
        }

        Assert.That(section.Name, Is.Null);
        Assert.That(section.Values.Count, Is.EqualTo(5));

        assertEquals(section.Values["boolianing"], "True");
        assertEquals(section.Values["doubling"], "0.4");
        assertEquals(section.Values["floating"], "6.7f");
        assertEquals(section.Values["strining"], "ayoo");
        assertEquals(section.Values["strining2"], "ayoo");
    }

    private static void assertEquals(IniValue iniValue, string? value)
    {
        Assert.That(iniValue.Value, Is.EqualTo(value));
    }
}
