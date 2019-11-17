using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using ScienceFuzz.Serverless.Constants;
using ScienceFuzz.Serverless.State;
using System.Linq;

namespace ScienceFuzz.Serverless.Functions
{
    public class GetScientist
    {
        private readonly AppState _appState;

        public GetScientist(AppState appState)
        {
            _appState = appState;
        }

        [FunctionName(nameof(GetScientist))]
        public IActionResult Execute(
            [HttpTrigger(AuthorizationLevel.Anonymous, HTTP.GET, Route = "scientists/{scientistName}")] HttpRequest request,
            string scientistName)
        {
            var scientist = _appState.Scientists.FirstOrDefault(x => x.Name == scientistName);
            if (scientist == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(scientist);
        }
    }
}
