using ScienceFuzz.Models.Shared;
using ScienceFuzz.Web.Spa.Models.Charts.Common;
using System.Collections.Generic;
using System.Linq;

namespace ScienceFuzz.Web.Spa.Models.Charts
{
    public class DisciplinesChart
    {
        public DisciplinesChart(IEnumerable<ContributionModel> contributions)
        {
            Data = new Data
            {
                Labels = contributions.Select(x => x.Name).ToArray(),
                Datasets = new Dataset[]
                {
                    new Dataset
                    {
                        Data = contributions.Select(x => x.Value).ToArray(),
                        BackgroundColor = contributions.Select(x => "rgba(13, 162, 0, .5)").ToArray(),
                        BorderColor = contributions.Select(x => "rgba(13, 162, 0, .5)").ToArray(),
                        BorderWidth = 1
                    }
                }
            };
        }

        public string Type { get; } = "horizontalBar";

        public Data Data { get; private set; }

        public Options Options { get; } = new Options
        {
            Legend = new Legend
            {
                Display = false
            },

            Tooltips = new Tooltips
            {
                Enabled = true,
                DisplayColors = true
            }
        };
    }
}
