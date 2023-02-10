using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;

namespace ChatBeet.Pages;

public class DefsModel : PageModel
{
    private readonly MemoryCellContext _db;
    private readonly IMediator _messageQueue;

    public IEnumerable<MemoryCell> Cells { get; private set; }

    [BindProperty]
    public DefinitionChange Info { get; set; }

    public DefsModel(MemoryCellContext db, IMediator messageQueue)
    {
        _db = db;
        _messageQueue = messageQueue;
    }

    public async Task OnGet()
    {
        var defs = await _db.MemoryCells.ToListAsync();
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
                var oldDef = await _db.MemoryCells.FirstOrDefaultAsync(m => m.Key.ToLower() == Info.Key.ToLower());
                Info.NewNick = User.GetNick();
                if (oldDef == default)
                {
                    _db.MemoryCells.Add(new MemoryCell
                    {
                        Key = Info.Key,
                        Value = Info.NewValue,
                        Author = Info.NewNick
                    });
                }
                else
                {
                    Info.OldValue = oldDef.Value;
                    Info.OldNick = oldDef.Author;

                    oldDef.Author = Info.NewNick;
                    oldDef.Value = Info.NewValue;
                }

                await _db.SaveChangesAsync();
                await _messageQueue.Publish(Info);
            }
        }

        return RedirectToPage("/Defs");
    }
}