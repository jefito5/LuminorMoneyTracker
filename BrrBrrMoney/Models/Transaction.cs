using System;
using System.Collections.Generic;
using System.Text;

namespace BrrBrrMoney.Models
{
    public abstract class Transaction
    {
        public Guid Id { get;} //no point actually for right now at least
        private string _Description = string.Empty;
        //TODO ADD TYPE FOR LOCALHOST API LIST TO WORK WITH EXPENSE INCOME
        public string Description
        {
            get { return _Description; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Description cannot be null or empty.");
                }
                _Description = value;
            }
        }
        private decimal _Amount;
        public decimal Amount
        {
            get { return _Amount; }
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Amount must be greater than zero.");
                }
                _Amount = value;
            }
        }
        public DateTime Date { get; private set; }
        
        protected Transaction(string description, decimal amount, DateTime date)
        {
            Id = Guid.NewGuid();
            Description = description;
            Amount = amount;
            Date = date;
        }

        public abstract decimal GetSignedAmount(); //expense or income imporant when inheritance
        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd HH:mm} {GetSignedAmount(),9:+0.00;-0.00}  {Description}";
        }
    }
}
