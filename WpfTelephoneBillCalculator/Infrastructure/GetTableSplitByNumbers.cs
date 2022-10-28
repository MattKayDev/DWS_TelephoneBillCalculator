using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using WpfTelephoneBillCalculator.Models;

namespace WpfTelephoneBillCalculator.Infrastructure
{
    public class GetTableSplitByNumbers
    {
        public static DataTable GetSplitByNumbers(List<Call> callsByNumber, string tarrifCard)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Number");
            table.Columns.Add("Total");

            //create list to hold all CLI
            List<string> listOfCLI = new List<string>();
            //get all CLI into the list
            foreach(Call call in callsByNumber)
            {
                if (!listOfCLI.Contains(call.CustomerCLI))
                    listOfCLI.Add(call.CustomerCLI);
            }


            List<List<Call>> ListOfCallsGroupedByCli = new List<List<Call>>();
            foreach(string cli in listOfCLI)
            {
                var group = callsByNumber.Where(x => x.CustomerCLI == cli).ToList();
                ListOfCallsGroupedByCli.Add(group);
            }

            string currentCli = "";
            double totalCli = 0;


            int i = 0;
            while (i <= ListOfCallsGroupedByCli.Count - 1)
            {
                foreach(var callsByCli in ListOfCallsGroupedByCli[i])
                {
                    if (currentCli != callsByCli.CustomerCLI)
                    {
                        currentCli = callsByCli.CustomerCLI;
                        totalCli = 0;
                        double salesPrice = Convert.ToDouble(callsByCli.SalesPrice) * GetCodeMultiplier.GetMultiplier(callsByCli.ChargeCode,false, tarrifCard);
                        totalCli += Convert.ToDouble(salesPrice);
                    }
                    else
                    {
                        double salesPrice = Convert.ToDouble(callsByCli.SalesPrice) * GetCodeMultiplier.GetMultiplier(callsByCli.ChargeCode, false, tarrifCard);
                        totalCli += Convert.ToDouble(salesPrice);
                    }
                }
                table.Rows.Add(currentCli, totalCli);
                i++;
            }



            double grandTotal = 0;
            foreach(DataRow row in table.Rows)
            {
                grandTotal += Convert.ToDouble(row[1]);
            }
            table.Rows.Add("Grand Total", grandTotal);

            return table;
        }

    }
}
