using Booking.Models;

namespace Booking.Services.Interfaces
{
    public interface IBookingService
    {
        public TransmissionModel BookFlight(TransmissionModel model);

        public TransmissionModel SearchBooking(TransmissionModel model);
    }
}
