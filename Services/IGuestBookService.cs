using System.Threading.Tasks;
using System.Collections.Generic;
using WeddingPhotos.Models.Db;
using WeddingPhotos.Models.View;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace WeddingPhotos.Services
{
    public interface IGuestBookService 
    {
        Task<List<GuestBookEntry>> GetAllEntries();
        Task<int> GetTotalPageCount();
        Task<List<GuestBookEntry>> GetPageOfEntries(int page);
        Task<GuestBookEntry> CreateGuestBookEntry(GuestBookEntryView entry, int guestId);
        Task<GuestBookEntry> GetEntry(int id);
        Task<bool> DeleteEntry(GuestBookEntry entry);
    }

    public class GuestBookService : IGuestBookService
    {
        private WPContext _context;

        public GuestBookService(WPContext context)
        {
            _context = context;
        }

        public async Task<List<GuestBookEntry>> GetAllEntries()
        {
            await Task.Delay(0);
            return _context.GuestBookEntries
                .Include(e => e.Media)
                .ThenInclude(m => m.MediaType)
                .Include(e => e.Media)
                .ThenInclude(m => m.ContentType)
                .Include(e => e.Guest)
                .OrderByDescending(e => e.Id)
                .ToList();
        }

        public async Task<List<GuestBookEntry>> GetPageOfEntries(int page)
        {
            await Task.Delay(0);
            return _context.GuestBookEntries
                .Include(e => e.Media)
                .ThenInclude(m => m.MediaType)
                .Include(e => e.Media)
                .ThenInclude(m => m.ContentType)
                .Include(e => e.Guest)
                .OrderByDescending(e => e.Id)
                .Skip(page * 5)
                .Take(5)
                .ToList();
        }

        public async Task<int> GetTotalPageCount()
        {
            await Task.Delay(0);
            return (_context.GuestBookEntries.Count() / 5) + 1;
        }

        public async Task<GuestBookEntry> CreateGuestBookEntry(GuestBookEntryView entry, int guestId)
        {
            GuestBookEntry toAdd = new GuestBookEntry()
            {
                Message = entry.Message,
                GuestId = guestId
            };

            await Task.Delay(0);
            _context.GuestBookEntries.Add(toAdd);
            _context.SaveChanges();
            return toAdd;
        }

        public async Task<GuestBookEntry> GetEntry(int id)
        {
            await Task.Delay(0);
            return _context.GuestBookEntries
                .Include(e => e.Media)
                .ThenInclude(m => m.MediaType)
                .FirstOrDefault(e => e.Id == id);
        }

        public async Task<bool> DeleteEntry(GuestBookEntry entry)
        {
            await Task.Delay(0);
            try 
            {
                _context.GuestBookEntries.Remove(entry);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}