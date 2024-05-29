using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PROG7311POE.Common.Attributes
{

    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required");
            }

            // At least one upper case letter
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return new ValidationResult("Password must contain at least one uppercase letter");
            }

            // At least one lower case letter
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return new ValidationResult("Password must contain at least one lowercase letter");
            }

            // At least one digit
            if (!Regex.IsMatch(password, @"\d"))
            {
                return new ValidationResult("Password must contain at least one digit");
            }

            // At least one special character
            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?\{<>:}|]"))
            {
                return new ValidationResult("Password must contain at least one special character");
            }

            // Minimum length of 8 characters
            if (password.Length < 8)
            {
                return new ValidationResult("Password must be at least 8 characters long");
            }

            return ValidationResult.Success;
        }
    }

}
