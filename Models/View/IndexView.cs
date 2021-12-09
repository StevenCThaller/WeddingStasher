using System.Collections.Generic;
using WeddingPhotos.Models.Db;

namespace WeddingPhotos.Models.View
{
    public class IndexView
    {
        public List<GuestBookEntry> AllEntries { get; set; }

        public PageTurnView PageTurn { get; set; }
        
        public string Wedders = $"{AppSettings.appSettings.Wedders[0][0]} + {AppSettings.appSettings.Wedders[1][0]}";
    }
}