using Codon.Codec;
using Codon.IniTranscoder.Elements;
using Codon.IniTranscoder.Util;
using Codon.Tests.Util;

namespace Codon.Tests;

public class IniParserTests
{
    private static string SectionTestText = AssemblyUtil.GetTextResource(AssemblyInfo.TestAssembly, "Codon.Tests.Resources.section_test.ini");
    private static string SectionTestWithoutHeaderText = AssemblyUtil.GetTextResource(AssemblyInfo.TestAssembly, "Codon.Tests.Resources.section_test_without_header.ini");

    public record User(string Name, int Age, bool IsTester, List<string> Properties)
    {
        public static readonly ICodec<User> Codec = StructCodec.Of
        (
            "name", Codecs.String, u => u.Name,
            "age", Codecs.Int, u => u.Age,
            "is_tester", Codecs.Boolean, u => u.IsTester,
            "properties", Codecs.String.List(), u => u.Properties,
            (name, age, isTester, properties) => new User(name, age, isTester, properties)
        ).WithSection("User");
    }

    [Test]
    public void TestTranscoder()
    {
        var user = new User("Synesthesia", 20, false, ["is_trans", "is_pan"]);
        var ini = User.Codec.Encode(IniTranscoder.IniTranscoder.Instance, user);
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
        Assert.AreEqual(stringified, newList.ToString());
    }

    [Test]
    public void TestSectionToString()
    {
        var section = IniSection.Parse(SectionTestText);
        var stringified = section.ToString();

        var newSection = IniSection.Parse(stringified);
        Console.WriteLine(stringified);
        Assert.AreEqual(stringified, newSection.ToString());
    }

    [Test]
    public void TestParseSection()
    {
        var section = IniSection.Parse(SectionTestText);
        foreach (var iniKeyValuePair in section.Values)
        {
            Console.WriteLine(iniKeyValuePair);
        }

        Assert.That(section.Name, Is.EqualTo("section name Woaaah"));
        Assert.That(section.Values.Count, Is.EqualTo(5));

        AssertEquals(section.Values["boolianing"], "True");
        AssertEquals(section.Values["doubling"], "0.4");
        AssertEquals(section.Values["floating"], "6.7f");
        AssertEquals(section.Values["strining"], "ayoo");
        AssertEquals(section.Values["strining2"], "ayoo");
    }

    [Test]
    public void TestParseSectionWithoutHeader()
    {
        var section = IniSection.Parse(SectionTestWithoutHeaderText);
        foreach (var iniKeyValuePair in section.Values)
        {
            Console.WriteLine(iniKeyValuePair);
        }

        Assert.That(section.Name, Is.Null);
        Assert.That(section.Values.Count, Is.EqualTo(5));

        AssertEquals(section.Values["boolianing"], "True");
        AssertEquals(section.Values["doubling"], "0.4");
        AssertEquals(section.Values["floating"], "6.7f");
        AssertEquals(section.Values["strining"], "ayoo");
        AssertEquals(section.Values["strining2"], "ayoo");
    }

    private static void AssertEquals(IniValue iniValue, string? value)
    {
        Assert.That(iniValue.Value, Is.EqualTo(value));
    }
}