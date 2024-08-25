using Auth.utils;
using Common.Exceptions;
using Common.Logs;
using Common.Repositories;
using Common.Security;
using Newtonsoft.Json;
using Auth.Services.Interfaces;
using Common.Contexts.Models;

namespace Auth.Services
{
    public class AuthService : IAuthService
    {

        private readonly IRepository<UserTableModel> userRepo;
        private readonly IRepository<UserSessionTableModel> sessionRepo;
        private readonly JWTUtil jwtUtil = new JWTUtil();
        private readonly string serviceName = "Auth Service";

        public AuthService(IRepository<UserTableModel> userRepo, IRepository<UserSessionTableModel> sessionRepo)
        {
            this.userRepo = userRepo;
            this.sessionRepo = sessionRepo;
        }

        public TransmissionModel Login(TransmissionModel requestModel)
        {
            try
            {
                GlobalLoggingHandler.Logging.Info($"{serviceName}[Login] Start method");
                if (requestModel == null)
                {
                    GlobalLoggingHandler.Logging.Warn($"{serviceName}[Login] Model is null");
                    return null;
                }

                var objectString = Decryptor.DecryptText(requestModel.EncryptedString);
                LoginModel model = JsonConvert.DeserializeObject<LoginModel>(objectString);

                var user = userRepo.Find(p => p.Username == model.Username || p.Email == model.Username);

                if (user == null)
                {
                    GlobalLoggingHandler.Logging.Warn($"{serviceName}[Login] No user found");
                    return null;
                }

                var check = PasswordUtil.ArgonHashStringVerify(user.Password, model.Password, user.Salt);

                if (!check)
                {
                    GlobalLoggingHandler.Logging.Warn($"{serviceName}[Login] Incorrect password");
                    return null;
                }



                UserSessionTableModel session = new UserSessionTableModel()
                {
                    Username = user.Username,
                    AccessToken = jwtUtil.Create(new System.Security.Claims.Claim("username", user.Username)),
                    TokenIssued = DateTime.Now,
                    TokenExpiration = DateTime.Now.AddMinutes(30),
                    IsActive = true,
                };

                sessionRepo.Add(session);

                SessionModel result = new SessionModel()
                {
                    AccessToken = session.AccessToken,
                    Username = session.Username
                };

                var jsonString = JsonConvert.SerializeObject(result);

                TransmissionModel response = new TransmissionModel()
                {
                    EncryptedString = Encryptor.EncryptText("jsonString")
                };

                GlobalLoggingHandler.Logging.Info($"{serviceName}[Login] End method");
                return response;
            }
            catch
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[Login] Error occurred");
                throw new GeneralException();
            }
        }

        public TransmissionModel Logout(TransmissionModel requestModel)
        {
            try
            {
                GlobalLoggingHandler.Logging.Info($"{serviceName}[Logout] Start method");
                if (requestModel == null)
                {
                    GlobalLoggingHandler.Logging.Warn($"{serviceName}[Logout] Model is null");
                    throw new InvalidParameterException();
                }

                var objectString = Decryptor.DecryptText(requestModel.EncryptedString);
                SessionModel model = JsonConvert.DeserializeObject<SessionModel>(objectString);

                var sessions = sessionRepo.FindAll(p => p.Username == model.Username && p.IsActive).ToList();

                foreach (var session in sessions)
                {
                    session.IsActive = false;
                    sessionRepo.Update(session);
                }

                sessionRepo.SaveChanges();

                ActionCompleteResponseModel result = new ActionCompleteResponseModel()
                {
                    Success = true,
                };

                var jsonString = JsonConvert.SerializeObject(result);

                TransmissionModel response = new TransmissionModel()
                {
                    EncryptedString = Encryptor.EncryptText(jsonString)
                };

                GlobalLoggingHandler.Logging.Info($"{serviceName}[Logout] End method");
                return response;
            }
            catch
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[Logout] Error occurred");
                throw new GeneralException();
            }
        }
    }
}