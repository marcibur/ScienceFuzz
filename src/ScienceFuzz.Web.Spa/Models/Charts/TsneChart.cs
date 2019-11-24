using ScienceFuzz.Models.Shared;
using ScienceFuzz.Web.Spa.Configuration;
using ScienceFuzz.Web.Spa.Models.Charts.Common;
using System.Collections.Generic;
using System.Linq;

namespace ScienceFuzz.Web.Spa.Models.Charts
{
    public class TsneChart
    {
        public TsneChart(IEnumerable<TsneModel> tsne)
        {
            Data = new BubbleData
            {
                Datasets = new BubbleDataSet[tsne.Count()]
            };

            var i = 0;

            tsne.ToList().ForEach(x =>
            {
                Data.Datasets[i] = new BubbleDataSet
                {
                    Label = x.Scientist,
                    BackgroundColor = COLORS.SCIENTISTS[x.Scientist],
                    BorderColor = COLORS.SCIENTISTS[x.Scientist],
                    BorderWidth = 1,
                    Data = new Bubble[]
                    {
                        new Bubble
                        {
                        X = x.Point.X,
                        Y = x.Point.Y,
                        R = 20
                        }
                    }
                };
                i++;
            });
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