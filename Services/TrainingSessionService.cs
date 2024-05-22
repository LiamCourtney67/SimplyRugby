using Microsoft.EntityFrameworkCore;
using SimplyRugby.Data;
using SimplyRugby.Models.Member;
using SimplyRugby.Models.Team.TrainingSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplyRugby.Services
{
    /// <summary>
    /// Service for training session operations.
    /// </summary>
    public class TrainingSessionService
    {
        /// <summary>
        /// Adds a training session to the database.
        /// </summary>
        /// <param name="session">Session to be added.</param>
        public async Task AddTrainingSessionAsync(TrainingSession session)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    if (session.Coach != null)
                        context.Coaches.Attach(session.Coach); // Attach the coach without marking as new

                    foreach (var player in session.Players)
                        context.Players.Attach(player); // Attach players without marking as new

                    context.TrainingSessions.Add(session);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        /// <summary>
        /// Gets a training session by ID from the database.
        /// </summary>
        /// <param name="id">Session ID</param>
        /// <returns>Training Session from database.</returns>
        public TrainingSession GetTrainingSession(int id)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.TrainingSessions
                        .Include(ts => ts.Coach)
                        .Include(ts => ts.Players)
                            .ThenInclude(p => p.Skills)
                        .FirstOrDefault(ts => ts.TrainingSessionID == id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all training sessions from the database for a team.
        /// </summary>
        /// <param name="teamID">TeamID to get sessions for.</param>
        /// <returns>List of Training Sessions for a chosen team from database.</returns>
        public List<TrainingSession> GetAllTrainingSessionsForTeam(int teamID)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.TrainingSessions
                        .Include(ts => ts.Team)
                        .Include(ts => ts.Coach)
                        .Include(ts => ts.Players)
                            .ThenInclude(p => p.Skills)
                        .Where(ts => ts.TeamID == teamID)
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a training session in the database.
        /// </summary>
        /// <param name="session">Updated session.</param>
        /// <param name="addedPlayers">List of players added to the session.</param>
        /// <param name="removedPlayers">List of players removed from the session</param>
        public async Task UpdateTrainingSessionAsync(TrainingSession session, List<Player> addedPlayers, List<Player> removedPlayers)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    // Get the current state of the training session from the database
                    var currentSession = context.TrainingSessions
                        .Include(ts => ts.Players)
                        .Include(ts => ts.Coach)
                        .SingleOrDefault(ts => ts.TrainingSessionID == session.TrainingSessionID);

                    if (currentSession != null)
                    {
                        // Remove the players that are no longer attending the training session
                        foreach (var player in removedPlayers)
                        {
                            var currentPlayer = currentSession.Players.FirstOrDefault(p => p.MemberID == player.MemberID);
                            if (currentPlayer != null)
                                currentSession.Players.Remove(currentPlayer);
                        }

                        // Add new players to the training session
                        foreach (var player in addedPlayers)
                        {
                            if (!currentSession.Players.Any(p => p.MemberID == player.MemberID))
                            {
                                // Attach the player to the context and add to the training session
                                context.Players.Attach(player);
                                currentSession.Players.Add(player);
                            }
                        }

                        // Apply any other updates to the training session properties
                        context.Entry(currentSession).CurrentValues.SetValues(session);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a training session from the database.
        /// </summary>
        /// <param name="id">Session ID to be deleted.</param>
        public async Task DeleteTrainingSessionAsync(int id)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    var session = context.TrainingSessions.Find(id);
                    context.TrainingSessions.Remove(session);
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
