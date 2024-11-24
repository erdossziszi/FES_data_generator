using System.Text.Json.Serialization;

namespace FES_data_generator.Model;

public class Student
{
    public int Id { get; set; }
    public int Programm { get; set; }
    public int Degree { get; set; }
    public int SupervisorId { get; set; }
    public int[]? CourseIds { get; set; }
    public int[]? Availability { get; set; }
    [JsonIgnore]
    public int TheoryticalMinCard { get; set; }
}
