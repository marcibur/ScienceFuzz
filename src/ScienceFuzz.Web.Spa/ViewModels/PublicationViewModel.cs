using System.Collections.Generic;

namespace ScienceFuzz.Web.Spa.ViewModels
{
    public class PublicationViewModel
    {
        public string Description { get; set; }
        public List<DomainViewModel> Domains { get; set; }
    }
}
