using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        public IDatabaseAccessor db { get; }
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;
        public AdoNetCourseService(IDatabaseAccessor db, IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            this.coursesOptions = coursesOptions;
            this.db = db;
        }
        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {

            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Rating, Author, FullPrice_Amount, CurrentPrice_Amount,
                FullPrice_Currency, CurrentPrice_Currency
                FROM Courses WHERE Id = {id};
                SELECT Id, Title, Description, Duration
                FROM Lessons WHERE CourseId = {id}";

            DataSet dataSet = await db.QueryAsync(query);
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1)
            {
                throw new InvalidOperationException($"Il risultato deve essere 1, invece è {id}");
            }
            var courseRow = courseTable.Rows[0];
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

            var lessonDataTable = dataSet.Tables[1];
            foreach (DataRow lessonRow in lessonDataTable.Rows)
            {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                courseDetailViewModel.Lessons.Add(lessonViewModel);
            }

            return courseDetailViewModel;



        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {

            FormattableString query = $@"SELECT Id, Title, ImagePath, Rating, Author, FullPrice_Amount, 
                CurrentPrice_Amount, FullPrice_Currency, CurrentPrice_Currency
                FROM Courses";
            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0];
            var courseList = new List<CourseViewModel>();

            foreach (DataRow courseRow in dataTable.Rows)
            {
                CourseViewModel course = CourseViewModel.FromDataRow(courseRow);
                courseList.Add(course);
            }
            return courseList;
        }
    }
}