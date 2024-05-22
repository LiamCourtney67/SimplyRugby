using System;

namespace SimplyRugby.Models.Member.Skills
{
    /// <summary>
    /// Represents the passing skills of a member.
    /// </summary>
    public class Passing
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private int _standard;
        private int _spin;
        private int _pop;

        private string? _standardComments;
        private string? _spinComments;
        private string? _popComments;

        // Public properties.
        public int SkillsID { get; set; }
        public int Standard
        {
            get => _standard;
            set
            {
                if (ValidSkill(value))
                    _standard = value;
            }
        }
        public int Spin
        {
            get => _spin;
            set
            {
                if (ValidSkill(value))
                    _spin = value;
            }
        }
        public int Pop
        {
            get => _pop;
            set
            {
                if (ValidSkill(value))
                    _pop = value;
            }
        }
        public string? StandardComments
        {
            get => _standardComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _standardComments = value.Trim();
            }
        }
        public string? SpinComments
        {
            get => _spinComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _spinComments = value.Trim();
            }
        }
        public string? PopComments
        {
            get => _popComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _popComments = value.Trim();
            }
        }
        public double Average => CalculateAverage();

        /// <summary>
        /// Initializes a new instance of the <see cref="Passing"/> class.
        /// </summary>
        public Passing()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Sets the default skills values of 1.
        /// </summary>
        /// <returns>Fully initialised Passing object, with default values of 1.</returns>
        public Passing DefaultPassing()
        {
            this.Standard = 1;
            this.Spin = 1;
            this.Pop = 1;
            return this;
        }

        /// <summary>
        /// Calculates the average of the three skills.
        /// </summary>
        /// <returns>Average as a double.</returns>
        private double CalculateAverage() => (this.Standard + this.Spin + this.Pop) / 3.0;

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
                throw new Exception("Passing comments must be less than 100 characters.");

            // Special characters are allowed.

            // Check for consecutive spaces.
            if (comments.Contains("  "))
                throw new Exception("Passing comments cannot contain consecutive spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(comments))
                throw new Exception("Passing comments contain profanity.");

            return true;
        }
    }
}
