
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

        ////GET : AuthorPage/List/SearchKey={SearchKey}
        //[HttpGet]
        //public IActionResult List(string SearchKey)
        //{
        //    List<Teacher> Teachers = _api.ListTeachers(SearchKey);
        //    return View(Teachers);
        //}


        //GET : TeacherPage/List/SearchKey={SearchKey}
        [HttpGet]
        public IActionResult List(string SearchKey)
        {
            List<Teacher> Teachers = _api.ListTeachers(SearchKey);
            return View(Teachers);
        }


     

        // GET : TeacherPage/Show/{id}
        public IActionResult Show(int id)
        {
            Teacher SearchTeacher = _api.FindTeacher(id);
          //  return View("~/Views/TeacherPage/Show.cshtml",SearchTeacher);
            return View(SearchTeacher);
        }


        // GET : TeacherPage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View();
        }


        // POST: TeacherPage/Create
        [HttpPost]
        public IActionResult Create(Teacher NewTeacher)
        {
            int TeacherId = _api.AddTeacher(NewTeacher);

            // redirects to "Show" action on "Teacher" cotroller with id parameter supplied
            return RedirectToAction("Show", new { id = TeacherId });
        }



        // GET : TeacherPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }


        // POST: TeacherPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int AuthorId = _api.DeleteTeacher(id);
            // redirects to list action
            return RedirectToAction("List");
        }


        // GET : TeacherPage/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        // POST: TeacherPage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, string teacherfname, string teacherlname, string employeenumber, DateTime hiredate, decimal salary)
        {
            Teacher UpdatedTeacher = new Teacher();
            UpdatedTeacher.teacherfname = teacherfname;
            UpdatedTeacher.teacherlname = teacherlname;
            UpdatedTeacher.employeenumber = employeenumber;
            UpdatedTeacher.hiredate = hiredate;
            UpdatedTeacher.salary = salary;

            // not doing anything with the response
            _api.UpdateTeacher(id, UpdatedTeacher);
            // redirects to show author
            return RedirectToAction("Show", new { id = id });
        }



    }
}
