using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatBeet.Pages.Tags
{
    public class TagModel : PageModel
    {
        public string TagName { get; private set; }

        public async Task OnGet(string tagName)
        {
            TagName = tagName.ToLower();
        }
    }
}