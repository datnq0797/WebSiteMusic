using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using System.Net;
using be.Helpers;
using be.Models;
using be.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace be.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyMusicContext db;
        public UserController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.Users.FindAsync(id);
                if(data == null)
                {
                    return Ok(new
                    {
                        status = 400,
                        message = "The user doesn't exist in database"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data,
                    message = "Get user is success!"
                });
            }
            catch
            {
                return BadRequest();
            }            
        }
        [HttpGet("all")]
        public async Task<ActionResult> GetElements()
        {
            try
            {
                var data = from user in db.Users
                           join role in db.Roles on user.IdRole equals role.Id orderby user.CreateAt descending
                           select new
                           {
                               user.Id,
                               user.Name,
                               user.IdRole,
                               user.Status,
                               user.Email,
                               user.Birthday,
                               user.Phone,
                               user.Gender,
                               user.Image,
                               user.Password,
                               user.CreateAt,
                               roleName = role.Name,
                           };
                return Ok(new
                {
                    status = 200,
                    data
                });
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("add")]
        public async Task<ActionResult> Add([FromBody] User user)
        {
            try
            {
                if(await db.Users.Where(x => x.Email.Equals(user.Email)).FirstOrDefaultAsync() != null)
                {
                    return Ok(new
                    {
                        message = "Email is already register!",
                        status = 400,
                    });
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Add user success!",
                    status = 200,
                    data = user
                });
            }
            catch
            {
                return Ok(new
                {
                    message = "Add user error!",
                    status = 400,
                });
            }
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] User user)
        {
            var element = await db.Users.FindAsync(user.Id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The user doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.Users.FirstOrDefaultAsync(x => x.Id == user.Id)).CurrentValues.SetValues(user);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit user success!",
                status = 200,
                data = user
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.Users.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The user doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.Users.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete user success!",
                    status = 200
                });
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    message = "Error system!",
                    status = 400,
                    data = e.Message
                });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchEmail(string email)
        {
            var user = await db.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            if(user == null)
            {
                return Ok(new
                {
                    status = 404
                }
                );
            } return Ok(new {status = 200});
        }

        [HttpGet("info")]
        public async Task<ActionResult> GetInfo(string token)
        {
            try
            {
                if(token == "")
                {
                    return BadRequest();
                }
                string _token = token.Split(' ')[1];
                if (_token == null)
                {
                    return Ok(new
                    {
                        message = "Token is wrong!",
                        status = 400
                    });
                }
                var handle = new JwtSecurityTokenHandler();
                string email = handle.ReadJwtToken(_token).Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                var user = db.Users.Where(x => x.Email == email).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new
                    {
                        message = "User is not found!",
                        status = 404
                    });
                }
                var role = await db.Roles.FindAsync(user.IdRole);
                return Ok(new
                {
                    message = "Get information success!",
                    status = 200,
                    data = user,
                    role
                });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("changepass")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            try
            {
                var user = await db.Users.FindAsync(changePassword.idUser);
                if (user == null)
                {
                    return Ok(new
                    {
                        message = "The user is not found!",
                        status = 200
                    });
                }
                if (!BCrypt.Net.BCrypt.Verify(changePassword.oldPassword, user.Password))
                {
                    return Ok(new
                    {
                        message = "The password is wrong!",
                        status = 400
                    });
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.newPassword);
                db.Entry(db.Users.FirstOrDefault(x => x.Id == user.Id)).CurrentValues.SetValues(user);
                db.SaveChanges();
                return Ok(new
                {
                    message = "Change password is success!",
                    status = 200
                });
            }catch { return BadRequest(); }
        }
    }
}
