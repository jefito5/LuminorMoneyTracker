namespace BrrBrrMoney
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
    public class Transaction
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public Transaction(decimal amount, DateTime date, string description)
        {
            Amount = amount;
            Date = date;
            Description = description;
        }
    }

}
