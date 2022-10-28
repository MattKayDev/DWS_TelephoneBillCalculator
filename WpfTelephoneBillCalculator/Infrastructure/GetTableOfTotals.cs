using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfTelephoneBillCalculator.Models;

namespace WpfTelephoneBillCalculator.Infrastructure
{
    public class GetTableOfTotals
    {
        public static DataTable GetDataTable(List<Totals> totals, DataTable dtOfCustomers, string CompanyName)
        {
            DataTable table = new DataTable();
            try
            {

            
                table.Columns.Add("Charge Code");
                table.Columns.Add("Total Seconds");
                table.Columns.Add("Total Charge");

                double totalOfSeconds = 0;
                double totalOfSalePrice = 0;
                double totalOfCostPrice = 0;

                foreach (Totals total in totals)
                {
                    table.Rows.Add(total.ChargeCode, total.TotalSeconds, total.TotalSalePrice);
                    totalOfSeconds += total.TotalSeconds;
                    totalOfSalePrice += total.TotalSalePrice;
                        totalOfCostPrice += total.TotalCostPrice;
                }
                //create grand total in this table
                dtOfCustomers.Rows.Add(CompanyName, string.Format("{0:0.00}", totalOfCostPrice), string.Format("{0:0.00}", totalOfSalePrice));
                table.Rows.Add("Grand Total", totalOfSeconds, totalOfSalePrice);
                return table;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }
    }
}
