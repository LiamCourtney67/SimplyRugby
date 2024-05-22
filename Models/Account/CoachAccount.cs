using SimplyRugby.Models.Member;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplyRugby.Models.Account
{
    /// <summary>
    /// Represents a coach account inheriting from the account class.
    /// </summary>
    public class CoachAccount : Account
    {
        // Private fields.
        private Coach _coach;
        private Team.Team _team;

        // Public properties.
        [ForeignKey("Coach")]
        public int? MemberID { get; set; }
        public int? TeamID { get; set; }
        public Coach Coach
        {
            get => _coach;
            set
            {
                if (ValidCoach(value))
                    _coach = value;
            }
        }
        public Team.Team Team
        {
            get => _team;
            set
            {
                if (ValidTeam(value))
                    _team = value;
            }
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
                throw new Exception("Account's coach cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a team is valid.
        /// </summary>
        /// <param name="team">Team to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Team is not valid.</exception>
        private bool ValidTeam(Team.Team team)
        {
            if (team == null)
                throw new Exception("Account's team cannot be null.");

            return true;
        }
    }
}
