using System.ComponentModel.DataAnnotations;
namespace WeddingPhotos.Models.View
{
    public class GuestView 
    {
        [Required(ErrorMessage = "Let us know who you are!")]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage = "Needs to be a valid email address")]
        [Required(ErrorMessage = "We want to share all of these memories with you too! Let us know where to send the final collage!")]
        public string Email { get; set; }
        
        
    }
}