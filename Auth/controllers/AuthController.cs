using Auth.Services.Interfaces;
using Common.Controllers;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost, Route("login")]
        public Response<TransmissionModel> Login(TransmissionModel requestModel)
        {
            return Response(authService.Login(requestModel));
        }

        [HttpPost, Route("logout")]
        public Response<TransmissionModel> Logout(TransmissionModel requestModel)
        {
            return Response(authService.Logout(requestModel)).Success();
        }
    }
}