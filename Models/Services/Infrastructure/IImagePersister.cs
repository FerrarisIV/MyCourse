using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IImagePersister
    {
        Task<string> SaveCourseImageAsync(int courseId, IFormFile formFile);
    }
}