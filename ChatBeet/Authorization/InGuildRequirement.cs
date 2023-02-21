using System.Threading.Tasks;
using ChatBeet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChatBeet.Authorization;

public class InGuildRequirement : IAuthorizationRequirement
{
    public const string Policy = "InGuild";
    
    public InGuildRequirement(string routeParameter = "guildId")
    {
        RouteParameter = routeParameter;
    }

    public string RouteParameter { get; }
}

public class InGuildHandler : AuthorizationHandler<InGuildRequirement>
{
    private readonly GuildService _guilds;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WebIdentityService _identityService;

    public InGuildHandler(GuildService guilds, IHttpContextAccessor httpContextAccessor, WebIdentityService identityService)
    {
        _guilds = guilds;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, InGuildRequirement requirement)
    {
        var routeData = _httpContextAccessor.HttpContext!.GetRouteData();
        if (routeData.Values[requirement.RouteParameter] is string rawValue && ulong.TryParse(rawValue, out var guildId))
        {
            var guild = _guilds.GetGuilds().FirstOrDefault(g => g.Id == guildId);
            if (guild is not null)
            {
                var members = await _guilds.GetMembersAsync(guild);
                var currentUser = await _identityService.GetCurrentUserAsync();
                if (members.Any(m => m.Id == currentUser.Discord?.Id))
                    context.Succeed(requirement);
            }
        }
    }
}