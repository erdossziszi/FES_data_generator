using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Model
{
    internal class ConstraintWithParameter : Constraint
    {
        public ConstraintWithParameter(int value, bool? required, int? penalty = null) : base(required, penalty)
        {
            Value = value;
        }
        public ConstraintWithParameter(int value, int? penalty) : base(penalty)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}
