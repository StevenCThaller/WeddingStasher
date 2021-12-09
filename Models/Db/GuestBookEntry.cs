using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WeddingPhotos.Models.Db
{
    public class GuestBookEntry
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public int GuestId { get; set; }
        public Guest Guest { get; set; }
        public List<Media> Media { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}