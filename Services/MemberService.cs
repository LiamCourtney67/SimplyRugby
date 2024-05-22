using SimplyRugby.Data;
using SimplyRugby.Models.Account;
using SimplyRugby.Models.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplyRugby.Services
{
    /// <summary>
    /// Service for member operations.
    /// </summary>
    public class MemberService
    {
        /// <summary>
        /// Adds a member to the database.
        /// </summary>
        /// <param name="member">Member to be added</param>
        /// <exception cref="Exception">SRU Number already exists.</exception>
        public async Task AddMemberAsync(Member member)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if SRU Number already exists.
                if (context.Members.Any(m => m.SRUNumber == member.SRUNumber) ||
                    context.Players.Any(p => p.SRUNumber == member.SRUNumber) ||
                    context.JuniorPlayers.Any(jp => jp.SRUNumber == member.SRUNumber) ||
                    context.Coaches.Any(c => c.SRUNumber == member.SRUNumber))
                {
                    throw new Exception("SRU Number already exists.");
                }

                try
                {
                    context.Members.Add(member);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets a member by ID from the database.
        /// </summary>
        /// <param name="id">Member ID<./param>
        /// <returns>Member from the database.</returns>
        public Member GetMember(int id)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    return context.Members.FirstOrDefault(m => m.MemberID == id);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets all members from the database.
        /// </summary>
        /// <returns>List of all members from the database.</returns>
        public List<Member> GetAllMembers()
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    return context.Members.Where(m => m.TeamID == null).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates a member in the database.
        /// </summary>
        /// <param name="member">Updated member.</param>
        /// <exception cref="Exception">SRU Number already exists.</exception>
        public async Task UpdateMemberAsync(Member member)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if SRU Number already exists and is not the current member.
                if (context.Members.Any(m => m.SRUNumber == member.SRUNumber && m.MemberID != member.MemberID) ||
                    context.Players.Any(p => p.SRUNumber == member.SRUNumber && p.MemberID != member.MemberID) ||
                    context.JuniorPlayers.Any(jp => jp.SRUNumber == member.SRUNumber && jp.MemberID != member.MemberID) ||
                    context.Coaches.Any(c => c.SRUNumber == member.SRUNumber && c.MemberID != member.MemberID))
                {
                    throw new Exception("SRU Number already exists.");
                }

                try
                {
                    context.Members.Update(member);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a member from the database.
        /// </summary>
        /// <param name="memberID">ID of Member to be deleted.</param>
        public async Task DeleteMemberAsync(int memberID)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    var member = context.Members.FirstOrDefault(m => m.MemberID == memberID);
                    context.Members.Remove(member);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Adds a coach to the database.
        /// </summary>
        /// <param name="coach">Coach to be added</param>
        /// <exception cref="Exception">SRU Number already exists.</exception>
        public async Task AddCoachAsync(Coach coach)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if SRU Number already exists.
                if (context.Members.Any(m => m.SRUNumber == coach.SRUNumber) ||
                    context.Players.Any(p => p.SRUNumber == coach.SRUNumber) ||
                    context.JuniorPlayers.Any(jp => jp.SRUNumber == coach.SRUNumber) ||
                    context.Coaches.Any(c => c.SRUNumber == coach.SRUNumber))
                {
                    throw new Exception("SRU Number already exists.");
                }

                try
                {
                    context.Coaches.Add(coach);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets a coach by ID from the database.
        /// </summary>
        /// <param name="id">Member ID for coach.<./param>
        /// <returns>Coach from the database.</returns>
        public Coach GetCoach(int id)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    return context.Coaches.FirstOrDefault(c => c.MemberID == id);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets all coaches from the database for a team.
        /// </summary>
        /// <param name="teamID">TeamID to get coaches from.</param>
        /// <returns>List of coaches for a team from the database.</returns>
        public List<Coach> GetAllCoachesForTeam(int teamID)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    return context.Coaches.Where(c => c.TeamID == teamID).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets all coaches from the database without an account for a team.
        /// </summary>
        /// <param name="teamID">TeamID to get coaches from.</param>
        /// <returns>List of coaches for a team from the database.</returns>
        public List<Coach> GetAllCoachesWithoutAccountForTeam(int teamID)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    return context.Coaches.Where(c => c.TeamID == teamID && c.Account == null).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates a coach in the database.
        /// </summary>
        /// <param name="coach">Updated coach.</param>
        /// <exception cref="Exception">SRU Number already exists.</exception>
        public async Task UpdateCoachAsync(Coach coach)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if SRU Number already exists and is not the current coach.
                if (context.Members.Any(m => m.SRUNumber == coach.SRUNumber && m.MemberID != coach.MemberID) ||
                    context.Players.Any(p => p.SRUNumber == coach.SRUNumber && p.MemberID != coach.MemberID) ||
                    context.JuniorPlayers.Any(jp => jp.SRUNumber == coach.SRUNumber && jp.MemberID != coach.MemberID) ||
                    context.Coaches.Any(c => c.SRUNumber == coach.SRUNumber && c.MemberID != coach.MemberID))
                {
                    throw new Exception("SRU Number already exists.");
                }

                try
                {
                    context.Coaches.Update(coach);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a coach from the database.
        /// </summary>
        /// <param name="coachID">MemberID for coach to be deleted.</param>
        public async Task DeleteCoachAsync(int coachID)
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    var coach = context.Coaches.FirstOrDefault(c => c.MemberID == coachID);

                    // Check if the coach has an account and delete it
                    if (coach.Account != null)
                    {
                        var account = context.Accounts.OfType<CoachAccount>().FirstOrDefault(a => a.MemberID == coach.MemberID);
                        context.Accounts.Remove(account);
                    }

                    context.Coaches.Remove(coach);

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
