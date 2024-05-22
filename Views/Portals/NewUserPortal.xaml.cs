using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SimplyRugby.Services;
using SimplyRugby.Views.Account;
using System;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Portals
{
    /// <summary>
    /// New user portal page.
    /// </summary>
    public sealed partial class NewUserPortal : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewUserPortal"/> class.
        /// </summary>
        public NewUserPortal()
        {
            this.InitializeComponent();

            // Set the account username.
            AccountUsername.Content = $"{AccountManager.Instance.CurrentAccount.Username}'s Account";
        }

        /// <summary>
        /// Event handler for the item invoked event of the new user navigation view.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the item invoked event.</param>
        private void NewUserNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer.Tag is not null)
            {
                var itemTag = args.InvokedItemContainer.Tag.ToString();
                NavigateToPage(itemTag);
            }
        }

        /// <summary>
        /// Navigates to the selected page.
        /// </summary>
        /// <param name="pageTag">Tag for the invoked item.</param>
        private void NavigateToPage(string pageTag)
        {
            switch (pageTag)
            {
                // Help
                case "Help":
                    // Deselect the help button.
                    NewUserNav.SelectedItem = null;

                    // Open the help document.
                    OpenHelp();
                    break;

                // Account
                case "ViewAccount":
                    NewUserFrame.Navigate(typeof(ViewAccount));
                    break;

                case "Logout":
                    // Log out and navigate to the login page.
                    AccountManager.Instance.Logout();
                    MainWindow.Frame.Navigate(typeof(Login));
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Opens the help document.
        /// </summary>
        private async void OpenHelp()
        {
            try
            {
                // Link to the help document, change as needed
                Uri pdfUri = new Uri("file:///C:/Users/liamc/OneDrive%20-%20Dundee%20and%20Angus%20College/Documents/Software%20Development%20Year%202/Graded%20Unit/User%20Guide.pdf");

                // Open the help document.
                await Launcher.LaunchUriAsync(pdfUri);

                // Change the help button icon back to the help icon.
                HelpButtonIcon.Glyph = "\uE946";
                HelpButtonIcon.Foreground = new SolidColorBrush(Colors.White);
            }
            catch (Exception)
            {
                // Change the help button icon to a red 'X' if the help document cannot be opened.
                HelpButtonIcon.Glyph = "\uE711";
                HelpButtonIcon.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
