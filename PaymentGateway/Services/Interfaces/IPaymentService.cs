using PaymentGateway.Models;

namespace PaymentGateway.Services.Interfaces
{
    public interface IPaymentService
    {
        public PaymentResponseModel ProcessBookingCCPayment(BookingPaymentModel model);
    }
}
