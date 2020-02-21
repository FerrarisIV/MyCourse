using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Controllers;

namespace MyCourse.Models.InputModels
{
    public class CourseCreateInputModel
    {
        [Required(ErrorMessage = "Obbligatorio"),
        MinLength(10),
        MaxLength(100),
        RegularExpression(@"^[\w\s\.]+$"),
        Remote(action: nameof(CoursesController.IsTitleAvailable), controller: "Courses", ErrorMessage = "Titolo esistente") ]
        public string Title { get; set; }
    }
}