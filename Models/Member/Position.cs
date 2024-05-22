using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplyRugby.Models.Member
{
    public class Position
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private string _name;
        private ICollection<Player>? _players;

        // Public properties.
        public int PositionID { get; set; }
        public required string Name
        {
            get => _name;
            set
            {
                if (ValidName(value.Trim()))
                    _name = value.Trim();
            }
        }
        public ICollection<Player>? Players
        {
            get => _players;
            set
            {
                if (ValidPlayers(value))
                    _players = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        public Position()
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
                throw new Exception("Position name cannot be empty");

            // Check if the username is within the length constraints.
            if (name.Length < 2 || name.Length > 20)
                throw new Exception("Position name must be between 2 and 20 characters.");

            // Check for invalid characters (anything that's not a letter, number, or a space).
            if (name.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' '))
                throw new Exception("Position name can only contain letters, numbers, or single spaces.");

            // Check for consecutive spaces.
            if (name.Contains("  "))
                throw new Exception("Position name cannot contain consecutive spaces.");

            return true;
        }

        /// <summary>
        /// Checks if a collection of Players is valid.
        /// </summary>
        /// <param name="players">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidPlayers(ICollection<Player> players)
        {
            if (players == null)
                throw new Exception("Position's players cannot be null.");

            return true;
        }
    }
}
