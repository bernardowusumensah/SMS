using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sms.Models;
using System;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;


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
        public List<Teacher> ListTeachers(string SearchKey = null)
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

        /// <summary>
        /// Adds a new teacher to the system
        /// </summary>
        /// <param name="TeacherData"></param>
        /// <example>
        /// POST: api/TeacherData/AddTeacher
        /// Headers: Content-Type: application/json
        ///  Request Body:
        //   {
        //"teacherid": 19,
        //"teacherfname": "Mich",
        //"teacherlname": "Ann",
        //"employeenumber": "N012345",
        //"hiredate": "2025-04-10T21:28:50.420Z",
        //"salary": 2500
        //     }
        /// </example>
        /// <returns>
        /// The inserted Teacher Id from the database if successful. 0 if Unsuccessful
        /// </returns>

        [HttpPost(template: "AddTeacher")]
        public int AddTeacher([FromBody]Teacher TeacherData)
        {
            // 1. Check for empty names
            if (string.IsNullOrWhiteSpace(TeacherData.teacherfname) || string.IsNullOrWhiteSpace(TeacherData.teacherlname))
            {
                Console.WriteLine("Error: Teacher first name or last name cannot be empty.");
                return 0;
            }

            // 2. Check if employee number is in correct format: "T" followed by digits
            if (string.IsNullOrWhiteSpace(TeacherData.employeenumber) ||
                !System.Text.RegularExpressions.Regex.IsMatch(TeacherData.employeenumber, @"^T\d+$"))
            {
                Console.WriteLine("Error: Employee Number must start with 'T' followed by digits (e.g., T123).");
                return 0;
            }

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // CURRENT_DATE() for the author join date in this context
                // Other contexts the join date may be an input criteria!
                Command.CommandText = "insert into teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) values (@teacherfname, @teacherlname, @employeenumber, CURRENT_DATE(), @salary)";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.teacherfname);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.teacherlname);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.employeenumber);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.hiredate);
                Command.Parameters.AddWithValue("@salary", TeacherData.salary);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);

            }
            // if failure
            return 0;

        }





        /// <summary>
        /// /// Deletes a Teacher from the database
        /// </summary>
        /// <param name="teacherid">Primary key of the Teacher to delete</param>
        ///  <example>
        /// DELETE: api/TeacherData/DeleteTeacher -> 1
        /// </example>
        /// <returns>
        /// Number of rows affected by delete operation.
        /// </returns>

        [HttpDelete(template: "DeleteTeacher/{teacherid}")]
        public int DeleteTeacher(int teacherid)
        {
            try
            {
                // 'using' will close the connection after the code executes
                using (MySqlConnection Connection = _context.AccessDatabase())
                {
                    Connection.Open();

                    // Establish a new command (query) for our database
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = "DELETE FROM teachers WHERE teacherid=@teacherId";
                    Command.Parameters.AddWithValue("@teacherId", teacherid);

                    int rowsAffected = Command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine($"No teacher found with ID {teacherid}.");
                    }
                    else
                    {
                        Console.WriteLine($"Successfully deleted teacher with ID {teacherid}.");
                    }

                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"An error occurred while deleting the teacher: {ex.Message}");
                return 0;
            }
        }









        /// <summary>
        /// Updates a teacher  in the database. Data is Teacher object, request query contains ID
        /// </summary>
        /// <param name="TeacherData">Teacher Object</param>
        /// <param name="teacherid">The Teacher ID primary key</param>
        /// <example>
        /// PUT: api/Teacher/UpdateTeacher/3
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///	    "teacherfname":"Bernard",
        ///	    "teacherlname":"Owusu-Mensah",
        ///	    "employeenumber":"n01723151",
        ///	    "hiredate":"2025-04-15",
        ///	    "salary" " 7500"
        /// } -> 
        /// {
        ///     "teacherid":3,
        ///	    "teacherfname":"Bernard",
        ///	    "teacherlname":"Owusu-Mensah",
        ///	    "employeenumber":"n01723151",
        ///	     "hiredate":"2025-04-15",
        ///	      "salary" " 7500"
        /// }
        /// </example>
        /// <returns>
        /// The updated Teacher object
        /// </returns>
        [HttpPut(template: "UpdateTeacher/{teacherid}")]
        
        public ActionResult<Teacher> UpdateTeacher(int teacherid, [FromBody] Teacher TeacherData)
        {
            // Step 1: Check if teacher exists
            if (teacherid <= 0)
            {
                return BadRequest("Teacher does not exists");
            }



            // Step 2: Validate Teacher Name fields

            if (!string.IsNullOrWhiteSpace(TeacherData.teacherfname) || !string.IsNullOrWhiteSpace(TeacherData.teacherlname))
            {
                return BadRequest("Teacher first name and last name cannot be empty.");
            }

                // Step 3: Validate Salary
                if (TeacherData.salary < 0)
                {
                    return BadRequest("Salary must be a non-negative number.");
                }

                // Step 4: Validate Hire Date is not in the future
                if (TeacherData.hiredate > DateTime.Now)
                {
                    return BadRequest("Hire date cannot be in the future.");
                }

            // Step 5: Proceed to update

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // parameterize query
                Command.CommandText = "update teachers set teacherfname=@teacherfname, teacherlname=@teacherlname, employeenumber =@employeenumber, hiredate=@hiredate, salary=@salary where teacherid=@id";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.teacherfname);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.teacherlname);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.employeenumber);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.hiredate);
                Command.Parameters.AddWithValue("@salary", TeacherData.salary);

                Command.Parameters.AddWithValue("@id", teacherid);

                Command.ExecuteNonQuery();



            }

            return FindTeacher(teacherid);
        }

            
        }





    }



