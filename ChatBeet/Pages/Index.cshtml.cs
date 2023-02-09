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
    private readonly ILogger<IndexModel> logger;
    private readonly IMemoryCache cache;
    private readonly IWebHostEnvironment env;

    public string ReadmeHtml { get; private set; }

    public IndexModel(ILogger<IndexModel> logger, IMemoryCache cache, IWebHostEnvironment env)
    {
        this.logger = logger;
        this.cache = cache;
        this.env = env;
    }

    public async Task OnGet()
    {
        var readmeMarkdown = await cache.GetOrCreateAsync("readme", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(15);

            var filepath = Path.Combine(env.WebRootPath, "README.md");
            return await System.IO.File.ReadAllTextAsync(filepath);
        });

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        ReadmeHtml = Markdown.ToHtml(readmeMarkdown, pipeline);
    }
}