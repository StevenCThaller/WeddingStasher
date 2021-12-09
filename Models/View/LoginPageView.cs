using System.ComponentModel.DataAnnotations;
using WeddingPhotos.Models.Db;

namespace WeddingPhotos.Models.View
{
    public class LoginPageView 
    {
        public Guest Guest { get; set; }

        public string Wedders = $"{AppSettings.appSettings.Wedders[0][0]} + {AppSettings.appSettings.Wedders[1][0]}";
    }
}