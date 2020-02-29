using ScienceFuzz.Models.Shared;
using ScienceFuzz.Web.Spa.Configuration;
using ScienceFuzz.Web.Spa.Models.Charts.Common;
using System.Collections.Generic;

namespace ScienceFuzz.Web.Spa.Models.Charts
{
    public class KmeansChart
    {
        public KmeansChart(List<KmeansModel> kmeans)
        {
            var kmeansArray = kmeans.ToArray();

            Data = new BubbleData
            {
                Datasets = new BubbleDataSet[kmeansArray.Length]
            };

            for (int i = 0; i < Data.Datasets.Length; i++)
            {
                Data.Datasets[i] = new BubbleDataSet
                {
                    Label = kmeansArray[i].Scientist,
                    BackgroundColor = COLORS.CLUSTERS[kmeansArray[i].ClusterNumber],
                    BorderColor = COLORS.CLUSTERS[kmeansArray[i].ClusterNumber],
                    BorderWidth = 1,
                    Data = new Bubble[1]
                };

                Data.Datasets[i].Data[0] = new Bubble
                {
                    X = kmeansArray[i].Point.X,
                    Y = kmeansArray[i].Point.Y,
                    R = 10
                };
            }
        }

        public string Type { get; } = "bubble";

        public BubbleData Data { get; private set; }

        public Options Options { get; } = new Options
        {
            Legend = new Legend
            {
                Display = true
            },

            Tooltips = new Tooltips
            {
                Enabled = true,
                DisplayColors = true
            }
        };
    }
}
