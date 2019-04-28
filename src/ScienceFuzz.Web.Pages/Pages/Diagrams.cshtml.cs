using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ScienceFuzz.Data.InMemory;
using ScienceFuzz.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class DiagramsModel : PageModel
    {
        public bool Processed { get; set; } = false;



        public List<SelectListItem> Scientists { get; } = new List<SelectListItem>(
           InMemoryData.Publications
           .Select(x => x.Author)
           .Distinct()
               .Select(x => new SelectListItem(x, x))
               .Where(x => x.Value != "Naukowiec_1")
                    .Prepend(new SelectListItem("Naukowiec_1", "Naukowiec_1", true)))
                        .ToList();



        public class InputModel
        {
            [Required]
            public string Scientist { get; set; } = "Naukowiec_1";

            [Required]
            public double Disciplines_A { get; set; } = 1;
            [Required]
            public double Disciplines_B { get; set; } = 0.5;
            [Required]
            public double Disciplines_C { get; set; } = 0.2;

            [Required]
            public double Domains_A { get; set; } = 1;
            [Required]
            public double Domains_B { get; set; } = 0.5;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();


        public class ViewModel
        {
            public List<(Journal Journal, int Count)> Journals { get; set; } = new List<(Journal Journal, int Count)>();
            public List<string> LostJournals { get; set; } = new List<string>();

            public List<DisciplineResult> Disciplines { get; set; } = new List<DisciplineResult>();
            public string DisciplinesLabelsJson { get; set; }
            public string DisciplinesValuesJson { get; set; }

            public IEnumerable<DomainResult> Domains { get; set; } = new List<DomainResult>();
            public string DomainsLabelsJson { get; set; }
            public string DomainsValuesJson { get; set; }
        }


        public ViewModel View { get; set; } = new ViewModel();



        public class DisciplineResult
        {
            public string Name { get; set; }
            public double Result { get; set; }
            public string DomainsA { get; set; }
            public string DomainsB { get; set; }
        }



        public void OnGet()
        {

        }

        public void OnPost()
        {
            if (ModelState.IsValid)
            {
                var journalFullTitles = GetJournalFullTitles();
                var journals = GetJournals(journalFullTitles);
                SetJournalViewModels(journals);

                var diciplineContributions = GetDisciplineContributions(journals);
                View.Disciplines = CalculateDisciplines(diciplineContributions);
                View.DisciplinesLabelsJson = DisciplinesToLablesJson(View.Disciplines);
                View.DisciplinesValuesJson = DisciplinesToValuesJson(View.Disciplines);

                var domainContributions = GetDomainContributions(View.Disciplines);
                View.Domains = CalculateDomainResults(domainContributions);
                View.DomainsLabelsJson = DomainsToLabelsJson(View.Domains);
                View.DomainsValuesJson = DomainsToValuesJson(View.Domains);

                Processed = true;
            }
        }



        private IEnumerable<string> GetJournalFullTitles()
        {
            var journalFullTitles = InMemoryData.Publications
                   .Where(x => x.Author.Trim().ToLower() == Input.Scientist.Trim().ToLower())
                   .Select(x => x.JournalFull.Trim().ToLower())
                       .ToList();

            return journalFullTitles;
        }

        private IEnumerable<Journal> GetJournals(IEnumerable<string> journalFullTitles)
        {
            var journals = new List<Journal>();

            foreach (var journalFullTitle in journalFullTitles)
            {
                var journal = InMemoryData.Journals.FirstOrDefault(x => x.Title.Trim().ToLower() == journalFullTitle);
                if (journal != null)
                {
                    journals.Add(journal);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(journalFullTitle) && !View.LostJournals.Contains(journalFullTitle))
                    {
                        View.LostJournals.Add(journalFullTitle);
                    }
                }
            }

            return journals;
        }

        private void SetJournalViewModels(IEnumerable<Journal> journals)
        {
            var models = new List<(Journal Journal, int Count)>();

            foreach (var journal in journals)
            {
                if (!models.Select(x => x.Journal).Contains(journal))
                {
                    models.Add((journal, 1));
                }
                else
                {
                    var temp = models.First(x => x.Journal == journal);
                    var newCount = temp.Count + 1;
                    models.Remove(temp);
                    models.Add((journal, newCount));
                }
            }

            View.Journals = models.OrderBy(x => x.Journal.Title).ToList();
        }

        private Dictionary<string, List<double>> GetDisciplineContributions(IEnumerable<Journal> journals)
        {
            var contributions = new Dictionary<string, List<double>>();

            foreach (var journal in journals)
            {
                var aKeyName = journal.DisciplineA.Trim().ToLower();
                if (!string.IsNullOrWhiteSpace(aKeyName))
                {
                    if (!contributions.ContainsKey(aKeyName))
                    {
                        contributions[aKeyName] = new List<double>();
                        contributions[aKeyName].Add(Input.Disciplines_A);
                    }
                    else
                    {
                        contributions[aKeyName].Add(Input.Disciplines_A);
                    }
                }

                var bKeyName = journal.DisciplineB.Trim().ToLower();
                if (!string.IsNullOrWhiteSpace(bKeyName))
                {
                    if (!contributions.ContainsKey(bKeyName))
                    {
                        contributions[bKeyName] = new List<double>();
                        contributions[bKeyName].Add(Input.Disciplines_B);
                    }
                    else
                    {
                        contributions[bKeyName].Add(Input.Disciplines_B);
                    }
                }

                var cKeyNames = journal.DisciplinesCString.Trim().ToLower().Split(',');
                foreach (var cKeyName in cKeyNames)
                {
                    var keyName = cKeyName.Trim();
                    if (!string.IsNullOrWhiteSpace(keyName))
                    {
                        if (!contributions.ContainsKey(keyName))
                        {
                            contributions[keyName] = new List<double>();
                            contributions[keyName].Add(Input.Disciplines_C);
                        }
                        else
                        {
                            contributions[keyName].Add(Input.Disciplines_C);
                        }
                    }
                }
            }

            return contributions;
        }

        private List<DisciplineResult> CalculateDisciplines(Dictionary<string, List<double>> disciplines)
        {
            var result = new List<DisciplineResult>();

            foreach (var discipline in disciplines)
            {
                var score = Calculate(discipline.Value);
                string domainsA = "";
                string domainsB = "";

                var d = InMemoryData.Disciplines.FirstOrDefault(x => x.Title.Trim().ToLower() == discipline.Key);
                if (d != null)
                {
                    domainsA = d.DomainsA;
                    domainsB = d.DomainsB;
                }

                result.Add(new DisciplineResult
                {
                    Name = discipline.Key.Trim(),
                    Result = score,
                    DomainsA = domainsA,
                    DomainsB = domainsB
                });
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        private double Calculate(List<double> scores)
        {
            const double A = 0.01;
            double result = 0;

            foreach (var score in scores)
            {
                result = S(result, score * A);
            }

            return result;
        }

        private double S(double x, double y)
        {
            return x + y - x * y;
        }

        private string DisciplinesToLablesJson(List<DisciplineResult> disciplines)
        {
            var result = disciplines.Select(x => x.Name).ToArray();
            var resultJson = JsonConvert.SerializeObject(result);

            return resultJson;
        }

        private string DisciplinesToValuesJson(List<DisciplineResult> disciplines)
        {
            var result = disciplines.Select(x => x.Result).ToArray();
            var resultJson = JsonConvert.SerializeObject(result);
            return resultJson;
        }

        public class DomainResult
        {
            public string Name { get; set; }
            public double Result { get; set; }
        }

        private Dictionary<string, List<double>> GetDomainContributions(IEnumerable<DisciplineResult> disciplinesResults)
        {
            var contributions = new Dictionary<string, List<double>>();

            foreach (var result in disciplinesResults)
            {
                var discipline = InMemoryData.Disciplines
                    .FirstOrDefault(x => x.Title.Trim().ToLower() == result.Name.Trim().ToLower());

                if (discipline != null)
                {
                    var domainA = discipline.DomainsA.Trim().ToLower();

                    if (!string.IsNullOrWhiteSpace(domainA))
                    {
                        if (!contributions.ContainsKey(domainA))
                        {
                            contributions[domainA] = new List<double>();
                            contributions[domainA].Add(Input.Domains_A);
                        }
                        else
                        {
                            contributions[domainA].Add(Input.Domains_A);
                        }
                    }

                    var domainB = discipline.DomainsB.Trim().ToLower();

                    if (!string.IsNullOrWhiteSpace(domainB))
                    {
                        if (!contributions.ContainsKey(domainB))
                        {
                            contributions[domainB] = new List<double>();
                            contributions[domainB].Add(Input.Domains_B);
                        }
                        else
                        {
                            contributions[domainB].Add(Input.Domains_B);
                        }
                    }
                }

            }

            foreach (var domain in InMemoryData.Domains)
            {
                if (!contributions.ContainsKey(domain))
                {
                    contributions[domain] = new List<double>();
                    contributions[domain].Add(0);
                }
            }

            return contributions;
        }

        private IEnumerable<DomainResult> CalculateDomainResults(Dictionary<string, List<double>> domainContributions)
        {
            var results = new List<DomainResult>();

            foreach (var contribution in domainContributions)
            {
                var result = Calculate(contribution.Value);
                results.Add(new DomainResult
                {
                    Name = contribution.Key.Trim(),
                    Result = result
                });
            }

            return results.OrderBy(x => x.Name).ToList();
        }

        private string DomainsToLabelsJson(IEnumerable<DomainResult> domainResults)
        {
            var labels = domainResults.Select(x => x.Name).ToArray();
            var result = JsonConvert.SerializeObject(labels);
            return result;
        }

        private string DomainsToValuesJson(IEnumerable<DomainResult> domainResults)
        {
            var values = domainResults.Select(x => x.Result).ToArray();
            var result = JsonConvert.SerializeObject(values);
            return result;
        }
    }
}