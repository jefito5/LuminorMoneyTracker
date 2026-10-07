using BrrBrrMoney.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace BrrBrrMoney.Services
{
    internal class TransactionAnalyzer
    {
        public Dictionary<string, decimal> SumExpensesByCategory(List<Transaction> transactions)
        {
            Dictionary<string, decimal> categorySums = new Dictionary<string, decimal>();
            for (int i = 0; i < transactions.Count; i++)
            {
                if (transactions[i] is Expense expense)
                {
                    if (!categorySums.ContainsKey(expense.Category))
                    {
                        categorySums.Add(expense.Category, expense.Amount);
                    }
                    else
                    {
                        categorySums[expense.Category] += expense.Amount;
                    }
                }
            }
            return categorySums;
        }
        public List<KeyValuePair<string, decimal>> SortByAmountDescending(Dictionary<string, decimal> categoryTotals)
        {
            List<KeyValuePair<string, decimal>> sortedTotals = new List<KeyValuePair<string, decimal>>(categoryTotals);
            sortedTotals.Sort((a, b) => b.Value.CompareTo(a.Value));
            return sortedTotals;
        }
        public Transaction? FindBiggestSpend(List<Transaction> transactions)
        {
            Transaction? transactionBiggest = null;
            decimal largest = 0;
            for (int i = 0; i < transactions.Count; i++)
            {
                if (transactions[i] is Expense)
                {
                        if (transactions[i].Amount > largest)
                        {
                            largest = transactions[i].Amount;
                            transactionBiggest = transactions[i];
                        }
                }
            }
            return transactionBiggest;
        }
        public Transaction? FindBiggestIncome(List<Transaction> transactions)
        {
            Transaction? incomeBiggest = null;
            decimal largest = 0;
            foreach (Transaction transaction in transactions)
            {
                if (transaction is Income)
                {
                    if (transaction.Amount > largest)
                    {
                        largest = transaction.Amount;
                        incomeBiggest = transaction;
                    }
                }
            }
            return incomeBiggest;
        }
        public decimal FindTotalSum(List<Transaction> transactions)
        {
            decimal sum = 0;
            foreach (Transaction transaction in transactions)
            {
                sum += transaction.GetSignedAmount();
            }
            return sum;
        }
    }
}
