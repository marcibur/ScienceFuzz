using ScienceFuzz.Models.Shared;
using ScienceFuzz.Web.Spa.Configuration;
using ScienceFuzz.Web.Spa.Models.Charts.Common;
using System.Collections.Generic;
using System.Linq;

namespace ScienceFuzz.Web.Spa.Models.Charts
{
    public class KmeansChart
    {
        public KmeansChart(IEnumerable<KmeansModel> kmeans)
        {
            var kmeansArray = kmeans.ToArray();

            Data = new BubbleData
            {
                Datasets = new BubbleDataSet[kmeans.Count()]
            };

            for (int i = 0; i < Data.Datasets.Length; i++)
            {
                Data.Datasets[i] = new BubbleDataSet
                {
                    Label = kmeansArray[i].Unit,
                    BackgroundColor = COLORS.UNITS[kmeansArray[i].Unit],
                    BorderColor = COLORS.UNITS[kmeansArray[i].Unit],
                    BorderWidth = 1,
                    Data = new Bubble[kmeansArray[i].Points.Count]
                };

                for (int j = 0; j < kmeansArray[i].Points.Count; j++)
                {
                    Data.Datasets[i].Data[j] = new Bubble
                    {
                        X = kmeansArray[i].Points[j].X,
                        Y = kmeansArray[i].Points[j].Y,
                        R = 10
                    };
                }
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
                DisplayColors = false
            }
        };
    }
}
