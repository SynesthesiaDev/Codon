# 🧬 Codon

Codon is a lightweight codec library for .NET

Codon has five packages:

- `Codon.BinaryBuffer` - custom implementation of binary buffer, included in `BinaryCodec` package
- `Codon.BinaryCodec` - for serialization to binary format
- `Codon.Codec` - for serialization into any format using transcoders (JSON included)
- `Codon.Optional` - Optional class as replacement for nullability because nullable generics SUCK in C#
- `Codon.IniTranscoder` - Optional package that includes transcoder for .ini format

## Codecs

Lets define a basic codec for `Person` class:
(All examples are using `JsonTranscoder` but any transcoder may be used)

```csharp
public record Person(string name, int age, bool isAwesome)
{
    public static readonly StructCodec<Person> Codec = StructCodec.Of
    (
        "name", Codecs.String, p => p.name,
        "age", Codecs.Int, p => p.age,
        "is_awesome", Codecs.Boolean, p => p.isAwesome,
        (name, age, isAwesome) => new Person(name, age, isAwesome) 
    );
}
```

To Serialize or our `Person` object we can use:

```csharp
var person = new Person("Silly Billy", 18, true);

var encoded = Person.Codec.Encode(JsonTranscoder.Instance, person);
Console.WriteLine(encoded.GetRawText()); // {"name":"Silly Billy","age":18,"is_awesome":true}
```

We can then decode the json back with:

```csharp
var decoded = Person.Codec.Decode(JsonTranscoder.Instance, encoded);
Console.WriteLine(decoded); // Person { name = Silly Billy, age = 18, isAwesome = True }
```

---

You can nest codecs by referencing them in another codec. Lets add `PersonalInformation` class to our `Person` class:

```csharp
public record PersonalInformation(string address, int height, int weight)
{
    public static readonly StructCodec<PersonalInformation> Codec = StructCodec.Of
    (
        "address", Codecs.String, p => p.address,
        "height", Codecs.Int, p => p.height,
        "weight", Codecs.Int, p => p.weight,
        (address, height, weight) => new PersonalInformation(address, height, weight) 
    );
}
```

Now we can reference it in our `Person` class just like this:

```csharp
"personal_information", PersonalInformation.Codec, p => p.personalInformation,
(name, age, someBoolean, personalInformation) => new Person(name, age, someBoolean, personalInformation)
```

### Optional and Default fields

```csharp
using Codon.Codec;
using Codon.Codec.Transcoder.Transcoders;

public record User(string id, Optional<string> displayName, int level) 
{
    public static readonly StructCodec<User> Codec = StructCodec.Of(
        "id", Codecs.String, u => u.id,
        "display_name", Codecs.String.Optional(), u => u.displayName,
        "level", Codecs.Int.Default(1), u => u.level,
        (id, displayName, level) => new User(id, displayName, level)
    );
};
```

Missing optional field decodes to `Optional.Empty`

(Note: Optional class is a custom class for wrapping null values because generics in C# suck and don't pass nullable
generics properly)

```csharp
var json = JsonDocument.Parse("{\"id\":\"u1\"}").RootElement;
var decodedUser = User.Codec.Decode(JsonTranscoder.Instance, json);
Console.WriteLine(decodedUser) // User { id = "u1", displayName = null, level = 1 }
```

### Lists and Maps

```csharp
var listCodec = Codecs.Int.List(); // ICodec<List<int>>
var mapCodec  = Codecs.String.MapTo(Codecs.Int); // ICodec<Dictionary<string,int>>

var list = new List<int> { 1, 2, 3 };
var map = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };

var encodedList = listCodec.Encode(t, list);
var encodedMap = mapCodec.Encode(t, map);
```

### Enums

```csharp
enum Color { Red, Green, Blue }

var colorCodec = Codecs.Enum<Color>();
var encodedColor = colorCodec.Encode(t, Color.Green);
```

### Transformative codecs

Wrap one codec to expose values of another type using two conversion functions via the `.Transform<Out>(to, from)`
helper on the inner codec:

```csharp
// Store an int using an inner string codec (int <-> string)
var intAsString = Codecs.String.Transform<int>(
    to: s => int.Parse(s),
    from: i => i.ToString()
);

var encoded = intAsString.Encode(t, 12345);
var decoded = intAsString.Decode(t, enc); // 12345
```

### Polymorphic Unions (discriminator based)

```csharp
using Codon.Codec;
using Codon.Codec.Transcoder;
using Codon.Codec.Transcoder.Transcoders;

abstract record Shape;

record Rect(int w, int h) : Shape;

enum Kind { Rect }

// Base codec for Rect
var rectCodec = StructCodec.Of(
    "w", Codecs.Int, r => r.w,
    "h", Codecs.Int, r => r.h,
    (w, h) => new Rect(w, h)
);

// Sometimes you may need a small adapter to upcast StructCodec<Rect> to StructCodec<Shape>
StructCodec<Shape> Upcast(StructCodec<Rect> inner) => new UpcastStructCodec<Shape, Rect>(inner, s => (Rect)s, r => r);

// Build a union codec using an enum discriminator and `.Union(...)` helper
var shapeCodec = ((ICodec<Kind>)Codecs.Enum<Kind>()).Union<Shape>(
    keyField: "kind",
    serializers: kind => kind switch { Kind.Rect => Upcast(rectCodec), _ => throw new InvalidOperationException() },
    keyFunc: shape => shape switch { Rect => Kind.Rect, _ => throw new InvalidOperationException() }
);

// Encode automatically adds the discriminator
var encodedShape = shapeCodec.Encode(JsonTranscoder.Instance, new Rect(3, 4));
var encodedShape = shapeCodec.Decode(JsonTranscoder.Instance, encShape);
```

### Inline nested struct

`StructCodec` supports an "inline" key that allows embedding a nested struct without an extra object level. Use
`StructCodec.Inline` as the field name (Note: optional/default wrappers are handled when inlined)

```csharp
public record Outer(int id, Inner inner) 
{
    public static readonly StructCodec<Outer> OuterCodec = StructCodec.Of(
        "id", Codecs.Int, o => o.id,
        StructCodec.Inline, InnerCodec, o => o.inner,
        (id, inner) => new Outer(id, inner)
    );
}

public record Inner(string name) 
{
    public static readonly StructCodec<Inner> InnerCodec = StructCodec.Of(
        "name", Codecs.String, i => i.name,
        name => new Inner(name)
    );
    
}
```

```csharp
var example = new Outer(67, new Inner("funny"));
var encoded = Outer.Codec.Encode(JsonTranscoder.Instance, example);
```

This would be encoded as following:

```json
{
  "id": 67,
  "name": "Funny"
}
```

### Transcoders

- The examples use the JSON transcoder (`JsonTranscoder.Instance`), but any transport can be supported by implementing
  `ITranscoder<T>`.
- `StructCodec` encodes into the transcoder’s concept of a map/object and decodes from it using provided field
  definitions.
- Optional and Default wrappers help you model absent fields and fallback values.

---

See the test suite under `Codon.Tests` for broader coverage (lists, maps, enums, unions, forward refs, array helpers,
etc.).

## BinaryCodecs

BinaryCodecs work with a BinaryBuffer and have a
similar API to normal codecs (Optional, Default, List, MapTo, Transform, Enum, Recursive, and composite Of helpers).

Quick roundtrip with primitives:

```csharp
using Codon.Binary;
using Codon.Buffer;

var buf = new BinaryBuffer();

// Write
BinaryCodec.Int.Write(buf, 42);
BinaryCodec.String.Write(buf, "hello");
BinaryCodec.Boolean.Write(buf, true);

// Read back in the same order
var int = BinaryCodec.Int.Read(buf);       // 42
var string = BinaryCodec.String.Read(buf); // "hello"
var bool = BinaryCodec.Boolean.Read(buf);  // true
```

Lists, maps, optionals, enums:

```csharp
// List and Dictionary helpers
var listCodec = BinaryCodec.Int.List(); // IBinaryCodec<List<int>>
var mapCodec  = BinaryCodec.String.MapTo(BinaryCodec.Int); // IBinaryCodec<Dictionary<string,int>>

var list = new List<int> { 1, 2, 3 };
listCodec.Write(buffer, list);
var listBack = listCodec.Read(buffer);

// Optional
var optInt = BinaryCodec.Int.Optional();
optInt.Write(buffer, Optional.Of(123));
var readOpt = optInt.Read(buffer); // present 123

// Enums
enum Color { Red, Green, Blue }
var colorCodec = BinaryCodec.Enum<Color>();
colorCodec.Write(buffer, Color.Green);
var color = colorCodec.Read(buffer); // Color.Green
```

Composite codecs (struct-like) using Of(...):

```csharp
public record Person(string name, int age, bool active)
{
    public static readonly IBinaryCodec<Person> Codec = BinaryCodec.Of(
        BinaryCodec.String, p => p.name,
        BinaryCodec.Int, p => p.age,
        BinaryCodec.Boolean, p => p.active,
        (name, age, active) => new Person(name, age, active)
    );
}

Person.Codec.Write(buffer, new Person("Alice", 30, true));
var person = Person.Codec.Read(buffer);
```

Transform between types:

```csharp
// Store a string as its length using the inner Int codec
var lengthAsString = BinaryCodec.Int.Transform(
    from: (string s) => s.Length,
    to:   (int n)    => new string('x', n)
);

lengthAsString.Write(buffer, "abcde");
var restored = lengthAsString.Read(buffer); //
```

Recursive structures are supported via `BinaryCodec.Recursive(self => ...)`:

```csharp
public record Node(string name, List<Node> children)
{
    public static readonly IBinaryCodec<Node> Codec = BinaryCodec.Recursive<Node>(self =>
        BinaryCodec.Of(
            BinaryCodec.String, n => n.name,
            self.List(),        n => n.children,
            (name, children) => new Node(name, children)
        )
    );
}
```
---
BinaryBuffer has helpers like `ToArray()`/`FromArray` and `ReaderIndex`/`WriterIndex` if you need more control over IO.