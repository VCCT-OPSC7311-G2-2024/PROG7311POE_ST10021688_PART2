using System.ComponentModel.DataAnnotations;

namespace PROG7311POE.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")] // Using built-in email validations
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(80, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 80 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Farmer or Employee

        // IF registering a farmer user type farmer0user details
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Farm Name must be between 6 and 100 characters")]
        public string FarmName { get; set; }

        [StringLength(200, MinimumLength = 10, ErrorMessage = "Farm location must be between 10 and 200 characters")]
        public string Location { get; set; }
    }
}
