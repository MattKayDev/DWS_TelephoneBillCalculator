using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTelephoneBillCalculator.Models
{
    public class ChargeCode
    {
        public string Code { get; set; }
        public string? Name { get; set; }
        public double? Multiplier { get; set;}
    }
}
