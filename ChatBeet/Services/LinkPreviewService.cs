﻿using Microsoft.Extensions.Caching.Memory;
using OpenGraphNet;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ChatBeet.Services
{
    public class LinkPreviewService
    {
        private readonly IMemoryCache cache;
        private readonly HttpClient client;

        public LinkPreviewService(IMemoryCache cache, HttpClient client)
        {
            this.cache = cache;
            this.client = client;
        }

        public async Task<OpenGraph> GetMetadataAsync(Uri url)
        {
            var content = await GetUrlContentAsync(url);
            return OpenGraph.ParseHtml(content);
        }

        private Task<string> GetUrlContentAsync(Uri url) => cache.GetOrCreateAsync($"opengraph:{url}", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new("text/html"));
            request.Headers.Accept.Add(new("application/xhtml+xml"));
            request.Headers.Accept.Add(new("application/xml", 0.9));
            request.Headers.AcceptLanguage.Add(new("en-US", 0.9));
            request.Headers.AcceptLanguage.Add(new("en", 0.9));
            request.Headers.UserAgent.Add(new("Mozilla", "5.0"));
            request.Headers.UserAgent.Add(new("(Windows NT 10.0; Win64; x64)"));
            request.Headers.UserAgent.Add(new("AppleWebKit", "537.36"));
            request.Headers.UserAgent.Add(new("(KHTML, like Gecko)"));
            request.Headers.UserAgent.Add(new("Chrome", "96.0.4664.110"));
            request.Headers.UserAgent.Add(new("Safari", "537.36"));
            request.Headers.UserAgent.Add(new("Edg", "96.0.1054.62"));
            request.Headers.Referrer = new Uri("https://www.google.com/");
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new WebException($"Received non-success status from remote server {request.RequestUri} - {(int)response.StatusCode} ({response.StatusCode}).");
            }
        });
    }
}
