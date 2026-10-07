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
            Categorizer categorizer = new Categorizer(new Dictionary<string, string>
            {
                { "IKI", "Food shopping" },
                { "CAFFEINE", "Coffee shops" },
                { "LIDL", "Food shopping" },
                { "Spotify", "Subscriptions" },
                { "ORLEN", "Fuel" },
                { "Norfa", "Food shopping" }

            });
            StatementReader reader = new StatementReader(categorizer);
            List<Transaction> statementTransactions = reader.ReadStatement("PavyzdiniaiDuomenys.csv");
            foreach (var transaction in statementTransactions)
            {
                Console.WriteLine($"Transaction ID: {transaction.Id}");
                Console.WriteLine($"Description: {transaction.Description}");
                Console.WriteLine($"Amount: {transaction.Amount}");
                Console.WriteLine($"Date: {transaction.Date}");
                Console.WriteLine($"Signed Amount: {transaction.GetSignedAmount()}");
                if (transaction is Expense expense)
                {
                    string category = expense.Category;
                    Console.WriteLine($"Category: {category}");
                }
                sum += transaction.GetSignedAmount();
                Console.WriteLine();
            }
            TransactionAnalyzer analyzer = new TransactionAnalyzer();
            Dictionary<string, decimal> totals = analyzer.SumExpensesByCategory(statementTransactions);
            foreach(var pair in totals)
            {
                Console.WriteLine($"Category {pair.Key}: Sum {pair.Value}");
            }
            Console.WriteLine($"Total Sum: {sum}");
            Console.WriteLine($"Number of Transactions: {statementTransactions.Count}");
            Console.WriteLine($"Skipped Lines: {reader.SkippedLineCount}");

          
            
        }
    }
}
