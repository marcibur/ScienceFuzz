using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScienceFuzz.Data.InMemory;
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
            [Display(Name = "Dyscypliny_A")]
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

        public void OnGet()
        {

        }

        public void OnPost()
        {
            if (ModelState.IsValid)
            {
                Processed = true;
            }
        }
    }
}