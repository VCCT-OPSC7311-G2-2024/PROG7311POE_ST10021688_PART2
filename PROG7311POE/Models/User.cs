using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace PROG7311POE.Models
{
    /// <summary>
    /// This is a human, animals are not allowed.
    /// </summary>
    public class User
    {
        // -- FIELDS ____________________________________________________________________________________________________________________________________________________________

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")] // Using built-in email validations
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{6,}$", 
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Farmer or Employee

        // -- METHODS ____________________________________________________________________________________________________________________________________________________________

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="userID">usersID in databsae</param>
        /// <param name="username">users username</param>
        /// <param name="email">users email</param>
        /// <param name="password">password to be hashed</param>
        public User(string username, string email, string password, string role )
        {
            Username = username;
            Email = email;
            Password = BCrypt.Net.BCrypt.HashPassword(Password);
            Role = role;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public User() { }
    }
}
// 💻🌸✨💖 --<< End of File >>-- 💖✨🌸💻