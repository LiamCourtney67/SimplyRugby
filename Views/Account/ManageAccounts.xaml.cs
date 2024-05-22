using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SimplyRugby.Services;
using System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Account
{
    /// <summary>
    /// Page for updating accounts.
    /// </summary>
    public sealed partial class ManageAccounts : Page
    {
        // User selections
        private Models.Account.Account selectedAccount;
        private string selectedAccountType;
        private Models.Team.Team selectedTeam;
        private Models.Member.Coach selectedCoach;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageAccounts"/> class.
        /// </summary>
        public ManageAccounts()
        {
            this.InitializeComponent();

            // Set the account type selection
            AccountTypeSelect.ItemsSource = new List<string> { "Admin", "Coach" };

            // Set the account selection
            AccountSelect.ItemsSource = new AccountService().GetAllAccounts();
        }

        /// <summary>
        /// Event handler for the account selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void AccountSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reset the UI
            ResetUI();
            AccountTypeSelect.SelectedItem = null;  // Clear the account type selection

            // Clear previous messages if an account is selected
            if (AccountSelect.SelectedItem != null)
                ErrorText.Text = string.Empty;

            // Clear any previous errors from not selecting an account
            if (ErrorText.Text.Equals("Please select an account."))
                ErrorText.Text = string.Empty;

            // Set the selected account
            selectedAccount = (Models.Account.Account)AccountSelect.SelectedItem;

            // Enable the account type selection
            AccountTypeSelect.IsEnabled = true;
        }

        /// <summary>
        /// Event handler for the account type selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void AccountTypeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (AccountTypeSelect.SelectedItem)
            {
                // Admin account.
                case "Admin":
                    TeamSelect.Visibility = Visibility.Collapsed;
                    TeamSelectTitle.Visibility = Visibility.Collapsed;

                    CoachSelect.Visibility = Visibility.Collapsed;
                    CoachSelectTitle.Visibility = Visibility.Collapsed;

                    TeamSelect.SelectedItem = null;
                    CoachSelect.SelectedItem = null;

                    selectedAccountType = "Admin";
                    break;

                // Coach account.
                case "Coach":
                    TeamSelect.Visibility = Visibility.Visible;
                    TeamSelectTitle.Visibility = Visibility.Visible;

                    CoachSelect.Visibility = Visibility.Visible;
                    CoachSelectTitle.Visibility = Visibility.Visible;

                    TeamSelect.SelectedItem = null;
                    CoachSelect.SelectedItem = null;

                    // Set the team selection
                    TeamSelect.ItemsSource = new TeamService().GetAllTeams();

                    selectedAccountType = "Coach";
                    break;
            }

            // Clear any previous errors from not selecting an account type
            if (ErrorText.Text.Equals("Please select an account type."))
                ErrorText.Text = string.Empty;
        }

        /// <summary>
        /// Event handler for the team selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTeam = (Models.Team.Team)TeamSelect.SelectedItem;
            CoachSelect.SelectedItem = null;

            if (selectedTeam != null)
                CoachSelect.ItemsSource = new MemberService().GetAllCoachesWithoutAccountForTeam(selectedTeam.TeamID);

            // Clear any previous errors from not selecting a team
            if (ErrorText.Text.Equals("Please select a team."))
                ErrorText.Text = string.Empty;
        }

        /// <summary>
        /// Event handler for the coach selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void CoachSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCoach = (Models.Member.Coach)CoachSelect.SelectedItem;

            // Clear any previous errors from not selecting a coach
            if (ErrorText.Text.Equals("Please select a coach."))
                ErrorText.Text = string.Empty;
        }

        /// <summary>
        /// Event handler for the update account button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountType == "Admin")
            {
                ConfirmAdminUpdate();
            }
            else if (selectedAccountType == "Coach")
            {
                // Check if the user has selected a team and coach
                if (selectedTeam == null)
                {
                    ErrorText.Text = "Please select a team.";
                    return;
                }
                if (selectedCoach == null)
                {
                    ErrorText.Text = "Please select a coach.";
                    return;
                }

                try
                {
                    await new AccountService().UpdateToCoachAccountAsync(selectedAccount.Username, selectedCoach);
                    ErrorText.Text = $"{selectedAccount.Username} updated to Coach as {selectedCoach.Name}.";
                    ResetUI();
                    ResetInputFields();
                }
                catch (Exception ex)
                {
                    ErrorText.Text = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                // Check if the user has selected an account and account type
                if (selectedAccount == null)
                {
                    ErrorText.Text = "Please select an account.";
                    return;
                }
                if (string.IsNullOrEmpty(selectedAccountType))
                {
                    ErrorText.Text = "Please select an account type.";
                    return;
                }
            }
        }

        /// <summary>
        /// Confirm the admin update.
        /// </summary>
        private async void ConfirmAdminUpdate()
        {
            // Create a TextBlock for formatting the content text
            TextBlock contentText = new TextBlock();
            contentText.Inlines.Add(new Run { Text = "Are you sure you want to update " });
            contentText.Inlines.Add(new Run { Text = selectedAccount.Username, FontWeight = FontWeights.Bold });
            contentText.Inlines.Add(new Run { Text = " to admin, they will have full access to the system?" });
            contentText.TextWrapping = TextWrapping.Wrap;

            // Create the dialog
            ContentDialog updateDialog = new ContentDialog
            {
                Title = "Confirm Update",
                Content = contentText,
                PrimaryButtonText = "Update",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot,
                Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0x04, 0x4D, 0x85)),
                Foreground = new SolidColorBrush(Colors.White),
            };

            // Set the event handler for the primary button
            updateDialog.PrimaryButtonClick += UpdateDialog_PrimaryButtonClick;
            await updateDialog.ShowAsync();
        }

        /// <summary>
        /// Event handler for the update dialog primary button click.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void UpdateDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await new AccountService().UpdateToAdminAccountAsync(selectedAccount.Username);
                ErrorText.Text = $"{selectedAccount.Username} updated to Admin.";
                ResetUI();
                ResetInputFields();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the DeleteButton click event to delete the account when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void DeleteButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Check if the user has selected an account
            if (selectedAccount == null)
            {
                ErrorText.Text = "Please select an account.";
                return;
            }

            // Create a TextBlock for formatting the content text
            TextBlock contentText = new TextBlock();
            contentText.Inlines.Add(new Run { Text = "Are you sure you want to delete " });
            contentText.Inlines.Add(new Run { Text = selectedAccount.Username, FontWeight = FontWeights.Bold });
            contentText.Inlines.Add(new Run { Text = ", this cannot be undone?" });
            contentText.TextWrapping = TextWrapping.WrapWholeWords;

            // Create the delete dialog
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Confirm Deletion",
                Content = contentText,
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot,
                Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0x04, 0x4D, 0x85)),
                Foreground = new SolidColorBrush(Colors.White),
            };

            // Event handler for the PrimaryButtonClick event of the delete dialog to delete the training session when the button is clicked
            deleteDialog.PrimaryButtonClick += DeleteDialog_PrimaryButtonClick;

            await deleteDialog.ShowAsync();

        }

        /// <summary>
        /// Event handler for the PrimaryButtonClick event of the delete dialog to delete the account when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await new AccountService().DeleteAccountAsync(selectedAccount.Username);

                ResetInputFields();
                ResetUI();

                ErrorText.Text = "Account deleted.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the ResetPasswordButton click event to reset the password when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the user has selected an account
            if (selectedAccount == null)
            {
                ErrorText.Text = "Please select an account.";
                return;
            }

            bool isPasswordValid = false;

            // Create a password box for the new password
            PasswordBox passwordBox = new PasswordBox
            {
                PlaceholderText = "Enter new password",
            };

            // Create a TextBlock for displaying the error message
            TextBlock passwordResetErrorText = new TextBlock
            {
                Text = "",
                Margin = new Thickness(0, 10, 0, 0),
                FontStyle = Windows.UI.Text.FontStyle.Italic,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Visibility = Visibility.Collapsed,
            };

            // Create the StackPanel and add controls
            StackPanel contentPanel = new StackPanel();
            contentPanel.Children.Add(passwordBox);
            contentPanel.Children.Add(passwordResetErrorText);

            // Define the dialog outside the loop to use it repeatedly
            ContentDialog passwordResetDialog = new ContentDialog
            {
                Title = "Reset Password",
                Content = contentPanel,
                PrimaryButtonText = "Submit",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot,
                Background = new SolidColorBrush(ColorHelper.FromArgb(0xFF, 0x04, 0x4D, 0x85)),
                Foreground = new SolidColorBrush(Colors.White),
            };

            // Create a loop to keep showing the dialog until the password is valid or the user cancels (override the default on closing behavior not working as expected).
            while (!isPasswordValid)
            {
                ContentDialogResult result = await passwordResetDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    try
                    {
                        await new AccountService().UpdatePasswordAsync(selectedAccount.Username, passwordBox.Password);
                        ErrorText.Text = "Password updated.";
                        isPasswordValid = true; // Break the loop since the password is valid
                    }
                    catch (Exception ex)
                    {
                        passwordBox.ClearValue(PasswordBox.PasswordProperty);               // Clear the password box
                        passwordResetErrorText.Visibility = Visibility.Visible;             // Show the error message
                        passwordResetErrorText.Text = $"An error occurred: {ex.Message}";   // Populate the error message
                    }
                }
                else
                {
                    break; // Exit the loop if user clicks "Cancel"
                }
            }
        }

        /// <summary>
        /// Resets the input fields and selections.
        /// </summary>
        private void ResetUI()
        {
            // Reset the selections
            selectedAccount = null;
            selectedAccountType = null;
            selectedTeam = null;
            selectedCoach = null;

            // Reset the UI
            TeamSelect.Visibility = Visibility.Collapsed;
            TeamSelectTitle.Visibility = Visibility.Collapsed;

            CoachSelect.Visibility = Visibility.Collapsed;
            CoachSelectTitle.Visibility = Visibility.Collapsed;

            // Disable the account type selection
            AccountTypeSelect.IsEnabled = false;
        }

        /// <summary>
        /// Resets the input fields.
        /// </summary>
        private void ResetInputFields()
        {

            // Reset the input fields
            AccountSelect.SelectedItem = null;
            AccountTypeSelect.SelectedItem = null;
            TeamSelect.SelectedItem = null;
            CoachSelect.SelectedItem = null;

            // Disable the account type selection
            AccountTypeSelect.IsEnabled = false;

            // Set the account selection
            AccountSelect.ItemsSource = new AccountService().GetAllAccounts();
        }
    }
}