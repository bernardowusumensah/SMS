using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sms.Models;
using System;
using MySql.Data.MySqlClient;


namespace sms.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {

        private readonly SchoolDbContext _context;
        // dependency injection of database context
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }
        /// <summary>
        ///  Returns a list of Teachers in the system
        /// </summary>
        /// <example>
        /// GET api/Teacher/ListTeachers -> {"teacherid":1,"teacherfname":"Alexander", "teacherlname":"Bennett"},
        /// {"employeenumber":T378,"hiredate":"2016-08-05 00:00:00", "salary":"55.30"},
        /// </example>
        /// <returns>
        /// A list of Teacher objects 
        /// </returns>
        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            // Create an empty list of Teachers
            List<Teacher> Teachers = new List<Teacher>();


            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                //SQL QUERY
                Command.CommandText = "select * from teachers";

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {

                        //Access Column information by the DB column name as an index
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();


                        //short form for setting all properties while creating the object
                        Teacher CurrentTeacher = new Teacher()
                        {
                            teacherid = Id,
                            teacherfname = FirstName,
                            teacherlname = LastName,
                           
                        };

                        Teachers.Add(CurrentTeacher);



                    }
                }
            }
            //Return the final list of Teachers
            return Teachers;
        }

        /// <summary>
        /// A specific user based on id's in the system
        /// </summary>
        /// <example> GET api/Teacher/FindTeacher/5 ->  {"teacherid":5,"teacherfname":"Jessica","teacherlname":"Morris","employeenumber":"T389","hiredate":"2012-06-04T00:00:00","salary":48.62}
        /// </example>
        /// <param name="id"></param>
        /// <returns>
        /// A specificied user by its id
        /// </returns>

        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            // create an empty teacher object or instantiate the teacher Model
            Teacher SearchTeacher = new Teacher();

            // 'using' will close the connection after the code executes
            using (MySqlConnection connection = _context.AccessDatabase())
            {

                connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = connection.CreateCommand();
                // SQL QUERY to fetch a specific teacher
                Command.CommandText = "SELECT * FROM teachers WHERE teacherid = @teacherId";
                // @id is replaced with a 'sanitized' id
                Command.Parameters.AddWithValue("@teacherId", id);


                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {


                    if (ResultSet.HasRows)
                    {

                        //Loop Through Each Row the Result Set
                        while (ResultSet.Read())
                        {
                            // Access Column information by the DB column name as an index
                            int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                            string FirstName = ResultSet["teacherfname"].ToString();
                            string LastName = ResultSet["teacherlname"].ToString();
                            string EmployeeNumber = ResultSet["employeenumber"].ToString();
                            DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                            decimal Salary = Convert.ToDecimal(ResultSet["salary"]);


                            SearchTeacher.teacherid = TeacherId;
                            SearchTeacher.teacherfname = FirstName;
                            SearchTeacher.teacherlname = LastName;
                            SearchTeacher.employeenumber = EmployeeNumber;
                            SearchTeacher.hiredate = HireDate;
                            SearchTeacher.salary = Salary;

                        }

                    }
                    else {
                        Console.WriteLine("Error: Teacher not found! Returning a default teacher object.");

                        // Return a default teacher object with empty values
                        SearchTeacher = new Teacher
                        {
                            teacherid = 0,
                            teacherfname = "Not Found",
                            teacherlname = "Not Found",
                            employeenumber = "N/A",
                            hiredate = DateTime.MinValue,
                            salary = 0
                        };
                    }


                }
            }




            //Return the final search of teacher
            return SearchTeacher;

        }
    }
}
