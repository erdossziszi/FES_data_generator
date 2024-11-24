using System.Text.Json.Serialization;

namespace FES_data_generator.Model;

public class Exam
{
    public int NumberOfStudents { get; set; }
    public int NumberOfInstructors { get; set; }
    public int NumberOfDays { get; set;}
    public int SlotsPerDay { get; set; }
    public int AvailabilitySlotsLenght { get; set; }
    public int NumberOfRooms { get; set; }
    public int NumberOfProgrammes { get; set; }
    public int NumberOfDegrees { get; set; }
    public int NumberOfRoles { get; set; }
    public int NumberOfCourses { get; set; }
    public Instructor[] Instructors { get; set; } = [];
    [JsonIgnore]
    public List<int>[,]? InstructorRolesPerProgramme { get; set; }
    public Student[] Students { get; set; } = [];
    public Course[] Courses { get; set; } = [];
}
