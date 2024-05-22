using SimplyRugby.Data;
using SimplyRugby.Models.Account;

namespace SimplyRugby.Services
{
    /// <summary>
    /// A singleton class to manage accounts.
    /// </summary>
    public class AccountManager
    {
        private static AccountManager _instance;                                        // The singleton instance
        public Account CurrentAccount { get; private set; }                             // The current account property
        public static AccountManager Instance => _instance ??= new AccountManager();    // The singleton instance property

        /// <summary>
        /// Logs in the user.
        /// </summary>
        /// <param name="username">User's username.</param>
        /// <param name="password">User's password.</param>
        public void Login(string username, string password)
        {
            // Logic to authenticate and set the current account
            CurrentAccount = AuthenticateAccount(username, password);
        }

        /// <summary>
        /// Logs out the user.
        /// </summary>
        public void Logout()
        {
            // Logic to handle user logout
            CurrentAccount = null;
        }

        /// <summary>
        /// Authenticates an account.
        /// </summary>
        /// <param name="username">User's username.</param>
        /// <param name="password">User's password.</param>
        /// <returns>User's account.</returns>
        public Account AuthenticateAccount(string username, string password)
        {
            using (var context = new SimplyRugbyContext())
            {
                // Attempt to retrieve the account by username.
                AccountService accountService = new AccountService();
                Account account = accountService.GetAccount(username);

                // The password is correct.
                if (account != null && account.VerifyPassword(password))
                    return account;

                // Authentication failed.
                return null;
            }
        }
    }

}
