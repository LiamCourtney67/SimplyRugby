using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimplyRugby.Data;
using SimplyRugby.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplyRugby.Services
{
    /// <summary>
    /// Service for account operations.
    /// </summary>
    public class AccountService
    {
        /// <summary>
        /// Adds an account to the database.
        /// </summary>
        /// <param name="account">Account to be added.</param>
        /// <exception cref="Exception">Username is already taken.</exception>
        public async Task AddAccountAsync(Account account)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if the username is already taken
                if (context.Accounts.Any(a => a.Username == account.Username))
                    throw new Exception("Username is already taken.");

                var entity = context.Accounts.Add(account);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets an account by username from the database.
        /// </summary>
        /// <param name="username">Username of the account.</param>
        /// <returns>Account with the chosen username.</returns>
        public Account GetAccount(string username)
        {
            using (var context = new SimplyRugbyContext())
            {
                var account = context.Accounts
                    .FirstOrDefault(a => a.Username == username);

                if (account is CoachAccount coachAccount)
                {
                    // Manually load Coach and Team if they are not loaded
                    if (coachAccount.Coach == null && coachAccount.MemberID.HasValue)
                        coachAccount.Coach = context.Coaches.Find(coachAccount.MemberID);

                    if (coachAccount.Team == null && coachAccount.TeamID.HasValue)
                        coachAccount.Team = context.Teams.Find(coachAccount.TeamID.Value);

                    return coachAccount;
                }

                return account;
            }
        }

        /// <summary>
        /// Updates an account in the database.
        /// </summary>
        /// <param name="account">Updated account.</param>
        public async Task UpdateAccountAsync(Account account)
        {
            using (var context = new SimplyRugbyContext())
            {
                context.Accounts.Update(account);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates an account's password in the database.
        /// </summary>
        /// <param name="username">Username of account to update password for.</param>
        /// <param name="password">Updated password.</param>
        /// <exception cref="Exception">Account not found.</exception>
        public async Task UpdatePasswordAsync(string username, string password)
        {
            using (var context = new SimplyRugbyContext())
            {
                var account = context.Accounts.FirstOrDefault(a => a.Username == username);

                if (account == null)
                    throw new Exception("Account not found.");

                account.Password = password;

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes an account from the database.
        /// </summary>
        /// <param name="username">Username of the account.</param>
        /// <exception cref="Exception">Account not found.</exception>
        public async Task DeleteAccountAsync(string username)
        {
            using (var context = new SimplyRugbyContext())
            {
                var account = context.Accounts.FirstOrDefault(a => a.Username == username);

                // Check if the account exists
                if (account == null)
                    throw new Exception("Account not found.");

                // Check if the account is the master account
                if (account.Username.Equals("Admin"))
                    throw new Exception("Cannot delete the master account.");

                // If the account is a coach account, remove the link to the coach
                if (account is CoachAccount coachAccount)
                {
                    if (coachAccount.Coach != null)
                    {
                        coachAccount.Coach.Account = null;
                        context.Coaches.Update(coachAccount.Coach);
                    }
                }

                context.Accounts.Remove(account);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets all accounts from the database.
        /// </summary>
        /// <returns>List of accounts.</returns>
        public List<Account> GetAllAccounts()
        {
            using (var context = new SimplyRugbyContext())
            {
                // Don't add 'Admin' account to the list
                return context.Accounts.Where(a => a.Username != "Admin").ToList();
            }
        }

        /// <summary>
        /// Updates an account to a coach account.
        /// </summary>
        /// <param name="username">Username of the account.</param>
        /// <param name="coach">Coach to be attached to the account.</param>
        public async Task UpdateToCoachAccountAsync(string username, Models.Member.Coach coach)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Check if the coach is already linked to an account
                if (context.Accounts.OfType<CoachAccount>().Any(ca => ca.MemberID == coach.MemberID))
                    throw new Exception($"{coach.Name} is already linked to an account.");

                // Update the account to a CoachAccount
                string sql = "UPDATE Accounts SET AccountType = 'Coach', TeamID = @teamID, MemberID = @memberID, AccessLevel= null WHERE Username = @username";

                // Execute raw SQL command
                int affectedRows = context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@teamID", coach.TeamID),
                    new SqlParameter("@memberID", coach.MemberID),
                    new SqlParameter("@username", username));
            }
        }

        /// <summary>
        /// Updates an account to an admin account.
        /// </summary>
        /// <param name="username">Username of the account.</param>
        public async Task UpdateToAdminAccountAsync(string username)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Update the account to an AdminAccount
                string sql = "UPDATE Accounts SET AccountType = 'Admin', AccessLevel = 'Full', TeamID = null, MemberID = null WHERE Username = @username";

                // Execute raw SQL command
                int affectedRows = context.Database.ExecuteSqlRaw(sql, new SqlParameter("@username", username));
            }
        }
    }
}
