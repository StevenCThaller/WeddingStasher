using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace WeddingPhotos.Models
{
    public class FileModel
    {
        public List<string> FileNames { get; set; }
        public List<IFormFile> FormFiles { get; set; }
    }
}
