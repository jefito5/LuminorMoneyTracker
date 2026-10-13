using BrrBrrMoney.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace BrrBrrMoney.Services
{
 
    public class StatementReader
    {
        public int SkippedLineCount { get; private set; }
        private const int TypeIndex = 5; //to not forget what numbers hold what value in the future
        private const int AmountIndex = 3;
        private const int DataIndex = 1;
        private const int TimeIndex = 2;
        private const int DescriptionIndex = 12;
        private readonly ICategorizer _categorizer;
        public StatementReader(ICategorizer categorizer)
        {
            _categorizer = categorizer ?? throw new ArgumentNullException(nameof(categorizer));
        }
        private const string CurrencyMarker = "EUR";
        private const string CountryMarker = "LTU";
        private const int MarkerToTextOffset = 14;   // "EUR" + 1 digit + date (10 characters)
        /// <summary>
        /// Turns a raw bank statement line into a short description.
        /// </summary>
        private static string CleanDescription(string raw)
        {
            string? text = ExtractText(raw);
            return text is null ? raw : RemoveRepeatedCity(text);
        }

        // Returns null when the line doesn't match the expected format.
        private static string? ExtractText(string raw)
        {
            if (!raw.StartsWith('*')) return null;

            int currencyIndex = raw.IndexOf(CurrencyMarker, StringComparison.Ordinal);
            if (currencyIndex < 0) return null;

            int textStart = currencyIndex + MarkerToTextOffset;
            if (textStart >= raw.Length) return null;

            int textEnd = raw.IndexOf(CountryMarker, textStart, StringComparison.Ordinal);
            if (textEnd < 0) return null;

            string text = raw.Substring(textStart, textEnd - textStart);
            return text.Length == 0 ? null : text;
        }

        // The city is repeated (KAUNASKAUNAS), keep only one.
        private static string RemoveRepeatedCity(string text)
        {
            string lastWord = GetLastWord(text);

            if (!IsDoubled(lastWord)) return text;

            int cityLength = lastWord.Length / 2;
            return text.Substring(0, text.Length - cityLength);   // drop the second copy
        }

        private static string GetLastWord(string text)
        {
            int lastSpace = text.LastIndexOf(' ');   // -1 if there is no space
            return text.Substring(lastSpace + 1);
        }

        private static bool IsDoubled(string word)
        {
            if (word.Length == 0 || word.Length % 2 != 0) return false;

            int half = word.Length / 2;
            return word.Substring(0, half) == word.Substring(half);
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
