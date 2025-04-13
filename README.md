# ASP.NET Core School Management System Application
This example connects our server to a MySQL Database with MySql.Data.MySqlClient.

- Models/SchoolDbContext.cs
    - A class which represents the connection to the database. Be mindful of the connection string fields!
- Controllers/TeacherAPIController.cs
- Controllers/StudentsAPIController.cs
- Controllers/CourseApiController.cs
    - An API Controller which allows us to access information about Teachers, Students and Courses
- Program.cs
    - Configuration of the application
 

# NEW IMPLEMENTATION
- An API which adds a Teacher
- An API which deletes a Teacher
- A web page that allows a user to enter new T eacher information
- A web page that confirms the action to delete a T eacher

 
    - 
## Testing:
Navigate through to sms/add-delete-testing to see various API endpoint test using cURL and MVC as well


## Models:
Three Basic Models comprising of Students, Courses and Teachers that makes up the School Management System

## Tools:
.net Core API, .net Core MVC, Mysql on Xampp, Swagger 
