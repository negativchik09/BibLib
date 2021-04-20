using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BibLib.Models
{
    public class RegexValidatorAttribute : ValidationAttribute
    {
        private readonly string _pattern;
        public RegexValidatorAttribute(string pattern)
        {
            _pattern = pattern;
        }

        public override bool IsValid(object value)
        {
            return Regex.IsMatch(value?.ToString() ?? string.Empty, _pattern);
        }
    }
}