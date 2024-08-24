using Booking.Models;
using Booking.Services.Interfaces;
using Common.Controllers;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : BaseController
    {
        private readonly IBookingService bookingService;

        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpPost, Route("flights")]
        public Response<TransmissionModel> BookFlight(TransmissionModel requestModel)
        {
            return Response(bookingService.BookFlight(requestModel)).Success();
        }

        [HttpPost, Route("find")]
        public Response<TransmissionModel> FindBooking(TransmissionModel requestModel)
        {
            return Response(bookingService.SearchBooking(requestModel)).Success();
        }
    }
}
