using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Data;
using SimplyRugby.Models.Member;
using SimplyRugby.Services;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Team.TrainingSession
{
    /// <summary>
    /// Page for adding a training session.
    /// </summary>
    public sealed partial class AddTrainingSession : Page
    {
        private Models.Team.Team selectedTeam;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddTrainingSession"/> class.
        /// </summary>
        public AddTrainingSession()
        {
            this.InitializeComponent();

            // Select the team based on the account type.
            TeamSelectionFromAccount();
        }

        /// <summary>
        /// Selects the team based on the account type.
        /// </summary>
        private void TeamSelectionFromAccount()
        {
            using (var context = new SimplyRugbyContext())
            {
                // Admin accounts can select a team.
                if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                {
                    TeamSelect.Visibility = Visibility.Visible;
                    TeamSelectTitle.Visibility = Visibility.Visible;

                    TeamSelect.ItemsSource = context.Teams.ToList();
                }
                // Coach accounts are assigned to a team.
                else if (AccountManager.Instance.CurrentAccount is Models.Account.CoachAccount)
                {
                    TeamSelect.Visibility = Visibility.Collapsed;
                    TeamSelectTitle.Visibility = Visibility.Collapsed;

                    // Get the coach's team.
                    var coachAccount = (Models.Account.CoachAccount)AccountManager.Instance.CurrentAccount;
                    selectedTeam = coachAccount.Team;

                    // Set the player and coach select
                    SetPlayerSelect();
                    SetCoachSelect();
                }
            }
        }

        /// <summary>
        /// Sets the player select combobox.
        /// </summary>
        private void SetPlayerSelect() => PlayersSelect.ItemsSource = new PlayerService().GetAllPlayersForTeam(selectedTeam.TeamID);

        /// <summary>
        /// Sets the coach select combobox.
        /// </summary>
        private void SetCoachSelect() => CoachSelect.ItemsSource = new MemberService().GetAllCoachesForTeam(selectedTeam.TeamID);

        /// <summary>
        /// Event handler for the team selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the slection changed event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearInputFields();
            selectedTeam = (Models.Team.Team)TeamSelect.SelectedItem;

            // Set the player and coach select
            SetPlayerSelect();
            SetCoachSelect();

            ErrorText.Text = string.Empty;  // Clear any previous errors
        }

        /// <summary>
        /// Event handler for the add training session button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void AddTrainingSessionButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Check if the team is selected
            if (selectedTeam == null)
            {
                ErrorText.Text = "Please select a team.";
                return;
            }

            try
            {
                // Add the match
                await new TrainingSessionService().AddTrainingSessionAsync(CreateTrainingSession());
                ClearInputFields();
                ErrorText.Text = "Training session added successfully.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Creates a training session from the input fields.
        /// </summary>
        /// <returns>Session from input fields.</returns>
        private Models.Team.TrainingSession.TrainingSession CreateTrainingSession()
        {
            // Create a new training session object with the data from the form
            Models.Team.TrainingSession.TrainingSession session = new Models.Team.TrainingSession.TrainingSession
            {
                TeamID = selectedTeam.TeamID,
                Location = LocationBox.Text,
                Date = DateBox.Date.Date + TimeBox.Time,
                SkillsAndActivities = SkillsAndActivitiesBox.Text,
                InjuriesAndAccidents = InjuriesAndAccidentsBox.Text,
            };

            // Add selected coach from the combobox to the session if there is one
            if (CoachSelect.SelectedItem != null)
                session.Coach = (Coach)CoachSelect.SelectedItem;

            // Add selected players from the combobox to the session
            // This code is a bit more nested than it should be, but it's necessary to get a ListView style combobox to work
            foreach (var item in PlayersSelect.Items)
            {
                // Get the container for the item
                ComboBoxItem container = PlayersSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox from the container
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;
                    if (checkBox.IsChecked == true)
                    {
                        // Get the player from the checkbox
                        Player player = checkBox.Tag as Player;
                        if (player != null)
                        {
                            // Add the player to the session
                            session.Players.Add(player);
                        }
                    }
                }
            }

            // Return the session
            return session;
        }

        /// <summary>
        /// Event handler for the players select combobox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void PlayersSelect_SelectionChanged(object sender, SelectionChangedEventArgs e) => PlayersSelect.SelectedItem = null;   // Prevent selection

        /// <summary>
        /// Clears the input fields.
        /// </summary>
        private void ClearInputFields()
        {
            LocationBox.ClearValue(TextBox.TextProperty);
            DateBox.ClearValue(DatePicker.DateProperty);
            TimeBox.ClearValue(TimePicker.TimeProperty);
            SkillsAndActivitiesBox.ClearValue(TextBox.TextProperty);
            InjuriesAndAccidentsBox.ClearValue(TextBox.TextProperty);
            CoachSelect.SelectedItem = null;
            foreach (var item in PlayersSelect.Items)
            {
                ComboBoxItem container = PlayersSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;
                    checkBox.IsChecked = false;
                }
            }
            PlayersSelect.PlaceholderText = "Select players (multiple or single)...";
        }

        /// <summary>
        /// Event handler for the player checkbox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the checked changed event.</param>
        private void PlayerCheckBox_Checked(object sender, RoutedEventArgs e) => PlayersSelect.PlaceholderText = "Edit selected players...";

        /// <summary>
        /// Event handler for the date changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the date changed event.</param>
        private void DateBox_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            ChangeInjuriesDisplay();
        }

        /// <summary>
        /// Event handler for the time changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the time changed event.</param>
        private void TimeBox_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            ChangeInjuriesDisplay();
        }

        /// <summary>
        /// Changes the display of the injuries and accidents based on the date and time.
        /// </summary>
        private void ChangeInjuriesDisplay()
        {
            // Set the date and time
            DateTime dateTime = DateBox.Date.Date + TimeBox.Time;

            // Clear the input fields for injuries/accidents and disable them if the date is in the future
            if (dateTime > DateTime.Now)
            {
                InjuriesAndAccidentsBox.ClearValue(TextBox.TextProperty);
                InjuriesAndAccidentsBox.IsEnabled = false;
            }
            else if (InjuriesAndAccidentsBox != null)
            {
                InjuriesAndAccidentsBox.IsEnabled = true;
            }
        }
    }
}
