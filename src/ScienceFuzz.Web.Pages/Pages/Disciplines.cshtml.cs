using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScienceFuzz.Data.InMemory;
using ScienceFuzz.Models;
using System.Collections.Generic;
using System.Linq;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class DisciplinesModel : PageModel
    {
        public List<Discipline> Disciplines { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(Search))
            {
                Disciplines = InMemoryData.Disciplines
                    .OrderBy(x => x.Title)
                        .ToList();
            }
            else
            {
                Disciplines = InMemoryData.Disciplines.Where(x =>
                    x.Title.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.DomainsA.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.DomainsB.Trim().ToLower().Contains(Search.Trim().ToLower()))
                        .OrderBy(x => x.Title)
                            .ToList();
            }
        }
    }
}