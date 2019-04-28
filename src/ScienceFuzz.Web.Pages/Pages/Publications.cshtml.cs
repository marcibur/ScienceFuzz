using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScienceFuzz.Data.InMemory;
using ScienceFuzz.Models;
using System.Collections.Generic;
using System.Linq;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class PublicationsModel : PageModel
    {
        public List<SelectListItem> Scientists { get; } = new List<SelectListItem>(
            InMemoryData.Publications
            .Select(x => x.Author)
            .Distinct()
                .Select(x => new SelectListItem(x, x))
                .Prepend(new SelectListItem("Wszyscy", ""))
                    .ToList());

        [BindProperty(SupportsGet = true)]
        public string Author { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public List<Publication> Publications { get; set; }

        public void OnGet()
        {
            Publications = InMemoryData.Publications.ToList();

            if (!string.IsNullOrWhiteSpace(Author))
            {
                Publications = Publications.Where(x => x.Author == Author).ToList();
            }

            if (!string.IsNullOrEmpty(Search))
            {
                Publications = Publications.Where(x =>
                    x.Title.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.JournalFull.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.JournalShort.Trim().ToLower().Contains(Search.Trim().ToLower()) ||
                    x.FormalType.Trim().ToLower().Contains(Search.Trim().ToLower()))
                        .ToList();
            }

            Publications = Publications
                .OrderBy(x => x.Author)
                .ThenBy(x => x.Title)
                    .ToList();
        }
    }
}