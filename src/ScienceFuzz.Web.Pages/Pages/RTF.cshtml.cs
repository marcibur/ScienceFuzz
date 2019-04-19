using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScienceFuzz.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScienceFuzz.Web.Pages.Pages
{
    public class RTFModel : PageModel
    {
        public class InputModel
        {
            [Required]
            public string AuthorSurname { get; set; }

            [Required]
            public string FormalTypes { get; set; }

            [Required]
            public IFormFile Rtv { get; set; }
        }


        [BindProperty]
        public InputModel Input { get; set; }
        public List<Publication> Publications { get; set; } = new List<Publication>();



        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string rtvString = string.Empty;

            using (var stream = Input.Rtv.OpenReadStream())
            {
                rtvString = await new StreamReader(stream, Encoding.UTF8).ReadToEndAsync();
            }

            var publicationRegex = new Regex(@"<BR> \d*\. <BR>.+?txt end");
            var publicationStrings = publicationRegex.Matches(rtvString).Select(m => m.Value).ToArray();

            foreach (var publicationString in publicationStrings)
            {
                var authorsRegex = new Regex(@"Aut.:.+?<BR>");
                var authors = authorsRegex.Match(publicationString).Value
                    .Replace("Aut.: </span>", "")
                    .Replace("<BR>", "")
                    .Split(',')
                    .Select(a => a.Trim())
                        .ToArray();

                if (!authors.Any(a => a.Contains(Input.AuthorSurname)))
                {
                    continue;
                }

                var formalTypeRegex = new Regex(@"Typ formalny publikacji: </span>.+?<BR>");
                var formalType = formalTypeRegex.Match(publicationString).Value
                   .Replace("Typ formalny publikacji: </span>", "")
                   .Replace("<BR>", "")
                   .Trim();

                if (!string.IsNullOrWhiteSpace(Input.FormalTypes) &&
                    !Input.FormalTypes.Split(',').Contains(formalType))
                {
                    continue;
                }

                var titleRegex = new Regex(@"Tytu.+?<BR>");
                var title = titleRegex.Match(publicationString).Value
                    .Replace("Tytuł: </span>", "")
                    .Replace("<BR>", "")
                    .Trim();

                var journalShortRegex = new Regex(@"Czasopismo: </span>.+?<BR>");
                var journalShort = journalShortRegex.Match(publicationString).Value
                    .Replace("Czasopismo: </span>", "")
                    .Replace("<BR>", "")
                    .Trim();

                var journalFullRegex = new Regex(@"czasop.: </span>.+?<BR>");
                var journalFull = journalFullRegex.Match(publicationString).Value
                   .Replace("Pełny tytuł czasop.: </span>", "")
                   .Replace("<BR>", "")
                   .Trim();

                Publications.Add(new Publication
                {
                    Author = Input.AuthorSurname,
                    Title = title,
                    JournalShort = journalShort,
                    JournalFull = journalFull,
                    FormalType = formalType
                });
            }

            return Page();
        }
    }
}
