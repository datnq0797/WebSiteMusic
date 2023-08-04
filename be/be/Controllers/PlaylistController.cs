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
    [Route("api/playlist")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly MyMusicContext db;
        public PlaylistController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.Playlists.FindAsync(id);
                if (data == null)
                {
                    return Ok(new
                    {
                        status = 400,
                        message = "The playlist doesn't exist in database"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data,
                    message = "Get playlist is success!"
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
                var data = await db.Playlists.ToListAsync();
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
        public async Task<ActionResult> Add([FromBody] Playlist playlist)
        {
            await db.Playlists.AddAsync(playlist);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Add playlist success!",
                status = 200,
                data = playlist
            });
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] Playlist playlist)
        {
            var element = await db.Playlists.FindAsync(playlist.Id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The playlist doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.Playlists.FirstOrDefaultAsync(x => x.Id == playlist.Id)).CurrentValues.SetValues(playlist);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit playlist success!",
                status = 200
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.Playlists.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The playlist doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.Playlists.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete playlist success!",
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
        [HttpDelete("delete/detail")]

        public async Task<ActionResult> DeleteDetail([FromBody] DeleteDetailPlaylist detail)
        {
            var element = await db.PlaylistDetails.Where(x =>x.IdPlaylist == detail.IdPlaylist).Where(x => x.IdSong == detail.IdSong).FirstOrDefaultAsync();
            if (element == null)
            {
                return Ok(new
                {
                    message = "The detail playlist doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.PlaylistDetails.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete detail playlist success!",
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
        public async Task<ActionResult> GetPlaylists(Guid idUser)
        {
            var data = await db.Playlists.Where(x => x.IdUser == idUser).ToListAsync();
            if (data.Count >= 0)
            {
                return Ok(new
                {
                    status = 200,
                    data
                });
            }
            return BadRequest();
        }
        [HttpGet("song")]
        public async Task<ActionResult> GetSongByPlaylist(Guid id)
        {
            var data = await db.PlaylistDetails.Where(x => x.IdPlaylist == id).ToListAsync();
            if (data.Count >= 0)
            {
                var listdata = new List<Song>();
                foreach (var items in data)
                {
                    var item = await db.Songs.FindAsync(items.IdSong);
                    if (item != null)
                    {
                        listdata.Add(item);
                    }
                }

                return Ok(new
                {
                    status = 200,
                    data = listdata
                });
            }
            return BadRequest();
        }
    }
    public class DeleteDetailPlaylist
    {
        public Guid IdSong { get; set; }
        public Guid IdPlaylist { get; set; }
    }
}
