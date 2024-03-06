using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Model
{
    internal class Student
    {
        public int Id { get; set; }
        public int Programm { get; set; }
        public int Degree { get; set; }
        public int SupervisorId { get; set; }
        public int[]? CourseIds { get; set; }
        public bool[]? Availability { get; set; }
    }
}
