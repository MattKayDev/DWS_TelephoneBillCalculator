using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfTelephoneBillCalculator.Models;

namespace WpfTelephoneBillCalculator.Infrastructure
{
    public class GetTotalNames
    {
        public static DataTable GetNames(DataTable totals)
        {
            try
            {

                //get list of names to work with
                List<ChargeCode> names = new List<ChargeCode>();
                using(var con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "data source=data.mdb;"))
                {
                    con.Open();
                    var query = "SELECT * FROM tblTarrifNames";
                    var command = new OleDbCommand(query, con); 
                    var reader  = command.ExecuteReader();
                    while (reader.Read())
                        names.Add(new ChargeCode() { Code = reader[0].ToString(), Name = reader[1].ToString() });
                }

                foreach(DataRow row in totals.Rows)
                {
                    string nameOfCode = names.Where(x => x.Code == row[0].ToString()).First().Name; 
                    row[0] = nameOfCode;
                }

                return totals;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }
    }
}
