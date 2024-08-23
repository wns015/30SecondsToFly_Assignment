using Common.Controllers;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Models;
using PaymentGateway.Services.Interfaces;

namespace PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController: BaseController
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpPost("bookingCCPayment")]
        public Response<PaymentResponseModel> ProcessBookingCCPayment(BookingPaymentModel model)
        {
            return Response(paymentService.ProcessBookingCCPayment(model)).Success();
        }
    }
}
