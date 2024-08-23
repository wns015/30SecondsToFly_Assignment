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
        public Response<BookingResponseModel> BookFlight(BookingRequestModel model)
        {
            return Response(bookingService.BookFlight(model)).Success();
        }

        [HttpPost, Route("find")]
        public Response<BookingResponseModel> FindBooking(BookingSearchModel model)
        {
            return Response(bookingService.SearchBooking(model)).Success();
        }
    }
}
