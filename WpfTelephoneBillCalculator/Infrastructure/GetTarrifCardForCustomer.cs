using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WpfTelephoneBillCalculator.Models;

namespace WpfTelephoneBillCalculator.Infrastructure
{
    public class GetTarrifCardForCustomer
    {
        public static string GetCard(string customerName)
        {
            string cardName = "";

            using (var con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "data source=data.mdb;"))
            {
                con.Open();
                var query = "SELECT TarrifCard FROM tblCustomers WHERE Customer = '"+ customerName + "'";
                var command = new OleDbCommand(query, con);
                var card = command.ExecuteScalar();
                if (card != null)
                    cardName = card.ToString();
                else
                    cardName = "Template";
            }
            return cardName;
        }

    }
}
