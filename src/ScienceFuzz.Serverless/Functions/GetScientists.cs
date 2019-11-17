using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using ScienceFuzz.Serverless.Constants;
using ScienceFuzz.Serverless.State;
using System.Linq;

namespace ScienceFuzz.Serverless.Functions
{
    public class GetScientists
    {
        private readonly AppState _appState;

        public GetScientists(AppState appState)
        {
            _appState = appState;
        }

        [FunctionName(nameof(GetScientists))]
        public IActionResult Execute(
            [HttpTrigger(AuthorizationLevel.Anonymous, HTTP.GET, Route = "scientists")] HttpRequest request)
        {
            var scientistsNames = _appState.Scientists.Select(x => x.Name).ToArray();
            return new OkObjectResult(scientistsNames);
        }
    }

    //public static class Func
    //{
    //    [FunctionName("Temp")]
    //    public static string Execute(
    //            [HttpTrigger(AuthorizationLevel.Anonymous, HTTP.GET, Route = "scientists")] HttpRequest request)
    //    {
    //        return "OK"
    //    }
    //}
}
