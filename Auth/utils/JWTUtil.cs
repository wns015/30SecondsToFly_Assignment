using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.utils
{
    public class JWTUtil
    {
        public string Create(Claim data)
        {
            // To set payload
            var claims = new List<Claim>();
            claims.Add(data);
            var timeExp = 30;
            var payload = new JwtPayload(
                "30secondstofly",
                "API",
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(timeExp),
                DateTime.UtcNow
            );
            // To set signature
            var key = Guid.NewGuid().ToString("N");
            var secretKey = TextEncodings.Base64Url.Decode(key);
            var signingKey = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);
            // To set header
            var header = new JwtHeader(signingKey);
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(header, payload));
        }
    }
}