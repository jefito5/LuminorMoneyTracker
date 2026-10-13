using System;
using System.Collections.Generic;
using System.Text;

namespace BrrBrrMoney.Services
{
    public interface ICategorizer
    {

        public string Categorize(string description);
    }
    public class Categorizer : ICategorizer
    {
        private readonly Dictionary<string, string> _categoryMappings; //Key word found in description means category name
        private const string DefaultCategory = "Uncategorized";
        
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
