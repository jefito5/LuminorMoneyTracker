using Microsoft.AspNetCore.Mvc;
using BrrBrrMoney.Models;
using BrrBrrMoney.Services;

namespace BrrBrrMoney.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Transaction>> ListTransactions()
        {
            CategoryFileLoader loader = new CategoryFileLoader();
            Dictionary<string, string> categoryMapping = loader.Load("categories.json");
            Categorizer categorizer = new Categorizer(categoryMapping);
            StatementReader reader = new StatementReader(categorizer);

            List<Transaction> transactions = reader.ReadStatement("PavyzdiniaiDuomenys.csv");

            return Ok(transactions);
        }
    }
}