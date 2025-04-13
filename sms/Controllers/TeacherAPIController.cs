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
        public int AddTeacher([FromBody] Teacher TeacherData)
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

            try
            {
                using (MySqlConnection connection = _context.AccessDatabase())
                {
                    connection.Open();

                    // 3. Check if employee number already exists
                    MySqlCommand checkCommand = connection.CreateCommand();
                    checkCommand.CommandText = "SELECT COUNT(*) FROM teachers WHERE employeenumber = @empNo";
                    checkCommand.Parameters.AddWithValue("@empNo", TeacherData.employeenumber);

                    long count = (long)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        Console.WriteLine($"Error: Employee Number '{TeacherData.employeenumber}' is already taken.");
                        return 0;
                    }

                    // 4. Insert new teacher
                    MySqlCommand insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = @"
                INSERT INTO teachers (teacherfname, teacherlname, employeenumber, salary, hiredate)
                VALUES (@teacherfname, @teacherlname, @employeenumber, @salary, CURRENT_DATE())";

                    insertCommand.Parameters.AddWithValue("@teacherfname", TeacherData.teacherfname);
                    insertCommand.Parameters.AddWithValue("@teacherlname", TeacherData.teacherlname);
                    insertCommand.Parameters.AddWithValue("@employeenumber", TeacherData.employeenumber);
                    insertCommand.Parameters.AddWithValue("@salary", TeacherData.salary);

                    insertCommand.ExecuteNonQuery();

                    Console.WriteLine("Teacher successfully added.");
                    return Convert.ToInt32(insertCommand.LastInsertedId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the teacher: {ex.Message}");
                return 0;
            }
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







    }
}

