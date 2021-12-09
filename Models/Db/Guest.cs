using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using WeddingPhotos.Models.View;

namespace WeddingPhotos.Models.Db 
{
    public class Guest
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        [Required(ErrorMessage = "We need your email")]
        public string Email { get; set; }
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }
        [NotMapped]
        public string NewPassword { get; set; }
        [NotMapped]
        public string ConfirmNewPassword { get; set; }
        public int RoleId { get; set; } = 3;
        public Role Role { get; set; } 
        public List<GuestBookEntry> Entries { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public PageTurnView PageTurn { get; set; }
    }
}