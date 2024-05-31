using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace PROG7311POE.Models
{
    /// <summary>
    /// Users uses this website.
    /// </summary>
    public class User
    {
        // -- FIELDS ____________________________________________________________________________________________________________________________________________________________
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Farmer or Employee

        // -- METHODS ____________________________________________________________________________________________________________________________________________________________
        /// <summary>
        /// Creates a new user and hashes password!
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