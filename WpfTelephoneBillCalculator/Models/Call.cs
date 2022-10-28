using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTelephoneBillCalculator.Models
{
    public class Call
    {
        public string CustomerCLI { get; set; }
        public string CallDate { get; set; }
        public string CallTime { get; set; }
        public string Duration { get; set; }
        public string SalesPrice { get; set; }
        public string ChargeCode {get; set;}


    }
}
