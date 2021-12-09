using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WeddingPhotos.Models.View
{
    public class GuestBookEntryView
    {
        [Required(ErrorMessage = "Let us know who you are!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "We want to share all of these memories with you too! Let us know where to send the final collage!")]
        public string Email { get; set; }
        public string Message { get; set; }
        public List<IFormFile> Files { get; set; }
        public string Wedders = $"{AppSettings.appSettings.Wedders[0][0]} + {AppSettings.appSettings.Wedders[1][0]}";

    }
}