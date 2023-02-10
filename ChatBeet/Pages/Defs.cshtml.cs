using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Pages;

public class DefsModel : PageModel
{
    private readonly IDefinitionsRepository _db;
    private readonly IMediator _messageQueue;
    private readonly WebIdentityService _webIdentity;

    public IEnumerable<Definition> Cells { get; private set; }

    [BindProperty]
    public DefinitionChange Info { get; set; }

    public DefsModel(IDefinitionsRepository db, IMediator messageQueue, WebIdentityService webIdentity)
    {
        _db = db;
        _messageQueue = messageQueue;
        _webIdentity = webIdentity;
    }

    public async Task OnGet()
    {
        var defs = await _db.Definitions.ToListAsync();
        defs.Reverse();
        Cells = defs;
    }

    public async Task<IActionResult> OnPost()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            if (ModelState.IsValid)
            {
                Info.Key = Info.Key.Trim();
                Info.NewValue = Info.NewValue.Trim();
                var oldDef = await _db.Definitions
                    .Include(d => d.Author)
                    .FirstOrDefaultAsync(m => m.Key.ToLower() == Info.Key.ToLower());
                var currentUser = await _webIdentity.GetCurrentUserAsync();
                Info.NewUser = currentUser;
                if (oldDef == default)
                {
                    _db.Definitions.Add(new Definition
                    {
                        Key = Info.Key,
                        Value = Info.NewValue,
                        CreatedBy = currentUser.Id
                    });
                }
                else
                {
                    Info.OldValue = oldDef.Value;
                    Info.OldUser = oldDef.Author;

                    oldDef.CreatedBy = currentUser.Id;
                    oldDef.Value = Info.NewValue;
                }

                await _db.SaveChangesAsync();
                await _messageQueue.Publish(Info);
            }
        }

        return RedirectToPage("/Defs");
    }
}