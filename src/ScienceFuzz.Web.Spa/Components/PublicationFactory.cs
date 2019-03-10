using ScienceFuzz.Models;
using ScienceFuzz.Models.Facade;
using ScienceFuzz.Web.Spa.Components.Abstract;

namespace ScienceFuzz.Web.Spa.Components
{
    public class PublicationFactory : IPublicationFactory
    {
        public Publication CreatePublication() =>
            new Publication(ScientificDomains.All);
    }
}
