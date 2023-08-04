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
    [Route("api/song/favorite")]
    [ApiController]
    public class SongFavoriteController : ControllerBase
    {
        private readonly MyMusicContext db;
        public SongFavoriteController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.SongFavourites.FindAsync(id);
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
                var data = await db.SongFavourites.ToListAsync();
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
        public async Task<ActionResult> Add([FromBody] SongFavourite songRated)
        {
            await db.SongFavourites.AddAsync(songRated);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Add song favorite success!",
                status = 200,
                data = songRated
            });
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] SongFavourite songRated)
        {
            var element = await db.SongFavourites.FindAsync(songRated.Id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The song favorite doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.SongFavourites.FirstOrDefaultAsync(x => x.Id == songRated.Id)).CurrentValues.SetValues(songRated);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit song favorite success!",
                status = 200,
                data = songRated
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.SongFavourites.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The song favorite doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.SongFavourites.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete song favorite success!",
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
        public async Task<ActionResult> GetSongFavorite(Guid idUser)
        {
            var user = await db.Users.FindAsync(idUser);
            if (user == null)
            {
                return Ok(new
                {
                    status = 400,
                });
            }
            var data = await db.SongFavourites.Where(x => x.IdUser == idUser).Where(x => x.Status == true).ToListAsync();
            if(data.Count >= 0)
            {
                List<Song> listSong = new List<Song>();
                foreach (var items in data)
                {
                    var item = await db.Songs.FindAsync(items.IdSong);
                    if (item != null)
                    {
                        listSong.Add(item);
                    }
                }
                return Ok(new
                {
                    status = 200,
                    data = listSong
                });
            }
            return BadRequest();
            
        }
        [HttpGet("check")]
        public async Task<ActionResult> CheckSongFavorite(Guid idUser, Guid idSong)
        {
            try
            {
                var data = await db.SongFavourites.Where(x => x.IdUser == idUser).Where(x => x.IdSong == idSong).FirstOrDefaultAsync();
                if (data == null)
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
