using System;
using System.Collections.Generic;
using System.Text;

namespace BrrBrrMoney.Models
{
    internal class Income : Transaction
    {
        public Income(string description, decimal amount, DateTime date)
            : base(description, amount, date)
        {
        }
        public override decimal GetSignedAmount()
        {
            return Amount; // Income is positive
        }
        public override string ToString()
        {
            return $"{Description} {Amount} {Date} {GetSignedAmount()}";
        }
    }
}
