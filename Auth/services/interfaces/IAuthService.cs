using Common.Models;

public interface IAuthService {
    public TransmissionModel Login(TransmissionModel requestModel);
}