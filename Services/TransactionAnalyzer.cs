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
                        categorySums[expense.Category] =+ expense.Amount;
                    }
                }
            }
            return categorySums;
        }
    }
}
