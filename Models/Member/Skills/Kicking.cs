using System;

namespace SimplyRugby.Models.Member.Skills
{
    /// <summary>
    /// Represents the kicking skills of a member.
    /// </summary>
    public class Kicking
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private int _drop;
        private int _punt;
        private int _grubber;
        private int _goal;

        private string? _dropComments;
        private string? _puntComments;
        private string? _grubberComments;
        private string? _goalComments;

        // Public properties.
        public int SkillsID { get; set; }
        public int Drop
        {
            get => _drop;
            set
            {
                if (ValidSkill(value))
                    _drop = value;
            }
        }
        public int Punt
        {
            get => _punt;
            set
            {
                if (ValidSkill(value))
                    _punt = value;
            }
        }
        public int Grubber
        {
            get => _grubber;
            set
            {
                if (ValidSkill(value))
                    _grubber = value;
            }
        }
        public int Goal
        {
            get => _goal;
            set
            {
                if (ValidSkill(value))
                    _goal = value;
            }
        }
        public string? DropComments
        {
            get => _dropComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _dropComments = value.Trim();
            }
        }
        public string? PuntComments
        {
            get => _puntComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _puntComments = value.Trim();
            }
        }
        public string? GrubberComments
        {
            get => _grubberComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _grubberComments = value.Trim();
            }
        }
        public string? GoalComments
        {
            get => _goalComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _goalComments = value.Trim();
            }
        }
        public double Average => CalculateAverage();

        /// <summary>
        /// Initializes a new instance of the <see cref="Kicking"/> class.
        /// </summary>
        public Kicking()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Sets the default skills values of 1.
        /// </summary>
        /// <returns>Fully initialised Kicking object, with default values of 1.</returns>
        public Kicking DefaultKicking()
        {
            this.Drop = 1;
            this.Punt = 1;
            this.Grubber = 1;
            this.Goal = 1;
            return this;
        }

        /// <summary>
        /// Calculates the average of the four skills.
        /// </summary>
        /// <returns>Average as a double.</returns>
        private double CalculateAverage() => (this.Drop + this.Punt + this.Grubber + this.Goal) / 4.0;

        /// <summary>
        /// Checks if a skill is a valid int, between 1 and 5.
        /// </summary>
        /// <param name="skill">Skill to be checked as an int.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Skill is not valid, out of range 1-5.</exception>
        private bool ValidSkill(int skill)
        {
            if (skill < 1 || skill > 5)
                throw new Exception("Skills must be between 1 and 5.");

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
            // Check if the comments are within the length constraints.
            if (comments.Length > 100)
                throw new Exception("Kicking comments must be less than 100 characters.");

            // Special characters are allowed.

            // Check for consecutive spaces.
            if (comments.Contains("  "))
                throw new Exception("Kicking comments cannot contain consecutive spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(comments))
                throw new Exception("Kicking comments contain profanity.");

            return true;
        }
    }
}
