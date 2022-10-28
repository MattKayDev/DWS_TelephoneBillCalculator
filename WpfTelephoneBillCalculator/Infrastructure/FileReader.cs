using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfTelephoneBillCalculator.Models;

namespace WpfTelephoneBillCalculator.Infrastructure
{
    public class FileReader
    {
        public FileReader()
        {

        }

        public List<Call> CreateListFromCSV(string fileName)
        {
            try
            {
                List<Call> list = new List<Call>();
                string[] tokens;
                char[] separators = { ',' };
                string str = "";

                FileStream fs = new FileStream(fileName, FileMode.Open);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                while ((str = sr.ReadLine()) != null)
                {
                    tokens = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    int iMax = tokens.Count() -1;
                    int i = 0;
                    foreach(string token in tokens)
                    {

                        char[] ReservedChars = { '\\', '\"'};

                        foreach (char strChar in ReservedChars)
                        {
                            if (i <= iMax)
                            {
                                string tokenFixed = token.Replace(strChar.ToString(), String.Empty);
                                if (tokenFixed != String.Empty)
                                {
                                    tokenFixed.Remove(0);
                                    int lastIndex = tokenFixed.Length;
                                    tokenFixed.Remove(lastIndex - 1);
                                    tokens[i] = tokenFixed;
  
                                }
                    
                            }
                        }
                        i++;
                    }
                    if (tokens[0] != "Call Type")
                    {
                        Call call;
                        if (tokens[1].Length == 8)
                        {
                            call = new Call()
                            {
                                CustomerCLI = tokens[1],
                                CallDate = tokens[3],
                                CallTime = tokens[4],
                                Duration = tokens[5],
                                SalesPrice = tokens[8],
                                ChargeCode = tokens[11]
                            };
                        }
                        else
                        {
                            call = new Call()
                            {
                                CustomerCLI = tokens[1],
                                CallDate = tokens[3],
                                CallTime = tokens[4],
                                Duration = tokens[5],
                                SalesPrice = tokens[8],
                                ChargeCode = tokens[10]
                            };
                        }
                        list.Add(call);
                    }
                }
                sr.Close();
                fs.Close();
                return list;
            }
            catch (Exception ex)
            {
                if(ex.Message == "Empty path name is not legal. (Parameter 'path')")
                {
                    MessageBox.Show("Please select CSV file to get started!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<Call>();
                }else
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<Call>();
                }
                
            }
        }
    }
}
