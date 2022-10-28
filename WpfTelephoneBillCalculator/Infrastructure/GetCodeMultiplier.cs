using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfTelephoneBillCalculator.Models;

namespace WpfTelephoneBillCalculator.Infrastructure
{
    public class GetCodeMultiplier
    {
        public static double GetMultiplier(string Code, bool showError, string TarrifCard)
        {
            try
            {
                List<ChargeCode> codes = new List<ChargeCode>();
                double multi = 1.5;
                using (var con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "data source=data.mdb;"))
                {
                    con.Open();
                    var query = "SELECT Code, Standard, Sell1 FROM tblTarrifCard" + TarrifCard;
                    var command = new OleDbCommand(query, con);
                    var reader = command.ExecuteReader();
                
                    while (reader.Read())
                    {
                        double BuyAt = Convert.ToDouble(reader[1].ToString());
                        double SellAt = Convert.ToDouble(reader[2].ToString());
                        multi = SellAt / BuyAt;
                        if (BuyAt == 0)
                        {
                            multi = 2;
                        }
                        string cod = reader[0].ToString();
                        codes.Add(new ChargeCode() { Code = reader[0].ToString(), Multiplier = multi });

                    }
                }

                double multiplier = 0;
                double? _multiplier = codes.Where(x => x.Code == Code).First().Multiplier;
                var co = codes.Where(x => x.Code == Code).First();
                multiplier = Convert.ToDouble(_multiplier);

                if (multiplier != 0)
                {
                    return multiplier;
                }
                else
                    return 2;

            }
            catch (Exception ex)
            {
                if(showError)
                {
                    if (ex.Message == "Sequence contains no elements")
                    {
                        MessageBox.Show(Code + " is missing from the database. It needs to be added to make correct calculations. For now default multiplier of x2 has been applied.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return 2;
                    }
                    else
                    {
                        MessageBox.Show(ex.Message + " At this time default multiplier of x2 has been applied.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return 2;
                    }
                }
                else
                {
                    return 2;
                }
               
            }
        }
    }
}
