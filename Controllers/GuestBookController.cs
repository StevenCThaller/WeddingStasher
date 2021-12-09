using Microsoft.AspNetCore.Mvc;
using WeddingPhotos.Models.Db;
using WeddingPhotos.Models.View;
using WeddingPhotos.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;

namespace WeddingPhotos.Controllers
{
    [Route("guestbook")]
    public class GuestBookController : Controller 
    {
        private WPContext _context;
        private IMediaService _mediaService;
        private IGuestService _guestService;
        private IGuestBookService _guestBookService;

        public GuestBookController(WPContext context, IMediaService mediaService, IGuestService guestService, IGuestBookService guestBookService)
        {
            _context = context;
            _mediaService = mediaService;
            _guestService = guestService;
            _guestBookService = guestBookService;
        }

        [HttpGet("new")]
        public async Task<IActionResult> NewEntry()
        {
            int? GuestId = HttpContext.Session.GetInt32("GuestId");
            GuestBookEntryView entry = new GuestBookEntryView();
            if(GuestId != null) 
            {
                Guest inDb = await _guestService.FindGuestById((int)GuestId);
                entry.Name = inDb.Name;
                entry.Email = inDb.Email;
                ViewBag.LoggedIn = (int)GuestId;
            }

            return View(entry);
        }

        [HttpPost("new")]
        [RequestSizeLimit(200_000_000)]
        public async Task<IActionResult> CreateEntry(GuestBookEntryView guestBook)
        {
            try 
            {
                // System.Console.WriteLine("Is this even working?");
                int? GuestId = HttpContext.Session.GetInt32("GuestId");
                if(GuestId == null)
                {
                    // System.Console.WriteLine("Did it break here?");
                    Guest guestInDb = await _guestService.FindGuestByEmail(guestBook.Email);
                    if(guestInDb == null) 
                    {
                        GuestView guestToAdd = new GuestView()
                        {
                            Name = guestBook.Name,
                            Email = guestBook.Email
                        };
                        
                        Guest added = await _guestService.CreateGuest(guestToAdd);
                        GuestId = added.Id;
                    } 
                    else 
                    {
                        GuestId = guestInDb.Id;
                        // return new JsonResult(new { Result = "Validation Error", Message = "Please log into your account!" });
                    }
                    HttpContext.Session.SetInt32("GuestId", (int)GuestId);
                }

                // System.Console.WriteLine("Or here?");
                GuestBookEntry newEntry = await _guestBookService.CreateGuestBookEntry(guestBook, (int)GuestId);
                // System.Console.WriteLine("Or, perhaps HERE?!");
                if(guestBook.Files != null)
                {
                    // System.Console.WriteLine("Or all the way in here?");
                    await _mediaService.UploadMedia(newEntry.Id, guestBook.Files);
                }
                return new JsonResult(new { Result = "Success", Message = "Added to DB" });
            }
            catch(Exception ex)
            {
                return new JsonResult(new { Result = "Error", Message = ex.Message });
            }

        }

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            int? GuestId = HttpContext.Session.GetInt32("GuestId");
            if(GuestId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            GuestBookEntry toDelete = await _guestBookService.GetEntry(id);
            Guest loggedInUser = await _guestService.GetLoggedInGuest((int)GuestId);

            if(toDelete.GuestId != (int)GuestId && loggedInUser.RoleId > 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if(toDelete.Media.Count > 0) 
            {
                await _mediaService.DeleteFromS3(toDelete.Media);
            }
            await _guestBookService.DeleteEntry(toDelete);
            
            return RedirectToAction("Profile", "Guest");
        }
    }
}

// using Microsoft.AspNetCore.Mvc;
// using WeddingPhotos.Models.Db;
// using WeddingPhotos.Models.View;
// using WeddingPhotos.Services;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Http.Features;
// using System.IO;
// using System.Collections.Generic;
// using WeddingPhotos.Attributes;
// using WeddingPhotos.Utilities;
// using WeddingPhotos.Models;
// using Microsoft.Net.Http.Headers;
// using Microsoft.AspNetCore.WebUtilities;
// using System.Net;

// namespace WeddingPhotos.Controllers
// {
//     [Route("guestbook")]
//     public class GuestBookController : Controller 
//     {
//         private WPContext _context;
//         private IMediaService _mediaService;
//         private FormOptions _defaultFormOptions = new FormOptions();
//         private readonly string[] _permittedExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".mov", ".mpg" };

//         public GuestBookController(WPContext context, IMediaService mediaService)
//         {
//             _context = context;
//             _mediaService = mediaService;
//         }

//         [HttpGet("")]
//         public IActionResult Home()
//         {
            

//             return View();
//         }

//         [HttpGet("new")]
//         public IActionResult NewEntry()
//         {
//             int? GuestId = HttpContext.Session.GetInt32("GuestId");
//             if(GuestId == null)
//             {
//                 HttpContext.Session.SetString("RedirectToAction", "/guestbook");
//                 return RedirectToAction("NewGuest", "Guest");
//             }

//             return View();
//         }

//         [HttpPost("new")]
//         [DisableFormValueModelBinding]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> CreateEntry()
//         {
//             if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
//             {
//                 ModelState.AddModelError("File", 
//                     $"The request couldn't be processed (Error 1).");
//                 // Log error

//                 return BadRequest(ModelState);
//             }

//             var boundary = MultipartRequestHelper.GetBoundary(
//                 MediaTypeHeaderValue.Parse(Request.ContentType),
//                 _defaultFormOptions.MultipartBoundaryLengthLimit);
//             var reader = new MultipartReader(boundary, HttpContext.Request.Body);
//             var section = await reader.ReadNextSectionAsync();

//             while (section != null)
//             {
//                 var hasContentDispositionHeader = 
//                     ContentDispositionHeaderValue.TryParse(
//                         section.ContentDisposition, out var contentDisposition);

//                 if (hasContentDispositionHeader)
//                 {
//                     // This check assumes that there's a file
//                     // present without form data. If form data
//                     // is present, this method immediately fails
//                     // and returns the model error.
//                     if (!MultipartRequestHelper
//                         .HasFileContentDisposition(contentDisposition))
//                     {
//                         ModelState.AddModelError("File", 
//                             $"The request couldn't be processed (Error 2).");
//                         // Log error

//                         return BadRequest(ModelState);
//                     }
//                     else
//                     {
//                         // Don't trust the file name sent by the client. To display
//                         // the file name, HTML-encode the value.
//                         var trustedFileNameForDisplay = WebUtility.HtmlEncode(
//                                 contentDisposition.FileName.Value);
//                         var trustedFileNameForFileStorage = Path.GetRandomFileName();

//                         // **WARNING!**
//                         // In the following example, the file is saved without
//                         // scanning the file's contents. In most production
//                         // scenarios, an anti-virus/anti-malware scanner API
//                         // is used on the file before making the file available
//                         // for download or for use by other systems. 
//                         // For more information, see the topic that accompanies 
//                         // this sample.

//                         var streamedFileContent = await FileHelpers.ProcessStreamedFile(
//                             section, contentDisposition, ModelState, 
//                             _permittedExtensions, AppSettings.appSettings.FileSizeLimit);

//                         if (!ModelState.IsValid)
//                         {
//                             return BadRequest(ModelState);
//                         }

//                         using (var targetStream = System.IO.File.Create(
//                             Path.Combine(Directory.GetCurrentDirectory(), "www3root/s3", trustedFileNameForFileStorage)))
//                         {
//                             await targetStream.WriteAsync(streamedFileContent);

//                             // _logger.LogInformation(
//                             //     "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
//                             //     "'{TargetFilePath}' as {TrustedFileNameForFileStorage}", 
//                             //     trustedFileNameForDisplay, _targetFilePath, 
//                             //     trustedFileNameForFileStorage);
//                         }
//                     }
//                 }

//                 // Drain any remaining section body that hasn't been consumed and
//                 // read the headers for the next section.
//                 section = await reader.ReadNextSectionAsync();
//             }

//             return Ok("yes");
//         }
//     }
// }