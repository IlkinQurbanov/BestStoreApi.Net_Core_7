using BestStoreApi.Net_Core_7.Models;
using BestStoreApi.Net_Core_7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BestStoreApi.Net_Core_7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly EmailSender emailSender;

        public AccountController(IConfiguration configuration, ApplicationDbContext context, EmailSender emailSender)
        {
            this.configuration = configuration;
            this.context = context;
            this.emailSender = emailSender;
        }


        [HttpPost("Register")]
        public IActionResult Register(UserDto userDto)
        {
            //check email address is user or not
            var emailCount = context.Users.Count(u => u.Email == userDto.Email);
            if(emailCount > 0)
            {
                ModelState.AddModelError("Email", "This Email Address is alredy use!");
                return BadRequest(ModelState);
            }

            //encrypt the password
            var passwordHasher = new PasswordHasher<User>();
            var encryptedPassword = passwordHasher.HashPassword(new Models.User(), userDto.Password);

            //create new account

            User user = new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Phone = userDto.Phone  ?? "",
                Address = userDto.Address,
                Password = encryptedPassword,
                Created = DateTime.Now

            };
            context.Users.Add(user);
            context.SaveChanges();
            var jwt = CreateJWToken(user);

            UserProfileDto userProfileDto = new UserProfileDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = DateTime.Now

            };

            var response = new
            {
                Token = jwt,
                User = userProfileDto
            };

            return Ok(response);
        }
        [HttpPost("Login")]

        public IActionResult Login( string email, string password)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("Error", "The Email or Password is not valid");
                return BadRequest(ModelState);
            }
            //verify the password

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(new Models.User(), user.Password, password);

            if(result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "Wrong Password");
                return BadRequest(ModelState);
            }

            var jwt = CreateJWToken(user);
            UserProfileDto userProfileDto = new UserProfileDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = DateTime.Now

            };

            var response = new
            {
                Token = jwt,
                User = userProfileDto
            };

            return Ok(response);

        }

        [Authorize]
        [HttpGet("GetTokenClaims")]
        public IActionResult GetTokenClaims()
        {
            var identity = User.Identity as ClaimsIdentity;

            if(identity != null)
            {
                Dictionary<string, string> claims = new Dictionary<string, string>();
                foreach(Claim claim in identity.Claims)
                {
                    claims.Add(claim.Type, claim.Value);

                }
                return Ok(claims);
            }
            return Ok();
        }


        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword(string email)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                return NotFound();
            }

            // delete an old password reset request
            var oldPwdReset = context.PasswordResets.FirstOrDefault(r => r.Email == email);
            if (oldPwdReset != null)
            {
                //delete old password reset request
                context.Remove(oldPwdReset);
            }
            //create password reset token
            string token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

            var pwdReset = new PasswordReset()
            {
                Email = email,
                Token = token,
                CreatedAt = DateTime.Now

            };
            context.PasswordResets.Add(pwdReset);
            context.SaveChanges();

            // Send the Password Rest Token by email to user

            string emailSubject = "Password Reset";
            string username = user.FirstName + " " + user.LastName;
            string emailMessage = "Dear " + username + "\n" +
                "We received your password reset request. \n" +
                "Please copy the following token and paste it in the Password Reset Form: \n" +
                token + "\n\n" +
                "Best Regards\n";

            emailSender.SendEmail(emailSubject, email, username, emailMessage).Wait();
            return Ok();
            

        }


        [HttpPost("ResetPassword")]

        public IActionResult ResetPassword(string token, string password)
        {
            var pwdReset = context.PasswordResets.FirstOrDefault(r =>r.Token == token);
            if (pwdReset == null)
            {
                ModelState.AddModelError("Token", "Wrong or Expired Token ");
                return BadRequest("ModelState");
            }

            var user = context.Users.FirstOrDefault(u => u.Email == pwdReset.Email);
            if(user == null)
            {
                ModelState.AddModelError("Token", "Wrong or Expired Token");
                return BadRequest(ModelState);
            }

            //encrypt password
            var passwordHasher = new PasswordHasher<User>();
            string encryptedPassword = passwordHasher.HashPassword(new User(), password);

            //save he encrypted password

            user.Password = encryptedPassword;

            //delete the token
            context.PasswordResets.Remove(pwdReset);

            context.SaveChanges();

            return Ok();





        }




        
        [Authorize]
        [HttpGet("AuthorizeAuthenticatedUsers")]
        public IActionResult AuthorizeAuthenticatedUsers()
        {
            return Ok("You are Authorized");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("AuthorizeAdmin")]
        public IActionResult AuthorizeAdmin()
        {
            return Ok("You are Authorized");
        }

        [Authorize(Roles = "admin, seller")]
        [HttpGet("AuthorizeAdminAndSeller")]
        public IActionResult AuthorizeAdminAndSeller()
        {
            return Ok("You are Authorized");
        }

        



        /*
            [HttpGet("TestToken")]

            public IActionResult TestToken()
            {
                User user = new User() { Id = 2, Role = "admin" };
                string jwt = CreateJWToken(user);
                var response = new { JWToken = jwt };


                return Ok(response);
            }

            */


        private string CreateJWToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
            new Claim("id", "" + user.Id),
            new Claim("role", user.Role)
            };

            string strKey = configuration["JwtSettings:Key"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(strKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken
                (
                issuer: configuration["Jwtsettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds

                
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }


       
    }
}
