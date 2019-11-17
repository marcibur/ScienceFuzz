using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using ScienceFuzz.Models;
using ScienceFuzz.Serverless.Constants;
using ScienceFuzz.Serverless.State;
using System.Linq;

namespace ScienceFuzz.Serverless.Functions
{
    public class GetScientistDomains
    {
        private readonly AppState _appState;

        public GetScientistDomains(AppState appState)
        {
            _appState = appState;
        }

        [FunctionName(nameof(GetScientistDomains))]
        public IActionResult Execute(
            [HttpTrigger(AuthorizationLevel.Anonymous, HTTP.GET, Route = "scientists/contributions/domains")] HttpRequest request)
        {
            var scientists = _appState.Scientists.Select(x => new TsneModel
            {
                Scientist = x.Name,
                Contributions = x.DomainContributions.Select(y => y.Value).ToArray()
            });

            return new OkObjectResult(scientists);
        }
    }
}
