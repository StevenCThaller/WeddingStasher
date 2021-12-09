using System.Collections.Generic;

namespace WeddingPhotos.Models
{
    public class AppSettings
    {
        public static AppSettings appSettings { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public JsonUser[] Users { get; set; }
        public string DefaultPassword { get; set; }
        public long FileSizeLimit { get; set; }
        public string[] Wedders { get; set; }
    }
}