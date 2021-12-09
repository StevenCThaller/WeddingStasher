using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WeddingPhotos.Models.Db
{
    public class MediaType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public List<Media> Media { get; set; }
    }
}