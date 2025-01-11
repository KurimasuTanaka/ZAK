using BlazorApp.DA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ZAK.Components.Pages.ApplicationsPage
{
    public class ApplicationsPageModel : PageModel
    {

        public IActionResult OnGet()
        {
            return Page();
        }

    }
}
