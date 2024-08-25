

using Common.Exceptions;
using Common.Logs;
using Common.Repositories;
using Common.Security;

public class AuthService : IAuthService {
    
    private readonly IRepository<UserTableModel> userRepo;
    private readonly IRepository<UserSessionTableModel> sessionRepo;
    private readonly string serviceName = "Auth Service";

    public AuthService (IRepository<UserTableModel> userRepo, IRepository<UserSessionTableModel> sessionRepo) {
        this.userRepo = userRepo;
        this.sessionRepo = sessionRepo;
    }

    public TransmissionModel Login(TransmissionModel requestModel) {
        try{
            GlobalLoggingHandler.Logging.Info($"{serviceName}[Login] Start method");
            var encryptedObjectString = Decryptor.DecryptText(requestModel.EncryptedString);


            TransmissionModel response = new TransmissionModel() {
                EncryptedString = Encryptor.EncryptText("sample")
            };

            GlobalLoggingHandler.Logging.Info($"{serviceName}[Login] End method");
            return response;
        }
        catch (Exception ex) {
            throw new GeneralException();
        }
        

    }
}