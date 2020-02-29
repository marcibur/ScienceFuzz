using Microsoft.ML.Data;

namespace ScienceFuzz.Serverless.Kmeans.Models
{
    public class Point
    {
        //public string Label { get; set; }

        public string Scientist { get; set; }

        [VectorType(2)]
        public float[] Coordinates { get; set; }

        //[VectorType(2)]
        //public float[] Score { get; set; }

        public uint PredictedLabel { get; set; }
    }
}
