using System.ComponentModel.DataAnnotations;

namespace MekanikApi.Domain.Validation
{
    public class NoWhitespace : ValidationAttribute
    {
        public virtual string ErrorMessage { get; set; }

        public NoWhitespace(string errorMessage = null)
        {
            ErrorMessage = string.IsNullOrEmpty(errorMessage)
                ? "Input cannot contain leading or trailing whitespaces."
                : errorMessage;
        }

        public override bool IsValid(object value)
        {
            if (value is string str)
            {
                return str.Trim().Length > 0;
            }
            return true;
        }
    }
}