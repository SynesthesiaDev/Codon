using Codon.Codec;
using Codon.Codec.Json;

namespace Codon.Tests;

public class RecursiveCodecTests
{
    public record Node(string Name, List<Node> Children)
    {
        public static readonly Codec<Node> CODEC = Codecs.Recursive<Node>(self =>
            StructCodec.For<Node>()
                .Field("name", Codecs.STRING, n => n.Name)
                .Field("children", self.List(), n => n.Children)
                .Build((name, children) => new Node(name, children)));
    }

    [Test]
    public void TestRecursiveCodec_EncodeDecodeRoundtrip()
    {
        var tree = new Node(
            "root",
            [
                new Node("child1", []),
                new Node("child2", [new Node("grandchild1", [])])
            ]
        );

        var encoded = Node.CODEC.Encode(JsonTranscoder.INSTANCE, tree);

        var json = encoded.GetRawText();
        Console.WriteLine(json);
        Assert.That(json, Does.Contain("\"name\":\"root\""));
        Assert.That(json, Does.Contain("\"children\""));

        var decoded = Node.CODEC.Decode(JsonTranscoder.INSTANCE, encoded);

        Assert.That(decoded.Name, Is.EqualTo("root"));
        Assert.That(decoded.Children, Has.Count.EqualTo(2));
        Assert.That(decoded.Children[0].Name, Is.EqualTo("child1"));
        Assert.That(decoded.Children[0].Children, Has.Count.EqualTo(0));
        Assert.That(decoded.Children[1].Name, Is.EqualTo("child2"));
        Assert.That(decoded.Children[1].Children, Has.Count.EqualTo(1));
        Assert.That(decoded.Children[1].Children[0].Name, Is.EqualTo("grandchild1"));
    }
}
