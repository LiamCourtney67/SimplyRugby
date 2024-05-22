using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Models.Account;
using SimplyRugby.Services;
using SimplyRugby.Views.Portals;
using System;

namespace SimplyRugby.Views.Account
{
    /// <summary>
    /// Contains the logic for the Login page.
    /// </summary>
    public sealed partial class Login : Page
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Login"/> class.
        /// </summary>
        public Login()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Logs the user in and navigates to the appropriate portal.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the username and password
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            // Attempt to log the user in
            LoginUser(username, password);
        }

        /// <summary>
        /// Logs the user in and navigates to the appropriate portal.
        /// </summary>
        /// <param name="username">Username for the account.</param>
        /// <param name="password">Password for the account.</param>
        private void LoginUser(string username, string password)
        {
            try
            {
                // Retrieve the account
                AccountManager.Instance.Login(username, password);

                // Navigate to the appropriate portal
                NavigateToPortal(AccountManager.Instance.CurrentAccount);
            }
            catch (Exception)
            {
                // Display an error message
                ErrorText.Text = "There is an issue connecting to the database.";
            }
        }

        /// <summary>
        /// Naviates to the appropriate portal based on the account type.
        /// </summary>
        /// <param name="account">The user's Account.</param>
        private void NavigateToPortal(Models.Account.Account account)
        {
            // Navigate to the appropriate portal
            if (account != null)
            {
                switch (account)
                {
                    case AdminAccount admin:
                        // Navigate to the admin portal
                        MainWindow.Frame.Navigate(typeof(AdminPortal));
                        break;

                    case CoachAccount coach:
                        // Navigate to the coach portal
                        MainWindow.Frame.Navigate(typeof(CoachPortal));
                        break;

                    default:
                        // Navigate to the new user portal
                        MainWindow.Frame.Navigate(typeof(NewUserPortal));
                        break;
                }
            }
            else
            {
                // Incorrect username or password
                ErrorText.Text = "Invalid username or password.";
                PasswordBox.ClearValue(PasswordBox.PasswordProperty);
            }
        }

        /// <summary>
        /// Navigates to the CreateAccount page.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CreateAccountButton_Click(object sender, RoutedEventArgs e) => MainWindow.Frame.Navigate(typeof(CreateAccount));
    }
}
