using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Model
{
    internal class Instructor
    {
        public int Id { get; set; }
        public int[]? Programm { get; set; }
        public int[]? Roles { get; set; }
        public int[]? Availability { get; set; }


    }
}
