using SimplyRugby.Models.Account;
using SimplyRugby.Models.Team.TrainingSession;
using System;
using System.Collections.Generic;

namespace SimplyRugby.Models.Member
{
    /// <summary>
    /// Represents a coach inheriting from the member class.
    /// </summary>
    public class Coach : Member
    {
        // Private fields.
        private CoachAccount _account;
        private ICollection<TrainingSession> _sessions;

        // Public properties.
        public CoachAccount Account
        {
            get => _account;
            set
            {
                if (ValidAccount(value))
                    _account = value;
            }
        }
        public ICollection<TrainingSession>? TrainingSessions
        {
            get => _sessions;
            set
            {
                if (ValidTrainingSessions(value))
                    _sessions = value;
            }
        }

        /// <summary>
        /// Checks if an Account is valid.
        /// </summary>
        /// <param name="account">Account to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Account is not valid.</exception>
        private bool ValidAccount(CoachAccount account)
        {
            if (account == null)
                throw new Exception("Coach's account cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a collection of Training Sessions is valid.
        /// </summary>
        /// <param name="sessions">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidTrainingSessions(ICollection<TrainingSession> sessions)
        {
            if (sessions == null)
                throw new Exception("Coach's training sessions cannot be null.");

            return true;
        }
    }
}
