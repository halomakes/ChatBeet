using ChatBeet.Models;
using ChatBeet.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class FunTranslationService
    {
        private readonly HttpClient client;

        public FunTranslationService(IHttpClientFactory clientFactory)
        {
            client = clientFactory.CreateClient();
        }

        public async Task<string> TranslateAsync(string text, string language)
        {
            var content = new Dictionary<string, string>
            {
                ["text"] = text
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://api.funtranslations.com/translate/{language}.json"),
                Content = new FormUrlEncodedContent(content)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                return contentStream.DeserializeJson<FunTranslation>()?.Contents?.Translated;
            }

            return null;
        }
    }
}
