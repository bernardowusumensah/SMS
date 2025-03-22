using Microsoft.AspNetCore.Mvc;
using sms.Models;

namespace sms.Controllers
{
    public class StudentsPageController : Controller
    {

        // currently relying on the API to retrieve Student information
        // this is a simplified example. In practice, both the StudentAPI and StudentPage controllers
        // should rely on a unified "Service", with an explicit interface

        private readonly StudentsAPIController _api;

        public StudentsPageController(StudentsAPIController api)
        {
            _api = api;
        }

        //GET : StudentsPage/List
        public IActionResult List()
        {

            List<Students> Students = _api.ListStudents();
            return View(Students);

         
        }



        // GET : StudentPage/Show/{id}
        public IActionResult Show(int id)
        {
            Students SearchStudent = _api.FindStudent(id);
            
            return View(SearchStudent);
        }
    }
}
