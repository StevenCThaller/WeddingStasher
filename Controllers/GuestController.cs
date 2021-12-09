using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WeddingPhotos.Services;
using WeddingPhotos.Models.View;
using WeddingPhotos.Models.Db;
using System.Threading.Tasks;

namespace WeddingPhotos.Models
{
    [Route("guest")]
    public class GuestController : Controller 
    {
        private IGuestService _guestService;
        private IGuestBookService _guestBookService;
        public GuestController(IGuestService guestService, IGuestBookService guestBookService)
        {
            _guestService = guestService;
            _guestBookService = guestBookService;
        }
        
        [HttpGet("{page ?}")]
        public async Task<IActionResult> Profile(int? page = null)
        {
            int? GuestId = HttpContext.Session.GetInt32("GuestId");
            if(GuestId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.LoggedIn = (int)GuestId;
            }
            if(page == null)
            {
                page = 1;
            }

            Guest loggedIn = await _guestService.GetGuestProfile((int)GuestId, (int)page - 1);

            loggedIn.PageTurn = new PageTurnView()
            {
                CurrentPage = (int)page,
                TotalPages = await _guestService.TotalPagesByGuest((int)GuestId)
            };

            if(loggedIn.RoleId < 3)
            {
                loggedIn.Entries = await _guestBookService.GetPageOfEntries((int)page - 1);
                loggedIn.PageTurn.TotalPages = await _guestBookService.GetTotalPageCount();
            }

            ViewBag.Wedders = $"{AppSettings.appSettings.Wedders[0][0]} + {AppSettings.appSettings.Wedders[1][0]}";

            return View("Profile", loggedIn);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> UpdateGuest(Guest guest)
        {
            int? GuestId = HttpContext.Session.GetInt32("GuestId");
            if(GuestId == null)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewBag.LoggedIn = (int)GuestId;
            }
            System.Console.WriteLine(guest.Email);
            guest.Id = (int)GuestId;

            if(guest.NewPassword != null && guest.NewPassword.Length < 8)
            {
                ModelState.AddModelError("NewPassword", "Password must be at least 8 characters. Leave it blank to undo update.");
                return await Profile();
            }
            else if(guest.NewPassword != guest.ConfirmNewPassword)
            {
                ModelState.AddModelError("NewPassword", "Passwords do not match.");
                return await Profile();
            }

            await _guestService.UpdateGuest(guest);
            return RedirectToAction("Profile");
        }

        [HttpGet("login")]
        public IActionResult LoginPage()
        {
            int? GuestId = HttpContext.Session.GetInt32("GuestId");
            if(GuestId != null)
            {
                return RedirectToAction("Index", "Home");
            }
            LoginPageView ToRender = new LoginPageView();
            return View("LoginPage", ToRender);
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Guest guest)
        {
            if(guest.Password == null)
            {
                ModelState.AddModelError("Password", "Password is required.");
                return LoginPage();
            }

            int guestId = await _guestService.Login(guest);

            if(guestId < 1)
            {
                ModelState.AddModelError("Email", $"Invalid email/password. Default password is {AppSettings.appSettings.DefaultPassword}");
                return LoginPage();
            }

            HttpContext.Session.SetInt32("GuestId", guestId);
            return RedirectToAction("Index", "Home");
        }
    }
}