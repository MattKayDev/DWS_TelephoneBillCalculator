using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfTelephoneBillCalculator.Infrastructure;
using WpfTelephoneBillCalculator.Models;

namespace WpfTelephoneBillCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Call> allCalls;
        public string SelectedCustomer = "";
        public List<string> ListOfCustomerNames = new List<string>();

        List<Customer> Customers = new List<Customer>();

        DataTable tblCustomers = new DataTable();

        public MainWindow()
        {
            
            InitializeComponent();
            //set binding
            DataContext = this;
            
            //get the list of customers
            Customers = GetCustomerList.GetCustomers();
            //grab customer names out of the list
            foreach (Customer customer in Customers)
            {
                ListOfCustomerNames.Add(customer.Name);
            }
            //display customer list on combobox
            listOfCustomers.ItemsSource = ListOfCustomerNames;
            listOfCustomers.SelectedIndex = 0;

            listOfCustomers.IsEnabled = false;
            btnRun.IsEnabled = false;
            chbSplitNumbers.IsEnabled = false;
            btnRunAll.IsEnabled = false;


            tblCustomers.Columns.Add("Customer");
            tblCustomers.Columns.Add("Cost Price");
            tblCustomers.Columns.Add("Sale Price");

            //foreach(string customer in ListOfCustomerNames)
            //{
            //    tblCustomers.Rows.Add(customer, "", "");
            //}
            gridCustomers.ItemsSource = tblCustomers.AsDataView();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "CSV Files (*.csv)|*.csv";
                openFile.ShowDialog();
                if(openFile.FileName != null)
                {
                    txtFileName.Text = openFile.FileName;
                }
                FileReader fr = new FileReader();
                //Get list of all calls
                allCalls = fr.CreateListFromCSV(txtFileName.Text);
                if(allCalls.Count > 0)
                {
                    listOfCustomers.IsEnabled = true;
                    btnRun.IsEnabled = true;
                    chbSplitNumbers.IsEnabled = true;
                    btnRunAll.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RunCalculations(listOfCustomers.SelectedValue.ToString(),"",true, false);
        }

        private void btnRunAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will run calculations for all customers. \n It will not do the number seperation. \n It will not show warning about missing Tarrif Codes and default multiplier of x2 will be applied. \n Do you want to continue?", "Run all?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = folderBrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    bool completed = false;
                    foreach (string customer in listOfCustomers.Items)
                    {
                        if(RunCalculations(customer, folderBrowser.SelectedPath, false, true))
                        {
                            completed = true;
                            MessageBox.Show("If you need to run bills with number seperation, delete the bills for the said customer from " + folderBrowser.SelectedPath + " . After you do that tick Split Numbers and then RUN (not RUN ALL). ", "Number seperation", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    if(completed)
                    {
                        MessageBox.Show("All bills have been generated!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("There was an issue generating the bills", "FAILURE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            else
            {
                MessageBox.Show("No calcuations will be ran at this point", "No calc", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool RunCalculations(string selectedCustomer, string preSelectedFolderPath, bool displayErrors, bool runAll)
        {
            try
            {
                lblPleaseWait.Visibility = Visibility.Visible;
                //init list of calls by customer
                List<Call> callsByCustomer = new List<Call>();
                //init customer object
                Customer customer;
                string tarrifCard = GetTarrifCardForCustomer.GetCard(selectedCustomer);

                if (selectedCustomer != string.Empty) //check if list of customers combo box is not empty
                {
                    //get first customer where name = selected value in combobox
                    customer = Customers.Where(x => x.Name == selectedCustomer).ToList().First();
                    foreach (string number in customer.TelephoneNumbers)
                    {
                        //for each number in array of number
                        //add range (each time it's a IEnurmerable<list>) of calls

                        string CLICorrection = "";
                        if (number.Length == 8)
                            CLICorrection = number;
                        else
                            CLICorrection = "0" + number;

                        List<Call>? filterCalls = allCalls.Where(x => x.CustomerCLI == CLICorrection).ToList();
                        if (filterCalls != null)
                            callsByCustomer.AddRange(filterCalls);
                    }
                }

                //sort calls by Charge code
                callsByCustomer = callsByCustomer.OrderBy(x => x.ChargeCode).ToList();

                //list of charge codes?
                List<string> chargeCodes = new List<string>();
                //create list of charge codes
                foreach (Call call in callsByCustomer)
                {
                    //if list doesnt contain current charge code add it
                    if (!chargeCodes.Contains(call.ChargeCode))
                        chargeCodes.Add(call.ChargeCode);
                }

                //create list of to hold lists of calls based on charge code
                List<List<Call>> listOfCallsByChargeCode = new List<List<Call>>();
                foreach (string code in chargeCodes)
                {
                    //grab calls based on charge code to add the the list
                    List<Call> callsWithCurrentCode = callsByCustomer.Where(x => x.ChargeCode == code).ToList();
                    //add those calls to the list
                    listOfCallsByChargeCode.Add(callsWithCurrentCode);
                }

                List<Totals> listOfTotals = new List<Totals>();
                int i = 0;
                while (i <= listOfCallsByChargeCode.Count - 1)
                {
                    string code = listOfCallsByChargeCode[i].First().ChargeCode;
                    //create init total for current charge code
                    Totals total = new Totals() { ChargeCode = code, TotalCostPrice = 0, TotalSalePrice = 0, TotalSeconds = 0 };
                    //loop through calls in current list
                    foreach (Call call in listOfCallsByChargeCode[i])
                    {
                        //parse duration to make up total seconds
                        total.TotalSeconds += TimeSpan.Parse(call.Duration).TotalSeconds;
                        //parse sales price(which is actually cost price to us) to make up total cost price
                        total.TotalCostPrice += Convert.ToDouble(call.SalesPrice);
                    }
                    //default charge of our sale price
                    //ideally the 1.5 would be coming from database
                    total.TotalSalePrice = total.TotalCostPrice * GetCodeMultiplier.GetMultiplier(code, displayErrors, tarrifCard);

                    //add the total to the list
                    listOfTotals.Add(total);
                    //move onto next list of calls by charge code
                    i++;
                }

                //create table based on totals with grand total inc
                DataTable tblTotalNames = GetTableOfTotals.GetDataTable(listOfTotals, tblCustomers, selectedCustomer);

                //sort out totals table - exchange codes for names
                tblTotalNames = GetTotalNames.GetNames(tblTotalNames);

                ExcelFile excelFile = new ExcelFile();
                //Check if you need to have the split by numbers
                if (chbSplitNumbers.IsChecked == true)
                { //if split required 
                    List<Call> callsByNumber = callsByCustomer.OrderBy(x => x.CustomerCLI).ToList(); //order calls by CLI 
                    DataTable tableByNumber = GetTableSplitByNumbers.GetSplitByNumbers(callsByNumber, tarrifCard);
                    excelFile.CreateExcelFile(tblTotalNames, selectedCustomer, tableByNumber, "", runAll);
                }
                else //if not just do the normal
                {
                    excelFile.CreateExcelFile(tblTotalNames, selectedCustomer, null, preSelectedFolderPath, runAll);
                }
                lblPleaseWait.Visibility = Visibility.Hidden;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
