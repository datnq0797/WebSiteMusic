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
    [Route("api/genre")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly MyMusicContext db;
        public GenreController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.Genres.FindAsync(id);
                if(data == null)
                {
                    return Ok(new
                    {
                        status = 400,
                        message = "The genre doesn't exist in database"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data,
                    message = "Get genre is success!"
                });
            }
            catch
            {
                return BadRequest();
            }            
        }

        [HttpGet("song")]
        public async Task<ActionResult> GetSongByGenre()
        {
            try
            {
                var data = await db.Genres.ToListAsync();
                if (data.Count > 0)
                {
                    var listData = new List<Data>();
                    foreach (var item in data)
                    {
                        var listSong = await db.Songs.Where(x => x.IdGenre == item.Id).OrderByDescending(x => x.Quantity).Take(4).ToListAsync();
                        var _data = new Data();
                        _data.Name = item.Name;
                        _data.Songs = listSong;
                        listData.Add(_data);
                    }
                    return Ok(new
                    {
                        status = 200,
                        data = listData,
                    });
                }
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

        [HttpGet("all")]
        public async Task<ActionResult> GetElements()
        {
            try
            {
                var data = await db.Genres.OrderByDescending(x => x.CreateAt).ToListAsync();               
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
        public async Task<ActionResult> Add([FromBody] Genre genre)
        {
            await db.Genres.AddAsync(genre);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Add genre success!",
                status = 200,
                data = genre
            });
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] Genre genre)
        {
            var element = await db.Genres.FindAsync(genre.Id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The genre doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.Genres.FirstOrDefaultAsync(x => x.Id == genre.Id)).CurrentValues.SetValues(genre);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit genre success!",
                status = 200
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.Genres.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The genre doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.Genres.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete genre success!",
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
    public class Data
    {
        public string Name { get; set; }
        public List<Song> Songs { get; set; }
    }
}
