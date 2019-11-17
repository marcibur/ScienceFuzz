using ScienceFuzz.Models;
using System.Collections.Generic;

namespace ScienceFuzz.Serverless.State
{
    public class AppState
    {
        public string[] Domains { get; set; }
        public string[] Disciplines { get; set; }
        public IEnumerable<Scientist> Scientists { get; set; }
    }
}
