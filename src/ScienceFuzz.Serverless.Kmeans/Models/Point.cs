using Microsoft.ML.Data;

namespace ScienceFuzz.Serverless.Kmeans.Models
{
    public class Point
    {
        public string Label { get; set; }

        [VectorType(8)]
        public float[] Contributions { get; set; }

        [VectorType(2)]
        public float[] Score { get; set; }

        public uint PredictedLabel { get; set; }
    }
}
