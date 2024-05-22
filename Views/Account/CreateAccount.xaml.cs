using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Services;
using System;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Account
{
    /// <summary>
    /// Contains the logic for the CreateAccount page.
    /// </summary>
    public sealed partial class CreateAccount : Page
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CreateAccount"/> class.
        /// </summary>
        public CreateAccount()
        {
            this.InitializeComponent();
        }

        private async void CreateAccountButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == ConfirmPasswordBox.Password)
            {
                try
                {
                    // Create a new account
                    await new AccountService().AddAccountAsync(CreateUserAccount());

                    // Redirect to the login page
                    MainWindow.Frame.Navigate(typeof(Login));
                }
                catch (Exception ex)
                {
                    // Display an error message
                    ErrorText.Text = $"An error occurred: {ex.Message}";

                    // Print the full exception to the debug output
                    Debug.WriteLine(ex.ToString());
                }
            }
            else
            {
                // Display an error message
                ErrorText.Text = "Passwords do not match.";
            }
        }

        /// <summary>
        /// Creates a new account based on the user input.
        /// </summary>
        /// <returns>Fully initialised Account object.</returns>
        private Models.Account.Account CreateUserAccount()
        {
            try
            {
                return new Models.Account.Account
                {
                    // Retrieve the user input
                    Username = UsernameBox.Text,
                    Password = PasswordBox.Password,
                    FirstName = FirstNameBox.Text,
                    LastName = LastNameBox.Text,
                    Email = EmailBox.Text
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Navigate back to the login page.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void ReturnToLoginButton_Click(object sender, RoutedEventArgs e) => MainWindow.Frame.Navigate(typeof(Login));
    }
}
