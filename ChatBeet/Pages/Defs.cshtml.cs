using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Utilities;
using GravyBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages
{
    public class DefsModel : PageModel
    {
        private readonly MemoryCellContext db;
        private readonly MessageQueueService messageQueue;

        public IEnumerable<MemoryCell> Cells { get; private set; }

        [BindProperty]
        public DefinitionChange Info { get; set; }

        public DefsModel(MemoryCellContext db, MessageQueueService messageQueue)
        {
            this.db = db;
            this.messageQueue = messageQueue;
        }

        public async Task OnGet()
        {
            var defs = await db.MemoryCells.ToListAsync();
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
                    var oldDef = await db.MemoryCells.FirstOrDefaultAsync(m => m.Key.ToLower() == Info.Key.ToLower());
                    Info.NewNick = User.GetNick();
                    if (oldDef == default)
                    {
                        db.MemoryCells.Add(new MemoryCell
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

                    await db.SaveChangesAsync();
                    messageQueue.Push(Info);
                }
            }

            return RedirectToPage("/Defs");
        }
    }
}
