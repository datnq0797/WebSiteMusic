using be.Models;
using be.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace be.Controllers
{
    [Route("api")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        private readonly MyMusicContext _db;
        private readonly IConfiguration _config;
        private UserService userService = new UserService();
        public ActionController(MyMusicContext db, IConfiguration cf)
        {
            _db = db;
            _config = cf;
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody] Login login)
        {
            try
            {
                var result = userService.Login(login.email, login.password, _config);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] User user)
        {
            try
            {
                var result = await userService.Register(user);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("confirm")]
        public async Task<ActionResult> ConfirmAccount(Guid id)
        {
            try
            {
                var _user = await _db.Users.FindAsync(id);
                if (_user == null)
                {
                    return NotFound();
                }
                _user.Status = true;
                _db.Entry(await _db.Users.FirstOrDefaultAsync(x => x.Id == id)).CurrentValues.SetValues(_user);
                await _db.SaveChangesAsync();
                return Ok(new
                {
                    status = 200,
                    message = "Confirm success"
                });
            }
            catch
            {
                return BadRequest();
            }
        }

    }
    public class Login
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
