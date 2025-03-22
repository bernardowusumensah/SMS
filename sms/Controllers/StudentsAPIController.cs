using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using sms.Models;

namespace sms.Controllers
{
    [Route("api/Students")]
    [ApiController]
    public class StudentsAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;
        // dependency injection of database context

        public StudentsAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Returns a list of Students in the system
        /// </summary>
        /// <example>
        /// GET api/Students/ListStudents -> [{"studentid ":1,"studentfname":"Alexander", "studentlname":"Bennett"},
        /// {"studentnumber":T378,"enroldate":"2016-08-05 00:00:00"]
        /// </example>
        /// <returns>
        /// A list of Students objects 
        /// </returns>
        [HttpGet]
        [Route(template: "ListStudents")]
        public List<Students> ListStudents()
        {
            // Create an empty list of Students
            List<Students> StudentsInSms = new List<Students>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                //SQL QUERY
                Command.CommandText = "select * from students";

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int Id = Convert.ToInt32(ResultSet["studentid"]);
                        string FirstName =  ResultSet["studentfname"].ToString();
                        string LastName =   ResultSet["studentlname"].ToString();
                        string Number =     ResultSet["studentnumber"].ToString();
                        DateTime Enrolment = Convert.ToDateTime(ResultSet["enroldate"]);

                        //short form for setting all properties while creating the object
                        Students CurrentStudent = new Students()
                        {
                            studentid = Id,
                            studentfname = FirstName,
                            studentlname = LastName,
                            studentnumber = Number,
                            enroldate = Convert.ToDateTime(Enrolment),
                        };

                        StudentsInSms.Add(CurrentStudent);
                    }
                }
            }
            //Return the final list of Students
            return StudentsInSms;
        }

        [HttpGet]
        [Route(template: "FindStudent/{id}")]

        public Students FindStudent(int id)
        {
            //   instantiate the Students Model
            Students SearchStudent = new Students();

            // 'using' will close the connection after the code executes
            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = connection.CreateCommand();
                // SQL QUERY to fetch a specific student
                Command.CommandText = "SELECT * FROM students WHERE studentid  = @studentid";
                // @id is replaced with a 'sanitized' id
                Command.Parameters.AddWithValue("@studentid", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        // Access Column information by the DB column name as an index
                        int StudentId = Convert.ToInt32(ResultSet["studentid"]);
                        string FirstName = ResultSet["studentfname"].ToString();
                        string LastName = ResultSet["studentlname"].ToString();
                        string StudentNumber = ResultSet["studentnumber"].ToString();
                        DateTime EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]);

                        SearchStudent.studentid = StudentId;
                        SearchStudent.studentfname = FirstName;
                        SearchStudent.studentlname = LastName;
                        SearchStudent.studentnumber = StudentNumber;
                        SearchStudent.enroldate = EnrolDate;
                    }
                }
            }
            //Return the final search of student
            return SearchStudent;
        }
    }
    }
