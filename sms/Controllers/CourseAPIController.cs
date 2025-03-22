using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using sms.Models;

namespace sms.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {

        private readonly SchoolDbContext _context;
        // dependency injection of database context
        public CourseAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Returns a list of Courses in the system
        /// </summary>
        /// <example>
        /// GET api/Courses/ListCourses -> [{"courseid ":1,"coursecode":"Http 5125", "studentlname":"Bennett"},
        /// {"studentnumber":T378,"enroldate":"2016-08-05 00:00:00"]
        /// </example>
        /// <returns>
        /// A list of Students objects 
        /// </returns>
        [HttpGet]
        [Route(template: "ListCourses")]
        public List<Courses> ListCourses()
        {
            // Create an empty list of Courses
            List<Courses> Course = new List<Courses>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                //SQL QUERY
                Command.CommandText = "select * from courses";

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int CourseId = Convert.ToInt32(ResultSet["courseid"]);
                        string CourseCode = ResultSet["coursecode"].ToString();
                        int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]);
                        DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]);
                        string CourseName = ResultSet["coursename"].ToString();

                        //short form for setting all properties while creating the object
                        Courses CurrentCourse = new Courses()
                        {
                            courseid = CourseId,
                            coursecode = CourseCode,
                            teacherid = TeacherId,
                            startdate = StartDate,
                            finishdate = FinishDate,
                            coursename = CourseName,
                        };

                        Course.Add(CurrentCourse);
                    }
                }
            }
            //Return the final list of Courses
            return Course;
        }



    }
}
