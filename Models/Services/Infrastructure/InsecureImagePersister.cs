using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MyCourse.Models.Services.Infrastructure
{
    public class InsecureImagePersister : IImagePersister
    {
        private readonly IWebHostEnvironment env;
        public InsecureImagePersister(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async Task<string> SaveCourseImageAsync(int courseId, IFormFile formFile)
        {
            string path = $"/Courses/{courseId}.jpg";
            string phisicalPath = Path.Combine(env.WebRootPath, "Courses", $"{courseId}.jpg");
            using FileStream fileStream = File.OpenWrite(phisicalPath);
            await formFile.CopyToAsync(fileStream);

            return path;
        }
    }
}