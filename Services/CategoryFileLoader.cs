using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BrrBrrMoney.Services
{
    internal class CategoryFileLoader
    {
        public Dictionary<string, string> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The {filePath} does not exist");
            }
            string fileText = File.ReadAllText(filePath);
            Dictionary<string, string>? categories = JsonSerializer.Deserialize<Dictionary<string, string>>(fileText);
            if (categories == null)
            {
                throw new InvalidDataException("No categories");
            }
            return categories;
        }
    }
}
