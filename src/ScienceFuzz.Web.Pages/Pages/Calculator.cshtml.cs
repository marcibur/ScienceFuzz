using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class CalculatorModel : PageModel
    {
        public string ResultsJSON { get; private set; }

        public void OnGet()
        {
            ResultsJSON = JsonConvert.SerializeObject(new double[] { });
        }
    }
}
