using be.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace be.Controllers
{
    [Route("api/statistical")]
    [ApiController]
    public class StatisticalController : ControllerBase
    {
        private readonly MyMusicContext db;
        public StatisticalController(MyMusicContext db)
        {
            this.db = db;
        }
        [HttpGet("songs")]
        public async Task<ActionResult> StatisticalSongs()
        {
            List<int> listData = new List<int>();
            DateTime day = DateTime.Now;
            int year = day.Year;
            for (int i = 1; i <= 12; i++)
            {
                int total = 0;
                total = await db.Songs.Where(x => x.CreateAt.Month == i).Where(x => x.CreateAt.Year == year).CountAsync();
                listData.Add(total);
            }
            return Ok(listData);
        }
        [HttpGet("favorites")]
        public async Task<ActionResult> StatisticalFavorites()
        {
            List<int> listData = new List<int>();
            DateTime day = DateTime.Now;
            int year = day.Year;
            for (int i = 1; i <= 12; i++)
            {
                int total = 0;
                total = await db.SongFavourites.Where(x => x.CreateAt.Month == i).Where(x => x.CreateAt.Year == year).Where(x => x.Status == true).CountAsync();
                listData.Add(total);
            }
            return Ok(listData);
        }
    }
}