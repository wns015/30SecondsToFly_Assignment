using Booking.Models;
using Booking.Services.Interfaces;
using Common.Controllers;
using Common.Models;
using System.Web.Http;

namespace Booking.Controllers
{
    [RoutePrefix("api/search")]
    public class SearchController : BaseController
    {
        private readonly ISearchService searchService;

        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }

        [HttpPost]
        [Route("flights")]
        public Response<SearchResultModel> SearchFlights(SearchModel searchModel)
        {
            return Response(searchService.Search(searchModel)).Success();
        }
    }
}
