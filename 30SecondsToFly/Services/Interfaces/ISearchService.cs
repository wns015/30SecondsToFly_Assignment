using Booking.Models;

namespace Booking.Services.Interfaces
{
    public interface ISearchService
    {
        public SearchResultModel SearchFlights(SearchModel model);
    }
}
