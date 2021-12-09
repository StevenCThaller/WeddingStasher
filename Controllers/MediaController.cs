using Microsoft.AspNetCore.Mvc;
using WeddingPhotos.Models.Db;
using WeddingPhotos.Models.View;
using WeddingPhotos.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;

using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;


namespace WeddingPhotos.Controllers
{
    [Route("media")]
    public class MediaController : Controller 
    {
        private IMediaService _mediaService;
        private IGuestService _guestService;

        public MediaController(IMediaService mediaService, IGuestService guestService)
        {
            _mediaService = mediaService;
            _guestService = guestService;
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download()
        {
            int? GuestId = HttpContext.Session.GetInt32("GuestId");
            if(GuestId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Guest loggedIn = await _guestService.GetLoggedInGuest((int)GuestId);

            if(loggedIn.RoleId > 2)
            {
                return RedirectToAction("Index", "Home");
            }

            await _mediaService.DownloadAll();


            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "attachment; filename=\"Wedding.zip\"");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "s3", "wedding");
            var filePaths = Directory.GetFiles(folderPath);

            using (ZipArchive archive = new ZipArchive(Response.BodyWriter.AsStream(), ZipArchiveMode.Create))
            {
                foreach(var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath);
                    var entry = archive.CreateEntry(fileName);
                    using(var entryStream = entry.Open())
                    using(var fileStream = System.IO.File.OpenRead(filePath))
                    {
                        await fileStream.CopyToAsync(entryStream);
                    }
                }
            }
            
            await _mediaService.DeleteZippedFiles();

            return new EmptyResult();

            // return 
        }

        [HttpGet("slideshow")]
        public async Task<IActionResult> SlideShow()
        {
            await Task.Delay(0);
            return View("SlideShow");
        }

        [HttpGet("images")]
        public async Task<IActionResult> JsonImages()
        {
            try 
            {
                List<Media> allPictures = await _mediaService.GetAllImages();
                await Task.Delay(10);
                JsonResult result = new JsonResult(new { Message = "Success", Results = allPictures });

                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}