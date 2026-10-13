using System;
using System.Collections.Generic;
using System.Text;

namespace BrrBrrMoney.Models
{
    public class Expense : Transaction
    {
        private string _Category = string.Empty;
        
        public string Category
        {
            get { return _Category; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Category cannot be null or empty.");
                }
                _Category = value;
            }
        }
        public Expense(string description, decimal amount, DateTime date, string category)
            : base(description, amount, date)
        {
            Category = category;
        }
        public override decimal GetSignedAmount()
        {
            return -Amount; // Expense is negative
        }
        public override string ToString()
        {
            return $"{base.ToString()}  {Category}";
        }
    }
}
