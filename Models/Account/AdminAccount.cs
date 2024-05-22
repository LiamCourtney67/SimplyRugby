using System;
using System.Globalization;

namespace SimplyRugby.Models.Account
{
    /// <summary>
    /// Represents an admin account inheriting from the account class.
    /// </summary>
    public class AdminAccount : Account
    {
        // Private fields.
        private string _accessLevel = string.Empty;

        // Public properties.
        public required string AccessLevel
        {
            get => _accessLevel;
            set
            {
                if (ValidAccessLevel(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim())))
                    _accessLevel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());
            }
        }

        /// <summary>
        /// Validates the access level.
        /// </summary>
        /// <param name="accessLevel">Access level to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Access level is not valid.</exception>
        private bool ValidAccessLevel(string accessLevel)
        {
            if (!accessLevel.Equals("Full") && !accessLevel.Equals("Limited"))
                throw new Exception("Invalid access level, must be either 'Full' or 'Limited'.");

            return true;
        }
    }
}
