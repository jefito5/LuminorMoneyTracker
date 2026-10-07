using System;
using System.Collections.Generic;
using System.Text;

namespace BrrBrrMoney.Services
{
    internal interface ICategorizer
    {
        public string Categorize(string description);
    }
    internal class Categorizer : ICategorizer
    {
        private readonly Dictionary<string, string> _categoryMappings; //Key word found in description -> Category name
        private const string DefaultCategory = "Nekategorizuota";
        
        public Categorizer(Dictionary<string, string> categoryMappings)
        {
            if (categoryMappings == null)
            {
                throw new ArgumentNullException(nameof(categoryMappings), "Category mappings cannot be null.");
            }
            _categoryMappings = categoryMappings;
            
        }
        public string Categorize(string description)
        {
            foreach (var mapping in _categoryMappings)
            {
                if (description.Contains(mapping.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return mapping.Value;
                }
            }
            return DefaultCategory; // Default category if no match is found
        }
       
    }
}
