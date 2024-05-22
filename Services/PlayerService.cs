using Microsoft.EntityFrameworkCore;
using SimplyRugby.Data;
using SimplyRugby.Models.Member;
using SimplyRugby.Models.Member.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplyRugby.Services
{
    /// <summary>
    /// Service for player operations.
    /// </summary>
    public class PlayerService
    {
        /// <summary>
        /// Adds a player to the database.
        /// </summary>
        /// <param name="player">Player to be added.</param>
        /// <exception cref="Exception">SRU Number already exists.</exception>
        public async Task AddPlayerAsync(Player player)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if SRU Number already exists.
                if (context.Members.Any(m => m.SRUNumber == player.SRUNumber) ||
                    context.Players.Any(p => p.SRUNumber == player.SRUNumber) ||
                    context.JuniorPlayers.Any(jp => jp.SRUNumber == player.SRUNumber) ||
                    context.Coaches.Any(c => c.SRUNumber == player.SRUNumber))
                {
                    throw new Exception("SRU Number already exists.");
                }

                var strategy = context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Attach the player's navigation properties to the context
                            AddPositions(context, player.Positions);
                            await context.Doctors.AddAsync(player.Doctor);
                            await context.NextOfKins.AddAsync(player.NextOfKin);

                            // Add the player to the database
                            await context.Players.AddAsync(player);
                            await context.SaveChangesAsync();

                            // Add the player's skills to the database
                            await AddSkillsAsync(context, player);

                            await context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Adds a junior player to the database.
        /// </summary>
        /// <param name="player">Player to be added.</param>
        /// <exception cref="Exception">SRU Number already exists.</exception>
        public async Task AddJuniorPlayerAsync(JuniorPlayer player)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if SRU Number already exists.
                if (context.Members.Any(m => m.SRUNumber == player.SRUNumber) ||
                    context.Players.Any(p => p.SRUNumber == player.SRUNumber) ||
                    context.JuniorPlayers.Any(jp => jp.SRUNumber == player.SRUNumber) ||
                    context.Coaches.Any(c => c.SRUNumber == player.SRUNumber))
                {
                    throw new Exception("SRU Number already exists.");
                }

                var strategy = context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Attach the player's navigation properties to the context
                            AddPositions(context, player.Positions);
                            await context.Doctors.AddAsync(player.Doctor);
                            await AddGuardiansAsync(context, player.Guardians);

                            // Add the player to the database
                            await context.JuniorPlayers.AddAsync(player);
                            await context.SaveChangesAsync();

                            // Add the player's skills to the database
                            await AddSkillsAsync(context, player);

                            await context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Updates a player in the database.
        /// </summary>
        /// <param name="player">Updated player.</param>
        /// <param name="addedPositions">List of positions added.</param>
        /// <param name="removedPositions">List of positions removed.</param>
        /// <exception cref="Exception">SRU Number already exists.</exception>
        public async Task UpdatePlayerAsync(Player player, List<Position> addedPositions, List<Position> removedPositions)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if SRU Number already exists and is not the current player.
                if (context.Members.Any(m => m.SRUNumber == player.SRUNumber && m.MemberID != player.MemberID) ||
                    context.Players.Any(p => p.SRUNumber == player.SRUNumber && p.MemberID != player.MemberID) ||
                    context.JuniorPlayers.Any(jp => jp.SRUNumber == player.SRUNumber && jp.MemberID != player.MemberID) ||
                    context.Coaches.Any(c => c.SRUNumber == player.SRUNumber && c.MemberID != player.MemberID))
                {
                    throw new Exception("SRU Number already exists.");
                }

                try
                {
                    // Get the current state of the player from the database
                    var currentPlayer = context.Players
                        .Include(p => p.Positions)
                        .Include(p => p.NextOfKin)
                        .Include(p => p.Doctor)
                        .SingleOrDefault(p => p.MemberID == player.MemberID);

                    if (currentPlayer != null)
                    {
                        // Remove the positions that are no longer attached to the player
                        foreach (var position in removedPositions)
                        {
                            var currentPosition = currentPlayer.Positions.FirstOrDefault(p => p.Name == position.Name);
                            if (currentPosition != null)
                                currentPlayer.Positions.Remove(currentPosition);
                        }

                        // Add new positions to the player
                        foreach (var position in addedPositions)
                        {
                            if (!currentPlayer.Positions.Any(p => p.Name == position.Name))
                            {
                                // Attach the position to the context and add to the player
                                context.Positions.Attach(position);
                                currentPlayer.Positions.Add(position);
                            }
                        }

                        // Update the doctor and next of kin
                        if (currentPlayer.Doctor != null)
                            context.Entry(currentPlayer.Doctor).CurrentValues.SetValues(player.Doctor);

                        if (currentPlayer.NextOfKin != null)
                            context.Entry(currentPlayer.NextOfKin).CurrentValues.SetValues(player.NextOfKin);

                        // Apply any other updates to the training session properties
                        context.Entry(currentPlayer).CurrentValues.SetValues(player);
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
        /// Updates a junior player in the database.
        /// </summary>
        /// <param name="player">Updated player.</param>
        /// <param name="addedPositions">List of positions added.</param>
        /// <param name="removedPositions">List of positions removed.</param>
        /// <exception cref="Exception">SRU Number already exists.</exception>
        public async Task UpdateJuniorPlayerAsync(JuniorPlayer player, List<Position> addedPositions, List<Position> removedPositions)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if SRU Number already exists and is not the current player.
                if (context.Members.Any(m => m.SRUNumber == player.SRUNumber && m.MemberID != player.MemberID) ||
                    context.Players.Any(p => p.SRUNumber == player.SRUNumber && p.MemberID != player.MemberID) ||
                    context.JuniorPlayers.Any(jp => jp.SRUNumber == player.SRUNumber && jp.MemberID != player.MemberID) ||
                    context.Coaches.Any(c => c.SRUNumber == player.SRUNumber && c.MemberID != player.MemberID))
                {
                    throw new Exception("SRU Number already exists.");
                }

                try
                {
                    // Get the current state of the player from the database
                    var currentPlayer = context.JuniorPlayers
                        .Include(jp => jp.Positions)
                        .Include(jp => jp.Guardians)
                        .Include(jp => jp.Doctor)
                        .SingleOrDefault(jp => jp.MemberID == player.MemberID);

                    if (currentPlayer != null)
                    {
                        // Remove the positions that are no longer attached to the player
                        foreach (var position in removedPositions)
                        {
                            var currentPosition = currentPlayer.Positions.FirstOrDefault(p => p.Name == position.Name);
                            if (currentPosition != null)
                                currentPlayer.Positions.Remove(currentPosition);
                        }

                        // Add new positions to the player
                        foreach (var position in addedPositions)
                        {
                            if (!currentPlayer.Positions.Any(p => p.Name == position.Name))
                            {
                                // Attach the position to the context and add to the player
                                context.Positions.Attach(position);
                                currentPlayer.Positions.Add(position);
                            }
                        }

                        // Update the doctor and guardians
                        if (currentPlayer.Doctor != null)
                            context.Entry(currentPlayer.Doctor).CurrentValues.SetValues(player.Doctor);

                        if (currentPlayer.Guardians != null)
                        {
                            context.Entry(currentPlayer.Guardians.ElementAt(0)).CurrentValues.SetValues(player.Guardians.ElementAt(0));
                            context.Entry(currentPlayer.Guardians.ElementAt(1)).CurrentValues.SetValues(player.Guardians.ElementAt(1));
                        }

                        // Apply any other updates to the player properties
                        context.Entry(currentPlayer).CurrentValues.SetValues(player);
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
        /// Deletes a player from the database.
        /// </summary>
        /// <param name="player">Player to be deleted.</param>
        public async Task DeletePlayerAsync(Player player)
        {
            using (var context = new SimplyRugbyContext())
            {
                var strategy = context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            context.Players.Remove(player);
                            await context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Deletes a junior player from the database.
        /// </summary>
        /// <param name="player">Player to be deleted.</param>
        public async Task DeleteJuniorPlayerAsync(JuniorPlayer player)
        {
            using (var context = new SimplyRugbyContext())
            {
                var strategy = context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            context.JuniorPlayers.Remove(player);
                            await context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Gets a player from the database.
        /// </summary>
        /// <param name="id">Member ID for chose player.</param>
        /// <returns>Player from database.</returns>
        public Player GetPlayer(int id)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.Players
                        .Include(p => p.Doctor)
                        .Include(p => p.NextOfKin)
                        .Include(p => p.Positions)
                        .Include(p => p.Skills)
                        .FirstOrDefault(p => p.MemberID == id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all players from the database for a team.
        /// </summary>
        /// <param name="teamID">Team ID for players.</param>
        /// <returns>List of players for chosen team.</returns>
        public List<Player> GetAllPlayersForTeam(int teamID)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.Players
                        .Include(p => p.Doctor)
                        .Include(p => p.NextOfKin)
                        .Include(p => p.Positions)
                        .Include(p => p.Skills)
                        .Where(p => p.TeamID == teamID)
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a junior player from the database.
        /// </summary>
        /// <param name="id">Member ID for chose player.</param>
        /// <returns>Junior player from database.</returns>
        public JuniorPlayer GetJuniorPlayer(int id)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.JuniorPlayers
                        .Include(p => p.Doctor)
                        .Include(p => p.Guardians)
                        .Include(p => p.Positions)
                        .Include(p => p.Skills)
                        .FirstOrDefault(p => p.MemberID == id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all junior players from the database for a team.
        /// </summary>
        /// <param name="teamID">Team ID for players.</param>
        /// <returns>List of junior players for chosen team.</returns>
        public List<JuniorPlayer> GetAllJuniorPlayersForTeam(int teamID)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.JuniorPlayers
                        .Include(p => p.Doctor)
                        .Include(p => p.Guardians)
                        .Include(p => p.Positions)
                        .Include(p => p.Skills)
                        .Where(p => p.TeamID == teamID)
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds the default skills to a player.
        /// </summary>
        /// <param name="context">Simply Rugby DB Context</param>
        /// <param name="player">Player to add skills to</param>
        private async Task AddSkillsAsync(SimplyRugbyContext context, Player player) => await context.Skills.AddAsync(new Skills().DefaultSkills(player));

        /// <summary>
        /// Attach player's positions to the context.
        /// </summary>
        /// <param name="context">Simply Rugby DB Context</param>
        /// <param name="positions">List of positions to attach.</param>
        private void AddPositions(SimplyRugbyContext context, ICollection<Position> positions)
        {
            foreach (Position position in positions)
                context.Positions.Attach(position);
        }

        /// <summary>
        /// Add player's guardians to the context.
        /// </summary>
        /// <param name="context">Simply Rugby DB Context</param>
        /// <param name="guardians">List of guardians to add.</param>
        private async Task AddGuardiansAsync(SimplyRugbyContext context, ICollection<Guardian> guardians)
        {
            foreach (Guardian guardian in guardians)
                await context.Guardians.AddAsync(guardian);
        }

        /// <summary>
        /// Gets all possible positions from the database.
        /// </summary>
        /// <returns>List of possible positions.</returns>
        public List<Position> GetPossiblePositions()
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    return context.Positions.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a player's skills in the database.
        /// </summary>
        /// <param name="skills">Updated skills.</param>
        public async Task UpdateSkillsAsync(Skills skills)
        {
            try
            {
                using (var context = new SimplyRugbyContext())
                {
                    // Get the current state of the skills from the database
                    var currentSkills = context.Skills
                        .Include(s => s.Kicking)
                        .Include(s => s.Passing)
                        .Include(s => s.Tackling)
                        .FirstOrDefault(s => s.SkillsID == skills.SkillsID);

                    // Update the skills
                    if (currentSkills != null)
                    {
                        context.Entry(currentSkills).CurrentValues.SetValues(skills);
                        context.Entry(currentSkills.Kicking).CurrentValues.SetValues(skills.Kicking);
                        context.Entry(currentSkills.Passing).CurrentValues.SetValues(skills.Passing);
                        context.Entry(currentSkills.Tackling).CurrentValues.SetValues(skills.Tackling);

                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
