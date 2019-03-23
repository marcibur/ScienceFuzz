using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ScienceFuzz.Web.Pages.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class CalculatorModel : PageModel
    {
        [BindProperty(SupportsGet = false)]
        [ValidFileExtensions("csv")]
        public IFormFile CsvFile { get; set; }
        public IEnumerable<Journal> Journals { get; private set; } = new List<Journal>();
        public string ResultsJSON { get; private set; } = JsonConvert.SerializeObject(new double[] { });

        public void OnGet() { }

        public class CsvRow
        {
            public string Journal { get; set; }
            public double Social { get; set; }
            public double Agriculture { get; set; }
            public double Exact { get; set; }
            public double Health { get; set; }
            public double Humanities { get; set; }
            public double Technology { get; set; }
            public double Natural { get; set; }
            public double Arts { get; set; }
            public int Contributions { get; set; }
        }

        public class Journal
        {
            public string Title { get; set; }
            public (double Value, string Class) Social { get; set; }
            public (double Value, string Class) Agriculture { get; set; }
            public (double Value, string Class) Exact { get; set; }
            public (double Value, string Class) Health { get; set; }
            public (double Value, string Class) Humanities { get; set; }
            public (double Value, string Class) Technology { get; set; }
            public (double Value, string Class) Natural { get; set; }
            public (double Value, string Class) Arts { get; set; }
            public int Contributions { get; set; }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid || CsvFile != null)
            {
                using (var reader = new StreamReader(CsvFile.OpenReadStream()))
                using (var csv = new CsvReader(reader))
                {
                    var journals = csv.GetRecords<CsvRow>().ToList();
                    MapToJournals(journals);
                }
            }

            return Page();
        }

        private void Calculate(IEnumerable<CsvRow> rows)
        {

        }

        private void MapToJournals(IEnumerable<CsvRow> rows)
        {
            var journals = new List<Journal>();

            foreach (var row in rows)
            {
                journals.Add(new Journal
                {
                    Title = row.Journal,
                    Social = (row.Social, AssignStyle(row.Social)),
                    Agriculture = (row.Agriculture, AssignStyle(row.Agriculture)),
                    Exact = (row.Exact, AssignStyle(row.Exact)),
                    Health = (row.Health, AssignStyle(row.Health)),
                    Humanities = (row.Humanities, AssignStyle(row.Humanities)),
                    Technology = (row.Technology, AssignStyle(row.Technology)),
                    Natural = (row.Natural, AssignStyle(row.Natural)),
                    Arts = (row.Arts, AssignStyle(row.Arts)),
                    Contributions = row.Contributions
                });
            }

            Journals = journals;
        }

        private string AssignStyle(double value)
        {
            if (value > 0)
            {
                return "domain-score-good";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
