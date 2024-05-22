using System;
using System.Globalization;
using System.Linq;

namespace SimplyRugby.Models.Member
{
    /// <summary>
    /// Represents the next of kin of a member.
    /// </summary>
    public class NextOfKin
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        //Private fields.
        private string _firstName;
        private string _lastName;
        private string _telephoneNumber;

        //Public properties.
        public int NextOfKinID { get; set; }
        public int MemberID { get; set; }
        public required string FirstName
        {
            get => _firstName;
            set
            {
                if (ValidName(value.Trim()))
                    _firstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());     // Capitalize the first letter of each word.
            }
        }
        public required string LastName
        {
            get => _lastName;
            set
            {
                if (ValidName(value.Trim()))
                    _lastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());      // Capitalize the first letter of each word.
            }
        }
        public required string TelephoneNumber
        {
            get => _telephoneNumber;
            set
            {
                if (ValidTelephoneNumber(value.Trim()))
                    _telephoneNumber = value.Trim();
            }
        }
        public string Name => $"{FirstName} {LastName}";

        /// <summary>
        /// Initializes a new instance of the <see cref="NextOfKin"/> class.
        /// </summary>
        public NextOfKin()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Checks if a name is valid.
        /// </summary>
        /// <param name="name">Name to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Name is not valid.</exception>
        private bool ValidName(string name)
        {
            // Check if the username is null or empty
            if (string.IsNullOrEmpty(name))
                throw new Exception("Next of kin/guardian first or last name cannot be empty");

            // Check if the username is within the length constraints.
            if (name.Length < 2 || name.Length > 20)
                throw new Exception("Next of kin/guardian  first or last name must be between 2 and 20 characters.");

            // Check for invalid characters (anything that's not a letter, a space, an apostrophe, or a hyphen).
            if (name.Any(ch => !char.IsLetter(ch) && ch != ' ' && ch != '\'' && ch != '-'))
                throw new Exception("Next of kin/guardian  first or last name can only contain letters, single spaces, apostrophes, or hyphens.");

            // Check for consecutive special characters ('--', "''", or '  ').
            if (name.Contains("--") || name.Contains("''") || name.Contains("  "))
                throw new Exception("Next of kin/guardian  first or last name cannot contain consecutive special characters or spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(name))
                throw new Exception("Next of kin/guardian  first or last name contains profanity.");

            return true;
        }

        /// <summary>
        /// Checks if a telephone number is valid.
        /// </summary>
        /// <param name="telephoneNumber">Telephone number to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Telephone number is not valid.</exception>
        private bool ValidTelephoneNumber(string telephoneNumber)
        {
            // Check if the telephone number is null, empty, or not within the length constraints.
            if (string.IsNullOrEmpty(telephoneNumber))
                throw new Exception("Next of kin/guardian telephone number cannot be empty.");

            // Check if the telephone number is within the length constraints.
            if (telephoneNumber.Length < 10 || telephoneNumber.Length > 11)
                throw new Exception("Next of kin/guardian telephone number must be 10-11 numbers.");

            // Check for invalid characters (anything that's not a number).
            if (telephoneNumber.Any(ch => !char.IsDigit(ch)))
                throw new Exception("Next of kin/guardian telephone number can only contain numbers.");

            return true;
        }
    }
}
