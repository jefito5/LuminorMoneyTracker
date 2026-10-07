using BrrBrrMoney.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Text;

namespace BrrBrrMoney.Services
{
 
    internal class StatementReader
    {
        public int SkippedLineCount { get; private set; }
        private const int TypeIndex = 5;
        private const int AmountIndex = 3;
        private const int DataIndex = 1;
        private const int TimeIndex = 2;
        private const int DescriptionIndex = 12;

        public List<Transaction> ReadStatement(string filePath)
        {
            SkippedLineCount = 0;
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{filePath}' does not exist.");
            }
            List<Transaction> transactions = new List<Transaction>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // Skip the header line
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        var parts = line.Split(';');
                        string type = parts[TypeIndex].Trim('"');
                        decimal amount = decimal.Parse(parts[AmountIndex].Trim('"'), CultureInfo.InvariantCulture);
                        string description = parts[DescriptionIndex].Trim('"');
                        string dateTimeStr = parts[DataIndex].Trim('"') + " " + parts[TimeIndex].Trim('"');
                        DateTime date = DateTime.ParseExact(dateTimeStr, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);

                        if (type == "D")
                        {
                            transactions.Add(new Expense(description, amount, date, "nekategorizuota"));
                        }
                        else if (type == "C")
                        {
                            transactions.Add(new Income(description, amount, date));
                        }
                       else
                        {
                            SkippedLineCount++;
                        }
                    }
                    catch (Exception ex) when (ex is FormatException || ex is IndexOutOfRangeException || ex is ArgumentException)
                    {
                        SkippedLineCount++;
                        Console.WriteLine(ex.Message);
                    }
                    
                }
            }
            return transactions;
        }
    }
}
