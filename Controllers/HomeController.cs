using Microsoft.AspNetCore.Mvc;
using WeddingPhotos.Models.Db;
using WeddingPhotos.Models.View;
using WeddingPhotos.Models;
using WeddingPhotos.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WeddingPhotos.Controllers
{
    [Route("")]
    public class HomeController : Controller 
    {
        private IGuestBookService _guestBookService;
        private IGuestService _guestService;

        public HomeController(IGuestBookService guestBookService, IGuestService guestService)
        {
            _guestBookService = guestBookService;
            _guestService = guestService;
        }

        [HttpGet("{page ?}")]
        public async Task<IActionResult> Index(int? page)
        {   
            if(!await _guestService.Any())
            {
                foreach(JsonUser user in AppSettings.appSettings.Users)
                {
                    Guest admin = new Guest()
                    {
                        Name = user.Name,
                        Email = user.Email,
                        Password = AppSettings.appSettings.DefaultPassword,
                        RoleId = user.RoleId,
                        CreatedAt = System.DateTime.Now,
                        UpdatedAt = System.DateTime.Now
                    };

                    await _guestService.CreateAdmin(admin);
                }
            }


            int? GuestId = HttpContext.Session.GetInt32("GuestId");
            if(GuestId != null)
            {
                ViewBag.LoggedIn = (int)GuestId;
            }
            if(page == null) 
            {
                page = 1;
            }

            IndexView ViewModel = new IndexView()
            {
                AllEntries = await _guestBookService.GetPageOfEntries((int)page-1),
                PageTurn = new PageTurnView()
                {
                    TotalPages = await _guestBookService.GetTotalPageCount(),
                    CurrentPage = (int)page
                }
            };



            if((int)page > ViewModel.PageTurn.TotalPages) 
            {
                return RedirectToAction("Index");
            }

            if(ViewModel.AllEntries.Count() == 0)
            {
                return RedirectToAction("NewEntry","GuestBook");
            }

            return View(ViewModel);
        }
    }
}