using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ScienceFuzz.Data;
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
            var query = new TableQuery<Scientist>();
            var queryResult = await scientistsTable.ExecuteQuerySegmentedAsync(query, null);
            var scientists = queryResult.Results.ToList();

            IEnumerable<ContributionsModel> domainContributions;
            var domainsUri = Environment.GetEnvironmentVariable("DOMAINS_URI");
            var domainsString = await _http.GetStringAsync(domainsUri);
            domainContributions = JsonConvert.DeserializeObject<IEnumerable<ContributionsModel>>(domainsString);

            var points = new List<Point>();
            foreach (var scientist in scientists)
            {
                var contributions = domainContributions.Where(x => x.Scientist == scientist.RowKey);
                foreach (var contribution in contributions)
                {
                    points.Add(new Point
                    {
                        Label = scientist.PartitionKey,
                        Contributions = contribution.Contributions.Select(x => (float)x.Value).ToArray()
                        //Result = new float[2]
                    });
                }
            }



            var mlContext = new MLContext(seed: 1);
            var kmeansOptions = new KMeansTrainer.Options
            {
                OptimizationTolerance = 0.1f,
                FeatureColumnName = nameof(Point.Contributions),
                MaximumNumberOfIterations = 100,
                NumberOfClusters = 2
            };

            var trainingData = mlContext.Data.LoadFromEnumerable(points);
            var dataProcessingPipeline = mlContext.Transforms.Categorical.OneHotEncoding(nameof(Point.PredictedLabel), nameof(Point.Label))
                .Append(mlContext.Transforms.ProjectToPrincipalComponents(nameof(Point.Score), nameof(Point.Contributions), rank: 2));
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
                    X = point.Score[0],
                    Y = point.Score[1]
                });
            }

            return output;
        }
    }
}
