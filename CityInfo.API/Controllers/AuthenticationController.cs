using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate
            (AuthenticationRequestBody authenticationRequestBody)
        {
            // validate creditntials
            var user = ValidateUser(authenticationRequestBody.UserName, authenticationRequestBody.Password);

            //Token
            var securityKey =
            new SymmetricSecurityKey(Convert
            .FromBase64String(_configuration["Authentication:SercretKey"]));

            // this will sign the token
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //payloads
            var claims = new List<Claim>
            {
                new Claim("sub", user.UserId.ToString()),
                new Claim("given_name", user.FirstName),
                new Claim("family_name", user.LastName),
                new Claim("city", user.City)
            };

            //all token
            var jwt = new JwtSecurityToken(
                _configuration["Authentication:ISSuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials
                );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUser(string? userName, string? password)
        {
            return new CityInfoUser(1, userName ?? "", "Bawaqna", "Antwerp");
        }

        public class CityInfoUser
        {
            public CityInfoUser(int userId, string firstName, string lastName, string city)
            {
                UserId = userId;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }

            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }
        }

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }
    }
}
