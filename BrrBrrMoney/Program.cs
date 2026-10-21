using BrrBrrMoney.Models;
using BrrBrrMoney.Services;
using System;
using System.Windows.Forms;
using System.ComponentModel;
namespace BrrBrrMoney
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string filePath;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Choose file";
                dialog.Filter = "CSV files (*.csv)|*.csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                }
                else
                {
                    Console.WriteLine("Bad file");
                    return;
                }
            }

            CategoryFileLoader loader = new CategoryFileLoader();
            Dictionary<string, string> categoryMapping = loader.Load("categories.json");
            Categorizer categorizer = new Categorizer(categoryMapping);
            StatementReader reader = new StatementReader(categorizer);
            List<Transaction> statementTransactions = reader.ReadStatement(filePath);
            foreach (var transaction in statementTransactions)
            {
                Console.WriteLine(transaction);
            }
            TransactionAnalyzer analyzer = new TransactionAnalyzer();
            Dictionary<string, decimal> totals = analyzer.SumExpensesByCategory(statementTransactions);
            List<KeyValuePair<string, decimal>> sortedTotals = analyzer.SortByAmountDescending(totals);
            Dictionary<string, decimal> shares = analyzer.CalculateCategoryShares(statementTransactions);
            foreach (var pair in sortedTotals)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value} ({shares[pair.Key]:F1} % from total expenses)");
            }
            Console.WriteLine($"Total income: {analyzer.FindTotalIncome(statementTransactions)}");
            Console.WriteLine($"Total expense: {analyzer.FindTotalExpense(statementTransactions)}");
            Console.WriteLine($"Total balance {analyzer.FindTotalBalance(statementTransactions)}");
            Console.WriteLine($"Number of Transactions: {statementTransactions.Count}");
            Transaction? biggestPsender = analyzer.FindBiggestSpend(statementTransactions);
            Transaction? biggestIncome = analyzer.FindBiggestIncome(statementTransactions);
            if (biggestPsender != null)
            { Console.WriteLine($"Biggest spend: {biggestPsender.Amount} on {biggestPsender.Description}"); }
            if (biggestIncome != null)
            {
                Console.WriteLine($"Biggest income: {biggestIncome.Amount} from {biggestIncome.Description}");
            }
            decimal? rate = analyzer.CalculateSavingsRate(statementTransactions);
            if (rate != null)
            {
                Console.WriteLine($"Savings rate: {rate:F2} %");
            }
            else
            {
                Console.WriteLine("Savings rate: no income");
            }
        }
    }
}
