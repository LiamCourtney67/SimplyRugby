using SimplyRugby.Models.Team.TrainingSession;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplyRugby.Models.Member
{
    public class Player : Member
    {
        // Profanity filter.
        private ProfanityFilter.ProfanityFilter filter;

        // Private fields.
        private ICollection<Position> _positions;
        private NextOfKin? _nextOfKin;
        private Doctor _doctor;
        private string _healthConditions;
        private ICollection<TrainingSession>? _trainingSessions;

        // Public properties.
        public required ICollection<Position> Positions
        {
            get => _positions;
            set
            {
                if (ValidPositions(value))
                    _positions = value;
            }
        }
        public NextOfKin? NextOfKin
        {
            get => _nextOfKin;
            set
            {
                if (ValidNextOfKin(value))
                    _nextOfKin = value;
            }
        }
        public required Doctor Doctor
        {
            get => _doctor;
            set
            {
                if (ValidDoctor(value))
                    _doctor = value;
            }
        }
        public required string HealthConditions
        {
            get => _healthConditions;
            set
            {
                if (ValidHealthConditions(value.Trim()))
                    _healthConditions = value.Trim();
            }
        }
        public ICollection<TrainingSession>? TrainingSessions
        {
            get => _trainingSessions;
            set
            {
                if (ValidTrainingSessions(value))
                    _trainingSessions = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
            // Initialize the profanity filter.
            this.filter = new ProfanityFilter.ProfanityFilter();
        }

        /// <summary>
        /// Checks if a collection of postions is valid.
        /// </summary>
        /// <param name="positions">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidPositions(ICollection<Position> positions)
        {
            if (positions == null)
                throw new Exception("Positions cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a next of kin is valid.
        /// </summary>
        /// <param name="account">Next of Kin to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Next of Kin is not valid.</exception>
        private bool ValidNextOfKin(NextOfKin nextOfKin)
        {
            if (nextOfKin == null)
                throw new Exception("Next of Kin cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a doctor is valid.
        /// </summary>
        /// <param name="doctor">Doctor to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Doctor is not valid.</exception>
        private bool ValidDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new Exception("Doctor cannot be null.");

            return true;
        }

        /// <summary>
        /// Checks if a health conditions string is valid.
        /// </summary>
        /// <param name="healthConditions">HealthConditions to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">HealthConditions are not valid.</exception>
        private bool ValidHealthConditions(string healthConditions)
        {
            // Check if the health conditions are null.
            if (healthConditions == null)
                return true;

            // Check if the health conditions are within the length constraints.
            if (healthConditions.Length < 0 || healthConditions.Length > 50)
                throw new Exception("Health conditions must be between 0 and 50 characters.");

            // Check for invalid characters (anything that's not a letter, or a space).
            if (healthConditions.Any(ch => !char.IsLetter(ch) && ch != ' '))
                throw new Exception("Health conditions can only contain letters, or single spaces.");

            // Check for consecutive spaces.
            if (healthConditions.Contains("  "))
                throw new Exception("Health conditions cannot contain consecutive spaces.");

            // Optionally, check for profanity.
            if (filter.IsProfanity(healthConditions))
                throw new Exception("Health conditions contain profanity.");

            return true;
        }

        /// <summary>
        /// Checks if a collection of training sessions is valid.
        /// </summary>
        /// <param name="sessions">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidTrainingSessions(ICollection<TrainingSession> sessions)
        {
            if (sessions == null)
                throw new Exception("Player's training sessions cannot be null.");

            return true;
        }
    }
}
