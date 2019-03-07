using Microsoft.AspNetCore.Http;

namespace AspNet.ServerBrowser.Models.Home
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
