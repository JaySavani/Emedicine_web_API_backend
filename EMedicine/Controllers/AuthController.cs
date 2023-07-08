using EMedicine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EMedicineContext _context;

        public AuthController(EMedicineContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] Loginuser model)
        {
            // Validate user input
           /* if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }*/

            // Look up user by email address
            var Loginuser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (Loginuser == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Verify user's password
            if (!VerifyPassword(model.Password, Loginuser.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Create and return a JWT token for the user
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("JDStOS5uphK3vmCJQrexSJ1RsyjZBjXWRgJMFPU4");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, Loginuser.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                User = Loginuser.Type,
                Name = Loginuser.FirstName,
                userid=Loginuser.Id,
                /*Expires = tokenDescriptor.Expires*/
            });
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] Users model)
        {
            // Validate user input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email address is already in use
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return Conflict("Email address already in use.");
            }

            // Hash user's password
            var hashedPassword = HashPassword(model.Password);

            // Create new user
            var users = new Users
            {
                FirstName= model.FirstName,
                LastName= model.LastName,
                Email = model.Email,
                Password = hashedPassword,
                Type= "user",
            };
            _context.Users.Add(users);
            _context.SaveChanges();

            // Create and return a JWT token for the new user
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("JDStOS5uphK3vmCJQrexSJ1RsyjZBjXWRgJMFPU4");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, users.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                Type= "user",
                Name = users.FirstName,
                id=users.Id,

                /* Expires = tokenDescriptor.Expires*/
            });
        }


        private string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[24]);

            // Hash the password using PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(24);

            // Combine the salt and hash into a single string for storage
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 24);
            Array.Copy(hash, 0, hashBytes, 24, 24);
            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Extract the salt and hash from the stored password
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[24];
            Array.Copy(hashBytes, 0, salt, 0, 24);

            // Hash the user-supplied password using the same salt and compare the hashes
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(24);
            for (int i = 0; i < 24; i++)
            {
                if (hashBytes[i + 24] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }



    }
}
