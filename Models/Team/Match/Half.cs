using System;

namespace SimplyRugby.Models.Team.Match
{
    /// <summary>
    /// Represents a half of a match.
    /// </summary>
    public class Half
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private int _teamScore;
        private int _opponentScore;
        private string _comments;
        private DateTime _datePlayed;
        private bool _futureHalf => DateTime.Now < _datePlayed;

        // Public properties.
        public int MatchID { get; set; }
        public int TeamScore
        {
            get => _teamScore;
            set
            {
                if (ValidScore(value))
                    _teamScore = value;
            }
        }
        public int OpponentScore
        {
            get => _opponentScore;
            set
            {
                if (ValidScore(value))
                    _opponentScore = value;
            }
        }
        public string? Comments
        {
            get => _comments;
            set
            {
                if (ValidComments(value.Trim()))
                    _comments = value.Trim();
            }
        }
        public DateTime DatePlayed
        {
            get => _datePlayed;
            set
            {
                if (ValidDatePlayed(value))
                    _datePlayed = value;
            }
        }
        public string Score => $"{TeamScore} - {OpponentScore}";    // This may redundantly store the score, but it is useful for display purposes

        /// <summary>
        /// Initializes a new instance of the <see cref="Half"/> class.
        /// </summary>
        public Half()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Checks if a score int is valid.
        /// </summary>
        /// <param name="score">Score to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Score is not valid.</exception>
        private bool ValidScore(int score)
        {
            // Check if the match is in the future and the score is not set.
            if (_futureHalf && score.Equals(Int32.MinValue))
                return true;

            // Check if the match is in the future and the score is set.
            if (_futureHalf && !score.Equals(Int32.MinValue))
                throw new Exception("Score cannot be set for a future match.");

            // Check if the score is between 0 and 99.
            if (score < 0 || score > 99)
                throw new Exception("Score must be between 0 and 99.");

            return true;
        }

        /// <summary>
        /// Checks if comments are valid.
        /// </summary>
        /// <param name="comments">Comments to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Comments are not valid.</exception>
        private bool ValidComments(string comments)
        {
            // Check if comments are set for a future match.
            if (_futureHalf && !string.IsNullOrEmpty(comments))
                throw new Exception("Half comments cannot be set for a future match.");

            // Check if the comments are within the length constraints, (using Twitter's 280 char limit for reference).
            if (comments.Length > 280)
                throw new Exception("Half comments must be less than 280 characters.");

            // Special characters are allowed.

            // Check for consecutive spaces.
            if (comments.Contains("  "))
                throw new Exception("Half comments cannot contain consecutive spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(comments))
                throw new Exception("Half comments contain profanity.");

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
    }
}
