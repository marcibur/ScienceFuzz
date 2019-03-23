using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ScienceFuzz.Web.Pages.Tools;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class CalculatorModel : PageModel
    {
        [BindProperty(SupportsGet = false)]
        [ValidFileExtensions("csv")]
        public IFormFile CsvData { get; set; }
        public string ResultsJSON { get; private set; }

        public void OnGet()
        {
            ResultsJSON = JsonConvert.SerializeObject(new double[] { });
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var t = CsvData;

            }

            return Page();
        }
    }
}
