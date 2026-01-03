using Codon.Codec;
using Codon.Codec.Transcoder.Transcoders;

namespace Codon.Tests;

public class RecursiveCodecTests
{
    public record Node(string name, List<Node> children)
    {
        public static readonly ICodec<Node> Codec = Codecs.Recursive<Node>(self =>
            StructCodec.Of(
                "name", Codecs.String, n => n.name,
                "children", self.List(), n => n.children,
                (name, children) => new Node(name, children)
            )
        );
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

        var encoded = Node.Codec.Encode(JsonTranscoder.Instance, tree);

        var json = encoded.GetRawText();
        Console.WriteLine(json);
        Assert.That(json, Does.Contain("\"name\":\"root\""));
        Assert.That(json, Does.Contain("\"children\""));

        var decoded = Node.Codec.Decode(JsonTranscoder.Instance, encoded);

        Assert.That(decoded.name, Is.EqualTo("root"));
        Assert.That(decoded.children, Has.Count.EqualTo(2));
        Assert.That(decoded.children[0].name, Is.EqualTo("child1"));
        Assert.That(decoded.children[0].children, Has.Count.EqualTo(0));
        Assert.That(decoded.children[1].name, Is.EqualTo("child2"));
        Assert.That(decoded.children[1].children, Has.Count.EqualTo(1));
        Assert.That(decoded.children[1].children[0].name, Is.EqualTo("grandchild1"));
    }
}