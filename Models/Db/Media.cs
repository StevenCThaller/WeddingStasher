using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using WeddingPhotos.Models;

namespace WeddingPhotos.Models.Db
{
    public class Media
    {
        [Key]
        public int Id { get; set; }
        public string S3FileName { get; set; }
        public int MediaTypeId { get; set; }
        public MediaType MediaType { get; set; }
        public int ContentTypeId { get; set; }
        public ContentType ContentType { get; set; }
        public int GuestBookEntryId { get; set; }
        public GuestBookEntry GuestBookEntry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string Url
        {
            get
            {
                // if(MediaType != null)
                // {
                    return $"https://{AppSettings.appSettings.BucketName}.s3.{AppSettings.appSettings.Region}.amazonaws.com/{MediaType.Name}/{S3FileName}";
                // }
                // return "";
            }
        }

        public string ObjectKey 
        {
            get 
            {
                return $"{MediaType.Name}/{S3FileName}";
            }
        }

        public string Extension
        {
            get
            {
                return ContentType.Name;
            }
        }

        public string Type
        {
            get 
            {
                return MediaType.Name;
            }
        }
    }
}