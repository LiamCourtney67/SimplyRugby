using SimplyRugby.Data;
using SimplyRugby.Models.Team.Match;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplyRugby.Services
{
    /// <summary>
    /// Service for match operations.
    /// </summary>
    public class MatchService
    {
        /// <summary>
        /// Adds a match to the database.
        /// </summary>
        /// <param name="match">Match to be added.</param>
        public async Task AddMatchAsync(Match match)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    await context.Matches.AddAsync(match);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        /// <summary>
        /// Gets a match by ID from the database.
        /// </summary>
        /// <param name="id">Match ID.</param>
        /// <returns>Match from database.</returns>
        public Match GetMatch(int id)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.Matches.FirstOrDefault(m => m.MatchID == id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all matches from the database for a team.
        /// </summary>
        /// <param name="teamID">Team ID for matches.</param>
        /// <returns>List of matches for th chosen team.</returns>
        public List<Match> GetAllMatchesForTeam(int teamID)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.Matches.Where(m => m.TeamID == teamID).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a match in the database.
        /// </summary>
        /// <param name="match">Updated match.</param>
        public async Task UpdateMatchAsync(Match match)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    context.Matches.Update(match);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a match from the database.
        /// </summary>
        /// <param name="id">Match ID to be deleted.</param>
        public async Task DeleteMatchAsync(int id)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    var match = context.Matches.Find(id);
                    context.Matches.Remove(match);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
