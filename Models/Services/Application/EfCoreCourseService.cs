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
using MyCourse.Models.Exceptions;
using Microsoft.Data.Sqlite;
using System.Linq.Dynamic.Core;

namespace MyCourse.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;
        private readonly IImagePersister imagePersister;

        public EfCoreCourseService(MyCourseDbContext dbContext, IImagePersister imagePersister, IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            this.imagePersister = imagePersister;
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

        public async Task<List<CourseViewModel>> GetBestRatingCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderby: "Rating",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order);

            ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
            return result.Results;
        }

        public async Task<List<CourseViewModel>> GetMostRecentCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderby: "Id",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order);

            ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
            return result.Results;
        }

        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {
            string orderby = model.OrderBy;
            if (orderby == "CurrentPrice")
            {
                orderby = "CurrentPrice.Amount";
            }
            string direction = model.Ascending ? "asc" : "desc";

            IQueryable<Course> baseQuery = dbContext.Courses.OrderBy($"{orderby} {direction}");

            // model.OrderBy model.Ascending


            IQueryable<CourseViewModel> queryLink = baseQuery
                .Select(course => new CourseViewModel
                {
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

        public async Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            string title = inputModel.Title;
            string author = "Paolo Rossi";

            var course = new Course(title, author);
            dbContext.Add(course);
            await dbContext.SaveChangesAsync();

            return CourseDetailViewModel.FromEntity(course);
        }

        public async Task<CourseEditInputModel> GetCourseForEditingAsync(int id)
        {
            IQueryable<CourseEditInputModel> queryLinq = dbContext.Courses
                .AsNoTracking()
                .Where(course => course.Id == id)
                .Select(course => CourseEditInputModel.FromEntity(course)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            CourseEditInputModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                //Logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<CourseDetailViewModel> EditCourseAsync(CourseEditInputModel inputModel)
        {
            Course course = await dbContext.Courses.FindAsync(inputModel.Id);

            course.ChangeTitle(inputModel.Title);
            course.ChangePrices(inputModel.FullPrice, inputModel.CurrentPrice);
            course.ChangeDescription(inputModel.Description);
            course.ChangeEmail(inputModel.Email);

            string imagePath = await imagePersister.SaveCourseImageAsync(inputModel.Id, inputModel.Image);
            course.ChangeImagePath(imagePath);

            //dbContext.Update(course);

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exc) when ((exc.InnerException as SqliteException)?.SqliteErrorCode == 19)
            {
                throw new CourseTitleUnavailableException(inputModel.Title, exc);
            }

            return CourseDetailViewModel.FromEntity(course);
        }

        public async Task<bool> IsTitleAvailableAsync(string title, int id)
        {
            bool titleExists = await dbContext.Courses.AnyAsync(course => EF.Functions.Like(course.Title, title) && course.Id != id);
            return !titleExists;
        }
    }
}