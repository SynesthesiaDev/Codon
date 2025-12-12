using Codon.Codec.Transcoder;

namespace Codon.Codec;

public static class Codecs
{
    public static readonly ICodec<bool> Boolean = new PrimitiveCodec<bool>
    (
        (transcoder, b) => transcoder.EncodeBool(b),
        (transcoder, o) => transcoder.DecodeBool(o)
    );

    public static readonly ICodec<byte> Byte = new PrimitiveCodec<byte>
    (
        (transcoder, b) => transcoder.EncodeByte(b),
        (transcoder, o) => transcoder.DecodeByte(o)
    );

    public static readonly ICodec<short> Short = new PrimitiveCodec<short>
    (
        (transcoder, b) => transcoder.EncodeShort(b),
        (transcoder, o) => transcoder.DecodeShort(o)
    );

    public static readonly ICodec<int> Int = new PrimitiveCodec<int>
    (
        (transcoder, b) => transcoder.EncodeInt(b),
        (transcoder, o) => transcoder.DecodeInt(o)
    );

    public static readonly ICodec<long> Long = new PrimitiveCodec<long>
    (
        (transcoder, b) => transcoder.EncodeLong(b),
        (transcoder, o) => transcoder.DecodeLong(o)
    );

    public static readonly ICodec<float> Float = new PrimitiveCodec<float>
    (
        (transcoder, b) => transcoder.EncodeFloat(b),
        (transcoder, o) => transcoder.DecodeFloat(o)
    );

    public static readonly ICodec<double> Double = new PrimitiveCodec<double>
    (
        (transcoder, b) => transcoder.EncodeDouble(b),
        (transcoder, o) => transcoder.DecodeDouble(o)
    );

    public static readonly ICodec<string> String = new PrimitiveCodec<string>
    (
        (transcoder, b) => transcoder.EncodeString(b),
        (transcoder, o) => transcoder.DecodeString(o)
    );

    public static readonly ICodec<byte[]> ByteArray = new PrimitiveCodec<byte[]>
    (
        (transcoder, b) => transcoder.EncodeByteArray(b),
        (transcoder, o) => transcoder.DecodeByteArray(o)
    );

    public static readonly ICodec<int[]> IntArray = new PrimitiveCodec<int[]>
    (
        (transcoder, b) => transcoder.EncodeIntArray(b),
        (transcoder, o) => transcoder.DecodeIntArray(o)
    );

    public static readonly ICodec<long[]> LongArray = new PrimitiveCodec<long[]>
    (
        (transcoder, b) => transcoder.EncodeLongArray(b),
        (transcoder, o) => transcoder.DecodeLongArray(o)
    );

    public static EnumCodec<E> Enum<E>() where E : Enum
    {
        return new EnumCodec<E>();
    }

    public static RecursiveCodec<T> Recursive<T>(Func<ICodec<T>, ICodec<T>> self)
    {
        return new RecursiveCodec<T>(self);
    }

    public class TransformativeCodec<T, S>(ICodec<T> innerCodec, Func<T, S> to, Func<S, T> from) : ICodec<S>
    {
        public D Encode<D>(ITranscoder<D> transcoder, S value)
        {
            return innerCodec.Encode(transcoder, from.Invoke(value));
        }

        public S Decode<D>(ITranscoder<D> transcoder, D value)
        {
            var innerValue = innerCodec.Decode(transcoder, value);
            return to.Invoke(innerValue);
        }
    }

    public class OptionalCodec<T>(ICodec<T> innerCodec) : ICodec<Optional<T>>
    {
        public readonly ICodec<T> Inner = innerCodec;

        public D Encode<D>(ITranscoder<D> transcoder, Optional<T> value)
        {
            return value.IsMissing ? transcoder.EncodeNull() : Inner.Encode(transcoder, value.Value!);
        }

        public Optional<T> Decode<D>(ITranscoder<D> transcoder, D value)
        {
            try
            {
                var decoded = Inner.Decode(transcoder, value);
                return new Optional<T>(decoded);
            }
            catch (Exception e)
            {
                return Optional.Empty<T>();
            }
        }
    }

    public class DefaultCodec<T>(ICodec<T> innerCodec, T defaultValue) : ICodec<T>
    {
        public readonly ICodec<T> Inner = innerCodec;
        public readonly T Default = defaultValue;

        public D Encode<D>(ITranscoder<D> transcoder, T? value)
        {
            return value == null ? Inner.Encode(transcoder, Default) : Inner.Encode(transcoder, value);
        }

        public T Decode<D>(ITranscoder<D> transcoder, D value)
        {
            try
            {
                return Inner.Decode(transcoder, value);
            }
            catch (Exception e)
            {
                return Default;
            }
        }
    }

    public class ForwardRefCodec<T>(Func<ICodec<T>> delegateFunc) : ICodec<T>
    {
        private ICodec<T> delegateCodec = delegateFunc.Invoke();

        public D Encode<D>(ITranscoder<D> transcoder, T value)
        {
            return delegateCodec.Encode(transcoder, value);
        }

        public T Decode<D>(ITranscoder<D> transcoder, D value)
        {
            return delegateCodec.Decode(transcoder, value);
        }
    }

    public class ListCodec<T>(ICodec<T> innerCodec) : ICodec<List<T>>
    {
        public D Encode<D>(ITranscoder<D> transcoder, List<T> value)
        {
            var encodedList = transcoder.EncodeList(value.Count);
            value.ForEach(item => encodedList.Add(innerCodec.Encode(transcoder, item)));
            return encodedList.Build();
        }

        public List<T> Decode<D>(ITranscoder<D> transcoder, D value)
        {
            var listResult = transcoder.DecodeList(value);
            var decodedList = new List<T>();
            listResult.ForEach(item => decodedList.Add(innerCodec.Decode(transcoder, item)));
            return decodedList;
        }
    }

    public class MapCodec<K, V>(ICodec<K> keyCodec, ICodec<V> valueCodec) : ICodec<Dictionary<K, V>> where K : notnull
    {
        public D Encode<D>(ITranscoder<D> transcoder, Dictionary<K, V> value)
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

        public Dictionary<K, V> Decode<D>(ITranscoder<D> transcoder, D value)
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

    public class UnionCodec<T, R>(string keyField, ICodec<T> keyCodec, Func<T, StructCodec<R>> serializers, Func<R, T> keyFunc) : StructCodec<R>
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

    public class EnumCodec<E> : ICodec<E> where E : Enum
    {
        public D Encode<D>(ITranscoder<D> transcoder, E value)
        {
            return String.Encode(transcoder, value.ToString());
        }

        public E Decode<D>(ITranscoder<D> transcoder, D value)
        {
            return (E)System.Enum.Parse(typeof(E), String.Decode(transcoder, value));
        }
    }

    public class RecursiveCodec<T> : ICodec<T>
    {
        private Lazy<ICodec<T>> Delegate;
        public ICodec<T> Inner => Delegate.Value;

        public RecursiveCodec(Func<ICodec<T>, ICodec<T>> self)
        {
            Delegate = new Lazy<ICodec<T>>(self.Invoke(this));
        }

        public D Encode<D>(ITranscoder<D> transcoder, T value)
        {
            return Delegate.Value.Encode(transcoder, value);
        }

        public T Decode<D>(ITranscoder<D> transcoder, D value)
        {
            return Delegate.Value.Decode(transcoder, value);
        }
    }
}