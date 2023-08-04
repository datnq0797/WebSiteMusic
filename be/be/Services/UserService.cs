using be.Helpers;
using be.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace be.Services
{
    public class UserService
    {
        private readonly MyMusicContext _context;
        public UserService()
        {
            _context = new MyMusicContext();
        }

        public string CreateToken(string email, Guid id, IConfiguration config)
        {
            string role = _context.Roles.Find(id).Name;
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                config.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return "bearer " + jwt;
        }
        public object Login(string email, string password, IConfiguration config)
        {
            string token = "";
            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                return new
                {
                    status = 404,
                    message = "The account is not found"
                };
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return new
                {
                    message = "Password is wrong",
                    status = 400
                };
            }
            if (user.Status == false)
            {
                return new
                {
                    message = "Please check your email to confirm your account",
                    status = 400
                };
            }
            token = CreateToken(user.Email, (Guid)user.IdRole, config);
            var role = _context.Roles.Find(user.IdRole);
            return new
            {
                message = "Login success",
                status = 200,
                data = user,
                role,
                token
            };
        }
        public async Task<object> Register(User user)
        {
            if ((_context.Users?.Any(x => x.Email == user.Email)).GetValueOrDefault())
            {
                return new
                {
                    message = "Email is found",
                    status = 400
                };
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            if (user.IdRole == null)
            {
                var role = await _context.Roles.Where(x => x.Name == "Guest").FirstOrDefaultAsync();
                if (role != null)
                {
                    user.IdRole = role.Id;
                }
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var id = (user.Id).ToString();
            if(user.Status != true)
            {
                EmailService.Instance.SendMail(user.Email, 2, id);
            }
            return new
            {
                message = "Add account success!",
                status = 200,
            };
        }
        public async Task<object> ForgotPassword(string email)
        {
            var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return new
                {
                    status = 404,
                    message = "Email is not found"
                };
            }
            if (user.Status == false)
            {
                return new
                {
                    status = 400,
                    message = "The account is unverified, please check email is verify."
                };
            }
            string _pass = RandomString.Instance.CreateString();
            string pass = BCrypt.Net.BCrypt.HashPassword(_pass);
            user.Password = pass;
            await _context.SaveChangesAsync();
            var isCheck = await EmailService.Instance.SendMail(user.Email, 1, _pass);
            if (isCheck == false)
            {
                return new
                {
                    message = "Send a new password error!",
                    status = 400,
                };
            }
            return new
            {
                message = "Send a new password success!",
                status = 200,
            };

        }
    }
    public class ChangePassword
    {
        public Guid idUser { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
