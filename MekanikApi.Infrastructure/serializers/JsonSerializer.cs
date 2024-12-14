using System.Text.Json;

namespace MekanikApi.Infrastructure.serializers
{
    public class JsonSerializerOptionsProvider
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static JsonSerializerOptions Options => _options;
    }
}