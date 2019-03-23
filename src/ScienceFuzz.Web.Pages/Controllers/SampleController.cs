using Microsoft.AspNetCore.Mvc;

namespace ScienceFuzz.Web.Pages.Controllers
{
    [ApiController]
    public class SampleController : ControllerBase
    {
        [HttpGet("/api/Samples")]
        public FileResult GetSample()
        {
            return File("sciencefuzz_sample.csv", "application/csv", "sciencefuzz_sample.csv");
            //return new VirtualFileResult("sciencefuzz_sample.csv", "text/csv");
            //return new PhysicalFileResult("sciencefuzz_sample.csv", "text/csv");
        }
    }
}
