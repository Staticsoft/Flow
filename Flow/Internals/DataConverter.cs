using Staticsoft.Serialization.Abstractions;
using System.Text;

namespace Staticsoft.Flow.Internals;

class DataConverter<Input, Output>(
    Serializer serializer
)
{
    readonly Serializer Serializer = serializer;

    public string ToId(Input input)
    {
        var uniqueData = $"{typeof(Input).Name}-{typeof(Output).Name}-{Serializer.Serialize(input)}";
        ReadOnlySpan<byte> span = Encoding.UTF8.GetBytes(uniqueData).AsSpan();
        var hash = MurmurHash.MurmurHash3.Hash32(ref span, seed: 0);
        return $"{hash}";
    }
}