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

namespace be.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly MyMusicContext db;
        public RoleController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.Roles.FindAsync(id);
                if(data == null)
                {
                    return Ok(new
                    {
                        status = 400,
                        message = "The role doesn't exist in database"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data,
                    message = "Get role is success!"
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
                var data = await db.Roles.ToListAsync();
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
        public async Task<ActionResult> Add([FromBody] Role role)
        {
            await db.Roles.AddAsync(role);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Add role success!",
                status = 200,
                data = role
            });
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] Role role)
        {
            var element = await db.Roles.FindAsync(role.Id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The role doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.Roles.FirstOrDefaultAsync(x => x.Id == role.Id)).CurrentValues.SetValues(role);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit role success!",
                status = 200
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.Roles.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The role doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.Roles.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete role success!",
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
    }
}
