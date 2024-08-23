using Booking.Models;
using Booking.Services.Interfaces;
using Common.Controllers;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : BaseController
    {
        private readonly ISearchService searchService;

        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }

        [HttpPost("flights")]
        public Response<SearchResultModel> SearchFlights(SearchModel searchModel)
        {
            return Response(searchService.SearchFlights(searchModel)).Success();
        }
    }
}
