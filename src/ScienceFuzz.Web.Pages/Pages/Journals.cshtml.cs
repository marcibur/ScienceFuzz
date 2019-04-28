using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScienceFuzz.Data.InMemory;
using ScienceFuzz.Models;
using System.Collections.Generic;
using System.Linq;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class JournalsModel : PageModel
    {
        public List<Journal> Journals { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(Search))
            {
                Journals = InMemoryData.Journals
                    .OrderBy(x => x.Title)
                        .ToList();
            }
            else
            {
                Journals = InMemoryData.Journals.Where(x =>
                    x.Title.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.DisciplineA.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.DisciplineB.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.DisciplinesCString.Trim().ToLower().Contains(Search.Trim().ToLower()))
                        .OrderBy(x => x.Title)
                            .ToList();
            }
        }
    }
}