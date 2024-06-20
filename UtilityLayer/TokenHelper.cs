using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLayer
{
    public static class TokenHelper
    {
        //private static string GenerateJWT(string email, int userId)
        //{

        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new Claim[]
        //    {
        //        new Claim("Email",email),
        //        new Claim("UserId",userId.ToString())
        //    };

        //    var token = new JwtSecurityToken(_config["Jwt:Issuer"],
        //        _config["Jwt:Audience"],
        //        claims,
        //        //expires: DateTime.Now.AddMinutes(15),
        //        expires: DateTime.Now.AddMonths(1),
        //        signingCredentials: credentials);


        //    return new JwtSecurityTokenHandler().WriteToken(token);

        //}
    }
}
