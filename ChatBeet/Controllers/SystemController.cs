using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ChatBeet.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SystemController : Controller
{
    private readonly DiscordClient _discord;

    public SystemController(DiscordClient discord)
    {
        _discord = discord;
    }

    [HttpGet("Commands")]
    public ActionResult<IReadOnlyCollection<DiscordApplicationCommand>> GetCommands() => Ok(_discord.GetSlashCommands().RegisteredCommands.FirstOrDefault().Value);

    [HttpGet("Status")]
    public ActionResult<StatusModel> GetStatus() => Ok(new StatusModel(Assembly.GetExecutingAssembly().GetName().Version!, DateTime.Now - Process.GetCurrentProcess().StartTime));
}