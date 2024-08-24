using Booking.Models;

namespace Booking.Services.Interfaces
{
    public interface ISearchService
    {
        public TransmissionModel SearchFlights(TransmissionModel model);
    }
}
