using Newtonsoft.Json;
using System.IO;

namespace ChatBeet.Utilities
{
    public static class StreamExtensions
    {
        public static T DeserializeJson<T>(this Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default;

            using var sr = new StreamReader(stream);
            using var jtr = new JsonTextReader(sr);
            var js = new JsonSerializer();
            var searchResult = js.Deserialize<T>(jtr);
            return searchResult;
        }
    }
}
