
using Microsoft.AspNetCore.Mvc;
using sms.Models;

namespace sms.Controllers
{
    public class TeacherPageController : Controller
    {
        // currently relying on the API to retrieve Teacher information
        // this is a simplified example. In practice, both the TeacherAPI and TeacherPage controllers
        // should rely on a unified "Service", with an explicit interface
        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }


        //GET : TeacherPage/List
        public IActionResult List()
        {
            List<Teacher> Teachers = _api.ListTeachers();
            return View(Teachers);
        }


     

        // GET : TeacherPage/Show/{id}
        public IActionResult Show(int id)
        {
            Teacher SearchTeacher = _api.FindTeacher(id);
          //  return View("~/Views/TeacherPage/Show.cshtml",SearchTeacher);
            return View(SearchTeacher);
        }

    }
}
