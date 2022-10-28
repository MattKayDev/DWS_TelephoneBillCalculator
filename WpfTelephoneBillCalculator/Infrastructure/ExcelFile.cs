using Microsoft.VisualBasic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTelephoneBillCalculator.Infrastructure
{
    public class ExcelFile
    {
        public ExcelFile()
        {
            //use the noncommercial licence (might have to get commercial licece)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public void CreateExcelFile(DataTable codesTable, string CustomerName, DataTable? splitNumbersTable, string preSelectedFolderPath, bool runAll)
        {
            try
            {
                //Need to be able to pick folder to save the file
                System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = new System.Windows.Forms.DialogResult();
                if (preSelectedFolderPath != string.Empty)
                {
                    Excel(codesTable,CustomerName,splitNumbersTable,preSelectedFolderPath, runAll);
                }
                else
                {
                    result = folderBrowser.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        Excel(codesTable, CustomerName, splitNumbersTable, folderBrowser.SelectedPath, runAll);
                    }
                    else
                    {
                        MessageBox.Show("No folder has been selected, excel sheet has now been generated! Please run again and select a folder!", "No folder selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                
                
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void Excel(DataTable codesTable, string CustomerName, DataTable? splitNumbersTable, string preSelectedFolderPath, bool runAll)
        {
            string folderPath = preSelectedFolderPath;
            using (var package = new ExcelPackage(folderPath + @"\" + CustomerName + ".xlsx"))
            {
                var sheet = package.Workbook.Worksheets.Add(CustomerName);
                sheet.Cells["A5"].Value = CustomerName;
                sheet.Cells["C5"].Value = "20/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year;

                sheet.Cells["A6"].Value = "Charge Code";
                sheet.Cells["B6"].Value = "Total Time";
                sheet.Cells["C6"].Value = "Total Charge";

                if (splitNumbersTable != null)
                {
                    sheet.Cells["E6"].Value = "Telephone Number";
                    sheet.Cells["F6"].Value = "Description";
                    sheet.Cells["G6"].Value = "Total";
                    using (var range = sheet.Cells["E6:G6"])
                    {
                        range.Style.Font.Bold = true;
                        //ABS Blue = #4472C4 or RGB(68, 114, 196)
                        Color absBlue = Color.FromArgb(68, 114, 196);
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(absBlue);
                        range.Style.Font.Color.SetColor(Color.White);
                    }
                }


                int i = 7;
                foreach (DataRow row in codesTable.Rows)
                {
                    TimeSpan seconds = TimeSpan.FromSeconds(Convert.ToDouble(row[1]));
                    sheet.Cells["A" + i].Value = row[0];
                    sheet.Cells["B" + i].Value = seconds.ToString();
                    double roundedVal = Convert.ToDouble(row[2]);
                    sheet.Cells["C" + i].Value = "£ " + string.Format("{0:0.00}", roundedVal);
                    //sheet.Cells["C" + i].Value =  string.Format("{0:0.00}",roundedVal);
                    i++;
                }
                i -= 1;
                sheet.Cells["B" + i].Value = ""; //empty out the total second value for grand total (could be optional)



                //styling
                sheet.Cells["A5"].Style.Font.Bold = true;
                sheet.Cells["C5"].Style.Font.Bold = true;
                using (var range = sheet.Cells["A6:C6"])
                {
                    range.Style.Font.Bold = true;
                    //ABS Blue = #4472C4 or RGB(68, 114, 196)
                    Color absBlue = Color.FromArgb(68, 114, 196);
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(absBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }
                sheet.Cells["D6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells["D6"].Style.Fill.BackgroundColor.SetColor(Color.White);

                //Bold Grand Total
                sheet.Cells["A" + i + ":C" + i].Style.Font.Bold = true;

                sheet.Columns[1].Width = 50;
                sheet.Columns[2].Width = 10;
                sheet.Columns[3].Width = 12;

                using (var range = sheet.Cells["C5:C" + i])
                {
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                //setup table border
                using (var range = sheet.Cells["A7:C" + i])
                {
                    var borderColor = Color.FromArgb(180, 198, 231);
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Top.Color.SetColor(borderColor);
                    range.Style.Border.Left.Color.SetColor(borderColor);
                    range.Style.Border.Right.Color.SetColor(borderColor);
                    range.Style.Border.Bottom.Color.SetColor(borderColor);
                }

                //using(var range = sheet.Cells[1,6,3,i]) //A6 Ci (i = row number)

                if (splitNumbersTable != null)
                {
                    sheet.Columns[5].Width = 17.2;
                    sheet.Columns[6].Width = 10;
                    sheet.Columns[7].Width = 10;
                    sheet.Columns[7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    i = 7;
                    foreach (DataRow row in splitNumbersTable.Rows)
                    {
                        sheet.Cells["E" + i].Value = row[0];
                        //sheet.Cells["G" + i].Value = string.Format("{0:0.00}", Convert.ToDouble(row[1]));
                        sheet.Cells["G" + i].Value = "£ " + string.Format("{0:0.00}", Convert.ToDouble(row[1]));
                        i++;
                    }
                    sheet.Cells["E" + i + ":G" + i].Style.Font.Bold = true;
                }

                //save to file
                package.Save();
                if(!runAll)
                    MessageBox.Show("Telephone bill for " + CustomerName + " has been created.", "SUCCESS!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
