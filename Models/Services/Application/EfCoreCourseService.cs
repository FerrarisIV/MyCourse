using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;

        public EfCoreCourseService(MyCourseDbContext dbContext, IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            this.coursesOptions = coursesOptions;
            this.dbContext = dbContext;
        }
        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            CourseDetailViewModel ViewModel = await dbContext.Courses
                .AsNoTracking()
                .Where(course => course.Id == id)
                .Select(course => new CourseDetailViewModel
                {
                    Id = course.Id,
                    Title = course.Title,
                    ImagePath = course.ImagePath,
                    Author = course.Author,
                    Description = course.Description,
                    Rating = course.Rating,
                    CurrentPrice = course.CurrentPrice,
                    FullPrice = course.FullPrice,
                    Lessons = course.Lessons.Select(lesson => new LessonViewModel
                    {
                        Id = lesson.Id,
                        Title = lesson.Title,
                        Description = lesson.Description,
                        Duration = lesson.Duration
                    })
                    .ToList()
                })
                //.FirstOrDefaultAsync(); è il più tollerante tra i metodi, restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
                //.FirstAsync(); FirstAsync restituisce, di un elenco, il primo elmento, sollevando un'eccezione solo se l'elenco è vuoto.
                //.SingleOrDefaultAsync(); a differenza di SingleAsync tollera il fatto che l'elenco sia vuoto (restituendo null)
                .SingleAsync(); //SingleAsync solleva invece un'eccezione se il risultato NON è 1

            return ViewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync(string search, int page)
        {
            page = Math.Max(1, page);
            int limit = coursesOptions.CurrentValue.PerPage;
            int offset = (page - 1) * limit;
            
            search = search ?? ""; //se search vale null viene sostituito dalle virgolette, in questo modo non si invalida la query
            IQueryable<CourseViewModel> queryLink = dbContext.Courses
                .Select(course => new CourseViewModel {
                    Id = course.Id,
                    Title = course.Title,
                    ImagePath = course.ImagePath,
                    Author = course.Author,
                    Rating = course.Rating,
                    CurrentPrice = course.CurrentPrice,
                    FullPrice = course.FullPrice
                })
                .Where(course => course.Title.Contains(search))
                .Skip(offset)
                .Take(limit)
                .AsNoTracking();

            List<CourseViewModel> courses = await queryLink
                .ToListAsync();

        return courses;
        }
    }
}