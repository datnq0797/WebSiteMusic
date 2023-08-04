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
    [Route("api/artist")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly MyMusicContext db;
        public ArtistController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ActionResult> GetElement(Guid id)
        {
            try
            {
                var data = await db.Artists.FindAsync(id);
                if(data == null)
                {
                    return Ok(new
                    {
                        status = 400,
                        message = "The artist doesn't exist in database"
                    });
                }
                return Ok(new
                {
                    status = 200,
                    data,
                    message = "Get artist is success!"
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
                var data = await db.Artists.OrderByDescending(x => x.CreateAt).ToListAsync();
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
        public async Task<ActionResult> Add([FromBody] Artist artist)
        {
            await db.Artists.AddAsync(artist);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Add artist success!",
                status = 200,
                data = artist
            });
        }
        [HttpPut("edit")]

        public async Task<ActionResult> Edit([FromBody] Artist artist)
        {
            var element = await db.Artists.FindAsync(artist.Id);
            UploadFileService.Instance.DeleteFileImage(element.Image);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The artist doesn't exist in database!",
                    status = 400
                });
            }
            db.Entry(await db.Artists.FirstOrDefaultAsync(x => x.Id == artist.Id)).CurrentValues.SetValues(artist);
            await db.SaveChangesAsync();
            return Ok(new
            {
                message = "Edit artist success!",
                status = 200
            });
        }
        [HttpDelete("delete")]

        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            var element = await db.Artists.FindAsync(id);
            if (element == null)
            {
                return Ok(new
                {
                    message = "The artist doesn't exist in database!",
                    status = 404
                });
            }
            try
            {
                UploadFileService.Instance.DeleteFileImage(element.Image);
                db.Artists.Remove(element);
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Delete artist success!",
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
