using BrrBrrMoney.Models;
using System;
using System.Collections.Generic;
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
        private readonly ICategorizer _categorizer;
        public StatementReader(ICategorizer categorizer)
        {
            _categorizer = categorizer ?? throw new ArgumentNullException(nameof(categorizer));
        }
        private static string CleanDescription(string raw)
        {
            if (!raw.StartsWith("*")) return raw;

            int eur = raw.IndexOf("EUR");
            if (eur < 0) return raw;

            int start = eur + 14;   // "EUR" + 1 digit + date (10 characters)
            if (start >= raw.Length) return raw;

            int end = raw.IndexOf("LTU", start);   // country code after the city
            if (end < 0) return raw;

            string text = raw[start..end];
            if (text.Length == 0) return raw;

            // The city is repeated (KAUNASKAUNAS), keep only one copy
            string last = text[(text.LastIndexOf(' ') + 1)..];
            int half = last.Length / 2;
            bool repeated = last.Length % 2 == 0 && last[..half] == last[half..];

            return repeated ? text[..^half] : text;
        }
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
                        string rawDescription = parts[DescriptionIndex].Trim('"');
                        string description = CleanDescription(rawDescription);
                        string dateTimeStr = parts[DataIndex].Trim('"') + " " + parts[TimeIndex].Trim('"');
                        DateTime date = DateTime.ParseExact(dateTimeStr, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);

                        if (type == "D")
                        {
                            transactions.Add(new Expense(description, amount, date, _categorizer.Categorize(description)));
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
