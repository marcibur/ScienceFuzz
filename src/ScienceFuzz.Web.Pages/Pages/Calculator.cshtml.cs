using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ScienceFuzz.Web.Pages.Tools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class CalculatorModel : PageModel
    {
        [BindProperty(SupportsGet = false), ValidFileExtensions("csv"), Required]
        public IFormFile CsvFile { get; set; }
        public IEnumerable<JournalViewModel> Journals { get; private set; } = new List<JournalViewModel>();
        public string ResultsJSON { get; private set; } = JsonConvert.SerializeObject(new double[] { });

        public void OnGet() { }

        public class JournalViewModel
        {
            public string Title { get; set; }
            public IList<DomainViewModel> Domains { get; set; }
            public int Contributions { get; set; }
        }

        public class DomainViewModel
        {
            public DomainViewModel(string name, double value, string @class, string icon)
            {
                Name = name;
                Value = value;
                Class = @class;
                Icon = icon;
            }

            public string Name { get; set; }
            public double Value { get; set; }
            public string Class { get; set; }
            public string Icon { get; set; }
        }

        public class JournalInputModel
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
            public IEnumerable<Domain> Domains { get; set; }
            public int Contributions { get; set; }
        }

        public class Domain
        {
            public Domain(string name, double value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public double Value { get; set; }
        }

        public Results Scores { get; set; } = new Results
        {
            {"Humanities", 0 },
            {"Social", 0 },
            {"Health", 0 },
            {"Technology", 0 },
            {"Exact", 0 },
            {"Natural", 0 },
            {"Agriculture", 0 },
            {"Arts", 0 }
        };

        public class Results : Dictionary<string, double> { }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid || CsvFile != null)
            {
                using (var reader = new StreamReader(CsvFile.OpenReadStream()))
                using (var csv = new CsvReader(reader))
                {
                    var journals = csv.GetRecords<JournalInputModel>().ToList();
                    var package = PackageDomainsForProcessing(journals);
                    Calculate(package);
                    ScoresToJson();
                    MapToJournalViewModels(journals);
                }
            }

            return Page();
        }

        private void Calculate(IEnumerable<Journal> journals)
        {
            const double A = 0.01;

            foreach (var journal in journals)
            {
                foreach (var domain in journal.Domains)
                {
                    Scores[domain.Name] = S(Scores[domain.Name] * A, domain.Value * A) * journal.Contributions;
                }
            }
        }

        private double S(double x, double y)
        {
            return x + y - x * y;
        }

        private void ScoresToJson()
        {
            ResultsJSON = JsonConvert.SerializeObject(new double[]
            {
                Scores["Humanities"],
                Scores["Social"],
                Scores["Health"],
                Scores["Technology"],
                Scores["Exact"],
                Scores["Natural"],
                Scores["Agriculture"],
                Scores["Arts"]
            });
        }

        private IEnumerable<Journal> PackageDomainsForProcessing(IEnumerable<JournalInputModel> rows)
        {
            var journals = new List<Journal>();
            foreach (var row in rows)
            {
                journals.Add(new Journal
                {
                    Contributions = row.Contributions,
                    Domains = new List<Domain>
                    {
                        new Domain("Social", row.Social),
                        new Domain("Agriculture", row.Agriculture),
                        new Domain("Exact", row.Exact),
                        new Domain("Health", row.Health),
                        new Domain("Humanities", row.Humanities),
                        new Domain("Technology", row.Technology),
                        new Domain("Natural", row.Natural),
                        new Domain("Arts", row.Arts)
                    }
                });
            }

            return journals;
        }

        private void MapToJournalViewModels(IEnumerable<JournalInputModel> rows)
        {
            var journals = new List<JournalViewModel>();

            foreach (var row in rows)
            {
                journals.Add(new JournalViewModel
                {
                    Title = row.Journal,
                    Domains = new List<DomainViewModel>
                    {
                        new DomainViewModel("Technology", row.Technology, AssignStyle(row.Technology), "fas fa-wrench"),
                        new DomainViewModel("Health", row.Health, AssignStyle(row.Health), "fas fa-medkit"),
                        new DomainViewModel("Exact", row.Exact, AssignStyle(row.Exact), "fas fa-search"),
                        new DomainViewModel("Agriculture", row.Agriculture, AssignStyle(row.Agriculture), "fas fa-leaf"),
                        new DomainViewModel("Humanities", row.Humanities, AssignStyle(row.Humanities), "fas fa-book"),
                        new DomainViewModel("Arts", row.Arts, AssignStyle(row.Arts), "fas fa-landmark"),
                        new DomainViewModel("Social", row.Social, AssignStyle(row.Social), "fas fa-users"),
                        new DomainViewModel("Natural", row.Natural, AssignStyle(row.Natural), "fas fa-cloud-sun-rain")
                    },
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
