using System;
using System.Globalization;
using System.Linq;

namespace SimplyRugby.Models.Member
{
    /// <summary>
    /// Represents a doctor.
    /// </summary>
    public class Doctor
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private string _name;
        private string _telephoneNumber;
        private string _address;

        // Public properties.
        public int DoctorID { get; set; }
        public int MemberID { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                if (ValidName(value.Trim()))
                    _name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());     // Capitalize the first letter of each word.
            }
        }
        public string TelephoneNumber
        {
            get => _telephoneNumber;
            set
            {
                if (ValidTelephoneNumber(value.Trim()))
                    _telephoneNumber = value.Trim();
            }
        }
        public string Address
        {
            get => _address;
            set
            {
                if (ValidAddress(value.Trim()))
                    _address = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());  // Capitalize the first letter of each word.
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Doctor"/> class.
        /// </summary>
        public Doctor()
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
                throw new Exception("Doctor name cannot be empty.");

            // Check if the username is within the length constraints.
            if (name.Length < 2 || name.Length > 40)
                throw new Exception("Doctor name must be between 2 and 40 characters.");

            // Check for invalid characters (anything that's not a letter, a space, an apostrophe, a period, or a hyphen).
            if (name.Any(ch => !char.IsLetter(ch) && ch != ' ' && ch != '\'' && ch != '-' && ch != '.'))
                throw new Exception("Doctor name can only contain letters, single spaces, apostrophes, periods, or hyphens.");

            // Check for consecutive special characters ('--', "''", or '  ').
            if (name.Contains("--") || name.Contains("''") || name.Contains("  "))
                throw new Exception("Doctor name cannot contain consecutive special characters or spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(name))
                throw new Exception("Doctor name contains profanity");

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
                throw new Exception("Doctor telephone number cannot be empty.");

            // Check if the telephone number is within the length constraints.
            if (telephoneNumber.Length < 10 || telephoneNumber.Length > 11)
                throw new Exception("Doctor telephone number must be 10-11 numbers.");

            // Check for invalid characters (anything that's not a number).
            if (telephoneNumber.Any(ch => !char.IsDigit(ch)))
                throw new Exception("Doctor telephone number can only contain numbers.");

            return true;
        }

        /// <summary>
        /// Checks if an address is valid.
        /// </summary>
        /// <param name="address">Address to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Address is not valid.</exception>
        private bool ValidAddress(string address)
        {
            // Check if the address is null or empty
            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("Doctor's address cannot be empty.");

            // Check if the address is within the length constraints.
            if (address.Length < 2 || address.Length > 50)
                throw new Exception("Doctor's address must be between 2-50.");

            // Check for invalid characters (anything that's not a letter, number, a space, or a hyphen).
            if (address.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' ' && ch != '-'))
                throw new Exception("Doctor's address can only contain letters, numbers, single spaces, or hyphens.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(address))
                throw new Exception("Doctor's address contains profanity.");

            return true;
        }
    }
}
