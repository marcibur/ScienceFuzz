using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScienceFuzz.Data.InMemory;
using ScienceFuzz.Models;
using System.Collections.Generic;
using System.Linq;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class ScientistsModel : PageModel
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

        public List<Publication> Publications { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(Author))
            {
                Publications = InMemoryData.Publications.ToList();
            }
            else
            {
                Publications = InMemoryData.Publications.Where(x => x.Author == Author).ToList();
            }
        }
    }
}