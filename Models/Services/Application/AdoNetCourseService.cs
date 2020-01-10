using System.Collections.Generic;
using System.Data;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application {
    public class AdoNetCourseService : ICourseService {
        public IDatabaseAccessor Db { get; }
        public AdoNetCourseService (IDatabaseAccessor db) {
            this.Db = db;

        }
        CourseDetailViewModel ICourseService.GetCourse (int id) {
            throw new System.NotImplementedException ();
        }

        List<CourseViewModel> ICourseService.GetCourses () {

            string query = "SELECT Id, Title, ImagePath, Rating, Author, FullPrice_Amount, CurrentPrice_Amount, "
                + "FullPrice_Currency, CurrentPrice_Currency FROM Courses";
            DataSet dataSet = Db.Query(query);
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