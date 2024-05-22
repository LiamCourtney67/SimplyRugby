using Microsoft.EntityFrameworkCore;
using SimplyRugby.Data;
using SimplyRugby.Models.Member;
using SimplyRugby.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplyRugby.Services
{
    /// <summary>
    /// Service for team operations.
    /// </summary>
    public class TeamService
    {
        /// <summary>
        /// Adds a team to the database.
        /// </summary>
        /// <param name="team">Team to be added.</param>
        /// <exception cref="Exception">Team name already exists.</exception>
        public async Task AddTeamAsync(Team team)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if team name already exists.
                if (context.Teams.Any(t => t.Name == team.Name))
                    throw new Exception("Team name already exists.");

                try
                {
                    context.Teams.Add(team);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets a team by ID from the database.
        /// </summary>
        /// <param name="id">Team ID.</param>
        /// <returns>Team from database.</returns>
        public Team GetTeam(int id)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.Teams
                    .Include(t => t.Players)
                        .ThenInclude(p => p.Skills)
                    .Include(t => t.Coaches)
                        .ThenInclude(c => c.Skills)
                    .Include(t => t.Matches)
                    .Include(t => t.TrainingSessions)
                    .FirstOrDefault(t => t.TeamID == id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all teams from the database.
        /// </summary>
        /// <returns>List of teams from the database.</returns>
        public List<Team> GetAllTeams()
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.Teams.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a team in the database.
        /// </summary>
        /// <param name="team">Updated team.</param>
        /// <exception cref="Exception">Team name already exists.</exception>
        public async Task UpdateTeamAsync(Team team)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if team name already exists.
                if (context.Teams.Any(t => t.Name == team.Name && t.TeamID != team.TeamID))
                    throw new Exception("Team name already exists.");

                try
                {
                    context.Teams.Update(team);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a team from the database.
        /// </summary>
        /// <param name="id">Team ID to be deleted.</param>
        public async Task DeleteTeamAsync(int id)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    var team = context.Teams
                    .Include(t => t.Players)
                        .ThenInclude(p => p.Skills)
                    .Include(t => t.Coaches)
                        .ThenInclude(c => c.Skills)
                    .Include(t => t.Matches)
                    .Include(t => t.TrainingSessions)
                    .FirstOrDefault(t => t.TeamID == id);

                    // Remove players.
                    if (team.Players != null && team.Players.Count > 0)
                    {
                        foreach (var player in team.Players)
                        {
                            if (player is JuniorPlayer juniorPlayer)
                                context.JuniorPlayers.Remove(juniorPlayer);
                            else
                                context.Players.Remove(player);
                        }
                    }

                    // Remove coaches, coach accounts, matches, and training sessions.
                    if (team.Coaches != null && team.Coaches.Count > 0)
                    {
                        foreach (var coach in team.Coaches)
                        {
                            if (coach.Account != null) context.Accounts.Remove(coach.Account);
                        }

                        context.Coaches.RemoveRange(team.Coaches);
                    }

                    if (team.Matches != null && team.Matches.Count > 0) context.Matches.RemoveRange(team.Matches);
                    if (team.TrainingSessions != null && team.TrainingSessions.Count > 0) context.TrainingSessions.RemoveRange(team.TrainingSessions);

                    // Remove the team.
                    context.Teams.Remove(team);
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
