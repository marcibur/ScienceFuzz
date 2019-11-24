using System.Collections.Generic;

namespace ScienceFuzz.Models.Shared
{
    public class KmeansModel
    {
        public string Unit { get; set; }
        public List<PointModel> Points { get; set; }
    }
}
