using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            [Required]
            public double Epsilon { get; set; } = 10;
            [Required]
            public double Perplexity { get; set; } = 3;
            [Required]
            public double Dimensionality { get; set; } = 2;
            [Required]
            public int Iterations { get; set; } = 10000;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();


        public class ViewModel
        {
            public List<(Journal Journal, int Count)> Journals { get; set; } = new List<(Journal Journal, int Count)>();
        }


        public ViewModel View { get; set; } = new ViewModel();



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

            View.Journals = models;
        }
    }
}