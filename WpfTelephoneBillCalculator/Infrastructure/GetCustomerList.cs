using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfTelephoneBillCalculator.Models;

namespace WpfTelephoneBillCalculator.Infrastructure
{
    public class GetCustomerList
    {



        public static List<Customer> GetCustomers()
        {
            try
            {

                    
            using (var con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "data source=data.mdb;"))
            {
                List<string> CustomerNames = new List<string>();
                con.Open();
                var SelectAll = "SELECT * FROM tblCustomerNumbers";
                var command = new OleDbCommand(SelectAll, con);
                var reader = command.ExecuteReader();

                string currentCust = "";
                while (reader.Read())
                {
                    CustomerNames.Add(reader[0].ToString());
                }
                reader.Close();

                CustomerNames.Sort();
                List<string> templist = new List<string>();
                string temp = "";
                foreach(string customerName in CustomerNames)
                {
                    if (temp == "")
                    {
                        templist.Add(customerName);
                        temp = customerName;
                    }
                    else
                    {
                        if(temp != customerName)
                        {
                            temp = customerName;
                            templist.Add(customerName);
                        }
                        
                    }
                }
                CustomerNames = templist;

                List<Customer> Customers = new List<Customer>();

                foreach(string customerName in CustomerNames)
                {
                    var Query = "SELECT Number FROM tblCustomerNumbers WHERE Customer='"+ customerName +"'";
                    command.CommandText = Query;
                    reader = command.ExecuteReader();
                    List<string> numberOut = new List<string>();
               
                    while (reader.Read())
                    {
                        numberOut.Add(reader[0].ToString());
                    }
                    reader.Close();
                    Customers.Add(new Customer() { Name = customerName, TelephoneNumbers = numberOut });

                }

                if (Customers != null)
                    return Customers;

               



            }


            return new List<Customer>();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Customer>();
            }
        }

    }
}
