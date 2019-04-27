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
                Journals = InMemoryData.Journals.ToList();
            }
            else
            {
                Journals = InMemoryData.Journals.Where(x =>
                    x.Title.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.Discipline1.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.Discipline2.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.MoreDisciplinesString.Trim().ToLower().Contains(Search.Trim().ToLower()))
                        .ToList();
            }
        }
    }
}