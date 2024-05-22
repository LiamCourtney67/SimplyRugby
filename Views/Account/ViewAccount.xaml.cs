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
    /// Page for viewing and editing account details.
    /// </summary>
    public sealed partial class ViewAccount : Page
    {
        // UI elements
        private List<UIElement> viewFields = new List<UIElement>();
        private List<UIElement> editFields = new List<UIElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAccount"/> class.
        /// </summary>
        public ViewAccount()
        {
            this.InitializeComponent();
            InitializeUILists();
            PopulateAccountDetails(AccountManager.Instance.CurrentAccount);
        }

        /// <summary>
        /// Updates the account properties with the values from the edit fields.
        /// </summary>
        private void UpdateAccountProperties()
        {
            AccountManager.Instance.CurrentAccount.FirstName = FirstNameBox.Text;
            AccountManager.Instance.CurrentAccount.LastName = LastNameBox.Text;
            AccountManager.Instance.CurrentAccount.Email = EmailBox.Text;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Event Handlers

        /// <summary>
        /// Event handler for the delete button. Shows a dialog to confirm the deletion of the account.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void DeleteButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Create a TextBlock for formatting the content text
            TextBlock contentText = new TextBlock();
            contentText.Inlines.Add(new Run { Text = "Are you sure you want to delete " });
            contentText.Inlines.Add(new Run { Text = AccountManager.Instance.CurrentAccount.Username, FontWeight = FontWeights.Bold });
            contentText.Inlines.Add(new Run { Text = ", this cannot be undone?" });

            // Create the dialog
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

            // Event handler for the primary button click
            deleteDialog.PrimaryButtonClick += DeleteDialog_PrimaryButtonClick;

            await deleteDialog.ShowAsync();

        }

        /// <summary>
        /// Event handler for the delete dialog primary button click. Deletes the account and logs out the user.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await new AccountService().DeleteAccountAsync(AccountManager.Instance.CurrentAccount.Username);
                ErrorText.Text = "Account deleted.";
                AccountManager.Instance.Logout();
                MainWindow.Frame.Navigate(typeof(Login));
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the edit button. Changes the view to the edit.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ChangeViewToEdit();

        /// <summary>
        /// Event handler for the update button. Updates the account details and changes the view to the view.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void UpdateButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                UpdateAccountProperties();
                await new AccountService().UpdateAccountAsync(AccountManager.Instance.CurrentAccount);
                ChangeViewToView();
                ErrorText.Text = "Account updated.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the cancel button. Changes the view to the view view.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = string.Empty; // Clear the error text
            ChangeViewToView();
        }

        /// <summary>
        /// Event handler for the ChangePasswordButton click event to change the password when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // This method is quite long, but it is necessary to handle the password change process in a ContentDialog.

            // Get the selected account in an easier to use variable
            var account = AccountManager.Instance.CurrentAccount;

            bool isPasswordValid = false;
            bool arePasswordsMatching = false;

            // Create a password box for the current password
            PasswordBox currentPasswordBox = new PasswordBox
            {
                PlaceholderText = "Enter current password...",
                Margin = new Thickness(0, 0, 0, 10),
            };

            // Create a password box for the current password
            PasswordBox newPasswordBox = new PasswordBox
            {
                PlaceholderText = "Enter new password...",
                Margin = new Thickness(0, 0, 0, 10),
            };

            // Create a password box to confirm the current password
            PasswordBox confirmNewPasswordBox = new PasswordBox
            {
                PlaceholderText = "Confirm new password...",
                Margin = new Thickness(0, 0, 0, 10),
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
            contentPanel.Children.Add(currentPasswordBox);
            contentPanel.Children.Add(newPasswordBox);
            contentPanel.Children.Add(confirmNewPasswordBox);
            contentPanel.Children.Add(passwordResetErrorText);

            // Define the dialog outside the loop to use it repeatedly
            ContentDialog changePasswordDialog = new ContentDialog
            {
                Title = "Change Password",
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
                ContentDialogResult result = await changePasswordDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Check if the current password is correct
                    if (!BCrypt.Net.BCrypt.EnhancedVerify(currentPasswordBox.Password, account.Password))
                    {
                        currentPasswordBox.ClearValue(PasswordBox.PasswordProperty);                            // Clear the password box
                        newPasswordBox.ClearValue(PasswordBox.PasswordProperty);                                // Clear the password box
                        confirmNewPasswordBox.ClearValue(PasswordBox.PasswordProperty);                         // Clear the password box
                        passwordResetErrorText.Visibility = Visibility.Visible;                                 // Show the error message
                        passwordResetErrorText.Text = $"An error occurred: Current password is incorrect.";     // Populate the error message
                    }
                    // Check if the new password and confirm new password match
                    else if (!newPasswordBox.Password.Equals(confirmNewPasswordBox.Password))
                    {
                        currentPasswordBox.ClearValue(PasswordBox.PasswordProperty);                        // Clear the password box
                        newPasswordBox.ClearValue(PasswordBox.PasswordProperty);                            // Clear the password box
                        confirmNewPasswordBox.ClearValue(PasswordBox.PasswordProperty);                     // Clear the password box
                        passwordResetErrorText.Visibility = Visibility.Visible;                             // Show the error message
                        passwordResetErrorText.Text = $"An error occurred: New passwords do not match.";    // Populate the error message
                    }
                    // If all checks pass, update the password
                    else
                    {
                        arePasswordsMatching = true;
                    }

                    // Update the password if all checks pass
                    if (arePasswordsMatching)
                    {
                        try
                        {
                            await new AccountService().UpdatePasswordAsync(account.Username, newPasswordBox.Password);
                            ErrorText.Text = "Password updated.";
                            isPasswordValid = true; // Break the loop since the password is valid
                        }
                        catch (Exception ex)
                        {
                            currentPasswordBox.ClearValue(PasswordBox.PasswordProperty);        // Clear the password box
                            newPasswordBox.ClearValue(PasswordBox.PasswordProperty);            // Clear the password box
                            confirmNewPasswordBox.ClearValue(PasswordBox.PasswordProperty);     // Clear the password box
                            passwordResetErrorText.Visibility = Visibility.Visible;             // Show the error message
                            passwordResetErrorText.Text = $"An error occurred: {ex.Message}";   // Populate the error message
                        }
                    }
                }
                else
                {
                    break; // Exit the loop if user clicks "Cancel"
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Management

        /// <summary>
        /// Populates the account details in the view and edit sections.
        /// </summary>
        /// <param name="user">Account details to use.</param>
        private void PopulateAccountDetails(Models.Account.Account user)
        {
            // Populate the view details
            UsernameBlock.Text = user.Username;
            FirstNameBlock.Text = user.FirstName;
            LastNameBlock.Text = user.LastName;
            EmailBlock.Text = user.Email;

            // Populate the edit details
            FirstNameBox.Text = user.FirstName;
            LastNameBox.Text = user.LastName;
            EmailBox.Text = user.Email;
        }

        /// <summary>
        /// Changes the visibility of a list of UI elements.
        /// </summary>
        /// <param name="list">UI elements.</param>
        /// <param name="visibility">Visible or collapsed.</param>
        private void ChangeVisibility(List<UIElement> list, Visibility visibility)
        {
            foreach (var item in list)
                item.Visibility = visibility;
        }

        /// <summary>
        /// Changes the view to the edit view.
        /// </summary>
        private void ChangeViewToEdit()
        {
            ErrorText.Margin = new Thickness(0); // Oversight in the design, change the margin
            ChangeVisibility(viewFields, Visibility.Collapsed);
            ChangeVisibility(editFields, Visibility.Visible);
            ErrorText.Text = string.Empty; // Clear the error text
        }

        /// <summary>
        /// Changes the view to the view view.
        /// </summary>
        private void ChangeViewToView()
        {
            ErrorText.Margin = new Thickness(10); // Oversight in the design, change the margin
            ChangeVisibility(editFields, Visibility.Collapsed);
            PopulateAccountDetails(AccountManager.Instance.CurrentAccount);
            ChangeVisibility(viewFields, Visibility.Visible);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Initialization (keeping UI elements in lists for easier management)

        /// <summary>
        /// Initializes the UI lists for easier management of UI elements.
        /// </summary>
        private void InitializeUILists()
        {
            InitializeViewFieldsList();
            InitializeEditFieldsList();
        }

        /// <summary>
        /// Initializes the view fields list.
        /// </summary>
        private void InitializeViewFieldsList()
        {
            viewFields.Add(UsernameBlock);
            viewFields.Add(FirstNameBlock);
            viewFields.Add(LastNameBlock);
            viewFields.Add(EmailBlock);
            viewFields.Add(EditButton);
            viewFields.Add(DeleteButton);
        }

        /// <summary>
        /// Initializes the edit fields list.
        /// </summary>
        private void InitializeEditFieldsList()
        {
            editFields.Add(UsernameBlock);
            editFields.Add(FirstNameBox);
            editFields.Add(LastNameBox);
            editFields.Add(EmailBox);
            editFields.Add(UpdateButton);
            editFields.Add(ChangePasswordButton);
            editFields.Add(CancelButton);
        }
    }
}
