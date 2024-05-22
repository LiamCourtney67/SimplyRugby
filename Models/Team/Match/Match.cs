using System;
using System.Globalization;
using System.Linq;

namespace SimplyRugby.Models.Team.Match
{
    /// <summary>
    /// Represents a match.
    /// </summary>
    public class Match
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private Team _team;
        private string _opponent;
        private DateTime _datePlayed;
        private string _location;
        private Half _firstHalf;
        private Half _secondHalf;

        // Public properties.
        public int MatchID { get; set; }
        public int TeamID { get; set; }

        public Team Team
        {
            get => _team;
            set
            {
                if (ValidTeam(value))
                    _team = value;
            }
        }
        public required string Opponent
        {
            get => _opponent;
            set
            {
                if (ValidOpponent(value.Trim()))
                    _opponent = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim());      // Capitalize the first letter of each word.
            }
        }
        public required DateTime DatePlayed
        {
            get => _datePlayed;
            set
            {
                if (ValidDatePlayed(value))
                {
                    _datePlayed = value;
                }
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

        public int TeamScore { get => FirstHalf.TeamScore + SecondHalf.TeamScore; } // This is a calculated property, based on the first and second half score
        public int OpponentScore { get => FirstHalf.OpponentScore + SecondHalf.OpponentScore; }  // This is a calculated property, based on the first and second half score
        public string Score => $"{TeamScore} - {OpponentScore}";    // This may redundantly store the score, but it is useful for display purposes
        public string Result => CalculateResult();                  // This is a calculated property, based on the score

        public required Half FirstHalf
        {
            get => _firstHalf;
            set
            {
                if (ValidHalf(value))
                    _firstHalf = value;
            }
        }
        public required Half SecondHalf
        {
            get => _secondHalf;
            set
            {
                if (ValidHalf(value))
                    _secondHalf = value;
            }
        }
        public bool FutureMatch => DateTime.Now < _datePlayed;

        public string Overview => SetOverview();    // Used to display a summary of the match, for selection in the UI

        /// <summary>
        /// Initializes a new instance of the <see cref="Match"/> class.
        /// </summary>
        public Match()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Checks if a team is valid.
        /// </summary>
        /// <param name="team">Team to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Team is not valid.</exception>
        private bool ValidTeam(Team team)
        {
            // Check if the team is null
            if (team == null)
                throw new Exception("Match's team cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if an opponent is valid.
        /// </summary>
        /// <param name="opponent">Opponent to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Opponent is not valid.</exception>
        private bool ValidOpponent(string opponent)
        {
            // Check if the opponent is null or empty
            if (string.IsNullOrEmpty(opponent))
                throw new Exception("Opponent name cannot be empty");

            // Check if the opponent is within the length constraints.
            if (opponent.Length < 2 || opponent.Length > 20)
                throw new Exception("Opponent name must be between 2 and 20 characters.");

            // Check for invalid characters (anything that's not a letter, a space, an apostrophe, or a hyphen).
            if (opponent.Any(ch => !char.IsLetter(ch) && ch != ' ' && ch != '\'' && ch != '-'))
                throw new Exception("Opponent name can only contain letters, single spaces, apostrophes, or hyphens.");

            // Check for consecutive special characters ('--', "''", or '  ').
            if (opponent.Contains("--") || opponent.Contains("''") || opponent.Contains("  "))
                throw new Exception("Opponent name cannot contain consecutive special characters or spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(opponent))
                throw new Exception("Opponent name contains profanity.");

            return true;
        }

        /// <summary>
        /// Checks if the date played is valid.
        /// </summary
        /// <param name="datePlayed">Date played to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Date played is not valid.</exception>
        private bool ValidDatePlayed(DateTime datePlayed)
        {
            // Check if the date played is null
            if (datePlayed.Equals(DateTime.MinValue))
                throw new Exception("Match's date played cannot be empty.");

            // Check if the date played is over a year ago
            if (datePlayed < DateTime.Now.AddYears(-1))
                throw new Exception("Match's date played cannot be over a year ago.");

            // Check if the date played is in the future.
            if (datePlayed > DateTime.Now.AddYears(1))
                throw new Exception("Match's date played cannot be more than a year away.");

            return true;
        }

        /// <summary>
        /// Checks if a location is valid.
        /// </summary>
        /// <param name="location">Location to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Location is not valid.</exception>
        private bool ValidLocation(string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new Exception("Match's location cannot be empty.");

            if (!location.Equals("Home") && !location.Equals("Away"))
                throw new Exception("Match's location must be either 'Home' or 'Away'.");

            return true;
        }

        /// <summary>
        /// Checks if a half is valid.
        /// </summary>
        /// <param name="half">Half to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Half is not valid.</exception>
        private bool ValidHalf(Half half)
        {
            // Check if the half is null
            if (half == null)
                throw new Exception("Match's half cannot be null.");

            return true;
        }

        /// <summary>
        /// Calculates the result of the match.
        /// </summary>
        /// <returns>Win, Loss, or Draw depending on the score</returns>
        private string CalculateResult()
        {
            switch (this.TeamScore.CompareTo(this.OpponentScore))
            {
                case 1:
                    return "Win";
                case -1:
                    return "Loss";
                default:
                    return "Draw";
            }
        }

        /// <summary>
        /// Sets the overview of the match.
        /// </summary>
        /// <returns>Overview, based on if it is a future match or not.</returns>
        private string SetOverview()
        {
            if (FutureMatch)
                return $"Playing {Opponent} on {DatePlayed.ToShortDateString()}";
            else
                return $"Played {Opponent} on {DatePlayed.ToShortDateString()}";
        }
    }
}
