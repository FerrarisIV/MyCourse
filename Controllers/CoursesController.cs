using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return Content("sono la index di MyCourse");
        }

         public IActionResult Details(string id)
         {
             return Content($"Sono qui in Detail e questo è il mio id: {id}");
         } 
    }
}