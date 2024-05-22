using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SimplyRugby.Data;
using SimplyRugby.Models.Member;
using SimplyRugby.Services;
using System;
using System.Collections.Generic;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Team.TrainingSession
{
    /// <summary>
    /// Page for viewing and editing training sessions for a team.
    /// </summary>
    public sealed partial class ViewTrainingSession : Page
    {
        private Models.Team.Team selectedTeam;
        private Models.Team.TrainingSession.TrainingSession selectedTrainingSession;

        // Player selections for the current session
        private Dictionary<int, bool> playerSelections = new Dictionary<int, bool>();
        private List<Models.Member.Player> addedPlayers = new List<Models.Member.Player>();
        private List<Models.Member.Player> removedPlayers = new List<Models.Member.Player>();

        // UI elements for easier management
        private List<UIElement> titles = new List<UIElement>();
        private List<UIElement> viewFields = new List<UIElement>();
        private List<UIElement> editFields = new List<UIElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTrainingSession"/> class.
        /// </summary>
        public ViewTrainingSession()
        {
            this.InitializeComponent();

            InitializeUILists();

            TeamSelectionFromAccount();
            HideTrainingSessionDetails();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getters and Setters

        /// <summary>
        /// Sets the selected team from the user account.
        /// </summary>
        private void TeamSelectionFromAccount()
        {
            using (var context = new SimplyRugbyContext())
            {
                // Admins can select any team
                if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                {
                    TeamSelect.Visibility = Visibility.Visible;

                    TeamSelect.ItemsSource = new TeamService().GetAllTeams();
                }

                // Coaches can only select their team
                else if (AccountManager.Instance.CurrentAccount is Models.Account.CoachAccount)
                {
                    TeamSelect.Visibility = Visibility.Collapsed;
                    TeamSelectTitle.Visibility = Visibility.Collapsed;

                    // Get the coach's team
                    var coachAccount = (Models.Account.CoachAccount)AccountManager.Instance.CurrentAccount;
                    selectedTeam = new TeamService().GetTeam(coachAccount.TeamID.Value);

                    SetTrainingSessionSelect(selectedTeam.TeamID);
                }
            }
        }

        /// <summary>
        /// Populates the player selections combobox with the players for the team.
        /// </summary>
        private void PopulatePlayerSelections()
        {
            // Get the players for the team
            var players = new PlayerService().GetAllPlayersForTeam(selectedTeam.TeamID);
            PlayersSelect.ItemsSource = players;

            // Clear the player selections
            playerSelections.Clear();

            // Set selection state based on whether they are part of the current session
            foreach (var player in players)
            {
                // Check if the player is part of the session
                playerSelections[player.MemberID] = selectedTrainingSession.Players.Any(p => p.MemberID == player.MemberID);
            }
        }

        /// <summary>
        /// Sets the training session select combobox based on the team ID.
        /// </summary>
        /// <param name="teamID">ID of team to select from.</param>
        private void SetTrainingSessionSelect(int teamID) => TrainingSessionSelect.ItemsSource = new TrainingSessionService().GetAllTrainingSessionsForTeam(teamID);

        /// <summary>
        /// Gets the updated training session from the UI.
        /// </summary>
        /// <returns>Updated session from the UI.</returns>
        private Models.Team.TrainingSession.TrainingSession GetUpdatedTrainingSession()
        {
            selectedTrainingSession.Location = LocationBox.Text;
            selectedTrainingSession.Date = DateBox.Date.Date + TimeBox.Time;
            selectedTrainingSession.SkillsAndActivities = SkillsAndActivitiesBox.Text;
            selectedTrainingSession.InjuriesAndAccidents = InjuriesAndAccidentsBox.Text;

            if (CoachSelect.SelectedItem is Models.Member.Coach)
                selectedTrainingSession.CoachMemberID = ((Models.Member.Coach)CoachSelect.SelectedItem).MemberID;

            return selectedTrainingSession;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Event Handlers

        /// <summary>
        /// Event handler for the TeamSelect selection changed event to set the training session select when a team is selected.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeamSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a team is selected)

            try
            {
                selectedTeam = (Models.Team.Team)TeamSelect.SelectedItem;
                SetTrainingSessionSelect(selectedTeam.TeamID);
                HideTrainingSessionDetails();
                ErrorText.Text = string.Empty;  // Clear any previous errors
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the TrainingSessionSelect selection changed event to view the training session when a session is selected.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void TrainingSessionSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TrainingSessionSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a Training Session is selected)

            try
            {
                // Get the TrainingSession
                var selectedTrainingSessionID = ((Models.Team.TrainingSession.TrainingSession)TrainingSessionSelect.SelectedItem).TrainingSessionID;
                selectedTrainingSession = new TrainingSessionService().GetTrainingSession(selectedTrainingSessionID);

                // Update the UI
                PopulatePlayerSelections();
                UpdateCheckboxesState();
                PopulateTrainingSessionDetails(selectedTrainingSession);
                ErrorText.Text = string.Empty;  // Clear any previous errors
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the DeleteButton click event to delete the training session when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void DeleteButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Create a TextBlock for formatting the content text
            TextBlock contentText = new TextBlock();
            contentText.Inlines.Add(new Run { Text = "Are you sure you want to delete " });
            contentText.Inlines.Add(new Run { Text = selectedTrainingSession.Overview, FontWeight = FontWeights.Bold });
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
        /// Event handler for the PrimaryButtonClick event of the delete dialog to delete the training session when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await new TrainingSessionService().DeleteTrainingSessionAsync(selectedTrainingSession.TrainingSessionID);

                ErrorText.Text = "Training session deleted.";
                selectedTrainingSession = null;
                HideTrainingSessionDetails();
                SetTrainingSessionSelect(selectedTeam.TeamID);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the UpdateButton click event to update the training session when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void UpdateButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                // Get the date and time
                DateTime dateTime = DateBox.Date.Date + TimeBox.Time;

                // Check if the date is in the past and the new date is in the future
                if (selectedTrainingSession.Date < DateTime.Now && dateTime > DateTime.Now)
                {
                    ErrorText.Text = "Cannot change a past session's date to a future date.";
                    return;
                }

                TrainingSessionService trainingService = new TrainingSessionService();

                // Update the training session and get the updated session
                await trainingService.UpdateTrainingSessionAsync(GetUpdatedTrainingSession(), addedPlayers, removedPlayers);
                selectedTrainingSession = trainingService.GetTrainingSession(selectedTrainingSession.TrainingSessionID);

                // Clear the added and removed players lists
                addedPlayers.Clear();
                removedPlayers.Clear();

                // Update the UI
                SetTrainingSessionSelect(selectedTeam.TeamID);
                ChangeViewToView();
                PopulatePlayerSelections();
                UpdateCheckboxesState();
                PopulateTrainingSessionDetails(selectedTrainingSession);
                ErrorText.Text = "TrainingSession updated.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for the EditButton click event to change the view to the edit mode when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ChangeViewToEdit();

        /// <summary>
        /// Event handler for the CancelButton click event to change the view to the view mode when the button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeViewToView();
            ErrorText.Text = string.Empty;  // Clear any previous errors
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Event Handlers for Player Selection

        /// <summary>
        /// Event handler for the PlayerCheckBox checked event to add the player to the session when the checkbox is checked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the check event.</param>
        private void PlayerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Get the checkbox
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.Tag is Player)
            {
                // Get the player
                Player player = checkBox.Tag as Player;

                // Update the player selection dictionary
                playerSelections[player.MemberID] = true;

                if (!addedPlayers.Any(p => p.MemberID == player.MemberID))
                {
                    addedPlayers.Add(player);

                    // Remove the player from the removed players list if they were there
                    if (removedPlayers.Any(p => p.MemberID == player.MemberID))
                        removedPlayers.Remove(player);
                }
            }
        }

        /// <summary>
        /// Event handler for the PlayerCheckBox unchecked event to remove the player from the session when the checkbox is unchecked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the check event.</param>
        private void PlayerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Get the checkbox
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.Tag is Player)
            {
                // Get the player
                Player player = checkBox.Tag as Player;

                // Update the player selection dictionary
                playerSelections[player.MemberID] = false;

                if (!removedPlayers.Any(p => p.MemberID == player.MemberID))
                {
                    removedPlayers.Add(player);

                    // Remove the player from the added players list if they were there
                    if (addedPlayers.Any(p => p.MemberID == player.MemberID))
                        addedPlayers.Remove(player);
                }
            }
        }

        /// <summary>
        /// Event handler for the PlayerCheckBox loaded event to set the checkbox state based on the player selection dictionary.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the loaded event.</param>
        private void PlayerCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is Player player)
                checkBox.IsChecked = playerSelections[player.MemberID];
        }

        /// <summary>
        /// Event handler for the PlayersSelect combobox selection changed event to prevent the combobox from being selected.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void PlayersSelect_SelectionChanged(object sender, SelectionChangedEventArgs e) => PlayersSelect.SelectedItem = null;

        /// <summary>
        /// Event handler for the DateBox date changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the date changed event.</param>
        private void DateBox_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            ChangeInjuriesDisplay();
        }

        /// <summary>
        /// Event handler for the TimeBox time changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the time changed event.</param>
        private void TimeBox_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            ChangeInjuriesDisplay();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Management

        /// <summary>
        /// Changes the display of the injuries and accidents based on the date and time.
        /// </summary>
        private void ChangeInjuriesDisplay()
        {
            // Check if a session is selected
            if (selectedTrainingSession == null)
                return;

            // Set the date and time
            DateTime dateTime = DateBox.Date.Date + TimeBox.Time;

            // Incase the user changes the date back to the future
            if (selectedTrainingSession.Date > DateTime.Now && dateTime > DateTime.Now)
            {
                InjuriesAndAccidentsBox.ClearValue(TextBox.TextProperty);
                InjuriesAndAccidentsBox.IsEnabled = false;
            }

            // Check if the date is in the futurw and the new date is in the past
            if (selectedTrainingSession.Date > DateTime.Now && dateTime < DateTime.Now)
                InjuriesAndAccidentsBox.IsEnabled = true;
        }

        /// <summary>
        /// Populates the training session details in the view and edit modes.
        /// </summary>
        /// <param name="selectedTrainingSession">Selected session to populate from.</param>
        private void PopulateTrainingSessionDetails(Models.Team.TrainingSession.TrainingSession selectedTrainingSession)
        {
            // Populate the view details
            LocationBlock.Text = selectedTrainingSession.Location;
            DateBlock.Text = selectedTrainingSession.Date.ToString("dd/MM/yyyy");
            TimeBlock.Text = selectedTrainingSession.Date.ToString("HH:mm");
            CoachSelectBlock.Text = selectedTrainingSession.Coach?.Name;
            SkillsAndActivitiesBlock.Text = selectedTrainingSession.SkillsAndActivities;
            InjuriesAndAccidentsBlock.Text = selectedTrainingSession.InjuriesAndAccidents;

            // Populate the edit details
            TeamBlock.Text = selectedTeam.Name;
            TrainingSessionBlock.Text = selectedTrainingSession.Overview;
            LocationBox.Text = selectedTrainingSession.Location;
            DateBox.Date = selectedTrainingSession.Date.Date;
            TimeBox.Time = selectedTrainingSession.Date.TimeOfDay;
            CoachSelect.ItemsSource = new MemberService().GetAllCoachesForTeam(selectedTeam.TeamID);
            CoachSelect.SelectedItem = CoachSelect.Items.FirstOrDefault(c => ((Coach)c).MemberID == selectedTrainingSession.CoachMemberID);
            SkillsAndActivitiesBox.Text = selectedTrainingSession.SkillsAndActivities;
            InjuriesAndAccidentsBox.Text = selectedTrainingSession.InjuriesAndAccidents;


            // Add selected players from the combobox to the session
            // This code is a bit more nested than it should be, but it's necessary to get a ListView style combobox to work
            foreach (var item in PlayersSelect.Items)
            {
                // Get the container
                ComboBoxItem container = PlayersSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox and the player
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;
                    Player player = checkBox.Tag as Player;

                    // Check if the player is selected
                    if (selectedTrainingSession.Players.Any(p => p.MemberID == player.MemberID))
                    {
                        // Add the player to the added players list
                        playerSelections[player.MemberID] = true;

                        // Check the checkbox
                        checkBox.IsChecked = true;
                    }
                }
            }

            ShowTrainingSessionDetails();
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
        /// Shows the training session details.
        /// </summary>
        private void ShowTrainingSessionDetails()
        {
            ChangeVisibility(titles, Visibility.Visible);
            ChangeVisibility(viewFields, Visibility.Visible);
        }

        /// <summary>
        /// Hides the training session details.
        /// </summary>
        private void HideTrainingSessionDetails()
        {
            ChangeVisibility(titles, Visibility.Collapsed);
            ChangeVisibility(viewFields, Visibility.Collapsed);
        }

        /// <summary>
        /// Changes the view to the edit mode.
        /// </summary>
        private void ChangeViewToEdit()
        {
            ChangeVisibility(viewFields, Visibility.Collapsed);

            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Collapsed;

            // Change the UI
            TrainingSessionSelect.Visibility = Visibility.Collapsed;
            ChangeVisibility(editFields, Visibility.Visible);

            PlayersSelect.PlaceholderText = "Edit players...";

            // Disable the injuries and accidents box if the session is in the future
            if (selectedTrainingSession.Date > DateTime.Now)
            {
                InjuriesAndAccidentsBox.ClearValue(TextBox.TextProperty);
                InjuriesAndAccidentsBox.IsEnabled = false;
            }
            else
                InjuriesAndAccidentsBox.IsEnabled = true;

            // Enable the checkboxes
            foreach (var item in PlayersSelect.Items)
            {
                // Get the container
                ComboBoxItem container = PlayersSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;

                    // Enable the checkbox
                    checkBox.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Changes the view to the view mode.
        /// </summary>
        private void ChangeViewToView()
        {
            ChangeVisibility(editFields, Visibility.Collapsed);

            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Visible;

            TrainingSessionSelect.Visibility = Visibility.Visible;

            PopulateTrainingSessionDetails(selectedTrainingSession);
            ChangeVisibility(viewFields, Visibility.Visible);

            PlayersSelect.PlaceholderText = "View players...";

            foreach (var item in PlayersSelect.Items)
            {
                // Get the container
                ComboBoxItem container = PlayersSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;

                    // Disable the checkbox
                    checkBox.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Updates the state of the checkboxes in the PlayersSelect combobox.
        /// </summary>
        private void UpdateCheckboxesState()
        {
            foreach (var item in PlayersSelect.Items)
            {
                // Get the container
                var container = PlayersSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox and the player
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;
                    Player player = checkBox.Tag as Player;

                    // Check if the player is selected
                    bool isSelected = playerSelections.TryGetValue(player.MemberID, out isSelected) && isSelected;

                    // Update the checkbox state
                    checkBox.IsChecked = isSelected;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Initialization (keeping UI elements in lists for easier management)

        /// <summary>
        /// Initializes the lists of UI elements for easier management.
        /// </summary>
        private void InitializeUILists()
        {
            InitializeTitleList();
            InitializeViewFieldsList();
            InitializeEditFieldsList();
        }

        /// <summary>
        /// Initializes the list of titles.
        /// </summary>
        private void InitializeTitleList()
        {
            titles.Add(LocationTitle);
            titles.Add(DateTitle);
            titles.Add(TimeTitle);
            titles.Add(CoachSelectTitle);
            titles.Add(PlayersSelectTitle);
            titles.Add(SkillsAndActivitiesTitle);
            titles.Add(InjuriesAndAccidentsTitle);
        }

        /// <summary>
        /// Initializes the list of view fields.
        /// </summary>
        private void InitializeViewFieldsList()
        {
            viewFields.Add(LocationBlock);
            viewFields.Add(DateBlock);
            viewFields.Add(TimeBlock);
            viewFields.Add(CoachSelectBlock);
            viewFields.Add(PlayersSelect);
            viewFields.Add(SkillsAndActivitiesBlock);
            viewFields.Add(InjuriesAndAccidentsBlock);

            viewFields.Add(EditButton);
            viewFields.Add(DeleteButton);
        }

        /// <summary>
        /// Initializes the list of edit fields.
        /// </summary>
        private void InitializeEditFieldsList()
        {
            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                editFields.Add(TeamBlock);

            editFields.Add(TrainingSessionBlock);
            editFields.Add(LocationBox);
            editFields.Add(DateBox);
            editFields.Add(TimeBox);
            editFields.Add(CoachSelect);
            editFields.Add(PlayersSelect);
            editFields.Add(SkillsAndActivitiesBox);
            editFields.Add(InjuriesAndAccidentsBox);

            editFields.Add(UpdateButton);
            editFields.Add(CancelButton);
        }
    }
}
