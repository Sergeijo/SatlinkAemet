using Newtonsoft.Json;

using Satlink.Logic;

namespace Satlink.Infrastructure;

public sealed class AemetJsonSerializer : IAemetJsonSerializer
{
    public T? Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(json);
    }
}
