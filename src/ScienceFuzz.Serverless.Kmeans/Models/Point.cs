using Microsoft.ML.Data;

namespace ScienceFuzz.Serverless.Kmeans.Models
{
    public class Point
    {
        public string Label { get; set; }

        [VectorType(2)]
        public float[] Location { get; set; }
    }
}
