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
            CategoryFileLoader loader = new CategoryFileLoader();
            Dictionary<string, string> categoryMapping = loader.Load("categories.json");
            Categorizer categorizer = new Categorizer(categoryMapping);
            StatementReader reader = new StatementReader(categorizer);
            List<Transaction> statementTransactions = reader.ReadStatement("PavyzdiniaiDuomenys.csv");
            foreach (var transaction in statementTransactions)
            {
                Console.WriteLine(transaction);
            }
            TransactionAnalyzer analyzer = new TransactionAnalyzer();
            Dictionary<string, decimal> totals = analyzer.SumExpensesByCategory(statementTransactions);
            List<KeyValuePair<string,decimal>> sortedTotals = analyzer.SortByAmountDescending(totals);
            foreach(var pair in sortedTotals)
            {
                Console.WriteLine($"Category {pair.Key}: Total spent {pair.Value}");
            }
            Console.WriteLine($"Total sum {analyzer.FindTotalSum(statementTransactions)}");
            Console.WriteLine($"Number of Transactions: {statementTransactions.Count}");
            Transaction? biggestPsender = analyzer.FindBiggestSpend(statementTransactions);
            Transaction? biggestIncome = analyzer.FindBiggestIncome(statementTransactions);
            if (biggestPsender != null)
            { Console.WriteLine($"Biggest spend: {biggestPsender.Amount} on {biggestPsender.Description}"); }
            if (biggestIncome != null)
            {
                Console.WriteLine($"Biggest income: {biggestIncome.Amount} from {biggestIncome.Description}");
            }
          
            
        }
    }
}
