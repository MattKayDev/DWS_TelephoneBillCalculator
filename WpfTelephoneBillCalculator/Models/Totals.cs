using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTelephoneBillCalculator.Models
{
    public class Totals
    {
        public string ChargeCode { get; set; }
        public double TotalSeconds { get; set; }
        public double TotalCostPrice { get; set; }
        public double TotalSalePrice { get; set; }

    }
}
