using Booking.Models;

namespace Booking.Services.Interfaces
{
    public interface IBookingService
    {
        public BookingResponseModel BookFlight(BookingRequestModel model);

        public BookingResponseModel SearchBooking(BookingSearchModel model);
    }
}
