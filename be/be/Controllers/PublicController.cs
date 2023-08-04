using be.Models;
using be.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers
{
    [Route("api/public")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        [HttpGet("display/music")]
        public async Task<ActionResult> GetMusic(string name)
        {
            var songPath = Path.Combine(Directory.GetCurrentDirectory(), @"Resources\Musics", name);
            byte[] fileBytes = System.IO.File.ReadAllBytes(songPath);
            string contentType = "audio/mpeg";
            return File(fileBytes, contentType);
        }
        [HttpGet("display/image")]
        public async Task<ActionResult> GetImage(string name)
        {
            var songPath = Path.Combine(Directory.GetCurrentDirectory(), @"Resources\Images", name);
            byte[] imageBytes = System.IO.File.ReadAllBytes(songPath);
            string contentType = "image/jpeg";
            return File(imageBytes, contentType);
        }
        [HttpPost("upload/music")]
        public ActionResult UploadFileMp3([FromForm] FileModel file)
        {
            try
            {
                var isSuccess = UploadFileService.Instance.UploadFileMp3(file);
                if (isSuccess)
                {
                    return Ok(new
                    {
                        status = 200,
                    });
                }
                return Ok(new
                {
                    status = 400,
                });
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("upload/image")]
        public ActionResult UploadFileIamge([FromForm] FileModel file)
        {
            try
            {
                var isSuccess = UploadFileService.Instance.UploadFileImage(file);
                if (isSuccess)
                {
                    return Ok(new
                    {
                        status = 200,
                    });
                }
                return Ok(new
                {
                    status = 400,
                });
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
