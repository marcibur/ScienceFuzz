using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ScienceFuzz.Models.Shared;
using ScienceFuzz.Serverless.Kmeans.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Kmeans.Functions
{
    // TODO: Refactor
    // TODO: 404
    public static class Calculate
    {
        public static HttpClient _http = new HttpClient();

        [FunctionName(nameof(Calculate))]
        public static async Task<IEnumerable<KmeansModel>> Execute(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/Kmeans")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.SCIENTISTS, Connection = ENV.STORAGE_CONNECTION)] CloudTable scientistsTable)
        {
            var query = new TableQuery<Data.Scientist>();
            var queryResult = await scientistsTable.ExecuteQuerySegmentedAsync(query, null);
            var scientists = queryResult.ToArray();
            var scientistsNames = scientists.Select(x => x.RowKey).ToArray();

            IEnumerable<TsneModel> tsne;
            var tsneUri = Environment.GetEnvironmentVariable("TSNE_URI");
            var tsneString = await _http.GetStringAsync(tsneUri);
            tsne = JsonConvert.DeserializeObject<IEnumerable<TsneModel>>(tsneString);

            var points = new List<Point>();
            foreach (var scientist in scientists)
            {
                var location = tsne.First(x => x.Scientist == scientist.RowKey).Point;
                points.Add(new Point
                {
                    Label = scientist.PartitionKey,
                    Location = new float[] { (float)location.X, (float)location.Y }
                });
            }



            var mlContext = new MLContext(seed: 1);
            var kmeansOptions = new KMeansTrainer.Options
            {
                InitializationAlgorithm = KMeansTrainer.InitializationAlgorithm.KMeansPlusPlus,
                FeatureColumnName = nameof(Point.Location),
                OptimizationTolerance = 0.000000000001f,
                MaximumNumberOfIterations = int.MaxValue,
                NumberOfClusters = 2
            };

            var trainingData = mlContext.Data.LoadFromEnumerable(points);
            var dataProcessingPipeline = mlContext.Transforms.Categorical.OneHotEncoding(nameof(Point.Location), nameof(Point.Label))
                .Append(mlContext.Transforms.ProjectToPrincipalComponents(nameof(Point.Location), rank: 2));
            var trainer = mlContext.Clustering.Trainers.KMeans(kmeansOptions);
            var trainingPipeline = dataProcessingPipeline.Append(trainer);
            var trainedModel = trainer.Fit(trainingData);
            var clusteredData = trainedModel.Transform(trainingData);
            var transformedPoints = mlContext.Data.CreateEnumerable<Point>(clusteredData, false).ToArray();



            var output = new List<KmeansModel>();
            foreach (var point in transformedPoints)
            {
                var unit = output.FirstOrDefault(x => x.Unit == point.Label);
                if (unit == null)
                {
                    unit = new KmeansModel
                    {
                        Unit = point.Label,
                        Points = new List<PointModel>()
                    };
                    output.Add(unit);
                }

                unit.Points.Add(new PointModel
                {
                    X = point.Location[0],
                    Y = point.Location[1]
                });
            }

            return output;
        }
    }
}
