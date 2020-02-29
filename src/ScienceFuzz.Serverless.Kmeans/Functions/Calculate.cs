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
            //var tsneResultsJson = await _http.GetStringAsync(tsneUri);
            //var tsneResults = JsonConvert.DeserializeObject<IEnumerable<TsneModel>>(tsneResultsJson);
            var tsneResults = JsonConvert.DeserializeObject<IEnumerable<TsneModel>>("[{\"scientist\":\"naukowiec_1_KIS\",\"point\":{\"x\":89.934448639533343,\"y\":-41.116955704331204}},{\"scientist\":\"naukowiec_2_KIS\",\"point\":{\"x\":-60.779810058456277,\"y\":-59.5275637349666}},{\"scientist\":\"naukowiec_3_KIS\",\"point\":{\"x\":64.924560104402133,\"y\":128.12514569595763}},{\"scientist\":\"naukowiec_4_KIS\",\"point\":{\"x\":-21.85342077368427,\"y\":-64.970885999044128}},{\"scientist\":\"naukowiec_5_KIS\",\"point\":{\"x\":45.906607040596931,\"y\":129.127599767193}},{\"scientist\":\"naukowiec_6_KIS\",\"point\":{\"x\":-90.163086252211926,\"y\":-32.74956295055204}},{\"scientist\":\"naukowiec_7_KIS\",\"point\":{\"x\":35.062732620044216,\"y\":15.106639920679973}},{\"scientist\":\"naukowiec_8_KIS\",\"point\":{\"x\":-68.7165715570873,\"y\":-28.838454358571845}},{\"scientist\":\"naukowiec_9_KIS\",\"point\":{\"x\":36.695259401618316,\"y\":158.94488309265202}},{\"scientist\":\"naukowiec_10_KIS\",\"point\":{\"x\":-52.405933985141061,\"y\":-4.6594470724159525}},{\"scientist\":\"naukowiec_11_ICNT\",\"point\":{\"x\":53.178445638477768,\"y\":9.5029852510440946}},{\"scientist\":\"naukowiec_12_ICNT\",\"point\":{\"x\":-2.2060861710327382,\"y\":-56.738805227176719}},{\"scientist\":\"naukowiec_13_ICNT\",\"point\":{\"x\":21.089389767409905,\"y\":192.45745549437834}},{\"scientist\":\"naukowiec_14_ICNT\",\"point\":{\"x\":91.87192517658174,\"y\":-61.669252834857296}},{\"scientist\":\"naukowiec_15_ICNT\",\"point\":{\"x\":-24.451524554218476,\"y\":-103.17444958812725}},{\"scientist\":\"naukowiec_16_ICNT\",\"point\":{\"x\":-39.234414438473877,\"y\":-121.33915711993704}},{\"scientist\":\"naukowiec_17_ICNT\",\"point\":{\"x\":-94.345278614685711,\"y\":-69.403701847291785}},{\"scientist\":\"naukowiec_18_ICNT\",\"point\":{\"x\":-70.297420453157358,\"y\":-81.176062738356578}},{\"scientist\":\"naukowiec_19_ICNT\",\"point\":{\"x\":7.2566147255552726,\"y\":178.50818183441953}},{\"scientist\":\"naukowiec_20_ICNT\",\"point\":{\"x\":114.7831259070756,\"y\":-66.111188808563071}}]");

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
