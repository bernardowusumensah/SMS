using Microsoft.AspNetCore.Mvc;
using sms.Models;

namespace sms.Controllers
{
    public class CoursePageController : Controller
    {
        // currently relying on the API to retrieve Courses information
        // this is a simplified example. In practice, both the StudentAPI and StudentPage controllers
        // should rely on a unified "Service", with an explicit interface

        private readonly CourseAPIController _api;


        public CoursePageController(CourseAPIController api)
        {
            _api = api;
        }



        //GET : CoursePage/List

    
        public IActionResult List()
        {
            List<Courses> Course = _api.ListCourses();
            return View(Course);
        }

    }



    //public IActionResult Index()
    //{
    //    return View();
    //}
}

