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
    [Route("api/song")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly MyMusicContext db;
        public SongController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.Songs.FindAsync(id);
                if(data == null)
                {
                    return Ok(new
                    {
                        status = 400,
                        message = "The song doesn't exist in database"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data,
                    message = "Get song is success!"
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
                var data = from song in db.Songs
                           orderby song.CreateAt descending
                           select new
                           {
                               song.Id,
                               song.Title,
                               song.Singer,
                               song.IdGenre,
                               song.Path,
                               song.Image,
                               song.Lyrics,
                               song.Status,
                               nameGenre =  db.Genres.Where( x => x.Id == song.IdGenre).Select(x => x.Name).FirstOrDefault(),
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
        public async Task<ActionResult> Add([FromBody] Song song)
        {
            var element = await db.Songs.Where(x => x.Title.Equals(song.Title)).ToListAsync();
            if (element.Count != 0)
            {
                return Ok(new
                {
                    message = "Add song error!",
                    status = 400,
                });
            }
            await db.Songs.AddAsync(song);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Add song success!",
                status = 200,
                data = song
            });
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] Song song)
        {
            var element = await db.Songs.FindAsync(song.Id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The song doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.Songs.FirstOrDefaultAsync(x => x.Id == song.Id)).CurrentValues.SetValues(song);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit song success!",
                status = 200
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.Songs.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The song doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.Songs.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete song success!",
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
        [HttpGet("new")]

        public async Task<ActionResult> GetNewSong()
        {
            var data = await db.Songs.OrderByDescending(x => x.CreateAt).Take(10).ToListAsync();
            return Ok(new
            {
                status = 200,
                data
            });
        }
        [HttpGet("top100")]
        public async Task<ActionResult> GetTop100()
        {
            var data = await db.Songs.OrderByDescending(x =>x.Quantity).Take(100).ToListAsync();
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

    }
}
