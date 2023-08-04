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
    [Route("api/playlistdetail")]
    [ApiController]
    public class PlaylistDetailController : ControllerBase
    {
        private readonly MyMusicContext db;
        public PlaylistDetailController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.PlaylistDetails.FindAsync(id);
                if(data == null)
                {
                    return Ok(new
                    {
                        status = 400,
                        message = "The playlist detail doesn't exist in database"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data,
                    message = "Get playlist detail is success!"
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
                var data = await db.PlaylistDetails.ToListAsync();
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
        public async Task<ActionResult> Add([FromBody] PlaylistDetail playlistDetail)
        {
            var data = await db.PlaylistDetails.Where(x => x.IdPlaylist == playlistDetail.IdPlaylist).Where(x => x.IdSong == playlistDetail.IdSong).FirstOrDefaultAsync();
            if(data != null)
            {
                return Ok(new
                {
                    message = "The song is already in the playlist!",
                    status = 400
                });
            }
            await db.PlaylistDetails.AddAsync(playlistDetail);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Add playlist detail success!",
                status = 200,
                data = playlistDetail
            });
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] PlaylistDetail playlistDetail)
        {
            var element = await db.PlaylistDetails.FindAsync(playlistDetail.Id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The playlist detail doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.PlaylistDetails.FirstOrDefaultAsync(x => x.Id == playlistDetail.Id)).CurrentValues.SetValues(playlistDetail);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit playlist detail success!",
                status = 200
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.PlaylistDetails.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The playlist detail doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                db.PlaylistDetails.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete playlist detail success!",
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
