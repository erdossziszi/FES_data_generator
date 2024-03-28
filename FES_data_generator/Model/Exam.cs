using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Model
{
    internal class Exam
    {
        public int StudentsNr { get; set; }
        public int InstructorsNr { get; set; }
        public int DaysNr { get; set;}
        public int SlotsPerDay { get; set; }
        public int AvailabilitySlotsLenght { get; set; }
        public int RoomNr { get; set; }
        public int ProgrammNr { get; set; }
        public int DegreeNr { get; set; }
        public int RolesNr { get; set; }
        public int CoursesNr { get; set; }
        public Instructor[]? Instructors { get; set; }
        public Student[]? Students { get; set; }
        public Course[]? Courses { get; set; }
    }
}
