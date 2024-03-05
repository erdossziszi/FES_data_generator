using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Model
{
    internal class Course
    {
        public int Id { get; set; }
        public Instructor[]? Instructors { get; set; }
    }
}
