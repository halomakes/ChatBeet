using Markdig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ChatBeet.Pages;

[ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _env;

    public string ReadmeHtml { get; private set; }

    public IndexModel(ILogger<IndexModel> logger, IMemoryCache cache, IWebHostEnvironment env)
    {
        _logger = logger;
        _cache = cache;
        _env = env;
    }

    public async Task OnGet()
    {
        var readmeMarkdown = await _cache.GetOrCreateAsync("readme", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(15);

            var filepath = Path.Combine(_env.WebRootPath, "README.md");
            return await System.IO.File.ReadAllTextAsync(filepath);
        });

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        ReadmeHtml = Markdown.ToHtml(readmeMarkdown, pipeline);
    }
}