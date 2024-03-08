using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Model
{
    internal class Constraint
    {
        public Constraint(int? penalty)
        {
            Penalty = penalty;
        }

        public Constraint(bool? required, int? penalty = null)
        {
            Required = required;
            Penalty = penalty;
        }

        public bool? Required { get; set; }
        public int? Penalty { get; set; }

    }
}
