using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace SimplyRugby.Models.Member
{
    /// <summary>
    /// Represents a member.
    /// </summary>
    public class Member
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private DateTime _dateOfBirth;
        private Skills.Skills? _skills;
        private Team.Team? _team;
        private int _sruNumber;
        private string _address = string.Empty;
        private string _postcode = string.Empty;
        private string _email = string.Empty;
        private string _telephoneNumber = string.Empty;
        private string _mobileNumber = string.Empty;

        // Public properties.
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
                    _lastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());     // Capitalize the first letter of each word.
            }
        }
        public string Name => $"{FirstName} {LastName}";
        public required DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (ValidDateOfBirth(value))
                    _dateOfBirth = value;
            }
        }
        public Skills.Skills? Skills
        {
            get => _skills;
            set
            {
                if (ValidSkills(value))
                    _skills = value;
            }
        }
        [ForeignKey("Skills")]
        public int? SkillsID { get; set; }
        public Team.Team? Team
        {
            get => _team;
            set
            {
                if (ValidTeam(value))
                    _team = value;
            }
        }
        public int? TeamID { get; set; }
        public required int SRUNumber
        {
            get => _sruNumber;
            set
            {
                if (ValidSRUNumber(value))
                    _sruNumber = value;
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
        public required string Postcode
        {
            get => _postcode;
            set
            {
                if (ValidPostcode(value.Trim()))
                    _postcode = value.Trim().ToUpper();
            }
        }
        public required string Email
        {
            get => _email;
            set
            {
                if (ValidEmail(value.Trim()))
                    _email = value.Trim();
            }
        }
        public string? TelephoneNumber
        {
            get => _telephoneNumber;
            set
            {
                if (ValidTelephoneNumber(value.Trim()))
                    _telephoneNumber = value.Trim();
            }
        }
        public string? MobileNumber
        {
            get => _mobileNumber;
            set
            {
                if (ValidMobilePhoneNumber(value.Trim()))
                    _mobileNumber = value.Trim();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Member"/> class.
        /// </summary>
        public Member()
        {
            filter = new ProfanityFilter.ProfanityFilter();
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
                throw new Exception("Member's first or last name cannot be empty");

            // Check if the username is within the length constraints.
            if (name.Length < 2 || name.Length > 20)
                throw new Exception("Member's first or last name must be between 2 and 20 characters.");

            // Check for invalid characters (anything that's not a letter, a space, an apostrophe, or a hyphen).
            if (name.Any(ch => !char.IsLetter(ch) && ch != ' ' && ch != '\'' && ch != '-'))
                throw new Exception("Member's first or last name can only contain letters, single spaces, apostrophes, or hyphens.");

            // Check for consecutive special characters ('--', "''", or '  ').
            if (name.Contains("--") || name.Contains("''") || name.Contains("  "))
                throw new Exception("Member's first or last name cannot contain consecutive special characters or spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(name))
                throw new Exception("Member's first or last name contains profanity.");

            return true;
        }

        /// <summary>
        /// Checks if a date of birth is valid.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Date of birth is not valid.</exception>
        private bool ValidDateOfBirth(DateTime dateOfBirth)
        {
            // Minimum date of birth, 100 years ago.
            DateTime minDate = new DateTime(1924, 1, 1);

            // Check if the date of birth is null
            if (dateOfBirth.Equals(DateTime.MinValue))
                throw new Exception("Member's date of birth cannot be empty.");

            // Check if the date of birth is over 100 years ago.
            if (dateOfBirth < minDate)
                throw new Exception("Member's date of birth cannot be over 100 years ago.");

            // Check if the date of birth is in the future.
            if (dateOfBirth > DateTime.Now)
                throw new Exception("Member's date of birth cannot be in the future.");

            return true;
        }

        /// <summary>
        /// Checks if a SRU number is valid.
        /// </summary>
        /// <param name="sruNumber">SRU number to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">SRU number is not valid.</exception>
        private bool ValidSRUNumber(int sruNumber)
        {
            // Check if the SRU number is null.
            if (sruNumber.Equals(Int32.MinValue))
                throw new Exception("Member's SRU number cannot be empty.");

            // Check if the SRU number is more than the maximum int value.
            if (sruNumber > Int32.MaxValue)
                throw new Exception("Member's SRU number is too large.");

            // Check if the SRU number is negative.
            if (sruNumber < 0)
                throw new Exception("Member's SRU number cannot be negative.");

            return true;
        }

        /// <summary>
        /// Checks if a skills object is valid.
        /// </summary>
        /// <param name="skills">Skills to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Skills is not valid.</exception>
        private bool ValidSkills(Skills.Skills skills)
        {
            if (skills == null)
                throw new Exception("Members's skills cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a team is valid.
        /// </summary>
        /// <param name="team">Team to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Team is not valid.</exception>
        private bool ValidTeam(Team.Team team)
        {
            if (team == null)
                throw new Exception("Members's team cannot be null.");

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
                throw new Exception("Members's address cannot be empty.");

            // Check if the address is within the length constraints.
            if (address.Length < 2 || address.Length > 35)
                throw new Exception("Members's address must be between 2-35.");

            // Check for invalid characters (anything that's not a letter, number, a space, or a hyphen).
            if (address.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' ' && ch != '-'))
                throw new Exception("Members's address can only contain letters, numbers, single spaces, or hyphens.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(address))
                throw new Exception("Members's address contains profanity.");

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
                throw new Exception("Members's post code cannot be empty.");

            // Check if the postcode is within the length constraints.
            if (postcode.Length < 6 || postcode.Length > 8)
                throw new Exception("Members's post code must be between 6-8.");

            // Check for invalid characters (anything that's not a letter or a number).
            if (postcode.Any(ch => (!char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch)) && ch != ' '))
                throw new Exception("Members's post code can only contain letters, numbers, and single spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(postcode))
                throw new Exception("Members's post code contains profanity.");

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
            // Check if the username is null or empty
            if (string.IsNullOrEmpty(email))
                throw new Exception("Member's email cannot be empty.");

            // Check if the email contains profanity.
            if (filter.IsProfanity(email))
                throw new Exception("Member's email contains profanity.");

            // Check if the email is valid.
            if (EmailValidation.EmailValidator.Validate(email)) return true;
            else throw new Exception("Member's email address is invalid.");
        }

        /// <summary>
        /// Checks if a telephone number is valid.
        /// </summary>
        /// <param name="telephoneNumber">Telephone number to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Telephone number is not valid.</exception>
        private bool ValidTelephoneNumber(string telephoneNumber)
        {
            // Check if the telephone number is within the length constraints if it is not null or empty.
            if (!string.IsNullOrEmpty(telephoneNumber) && (telephoneNumber.Length < 10 || telephoneNumber.Length > 11))
                throw new Exception("Member's telephone number must be 10-11 numbers.");

            // Check for invalid characters (anything that's not a number).
            if (telephoneNumber.Any(ch => !char.IsDigit(ch)))
                throw new Exception("Member's telephone number can only contain numbers.");

            return true;
        }

        /// <summary>
        /// Checks if a mobile phone number is valid.
        /// </summary>
        /// <param name="mobilePhoneNumber">Mobile phone number to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Mobile phone number is not valid.</exception>
        private bool ValidMobilePhoneNumber(string mobilePhoneNumber)
        {
            // Check if the mobile number is within the length constraints if it is not null or empty.
            if (!string.IsNullOrEmpty(mobilePhoneNumber) && (mobilePhoneNumber.Length < 11 || mobilePhoneNumber.Length > 11))
                throw new Exception("Member's mobile phone number must be 11 numbers, eg 07123456789.");

            // Check for invalid characters (anything that's not a number).
            if (mobilePhoneNumber.Any(ch => !char.IsDigit(ch)))
                throw new Exception("Member's mobile phone number can only contain numbers.");

            return true;
        }
    }
}
