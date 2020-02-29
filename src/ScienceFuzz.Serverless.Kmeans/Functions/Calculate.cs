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
            var tsneUri = Environment.GetEnvironmentVariable("TSNE_URI");
            var tsneResultsJson = await _http.GetStringAsync(tsneUri);
            var tsneResults = JsonConvert.DeserializeObject<IEnumerable<TsneModel>>(tsneResultsJson);

            var points = new List<Point>();
            foreach (var tsneResult in tsneResults)
            {
                points.Add(new Point
                {
                    Scientist = tsneResult.Scientist,
                    Coordinates = new float[]
                    {
                        (float)tsneResult.Point.X,
                        (float)tsneResult.Point.Y
                    }
                });
            }

            var mlContext = new MLContext(seed: 1);
            var kmeansOptions = new KMeansTrainer.Options
            {
                OptimizationTolerance = 0.1f,
                FeatureColumnName = nameof(Point.Coordinates),
                MaximumNumberOfIterations = 100,
                NumberOfClusters = 2
            };

            var trainingData = mlContext.Data.LoadFromEnumerable(points);
            var dataProcessingPipeline = mlContext.Transforms.Categorical.OneHotEncoding(nameof(Point.PredictedLabel), nameof(Point.Coordinates));
            //.Append(mlContext.Transforms.ProjectToPrincipalComponents(nameof(Point.Score), nameof(Point.Contributions), rank: 2));
            var trainer = mlContext.Clustering.Trainers.KMeans(kmeansOptions);
            var trainingPipeline = dataProcessingPipeline.Append(trainer);
            var trainedModel = trainer.Fit(trainingData);
            var clusteredData = trainedModel.Transform(trainingData);
            var transformedPoints = mlContext.Data.CreateEnumerable<Point>(clusteredData, false).ToArray();

            var output = new List<KmeansModel>();
            foreach (var point in transformedPoints)
            {
                output.Add(new KmeansModel
                {
                    Scientist = point.Scientist,
                    Point = new PointModel
                    {
                        X = point.Coordinates[0],
                        Y = point.Coordinates[1]
                    },
                    ClusterNumber = (int)point.PredictedLabel
                });
            }

            return output;
        }
    }
}
