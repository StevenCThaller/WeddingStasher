using WeddingPhotos.Models.Db;
using WeddingPhotos.Models;
using WeddingPhotos.Models.View;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace WeddingPhotos.Services
{
    public interface IGuestService
    {
        Task<Guest> CreateGuest(GuestView guestView);
        Task<Guest> FindGuestByEmail(string email);
        Task<Guest> FindGuestById(int id);
        Task<Guest> GetLoggedInGuest(int id);
        Task<Guest> GetGuestProfile(int id, int page);
        Task<Guest> UpdateGuest(Guest guest);
        Task<int> TotalPagesByGuest(int id);
        Task<bool> Any();
        Task<Guest> CreateAdmin(Guest guest);
        Task<int> Login(Guest guest);
    }

    public class GuestService : IGuestService
    {
        private WPContext _context;
        public GuestService(WPContext context)
        {
            _context = context;
        }

        public async Task<Guest> CreateAdmin(Guest guest)
        {
            await Task.Delay(0);
            PasswordHasher<Guest> hasher = new PasswordHasher<Guest>();
            guest.Password = hasher.HashPassword(guest, guest.Password);
            _context.Add(guest);
            _context.SaveChanges();
            return guest;
        }

        public async Task<Guest> CreateGuest(GuestView guestView)
        {
            await Task.Delay(0);
            Guest toAdd = new Guest()
            {
                Name = guestView.Name,
                Email = guestView.Email,
                Password = AppSettings.appSettings.DefaultPassword
            };
            PasswordHasher<Guest> hasher = new PasswordHasher<Guest>();
            toAdd.Password = hasher.HashPassword(toAdd, toAdd.Password);
            _context.Add(toAdd);
            _context.SaveChanges();
            return toAdd;
        }

        public async Task<Guest> FindGuestByEmail(string email)
        {
            await Task.Delay(0);
            return _context.Guests
                .FirstOrDefault(g => g.Email == email);
        }
        public async Task<Guest> FindGuestById(int id)
        {
            await Task.Delay(0);
            return _context.Guests
                .FirstOrDefault(g => g.Id == id);
        }

        public async Task<Guest> GetLoggedInGuest(int id)
        {
            await Task.Delay(0);
            Guest toReturn = _context.Guests
                .Include(g => g.Entries)
                .ThenInclude(e => e.Media)
                    .ThenInclude(m => m.ContentType)
                .Include(g => g.Entries)
                .ThenInclude(e => e.Media)
                    .ThenInclude(m => m.MediaType)
                .FirstOrDefault(g => g.Id == id);

            toReturn.NewPassword = "";
            toReturn.ConfirmNewPassword = "";
            return toReturn;
        }

        public async Task<Guest> GetGuestProfile(int id, int page)
        {
            await Task.Delay(0);
            Guest toReturn = _context.Guests
                .Include(g => g.Entries)
                .ThenInclude(e => e.Media)
                    .ThenInclude(m => m.ContentType)
                .Include(g => g.Entries)
                .ThenInclude(e => e.Media)
                    .ThenInclude(m => m.MediaType)
                .FirstOrDefault(g => g.Id == id);

            toReturn.Entries = toReturn.Entries.Skip(page * 5).Take(5).ToList();
            toReturn.NewPassword = "";
            toReturn.ConfirmNewPassword = "";
            return toReturn;
        }

        public async Task<Guest> UpdateGuest(Guest guest)
        {
            Guest guestToUpdate = await FindGuestById(guest.Id);
            guestToUpdate.Name = guest.Name;
            guestToUpdate.Email = guest.Email;
            if(guest.NewPassword != null)
            {
                PasswordHasher<Guest> hasher = new PasswordHasher<Guest>();
                guestToUpdate.Password = hasher.HashPassword(guest, guest.NewPassword);
            }
            guestToUpdate.UpdatedAt = System.DateTime.Now;
            _context.Guests.Update(guestToUpdate);
            _context.SaveChanges();
            return guestToUpdate;
        }

        public async Task<int> TotalPagesByGuest(int id)
        {
            await Task.Delay(0);
            return (_context.Guests
                .Include(g => g.Entries)
                .FirstOrDefault(g => g.Id == id)
                .Entries
                .Count() / 5) + 1;
        }

        public async Task<bool> Any()
        {
            await Task.Delay(0);
            return _context.Guests.ToList().Count > 0;
        }

        public async Task<int> Login(Guest guest)
        {
            Guest guestInDb = await FindGuestByEmail(guest.Email);

            if(guestInDb == null)
            {
                return -1;
            }

            PasswordHasher<Guest> hasher = new PasswordHasher<Guest>();

            var result = hasher.VerifyHashedPassword(guest, guestInDb.Password, guest.Password);

            if(result == 0)
            {
                return -1;
            }

            return guestInDb.Id;
        }
    }
}