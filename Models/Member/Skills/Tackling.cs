using System;

namespace SimplyRugby.Models.Member.Skills
{
    /// <summary>
    /// Represents the tackling skills of a member.
    /// </summary>
    public class Tackling
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private int _front;
        private int _rear;
        private int _side;
        private int _scramble;

        private string? _frontComments;
        private string? _rearComments;
        private string? _sideComments;
        private string? _scrambleComments;

        // Public properties.
        public int SkillsID { get; set; }
        public int Front
        {
            get => _front;
            set
            {
                if (ValidSkill(value))
                    _front = value;
            }
        }
        public int Rear
        {
            get => _rear;
            set
            {
                if (ValidSkill(value))
                    _rear = value;
            }
        }
        public int Side
        {
            get => _side;
            set
            {
                if (ValidSkill(value))
                    _side = value;
            }
        }
        public int Scramble
        {
            get => _scramble;
            set
            {
                if (ValidSkill(value))
                    _scramble = value;
            }
        }
        public string? FrontComments
        {
            get => _frontComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _frontComments = value.Trim();
            }
        }
        public string? RearComments
        {
            get => _rearComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _rearComments = value.Trim();
            }
        }
        public string? SideComments
        {
            get => _sideComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _sideComments = value.Trim();
            }
        }
        public string? ScrambleComments
        {
            get => _scrambleComments;
            set
            {
                if (ValidComments(value.Trim()))
                    _scrambleComments = value.Trim();
            }
        }
        public double Average => CalculateAverage();

        /// <summary>
        /// Initializes a new instance of the <see cref="Tackling"/> class.
        /// </summary>
        public Tackling()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Sets the default skills values of 1.
        /// </summary>
        /// <returns>Fully initialised Tackling object, with default values of 1.</returns>
        public Tackling DefaultTackling()
        {
            this.Front = 1;
            this.Rear = 1;
            this.Side = 1;
            this.Scramble = 1;
            return this;
        }

        /// <summary>
        /// Calculates the average of the four skills.
        /// </summary>
        /// <returns>Average as a double.</returns>
        private double CalculateAverage() => (this.Front + this.Rear + this.Side + this.Scramble) / 4.0;

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
                throw new Exception("Tackling comments must be less than 100 characters.");

            // Special characters are allowed.

            // Check for consecutive spaces.
            if (comments.Contains("  "))
                throw new Exception("Tackling comments cannot contain consecutive spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(comments))
                throw new Exception("Tackling comments contain profanity.");

            return true;
        }
    }
}
