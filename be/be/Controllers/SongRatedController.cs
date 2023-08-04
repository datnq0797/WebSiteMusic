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
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace be.Controllers
{
    [Route("api/songrated")]
    [ApiController]
    public class SongRatedController : ControllerBase
    {
        private readonly MyMusicContext db;
        public SongRatedController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.SongRateds.FindAsync(id);
                if(data == null)
                {
                    return Ok(new
                    {
                        status = 400,
                        message = "The song rated doesn't exist in database"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data,
                    message = "Get song rated is success!"
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
                var data = await db.SongRateds.ToListAsync();
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
        public async Task<ActionResult> Add([FromBody] SongRated songRated)
        {
            await db.SongRateds.AddAsync(songRated);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Add song rated success!",
                status = 200,
                data = songRated
            });
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] SongRated songRated)
        {
            var element = await db.SongRateds.FindAsync(songRated.Id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The song rated doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.SongRateds.FirstOrDefaultAsync(x => x.Id == songRated.Id)).CurrentValues.SetValues(songRated);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit song rated success!",
                status = 200
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.SongRateds.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The song rated doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.SongRateds.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete song rated success!",
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
        [HttpGet("user")]
        public async Task<ActionResult> GetSongRated(Guid idUser)
        {
            var user = await db.Users.FindAsync(idUser);
            if (user == null)
            {
                return Ok(new
                {
                    status = 400,
                });
            }
            var data = await db.SongRateds.Where(x => x.IdUser == idUser).ToListAsync();
            return Ok(new
            {
                status = 200,
                data
            });
        }
        [HttpGet("check")]
        public async Task<ActionResult> CheckSongRated(Guid idUser, Guid idSong)
        {
            try
            {
                var data = await db.SongRateds.Where(x => x.IdUser == idUser).Where(x => x.IdSong == idSong).FirstOrDefaultAsync();
                if(data == null)
                {
                    return Ok(new
                    {
                        status = 400
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data
                });
            }
            catch { return BadRequest(); }
        }
    }
}
