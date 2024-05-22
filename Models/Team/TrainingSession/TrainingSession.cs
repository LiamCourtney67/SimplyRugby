using SimplyRugby.Models.Member;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace SimplyRugby.Models.Team.TrainingSession
{
    /// <summary>
    /// Represents a training session.
    /// </summary>
    public class TrainingSession
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private DateTime _date;
        private string _location;
        private Team _team;
        private Coach? _coach;
        private ICollection<Player>? _players;
        private string _skillsAndActivities;
        private string _injuriesAndAccidents;

        // Public properties.
        public int TrainingSessionID { get; set; }
        public required DateTime Date
        {
            get => _date;
            set
            {
                if (ValidDatePlayed(value))
                    _date = value;      // Date and time of the session.
            }
        }
        public required string Location
        {
            get => _location;
            set
            {
                if (ValidLocation(value.Trim()))
                    _location = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());      // Capitalize the first letter of each word.
            }
        }
        public Team Team
        {
            get => _team;
            set
            {
                if (ValidTeam(value))
                    _team = value;
            }
        }
        public int TeamID { get; set; }
        public Coach? Coach
        {
            get => _coach;
            set
            {
                if (ValidCoach(value))
                    _coach = value;
            }
        }
        [ForeignKey("Members")]
        public int? CoachMemberID { get; set; }
        public ICollection<Player>? Players
        {
            get => _players;
            set
            {
                if (ValidPlayers(value))
                    _players = value;
            }
        }
        public string SkillsAndActivities
        {
            get => _skillsAndActivities;
            set
            {
                if (ValidSkills(value.Trim()))
                    _skillsAndActivities = value.Trim();
            }
        }
        public string InjuriesAndAccidents
        {
            get => _injuriesAndAccidents;
            set
            {
                if (ValidInjuries(value.Trim()))
                    _injuriesAndAccidents = value.Trim();
            }
        }
        public string Overview => $"Session on {Date.ToShortDateString()} at {Location}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingSession"/> class.
        /// </summary>
        public TrainingSession()
        {
            // Initialize the players list and profanity filter.
            Players = new List<Player>();
            filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Checks if the date is valid.
        /// </summary
        /// <param name="datePlayed">Date to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Date is not valid.</exception>
        private bool ValidDatePlayed(DateTime datePlayed)
        {
            // Check if the date is null
            if (datePlayed.Equals(DateTime.MinValue))
                throw new Exception("Session's date cannot be empty.");

            // Check if the date is over a year ago
            if (datePlayed < DateTime.Now.AddYears(-1))
                throw new Exception("Session's date cannot be over a year ago.");

            // Check if the date is in the future.
            if (datePlayed > DateTime.Now.AddYears(1))
                throw new Exception("Session's date cannot be more than a year away.");

            return true;
        }

        /// <summary>
        /// Checks if an location is valid.
        /// </summary>
        /// <param name="location">Location to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Location is not valid.</exception>
        private bool ValidLocation(string location)
        {
            // Check if the location is null or empty
            if (string.IsNullOrEmpty(location))
                throw new Exception("Location cannot be empty");

            // Check if the location is within the length constraints.
            if (location.Length < 2 || location.Length > 20)
                throw new Exception("Location must be between 2 and 20 characters.");

            // Check for invalid characters (anything that's not a letter, a space, an apostrophe, or a hyphen).
            if (location.Any(ch => !char.IsLetter(ch) && ch != ' ' && ch != '\'' && ch != '-'))
                throw new Exception("Location can only contain letters, single spaces, apostrophes, or hyphens.");

            // Check for consecutive special characters ('--', "''", or '  ').
            if (location.Contains("--") || location.Contains("''") || location.Contains("  "))
                throw new Exception("Location cannot contain consecutive special characters or spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(location))
                throw new Exception("Location contains profanity.");

            return true;
        }

        /// <summary>
        /// Checks if a coach is valid.
        /// </summary>
        /// <param name="coach">Coach to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Coach is not valid.</exception>
        private bool ValidCoach(Coach coach)
        {
            if (coach == null)
                throw new Exception("Session's coach cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a team is valid.
        /// </summary>
        /// <param name="team">Team to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Team is not valid.</exception>
        private bool ValidTeam(Team team)
        {
            if (team == null)
                throw new Exception("Session's team cannot be null.");

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
                throw new Exception("Session's players cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if skills are valid.
        /// </summary>
        /// <param name="skills">Skills to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Skills are not valid.</exception>
        private bool ValidSkills(string skills)
        {
            // Check if the skills are within the length constraints, (using Twitter's 280 char limit for reference).
            if (skills.Length > 280)
                throw new Exception("Session's skills must be less than 280 characters.");

            // Special characters are allowed.

            // Check for consecutive spaces.
            if (skills.Contains("  "))
                throw new Exception("Session's skills cannot contain consecutive spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(skills))
                throw new Exception("Session's conditions contain profanity.");

            return true;
        }

        /// <summary>
        /// Checks if injuries are valid.
        /// </summary>
        /// <param name="injuries">Injuries to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Injuries are not valid.</exception>
        private bool ValidInjuries(string injuries)
        {
            // Check if the injuries are within the length constraints, (using Twitter's 280 char limit for reference).
            if (injuries.Length > 280)
                throw new Exception("Session's injuries must be less than 280 characters.");

            // Special characters are allowed.

            // Check for consecutive spaces.
            if (injuries.Contains("  "))
                throw new Exception("Session's injuries cannot contain consecutive spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(injuries))
                throw new Exception("Session's conditions contain profanity.");

            return true;
        }
    }
}
