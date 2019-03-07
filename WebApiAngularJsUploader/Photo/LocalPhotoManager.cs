using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using WebApiAngularJsUploader.Models;

namespace WebApiAngularJsUploader.Photo
{
    public class LocalPhotoManager : IPhotoManager
    {
        private string WorkingFolder { get; set; }

        public LocalPhotoManager()
        {
        }

        public LocalPhotoManager(string workingFolder)
        {
            this.WorkingFolder = workingFolder;
            CheckTargetDirectory();
        }

        public async Task<IEnumerable<PhotoViewModel>> Get()
        {
            var photos = new List<PhotoViewModel>();

            var photoFolder = new DirectoryInfo(WorkingFolder);

            await Task.Factory.StartNew(() =>
            {
                photos = photoFolder.EnumerateFiles()
                    .Where(fi => new[] {".jpg", ".bmp", ".png", ".gif", ".tiff"}.Contains(fi.Extension.ToLower()))
                    .Select(fi => new PhotoViewModel
                    {
                        Name = fi.Name,
                        Created = fi.CreationTime,
                        Modified = fi.LastWriteTime,
                        Size = fi.Length / 1024
                    })
                    .ToList();
            });

            return photos;
        }

        public async Task<PhotoActionResult> Delete(string fileName)
        {
            try
            {
                var filePath = Directory.GetFiles(WorkingFolder, fileName)
                    .FirstOrDefault();

                await Task.Factory.StartNew(() => { File.Delete(filePath); });

                return new PhotoActionResult {Successful = true, Message = fileName + "deleted successfully"};
            }
            catch (Exception ex)
            {
                return new PhotoActionResult
                    {Successful = false, Message = "error deleting fileName " + ex.GetBaseException().Message};
            }
        }

        public async Task<IEnumerable<PhotoViewModel>> Add(HttpRequestMessage request)
        {
            var provider = new PhotoMultipartFormDataStreamProvider(WorkingFolder);

            await request.Content.ReadAsMultipartAsync(provider);

            var photos = new List<PhotoViewModel>();

            foreach (var file in provider.FileData)
            {
                var fileInfo = new FileInfo(file.LocalFileName);

                photos.Add(new PhotoViewModel
                {
                    Name = fileInfo.Name,
                    Created = fileInfo.CreationTime,
                    Modified = fileInfo.LastWriteTime,
                    Size = fileInfo.Length / 1024
                });
            }

            return photos;
        }

        public bool FileExists(string fileName)
        {
            var file = Directory.GetFiles(WorkingFolder, fileName)
                .FirstOrDefault();

            return file != null;
        }

        private void CheckTargetDirectory()
        {
            if (!Directory.Exists(WorkingFolder))
            {
                throw new ArgumentException("the destination path " + WorkingFolder + " could not be found");
            }
        }
    }
}