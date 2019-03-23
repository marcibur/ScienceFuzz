using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ScienceFuzz.Web.Pages.Tools;
using System.IO;
using System.Linq;

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

        public class CsvRow
        {
            public string Journal { get; set; }
            public double Social { get; set; }
            public double Agriculture { get; set; }
            public double Exact { get; set; }
            public double Medicine { get; set; }
            public double Humanities { get; set; }
            public double Technology { get; set; }
            public double Natural { get; set; }
            public double Arts { get; set; }
            public int Contributions { get; set; }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid || CsvData != null)
            {
                using (var reader = new StreamReader(CsvData.OpenReadStream()))
                using (var csv = new CsvReader(reader))
                {
                    var journals = csv.GetRecords<CsvRow>().ToList();
                }
            }

            return Page();
        }
    }
}
