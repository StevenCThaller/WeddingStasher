using System;
using System.Collections.Generic;
using WeddingPhotos.Models.Db;
using WeddingPhotos.Models;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.IO.Compression;


namespace WeddingPhotos.Services
{
    public interface IMediaService
    {
        Task<IEnumerable<Media>> GetAllMedia();
        Task<List<Media>> GetAllImages();
        Task<IEnumerable<Media>> GetAllVideos();
        Task<List<Media>> UploadMedia(int guestBookEntryId, List<IFormFile> files);
        Task<bool> DeleteFromS3(List<Media> toDelete);
        Task<bool> DownloadAll();

        Task<bool> DeleteZippedFiles();
    }

    public class MediaService : IMediaService 
    {
        private WPContext _context;
        private IAmazonS3 s3Client;
        private string bucketName = AppSettings.appSettings.BucketName;

        public MediaService(WPContext context)
        {
            _context = context;
            s3Client = new AmazonS3Client(AppSettings.appSettings.AWSAccessKey, AppSettings.appSettings.AWSSecretKey, RegionEndpoint.USEast2);
        }

        public async Task<IEnumerable<Media>> GetAllMedia()
        {
            await Task.Delay(0);
            return _context.Media 
                .Include(m => m.MediaType)
                .Include(m => m.ContentType);
        }

        public async Task<List<Media>> GetAllImages()
        {
            await Task.Delay(0);
            return _context.Media
                .Include(m => m.MediaType)
                .Include(m => m.ContentType)
                .Where(m => m.MediaTypeId == 1)
                .ToList();
        }
        public async Task<IEnumerable<Media>> GetAllVideos()
        {
            await Task.Delay(0);
            return _context.Media
                .Where(m => m.MediaTypeId == 2);
        }

        public async Task<bool> DeleteMedia(List<int> mediaIds)
        {
            try
            {   
                
                _context.Media.RemoveRange(
                    _context.Media.Where(m => mediaIds.Any(i => i == m.Id))
                );
            
                await Task.Delay(0);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<Media>> UploadMedia(int guestBookEntryId, List<IFormFile> files)
        {
            List<Media> mediaList = new List<Media>();
            System.Console.WriteLine("Media Upload Incoming");
            for(int i = 0; i < files.Count; i++) {
                var file = files[i];
                string contentType;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/s3", files[i].FileName);
                int mediaType = 1;
                using(Stream stream = new FileStream(path, FileMode.Create))
                {
                    System.Console.WriteLine($"Uploading {files[i].FileName}");
                    file.CopyTo(stream);
                    TransferUtility utility = new TransferUtility(s3Client);
                    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
                    string fileType = "";
                    contentType = MimeTypes.GetContentType(files[i].FileName);
                    // System.Console.WriteLine(contentType);
                    if(contentType.StartsWith("image"))
                    {
                        fileType = "Image";
                        mediaType = 1;
                    } 
                    else if(contentType.StartsWith("application/octet-stream") || (files[i].FileName.EndsWith(".mov") || files[i].FileName.EndsWith(".mp4")))
                    {
                        fileType = "Video";
                        mediaType = 2;
                    } else {
                        throw new Exception("Not a valid file type");
                    }
                    
                    request.BucketName = $"{AppSettings.appSettings.BucketName}/{fileType}";
                    // System.Console.WriteLine(request.BucketName);
                    request.Key = files[i].FileName;
                    request.InputStream = stream;
                    utility.Upload(request);
                    FileInfo toDelete = new FileInfo(path);
                    toDelete.Delete();

                    
                }
                // $"{AppSettings.appSettings.S3Bucket}.s3.{AppSettings.appSettings.Region}.amazonaws.com/{fileType}/{files.FileNames[i]}";
                // System.Console.WriteLine(files[i].FileName);
                // string ContentType = files[i].FileName.Substring(files[i].FileName.LastIndexOf('.')+1);

                ContentType thisFilesExtension = _context.ContentTypes.FirstOrDefault(e => e.Name == contentType);
                // System.Console.WriteLine(thisFilesExtension.Name);

                Media media = new Media()
                {
                    S3FileName = files[i].FileName,
                    MediaTypeId = mediaType,
                    GuestBookEntryId = guestBookEntryId,
                    ContentTypeId = thisFilesExtension.Id
                };

                mediaList.Add(media);
                _context.Add(media);
            }
            _context.SaveChanges();
            await Task.Delay(0);
            return mediaList;
        }

        public async Task<bool> DeleteFromS3(List<Media> toDelete)
        {
            List<KeyVersion> keysAndVersions = await GenerateKeys(toDelete);

            if(keysAndVersions.Count == 0) 
            {
                return true;
            }

            DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest
            {
                BucketName = AppSettings.appSettings.BucketName,
                Objects = keysAndVersions
            };

            try 
            {
                DeleteObjectsResponse response = await s3Client.DeleteObjectsAsync(multiObjectDeleteRequest);
                System.Console.WriteLine("Successfully deleted {0}/{1} items", response.DeletedObjects.Count, toDelete.Count);
                return true;
            }
            catch(DeleteObjectsException e)
            {
                PrintDeletionErrorStatus(e);
                return false;
            }
        }
        private static void PrintDeletionErrorStatus(DeleteObjectsException e)
        {
            // var errorResponse = e.ErrorResponse;
            DeleteObjectsResponse errorResponse = e.Response;
            Console.WriteLine("x {0}", errorResponse.DeletedObjects.Count);

            Console.WriteLine("No. of objects successfully deleted = {0}", errorResponse.DeletedObjects.Count);
            Console.WriteLine("No. of objects failed to delete = {0}", errorResponse.DeleteErrors.Count);

            Console.WriteLine("Printing error data...");
            foreach (DeleteError deleteError in errorResponse.DeleteErrors)
            {
                Console.WriteLine("Object Key: {0}\t{1}\t{2}", deleteError.Key, deleteError.Code, deleteError.Message);
            }
        }

        private async Task<List<KeyVersion>> GenerateKeys(List<Media> media)
        {
            IEnumerable<Media> allMedia = await GetAllMedia();
            List<KeyVersion> keys = new List<KeyVersion>();
            foreach(Media thing in media)
            {
                if(allMedia.Where(m => m.Url == thing.Url).ToList().Count == 1)
                {
                    PutObjectRequest request = new PutObjectRequest
                    {
                        BucketName = AppSettings.appSettings.BucketName,
                        Key = thing.ObjectKey,
                        ContentBody = "Delete this ish"
                    };

                    PutObjectResponse response = await s3Client.PutObjectAsync(request);

                    KeyVersion keyVersion = new KeyVersion 
                    {
                        Key = thing.ObjectKey
                    };
                    keys.Add(keyVersion);
                }
            }
            return keys;
        }

        public async Task<bool> DownloadAll()
        {
            try
            {
                IEnumerable<Media> allMedia = await GetAllMedia();
                List<Media> listOfMedia = allMedia.ToList();

                for(int i = 0; i < listOfMedia.Count(); i++)
                {
                    GetObjectRequest request = new GetObjectRequest();
                    string thing =  $"{listOfMedia[i].Type}_{i}.{listOfMedia[i].Extension}";
                    System.Console.WriteLine(thing);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "s3", "wedding", listOfMedia[i].S3FileName);
                    FileStream stream = new FileStream(path, FileMode.Create);
                    System.Console.WriteLine(listOfMedia[i].S3FileName);
                    request.Key = listOfMedia[i].S3FileName;
                    request.BucketName = $"{AppSettings.appSettings.BucketName}/{listOfMedia[i].Type}";

                    System.Console.WriteLine(request.Key);
                    System.Console.WriteLine(request.BucketName);

                    GetObjectResponse response = await s3Client.GetObjectAsync(request);

                    int numBytesToRead = (int)response.ContentLength;
                    int numBytesRead = 0;

                    byte[] buffer = new byte[numBytesToRead];

                    while(numBytesToRead > 0) 
                    {
                        int n = response.ResponseStream.Read(buffer, numBytesRead, numBytesToRead);
                        stream.Write(buffer, numBytesRead, n);

                        if(n == 0)
                            break;
                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    stream.Close();
                }

                return true;
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteZippedFiles()
        {
            await Task.Delay(100);
            try 
            {

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "s3", "wedding");
                System.Console.WriteLine(folderPath + " Line 269");
                var filePaths = Directory.GetFiles(folderPath);
                System.Console.WriteLine("Line 271");

                System.Console.WriteLine("Line 273");
                foreach(var filePath in filePaths) 
                {
                    System.Console.WriteLine("Line 276");
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "s3", "wedding", filePath);
                    System.Console.WriteLine(path + " Line 278");
                    // using(Stream stream = new FileStream(path, FileMode.Open))
                    // {
                        System.Console.WriteLine("Line 281");
                        // var fileName = Path.GetFileName(filePath);
                        // System.Console.WriteLine(fileName);
                        FileInfo toDelete = new FileInfo(path);
                        System.Console.WriteLine("Did this work?");
                        toDelete.Delete();
                }
                return true;
            }
            catch (Exception ex) 
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}