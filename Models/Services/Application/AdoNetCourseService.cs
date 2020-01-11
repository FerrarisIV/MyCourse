using System;
using System.Collections.Generic;
using System.Data;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application {
    public class AdoNetCourseService : ICourseService {
        public IDatabaseAccessor db { get; }
        public AdoNetCourseService (IDatabaseAccessor db) {
            this.db = db;

        }
        CourseDetailViewModel ICourseService.GetCourse (int id) {

            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Rating, Author, FullPrice_Amount, CurrentPrice_Amount,
                FullPrice_Currency, CurrentPrice_Currency
                FROM Courses WHERE Id = {id};
                SELECT Id, Title, Description, Duration
                FROM Lessons WHERE CourseId = {id}";

            DataSet dataSet = db.Query(query);
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1 )
            {
                throw new InvalidOperationException($"Il risultato deve essere 1, invece è {id}");
            }
            var courseRow = courseTable.Rows[0];
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);
            
            var lessonDataTable = dataSet.Tables[1];
            foreach(DataRow lessonRow in lessonDataTable.Rows)
            {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                courseDetailViewModel.Lessons.Add(lessonViewModel);
            }

            return courseDetailViewModel;



        }

        List<CourseViewModel> ICourseService.GetCourses () {

            FormattableString query = $@"SELECT Id, Title, ImagePath, Rating, Author, FullPrice_Amount, 
                CurrentPrice_Amount, FullPrice_Currency, CurrentPrice_Currency
                FROM Courses";
            DataSet dataSet = db.Query(query);
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