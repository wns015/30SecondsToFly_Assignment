using Common.Models;

namespace Auth.Services.Interfaces
{   
    public interface IAuthService
    {
        public TransmissionModel Login(TransmissionModel requestModel);

        public TransmissionModel Logout(TransmissionModel requestModel);
    }
}
