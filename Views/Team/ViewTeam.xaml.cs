using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SimplyRugby.Data;
using SimplyRugby.Services;
using SimplyRugby.Views.Account;
using System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Team
{
    /// <summary>
    /// Page for viewing and editing team details.
    /// </summary>
    public sealed partial class ViewTeam : Page
    {
        private Models.Team.Team selectedTeam;

        // UI elements
        private List<UIElement> titles = new List<UIElement>();
        private List<UIElement> viewFields = new List<UIElement>();
        private List<UIElement> editFields = new List<UIElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTeam"/> class.
        /// </summary>
        public ViewTeam()
        {
            this.InitializeComponent();

            InitializeUILists();
            HideTeamDetails();

            // Set the team selection based on the account type.
            TeamSelectionFromAccount();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getters and Setters

        /// <summary>
        /// Sets the team selection based on the account type.
        /// </summary>
        private void TeamSelectionFromAccount()
        {
            using (var context = new SimplyRugbyContext())
            {
                // Admin accounts can select any team.
                if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                {
                    TeamSelect.Visibility = Visibility.Visible;
                    TeamSelectTitle.Visibility = Visibility.Visible;

                    SetTeamSelect();
                }
                // Coach accounts can only select their team.
                else if (AccountManager.Instance.CurrentAccount is Models.Account.CoachAccount)
                {
                    TeamSelect.Visibility = Visibility.Collapsed;
                    TeamSelectTitle.Visibility = Visibility.Collapsed;

                    var coachAccount = (Models.Account.CoachAccount)AccountManager.Instance.CurrentAccount;
                    selectedTeam = new TeamService().GetTeam(coachAccount.TeamID.Value);

                    PopulateTeamDetails(selectedTeam);
                }
            }
        }

        /// <summary>
        /// Sets the team selection.
        /// </summary>
        private void SetTeamSelect() => TeamSelect.ItemsSource = new TeamService().GetAllTeams();

        /// <summary>
        /// Gets the updated team from the input fields.
        /// </summary>
        /// <returns>Updated team.</returns>
        private Models.Team.Team GetUpdatedTeam()
        {
            selectedTeam.Name = NameBox.Text;
            return selectedTeam;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Event Handlers

        /// <summary>
        /// Event handler for the delete button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void DeleteButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Create a TextBlock for formatting the content text
            TextBlock contentText = new TextBlock();
            contentText.Inlines.Add(new Run { Text = "Are you sure you want to delete " });
            contentText.Inlines.Add(new Run { Text = selectedTeam.Name, FontWeight = FontWeights.Bold });
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

            // Set the event handler for the primary button
            deleteDialog.PrimaryButtonClick += DeleteDialog_PrimaryButtonClick;
            await deleteDialog.ShowAsync();
        }

        /// <summary>
        /// Event handler for the delete dialog primary button click.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                // Delete the team and update the UI
                await new TeamService().DeleteTeamAsync(selectedTeam.TeamID);

                ErrorText.Text = "Team deleted.";
                selectedTeam = null;

                // For coach accounts, logout after deleting the team
                if (AccountManager.Instance.CurrentAccount is Models.Account.CoachAccount)
                {
                    AccountManager.Instance.Logout();
                    MainWindow.Frame.Navigate(typeof(Login));
                }

                HideTeamDetails();
                SetTeamSelect();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the update button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void UpdateButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                // Update the team and update the UI
                await new TeamService().UpdateTeamAsync(GetUpdatedTeam());

                SetTeamSelect();
                ChangeViewToView();

                ErrorText.Text = "Team updated.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the edit button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ChangeViewToEdit();

        /// <summary>
        /// Event handler for the cancel button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeViewToView();
            ErrorText.Text = string.Empty;  // Clear any previous errors
        }

        /// <summary>
        /// Event handler for the team selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeamSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a team is selected))

            try
            {
                // Get the selected team and populate the details
                var selectedTeamID = ((Models.Team.Team)TeamSelect.SelectedItem).TeamID;
                selectedTeam = new TeamService().GetTeam(selectedTeamID);

                PopulateTeamDetails(selectedTeam);
                ErrorText.Text = string.Empty;  // Clear any previous errors
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Management

        /// <summary>
        /// Populates the team details.
        /// </summary>
        /// <param name="selectedTeam">Selected team.</param>
        private void PopulateTeamDetails(Models.Team.Team selectedTeam)
        {
            if (selectedTeam != null)
            {
                // Populate the fields
                NameBlock.Text = selectedTeam.Name;
                LevelBlock.Text = selectedTeam.Level;

                TeamBlock.Text = selectedTeam.Name;
                NameBox.Text = selectedTeam.Name;

                ShowTeamDetails();
            }
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
        /// Shows the team details.
        /// </summary>
        private void ShowTeamDetails()
        {
            ChangeVisibility(titles, Visibility.Visible);
            ChangeVisibility(viewFields, Visibility.Visible);
        }

        /// <summary>
        /// Hides the team details.
        /// </summary>
        private void HideTeamDetails()
        {
            ChangeVisibility(titles, Visibility.Collapsed);
            ChangeVisibility(viewFields, Visibility.Collapsed);
        }

        /// <summary>
        /// Changes the view to edit mode.
        /// </summary>
        private void ChangeViewToEdit()
        {
            ChangeVisibility(viewFields, Visibility.Collapsed);

            // For admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Collapsed;

            ChangeVisibility(editFields, Visibility.Visible);
        }

        /// <summary>
        /// Changes the view to view mode.
        /// </summary>
        private void ChangeViewToView()
        {
            ChangeVisibility(editFields, Visibility.Collapsed);

            // For admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Visible;

            PopulateTeamDetails(selectedTeam);
            ChangeVisibility(viewFields, Visibility.Visible);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Initialization (keeping UI elements in lists for easier management)

        /// <summary>
        /// Initializes the UI lists.
        /// </summary>
        private void InitializeUILists()
        {
            InitializeTitleList();
            InitializeViewFieldsList();
            InitializeEditFieldsList();
        }

        /// <summary>
        /// Initializes the title list.
        /// </summary>
        private void InitializeTitleList()
        {
            titles.Add(NameTitle);
            titles.Add(LevelTitle);
        }

        /// <summary>
        /// Initializes the view fields list.
        /// </summary>
        private void InitializeViewFieldsList()
        {
            viewFields.Add(NameBlock);
            viewFields.Add(LevelBlock);
            viewFields.Add(EditButton);
            viewFields.Add(DeleteButton);
        }

        /// <summary>
        /// Initializes the edit fields list.
        /// </summary>
        private void InitializeEditFieldsList()
        {
            // For admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                editFields.Add(TeamBlock);

            editFields.Add(NameBox);
            editFields.Add(LevelBlock);
            editFields.Add(UpdateButton);
            editFields.Add(CancelButton);
        }
    }
}
