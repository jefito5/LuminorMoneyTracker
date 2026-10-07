using BrrBrrMoney.Models;
using BrrBrrMoney.Services;
using System;
using System.ComponentModel;
namespace BrrBrrMoney
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            decimal sum = 0;
            
            CategoryFileLoader loader = new CategoryFileLoader();
            Dictionary<string, string> categoryMapping = loader.Load("categories.json");
            Categorizer categorizer = new Categorizer(categoryMapping);
            StatementReader reader = new StatementReader(categorizer);
            List<Transaction> statementTransactions = reader.ReadStatement("PavyzdiniaiDuomenys.csv");
            foreach (var transaction in statementTransactions)
            {
                
                if (transaction is Expense expense)
                {
                    string desc = "";
                    if (transaction.Description.Length > 49)
                    {
                        desc = transaction.Description[30..^19];
                    }
                    else
                    {
                        desc = transaction.Description;
                    }
                    Console.WriteLine($"Description: {desc}");
                    Console.WriteLine($"Amount: {transaction.Amount}");
                    Console.WriteLine($"Date: {transaction.Date}");
                    Console.WriteLine($"Signed Amount: {transaction.GetSignedAmount()}");
                    string category = expense.Category;
                    Console.WriteLine($"Category: {category}");
                }
                else if (transaction is Income income)
                {
                    Console.WriteLine($"Description: {transaction.Description}");
                    Console.WriteLine($"Amount: {transaction.Amount}");
                    Console.WriteLine($"Date: {transaction.Date}");
                    Console.WriteLine($"Signed Amount: {transaction.GetSignedAmount()}");
                }
                sum += transaction.GetSignedAmount();
                Console.WriteLine();
            }
            TransactionAnalyzer analyzer = new TransactionAnalyzer();
            Dictionary<string, decimal> totals = analyzer.SumExpensesByCategory(statementTransactions);
            List<KeyValuePair<string,decimal>> sortedTotals = analyzer.SortByAmountDescending(totals);
            foreach(var pair in sortedTotals)
            {
                Console.WriteLine($"Category {pair.Key}: Total spent {pair.Value}");
            }
            Console.WriteLine($"Total sum {sum}");
            Console.WriteLine($"Number of Transactions: {statementTransactions.Count}");
            Console.WriteLine($"Skipped Lines: {reader.SkippedLineCount}");
            Transaction biggestPsender = analyzer.FindBiggestSpend(statementTransactions);
            Transaction biggestIncome = analyzer.FindBiggestIncome(statementTransactions);
            Console.WriteLine($"Biggest spend: {biggestPsender.Amount} on {biggestPsender.Description}");
            Console.WriteLine($"Biggest income: {biggestIncome.Amount} from {biggestIncome.Description}");
          
            
        }
    }
}
