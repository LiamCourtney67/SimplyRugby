using SimplyRugby.Models.Member;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimplyRugby.Models.Team
{
    /// <summary>
    /// Represents a team.
    /// </summary>
    public class Team
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private string _name;
        private string _level;
        private ICollection<Player>? _players;
        private ICollection<Coach>? _coaches;
        private ICollection<Match.Match>? _matches;
        private ICollection<TrainingSession.TrainingSession>? trainingSessions;

        // Public properties.
        public int TeamID { get; set; }
        public required string Name
        {
            get => _name;
            set
            {
                if (ValidName(value.Trim()))
                    _name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());      // Capitalize the first letter of each word.
            }
        }
        public required string Level
        {
            get => _level;
            set
            {
                if (ValidLevel(value.Trim()))
                    _level = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());      // Capitalize the first letter of each word.
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
        public ICollection<Coach>? Coaches
        {
            get => _coaches;
            set
            {
                if (ValidCoaches(value))
                    _coaches = value;
            }
        }
        public ICollection<Match.Match>? Matches
        {
            get => _matches;
            set
            {
                if (ValidMatches(value))
                    _matches = value;
            }
        }
        public ICollection<TrainingSession.TrainingSession>? TrainingSessions
        {
            get => trainingSessions;
            set
            {
                if (ValidTrainingSessions(value))
                    trainingSessions = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Team"/> class.
        /// </summary>
        public Team()
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
            // Check if the name is null or empty
            if (string.IsNullOrEmpty(name))
                throw new Exception("Team name cannot be empty");

            // Check if the name is within the length constraints.
            if (name.Length < 2 || name.Length > 20)
                throw new Exception("Team name must be between 2 and 20 characters.");

            // Check for invalid characters (anything that's not a letter, a space, an apostrophe, or a hyphen).
            if (name.Any(ch => !char.IsLetter(ch) && ch != ' ' && ch != '\'' && ch != '-'))
                throw new Exception("Team name can only contain letters, single spaces, apostrophes, or hyphens.");

            // Check for consecutive special characters ('--', "''", or '  ').
            if (name.Contains("--") || name.Contains("''") || name.Contains("  "))
                throw new Exception("Team name cannot contain consecutive special characters or spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(name))
                throw new Exception("Team name contains profanity.");

            return true;
        }

        /// <summary>
        /// Checks if the level is valid.
        /// </summary>
        /// <param name="level">Level to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Level is not valid.</exception>
        private bool ValidLevel(string level)
        {
            // Check if the level is null or empty
            if (string.IsNullOrEmpty(level))
                throw new Exception("Team level cannot be empty");

            // Check if the level is Junior or Senior
            if (level != "Junior" && level != "Senior")
                throw new Exception("Team level must be either Junior or Senior.");

            return true;
        }

        /// <summary>
        /// Checks if a collection of players is valid.
        /// </summary>
        /// <param name="players">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidPlayers(ICollection<Player> players)
        {
            if (players == null)
                throw new Exception("Team's players cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a collection of coaches is valid.
        /// </summary>
        /// <param name="coaches">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidCoaches(ICollection<Coach> coaches)
        {
            if (coaches == null)
                throw new Exception("Team's coaches cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a collection of matches is valid.
        /// </summary>
        /// <param name="matches">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidMatches(ICollection<Match.Match> matches)
        {
            if (matches == null)
                throw new Exception("Team's matches cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a collection of training sessions is valid.
        /// </summary>
        /// <param name="sessions">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidTrainingSessions(ICollection<TrainingSession.TrainingSession> sessions)
        {
            if (sessions == null)
                throw new Exception("Team's training sessions cannot be null.");

            return true;
        }
    }
}
