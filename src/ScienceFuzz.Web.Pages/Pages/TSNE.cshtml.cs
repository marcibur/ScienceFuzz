using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class TSNEModel : PageModel
    {
        public bool Calculate { get; set; } = false;

        public void OnGet()
        {

        }

        public void OnPost()
        {
            Calculate = true;
        }
    }
}