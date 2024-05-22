using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace SimplyRugby.Models.Account
{
    /// <summary>
    /// Represents an account.
    /// </summary>
    public class Account
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;

        // Public properties.
        [Key]
        public required string Username
        {
            get => _username;
            set
            {
                if (ValidUsername(value.Trim()))
                    _username = value.Trim();
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                if (ValidPassword(value))
                    _password = HashPassword(value);
            }
        }
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (ValidName(value.Trim()))
                    _firstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());     // Capitalize the first letter of each word.
            }
        }
        public string LastName
        {
            get => _lastName;
            set
            {
                if (ValidName(value.Trim()))
                    _lastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());      // Capitalize the first letter of each word.
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                if (ValidEmail(value.Trim()))
                    _email = value.Trim();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Checks if a username is valid.
        /// </summary>
        /// <param name="username">Username to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Username is not valid.</exception>
        private bool ValidUsername(string username)
        {
            // Check if the username is null or empty
            if (string.IsNullOrEmpty(username))
                throw new Exception("Username cannot be empty.");

            // Check if the username is within the length constraints.
            if (username.Length < 5 || username.Length > 20)
                throw new Exception("Username must be between 5 and 20 characters.");

            // Check for invalid characters (anything that's not a letter or a number).
            if (username.Any(ch => !char.IsLetterOrDigit(ch)))
                throw new Exception("Username can only contain letters and numbers.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(username))
                throw new Exception("Username contains profanity.");

            return true;
        }

        /// <summary>
        /// Checks if a password is valid.
        /// </summary>
        /// <param name="password">Password to be checked.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Password is not valid.</exception>
        private bool ValidPassword(string password)
        {
            // Check if the password is null, empty, or not within the length constraints.
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 20)
                throw new Exception("Password must be between 8 and 20 characters.");

            return true;
        }

        /// <summary>
        /// Checks if a name is valid.
        /// </summary>
        /// <param name="name">Name to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Name is not valid.</exception>
        private bool ValidName(string name)
        {
            // Check if the name is null or empty
            if (string.IsNullOrEmpty(name))
                throw new Exception("First or last name cannot be empty");

            // Check if the name is within the length constraints.
            if (name.Length < 2 || name.Length > 20)
                throw new Exception("First or last name must be between 2 and 20 characters.");

            // Check for invalid characters (anything that's not a letter, a space, an apostrophe, or a hyphen).
            if (name.Any(ch => !char.IsLetter(ch) && ch != ' ' && ch != '\'' && ch != '-'))
                throw new Exception("First or last name can only contain letters, single spaces, apostrophes, or hyphens.");

            // Check for consecutive special characters ('--', "''", or '  ').
            if (name.Contains("--") || name.Contains("''") || name.Contains("  "))
                throw new Exception("First or last name cannot contain consecutive special characters or spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(name))
                throw new Exception("First or last name contains profanity.");

            return true;
        }

        /// <summary>
        /// Checks if an email is valid.
        /// </summary>
        /// <param name="email">Email to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Email is not valid.</exception>
        private bool ValidEmail(string email)
        {
            // Check if the email is null or empty
            if (string.IsNullOrEmpty(email))
                throw new Exception("Email cannot be empty");

            // Check if the email contains profanity.
            if (filter.IsProfanity(email))
                throw new Exception("Email contains profanity");

            // Check if the email is valid.
            if (EmailValidation.EmailValidator.Validate(email)) return true;
            else throw new Exception("Invalid email address");
        }

        /// <summary>
        /// Hashes a password using BCrypt.
        /// </summary>
        /// <param name="password">Password to be hashed.</param>
        /// <returns>Hashed password using BCrypt.</returns>
        private string HashPassword(string password) { return BCrypt.Net.BCrypt.EnhancedHashPassword(password); }

        /// <summary>
        /// Verifies a password using BCrypt.
        /// </summary>
        /// <param name="password">Password to be verified.</param>
        /// <returns>True if verified using BCrypt, false if not valid.</returns>
        public bool VerifyPassword(string password) { return BCrypt.Net.BCrypt.EnhancedVerify(password, _password); }
    }
}
