using System.Linq;
using System.Security.Claims;

namespace ChatBeet.Utilities;

public static class UserExtensions
{
    public static string GetNick(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == "nick")?.Value;
}