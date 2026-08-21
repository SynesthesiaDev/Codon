using Codon.Codec.Transcoder;
using Codon.Optionals;

namespace Codon.Codec;

public static class Codecs
{
    public static readonly Codec<bool> BOOLEAN = new PrimitiveCodec<bool>
    (
        (transcoder, b) => transcoder.EncodeBool(b),
        (transcoder, o) => transcoder.DecodeBool(o)
    );

    public static readonly Codec<byte> BYTE = new PrimitiveCodec<byte>
    (
        (transcoder, b) => transcoder.EncodeByte(b),
        (transcoder, o) => transcoder.DecodeByte(o)
    );

    public static readonly Codec<short> SHORT = new PrimitiveCodec<short>
    (
        (transcoder, b) => transcoder.EncodeShort(b),
        (transcoder, o) => transcoder.DecodeShort(o)
    );

    public static readonly Codec<int> INT = new PrimitiveCodec<int>
    (
        (transcoder, b) => transcoder.EncodeInt(b),
        (transcoder, o) => transcoder.DecodeInt(o)
    );

    public static readonly Codec<long> LONG = new PrimitiveCodec<long>
    (
        (transcoder, b) => transcoder.EncodeLong(b),
        (transcoder, o) => transcoder.DecodeLong(o)
    );

    public static readonly Codec<float> FLOAT = new PrimitiveCodec<float>
    (
        (transcoder, b) => transcoder.EncodeFloat(b),
        (transcoder, o) => transcoder.DecodeFloat(o)
    );

    public static readonly Codec<double> DOUBLE = new PrimitiveCodec<double>
    (
        (transcoder, b) => transcoder.EncodeDouble(b),
        (transcoder, o) => transcoder.DecodeDouble(o)
    );

    public static readonly Codec<string> STRING = new PrimitiveCodec<string>
    (
        (transcoder, b) => transcoder.EncodeString(b),
        (transcoder, o) => transcoder.DecodeString(o)
    );

    public static readonly Codec<byte[]> BYTE_ARRAY = new PrimitiveCodec<byte[]>
    (
        (transcoder, b) => transcoder.EncodeByteArray(b),
        (transcoder, o) => transcoder.DecodeByteArray(o)
    );

    public static readonly Codec<int[]> INT_ARRAY = new PrimitiveCodec<int[]>
    (
        (transcoder, b) => transcoder.EncodeIntArray(b),
        (transcoder, o) => transcoder.DecodeIntArray(o)
    );

    public static readonly Codec<long[]> LONG_ARRAY = new PrimitiveCodec<long[]>
    (
        (transcoder, b) => transcoder.EncodeLongArray(b),
        (transcoder, o) => transcoder.DecodeLongArray(o)
    );

    public static readonly Codec<Guid> GUID = STRING.Transform(Guid.Parse, guid => guid.ToString());

    public static Codec<E> Enum<E>() where E : Enum
    {
        return new EnumCodec<E>();
    }

    public static Codec<T> Recursive<T>(Func<Codec<T>, Codec<T>> self) where T : notnull
    {
        return new RecursiveCodec<T>(self);
    }

    public class TransformativeCodec<T, S>(Codec<T> innerCodec, Func<T, S> to, Func<S, T> from) : Codec<S> where T : notnull where S : notnull
    {
        public override D Encode<D>(ITranscoder<D> transcoder, S value)
        {
            return innerCodec.Encode(transcoder, from.Invoke(value));
        }

        public override S Decode<D>(ITranscoder<D> transcoder, D value)
        {
            var innerValue = innerCodec.Decode(transcoder, value);
            return to.Invoke(innerValue);
        }
    }

    public class OptionalCodec<T>(Codec<T> innerCodec) : Codec<Optional<T>> where T : notnull
    {
        public readonly Codec<T> Inner = innerCodec;

        public override D Encode<D>(ITranscoder<D> transcoder, Optional<T> value)
        {
            return value.IsMissing ? transcoder.EncodeNull() : Inner.Encode(transcoder, value.Value!);
        }

        public override Optional<T> Decode<D>(ITranscoder<D> transcoder, D value)
        {
            try
            {
                var nullValue = transcoder.EncodeNull();

                return EqualityComparer<D>.Default.Equals(value, nullValue)
                    ? Optionals.Optional.Empty<T>()
                    : Optionals.Optional.Of<T>(Inner.Decode(transcoder, value));
            }
            catch (Exception)
            {
                return Optionals.Optional.Empty<T>();
            }
        }
    }

    public class DefaultCodec<T>(Codec<T> innerCodec, T defaultValueValue) : Codec<T> where T : notnull
    {
        public readonly Codec<T> Inner = innerCodec;
        public readonly T DefaultValue = defaultValueValue;

        public override D Encode<D>(ITranscoder<D> transcoder, T? value)
        {
            return value == null ? Inner.Encode(transcoder, DefaultValue) : Inner.Encode(transcoder, value);
        }

        public override T Decode<D>(ITranscoder<D> transcoder, D value)
        {
            try
            {
                return Inner.Decode(transcoder, value);
            }
            catch (Exception)
            {
                return DefaultValue;
            }
        }
    }

    public class ForwardRefCodec<T>(Func<Codec<T>> delegateFunc) : Codec<T> where T : notnull
    {
        private readonly Codec<T> delegateCodec = delegateFunc.Invoke();

        public override D Encode<D>(ITranscoder<D> transcoder, T value)
        {
            return delegateCodec.Encode(transcoder, value);
        }

        public override T Decode<D>(ITranscoder<D> transcoder, D value)
        {
            return delegateCodec.Decode(transcoder, value);
        }
    }

    public class ListCodec<T>(Codec<T> innerCodec) : Codec<List<T>> where T : notnull
    {
        public override D Encode<D>(ITranscoder<D> transcoder, List<T> value)
        {
            var encodedList = transcoder.EncodeList(value.Count);
            value.ForEach(item => encodedList.Add(innerCodec.Encode(transcoder, item)));
            return encodedList.Build();
        }

        public override List<T> Decode<D>(ITranscoder<D> transcoder, D value)
        {
            var listResult = transcoder.DecodeList(value);
            var decodedList = new List<T>();
            listResult.ForEach(item => decodedList.Add(innerCodec.Decode(transcoder, item)));
            return decodedList;
        }
    }

    public class MapCodec<K, V>(Codec<K> keyCodec, Codec<V> valueCodec) : Codec<Dictionary<K, V>> where K : notnull where V : notnull
    {
        public override D Encode<D>(ITranscoder<D> transcoder, Dictionary<K, V> value)
        {
            var mapBuilder = transcoder.EncodeMap();
            foreach (var keyValuePair in value)
            {
                var keyResult = keyCodec.Encode(transcoder, keyValuePair.Key);
                var valueResult = valueCodec.Encode(transcoder, keyValuePair.Value);
                mapBuilder.Put(keyResult, valueResult);
            }

            return mapBuilder.Build();
        }

        public override Dictionary<K, V> Decode<D>(ITranscoder<D> transcoder, D value)
        {
            var mapResult = transcoder.DecodeMap(value);
            var decodedMap = new Dictionary<K, V>();

            foreach (var key in mapResult.GetKeys())
            {
                var keyResult = keyCodec.Decode(transcoder, transcoder.EncodeString(key));
                var valueResult = valueCodec.Decode(transcoder, mapResult.GetValue(key));
                decodedMap[keyResult] = valueResult;
            }

            return decodedMap;
        }
    }

    public class UnionCodec<T, R>(string keyField, Codec<T> keyCodec, Func<T, StructCodec<R>> serializers, Func<R, T> keyFunc) : StructCodec<R> where T : notnull
    {
        public override T1 EncodeToMap<T1>(ITranscoder<T1> transcoder, R value, IVirtualMapBuilder<T1> mapBuilder)
        {
            var key = keyFunc.Invoke(value);
            var serializer = serializers.Invoke(key);
            mapBuilder.Put(keyField, keyCodec.Encode(transcoder, key));
            return serializer.EncodeToMap(transcoder, value, mapBuilder);
        }

        public override R DecodeFromMap<T1>(ITranscoder<T1> transcoder, IVirtualMap<T1> map)
        {
            var key = keyCodec.Decode(transcoder, map.GetValue(keyField));
            var serializer = serializers.Invoke(key);
            return serializer.DecodeFromMap(transcoder, map);
        }
    }

    public class EnumCodec<E> : Codec<E> where E : Enum
    {
        public override D Encode<D>(ITranscoder<D> transcoder, E value)
        {
            return STRING.Encode(transcoder, value.ToString());
        }

        public override E Decode<D>(ITranscoder<D> transcoder, D value)
        {
            return (E)System.Enum.Parse(typeof(E), STRING.Decode(transcoder, value));
        }
    }

    public class RecursiveCodec<T> : Codec<T> where T : notnull
    {
        private readonly Lazy<Codec<T>> @delegate;
        public Codec<T> Inner => @delegate.Value;

        public RecursiveCodec(Func<Codec<T>, Codec<T>> self)
        {
            @delegate = new Lazy<Codec<T>>(self.Invoke(this));
        }

        public override D Encode<D>(ITranscoder<D> transcoder, T value)
        {
            return @delegate.Value.Encode(transcoder, value);
        }

        public override T Decode<D>(ITranscoder<D> transcoder, D value)
        {
            return @delegate.Value.Decode(transcoder, value);
        }
    }
}
