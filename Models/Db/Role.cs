using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPhotos.Models.Db
{
    public class Role 
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Guest> GuestsInRole { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}