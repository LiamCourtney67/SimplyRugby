using System;

namespace SimplyRugby.Models.Member.Skills
{
    /// <summary>
    /// Represents the skills of a member.
    /// </summary>
    public class Skills
    {
        // Private fields.
        private Kicking _kicking;
        private Passing _passing;
        private Tackling _tackling;
        private Member _member;

        // Public properties.
        public int SkillsID { get; set; }
        public Kicking Kicking
        {
            get => _kicking;
            set
            {
                if (ValidKicking(value))
                    _kicking = value;
            }
        }
        public Passing Passing
        {
            get => _passing;
            set
            {
                if (ValidPassing(value))
                    _passing = value;
            }
        }
        public Tackling Tackling
        {
            get => _tackling;
            set
            {
                if (ValidTackling(value))
                    _tackling = value;
            }
        }
        public double Average => CalculateAverage();

        public int MemberID { get; set; }
        public Member Member
        {
            get => _member;
            set
            {
                if (ValidMember(value))
                    _member = value;
            }
        }

        /// <summary>
        /// Sets the default skills values of 1 for a member.
        /// </summary>
        /// <param name="member">Member who the Skills are attached to.</param>
        /// <returns>Fully initialised Skills object, with default values of 1.</returns>
        public Skills DefaultSkills(Member member)
        {
            this.Member = member;
            this.MemberID = member.MemberID;
            this.Kicking = new Kicking().DefaultKicking();
            this.Passing = new Passing().DefaultPassing();
            this.Tackling = new Tackling().DefaultTackling();
            return this;
        }

        /// <summary>
        /// Calculates the average of the three skills.
        /// </summary>
        /// <returns>Average as a double.</returns>
        private double CalculateAverage() => (this.Kicking.Average + this.Passing.Average + this.Tackling.Average) / 3.0;

        /// <summary>
        /// Checks if a Kicking object is valid.
        /// </summary>
        /// <param name="kicking">Kicking object to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Kicking object is not valid.</exception>
        private bool ValidKicking(Kicking kicking)
        {
            if (kicking == null)
                throw new Exception("Kicking cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a Passing object is valid.
        /// </summary>
        /// <param name="passing">Passing object to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Passing object is not valid.</exception>
        private bool ValidPassing(Passing passing)
        {
            if (passing == null)
                throw new Exception("Passing cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a Tackling object is valid.
        /// </summary>
        /// <param name="tackling">Tackling object to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Tackling object is not valid.</exception>
        private bool ValidTackling(Tackling tackling)
        {
            if (tackling == null)
                throw new Exception("Tackling cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a Member is valid.
        /// </summary>
        /// <param name="member">Member to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Member is not valid.</exception>
        private bool ValidMember(Member member)
        {
            if (member == null)
                throw new Exception("Member cannot be null.");

            return true;
        }
    }
}
