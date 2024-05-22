using System;
using System.Collections.Generic;

namespace SimplyRugby.Models.Member
{
    /// <summary>
    /// Represents a junior player inheriting from the player class.
    /// </summary>
    public class JuniorPlayer : Player
    {
        //Private fields.
        private bool _hasConsentForm;
        private ICollection<Guardian> _guardians;

        //Public properties.
        public required bool HasConsentForm { get => _hasConsentForm; set => _hasConsentForm = value; }
        public ICollection<Guardian>? Guardians
        {
            get => _guardians;
            set
            {
                if (ValidGuardians(value))
                    _guardians = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JuniorPlayer"/> class.
        /// </summary>
        public JuniorPlayer()
        {
            this.Guardians = new List<Guardian>();
        }

        /// <summary>
        /// Checks if a collection of Guardians is valid.
        /// </summary>
        /// <param name="guardians">Collection to be checked.</param>
        /// <returns>True if valid, an exception if not valid.</returns>
        /// <exception cref="Exception">Collection is not valid.</exception>
        private bool ValidGuardians(ICollection<Guardian> guardians)
        {
            if (guardians == null)
                throw new Exception("Guardians cannot be null.");

            if (guardians.Count > 2)
                throw new Exception("Guardians must only contain 2 Guardians.");

            return true;
        }
    }
}
