using BrrBrrMoney.Models;
using BrrBrrMoney.Services;
using System;
namespace BrrBrrMoney
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            decimal sum = 0;
            StatementReader reader = new StatementReader();
            List<Transaction> statementTransactions = reader.ReadStatement("PavyzdiniaiDuomenys.csv");
            foreach (var transaction in statementTransactions)
            {
                Console.WriteLine($"Transaction ID: {transaction.Id}");
                Console.WriteLine($"Description: {transaction.Description}");
                Console.WriteLine($"Amount: {transaction.Amount}");
                Console.WriteLine($"Date: {transaction.Date}");
                Console.WriteLine($"Signed Amount: {transaction.GetSignedAmount()}");
                sum += transaction.GetSignedAmount();
                Console.WriteLine();
            }
            Console.WriteLine($"Total Sum: {sum}");
            Console.WriteLine($"Number of Transactions: {statementTransactions.Count}");
            Console.WriteLine($"Skipped Lines: {reader.SkippedLineCount}");

            Categorizer categorizer = new Categorizer(new Dictionary<string, string>
            {
                { "IKI", "Food shopping" },
                { "CAFFEINE", "Coffee shops" },
                { "LIDL", "Food shopping" }
            });
            Console.WriteLine("Categorized Transactions:");
            foreach (var transaction in statementTransactions)
            {
                var category = categorizer.Categorize(transaction.Description);
                Console.WriteLine($"Description: {transaction.Description} Category: {category}");
            }
        }
    }
}
