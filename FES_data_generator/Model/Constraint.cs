using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Model
{
    internal class Constraint
    {
        public Constraint(bool required, int param)
        {
            Required = required;
            SingleParam = param;
        }
        public Constraint(bool required, int[] param)
        {
            Required = required;
            ArrayParam = param;
        }
        public Constraint(bool required, Dictionary<int, int> param)
        {
            Required = required;
            DictParam = param;
        }
        public Constraint(bool required, Dictionary<int, int[]> param)
        {
            Required = required;
            ArrayDictParam = param;
        }
        public bool? Required { get; set; }
        public int? SingleParam { get; set; }
        public int[]? ArrayParam { get; set; }
        public Dictionary<int, int>? DictParam { get; set; }
        public Dictionary<int, int[]>? ArrayDictParam { get; set; }

    }
}
