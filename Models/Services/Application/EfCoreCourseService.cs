using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyCourse.Models.Entities;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;
using MyCourse.Models.InputModels;

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

        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {
            IQueryable<Course> baseQuery = dbContext.Courses;
            
            switch(model.OrderBy)
            {
                case "Title":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Title);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Title);
                    }
                    break;
                case "Rating":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Rating);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Rating);
                    }
                    break;
                case "CurrentPrice":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.CurrentPrice.Currency);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.CurrentPrice.Currency);
                    }
                    break;
                case "Id":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Id);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Id);
                    }
                    break;
            }
            
            IQueryable<CourseViewModel> queryLink = baseQuery
                .Select(course => new CourseViewModel {
                    Id = course.Id,
                    Title = course.Title,
                    ImagePath = course.ImagePath,
                    Author = course.Author,
                    Rating = course.Rating,
                    CurrentPrice = course.CurrentPrice,
                    FullPrice = course.FullPrice
                })
                .Where(course => course.Title.Contains(model.Search))
                .AsNoTracking();

            List<CourseViewModel> courses = await queryLink
                .Skip(model.Offset)
                .Take(model.Limit)
                .ToListAsync();

            int TotalCount = await queryLink.CountAsync();

            ListViewModel<CourseViewModel> result = new ListViewModel<CourseViewModel>
            {
                Results = courses,
                TotalCount = TotalCount
            };

            return result;
        }
    }
}