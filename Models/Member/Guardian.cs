using System;
using System.Globalization;
using System.Linq;

namespace SimplyRugby.Models.Member
{
    /// <summary>
    /// Represents a guardian inheriting from the next of kin class.
    /// </summary>
    public class Guardian : NextOfKin
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private string _relationship;
        private string _address;
        private string _postCode;

        // Public properties.
        public required string Relationship
        {
            get => _relationship;
            set
            {
                if (ValidRelationship(value.Trim()))
                    _relationship = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());     // Capitalize the first letter of each word.
            }
        }
        public required string Address
        {
            get => _address;
            set
            {
                if (ValidAddress(value.Trim()))
                    _address = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());     // Capitalize the first letter of each word.
            }
        }
        public required string PostCode
        {
            get => _postCode;
            set
            {
                if (ValidPostcode(value.Trim()))
                    _postCode = value.Trim().ToUpper();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Guardian"/> class.
        /// </summary>
        public Guardian()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Checks if a relationship is valid.
        /// </summary>
        /// <param name="relationship">Relationship to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Relationship is not valid.</exception>
        private bool ValidRelationship(string relationship)
        {
            // Check if the relationship is null or empty
            if (string.IsNullOrWhiteSpace(relationship))
                throw new Exception("Next of kin/guardian relationship cannot be empty.");

            // Check if the relationship is within the length constraints.
            if (relationship.Length < 2 || relationship.Length > 20)
                throw new Exception("Next of kin/guardian relationship must be between 2-20.");

            // Check for invalid characters (anything that's not a letter, a space, or a hyphen).
            if (relationship.Any(ch => !char.IsLetter(ch) && ch != ' ' && ch != '-'))
                throw new Exception("Next of kin/guardian relationship can only contain letters, single spaces, or hyphens.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(relationship))
                throw new Exception("Next of kin/guardian relationship contains profanity.");

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
                throw new Exception("Next of kin/guardian address cannot be empty.");

            // Check if the address is within the length constraints.
            if (address.Length < 2 || address.Length > 35)
                throw new Exception("Next of kin/guardian address must be between 2-35.");

            // Check for invalid characters (anything that's not a letter, number, a space, or a hyphen).
            if (address.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' ' && ch != '-'))
                throw new Exception("Next of kin/guardian address can only contain letters, numbers, single spaces, or hyphens.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(address))
                throw new Exception("Next of kin/guardian address contains profanity.");

            return true;
        }

        /// <summary>
        /// Checks if a post code is valid.
        /// </summary>
        /// <param name="postcode">Post code to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Post code is not valid.</exception>
        private bool ValidPostcode(string postcode)
        {
            // Check if the postcode is null or empty
            if (string.IsNullOrWhiteSpace(postcode))
                throw new Exception("Next of kin/guardian post code cannot be empty.");

            // Check if the postcode is within the length constraints.
            if (postcode.Length < 6 || postcode.Length > 8)
                throw new Exception("Next of kin/guardian post code must be between 6-8.");

            // Check for invalid characters (anything that's not a letter or a number).
            if (postcode.Any(ch => (!char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch)) && ch != ' '))
                throw new Exception("Next of kin/guardian post code can only contain letters, numbers, and single spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(postcode))
                throw new Exception("Next of kin/guardian post code contains profanity.");

            return true;
        }
    }
}
