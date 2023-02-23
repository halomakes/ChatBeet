using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Authorization;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Controllers;

[Route("api/Guilds/{guildId}/[controller]")]
[ApiController]
[Authorize(Policy = InGuildRequirement.Policy)]
public class QuotesController : Controller
{
    private readonly IQuoteRepository _quotes;

    public QuotesController(IQuoteRepository quotes)
    {
        _quotes = quotes;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes([FromRoute] ulong guildId, CancellationToken cancellationToken)
    {
        var quotes = await _quotes.Quotes
            .AsNoTracking()
            .Include(q => q.SavedBy)
            .Where(q => q.GuildId == guildId)
            .ToListAsync(cancellationToken);
        return Ok(quotes);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<Quote>> GetQuotes([FromRoute] ulong guildId, [FromRoute] string slug, CancellationToken cancellationToken)
    {
        var quote = await _quotes.Quotes
            .AsNoTracking()
            .Include(q => q.SavedBy)
            .Include(q => q.Messages)!
            .ThenInclude(q => q.Author)
            .Where(q => q.GuildId == guildId && q.Slug == slug)
            .FirstOrDefaultAsync(cancellationToken);
        return quote is null ? NotFound() : Ok(quote);
    }
    
    [HttpGet("{slug}/Messages")]
    public async Task<ActionResult<IEnumerable<QuoteMessage>>> GetQuoteMessages([FromRoute] ulong guildId, [FromRoute] string slug, CancellationToken cancellationToken)
    {
        var quote = await _quotes.Quotes
            .AsNoTracking()
            .Include(q => q.SavedBy)
            .Include(q => q.Messages)!
            .ThenInclude(q => q.Author)
            .Where(q => q.GuildId == guildId && q.Slug == slug)
            .FirstOrDefaultAsync(cancellationToken);
        return quote is null ? NotFound() : Ok(quote.Messages);
    }
}