using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Data;
using SimplyRugby.Services;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Team.Match
{
    /// <summary>
    /// Page for adding a match.
    /// </summary>
    public sealed partial class AddMatch : Page
    {
        private Models.Team.Team selectedTeam;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddMatch"/> class.
        /// </summary>
        public AddMatch()
        {
            this.InitializeComponent();

            // Add the locations to the location select.
            LocationSelect.ItemsSource = new string[2] { "Home", "Away" };

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

                    var coachAccount = (Models.Account.CoachAccount)AccountManager.Instance.CurrentAccount;
                    selectedTeam = coachAccount.Team;
                }
            }
        }

        /// <summary>
        /// Event handler for the team selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTeam = (Models.Team.Team)TeamSelect.SelectedItem;
            ClearInputFields();
            ErrorText.Text = string.Empty;  // Clear any previous errors
        }

        /// <summary>
        /// Event handler for the add match button click event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void AddMatchButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Check if the team is selected
            if (selectedTeam == null)
            {
                ErrorText.Text = "Please select a team.";
                return;
            }

            // Check if the location is selected
            if (LocationSelect.SelectedItem == null)
            {
                ErrorText.Text = "Please select a location.";
                return;
            }

            try
            {
                // Add the match
                await new MatchService().AddMatchAsync(CreateMatch());
                ClearInputFields();
                ErrorText.Text = "Match added successfully.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Creates a match from the input fields.
        /// </summary>
        /// <returns>Match from input fields.</returns>
        private Models.Team.Match.Match CreateMatch()
        {
            return new Models.Team.Match.Match
            {
                TeamID = selectedTeam.TeamID,
                Opponent = OpponentBox.Text,
                DatePlayed = DateBox.Date.Date + TimeBox.Time,
                Location = LocationSelect.SelectedItem.ToString(),
                FirstHalf = new Models.Team.Match.Half
                {
                    DatePlayed = DateBox.Date.Date + TimeBox.Time,
                    TeamScore = (int)FirstHalfTeamScoreBox.Value,
                    OpponentScore = (int)FirstHalfOpponentScoreBox.Value,
                    Comments = FirstHalfCommentsBox.Text,
                },
                SecondHalf = new Models.Team.Match.Half
                {
                    DatePlayed = DateBox.Date.Date + TimeBox.Time,
                    TeamScore = (int)SecondHalfTeamScoreBox.Value,
                    OpponentScore = (int)SecondHalfOpponentScoreBox.Value,
                    Comments = SecondHalfCommentsBox.Text,
                }
            };
        }

        /// <summary>
        /// Clears the input fields.
        /// </summary>
        private void ClearInputFields()
        {
            OpponentBox.ClearValue(TextBox.TextProperty);
            DateBox.ClearValue(DatePicker.DateProperty);
            TimeBox.ClearValue(TimePicker.TimeProperty);
            LocationSelect.SelectedItem = null;
            FirstHalfTeamScoreBox.ClearValue(NumberBox.ValueProperty);
            FirstHalfOpponentScoreBox.ClearValue(NumberBox.ValueProperty);
            FirstHalfCommentsBox.ClearValue(TextBox.TextProperty);
            SecondHalfTeamScoreBox.ClearValue(NumberBox.ValueProperty);
            SecondHalfOpponentScoreBox.ClearValue(NumberBox.ValueProperty);
            SecondHalfCommentsBox.ClearValue(TextBox.TextProperty);
        }

        /// <summary>
        /// Event handler for the date changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the date changed event.</param>
        private void DateBox_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            ChangeScoresAndCommentsDisplay();
        }

        /// <summary>
        /// Event handler for the date changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the time changed event.</param>
        private void TimeBox_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            ChangeScoresAndCommentsDisplay();
        }

        /// <summary>
        /// Changes the display of the scores and comments based on the date and time.
        /// </summary>
        private void ChangeScoresAndCommentsDisplay()
        {
            // Set the date and time
            DateTime dateTime = DateBox.Date.Date + TimeBox.Time;

            if (dateTime > DateTime.Now)
            {
                // Clear the input fields for scores and comments if the date is in the future.
                FirstHalfTeamScoreBox.ClearValue(NumberBox.ValueProperty);
                FirstHalfOpponentScoreBox.ClearValue(NumberBox.ValueProperty);
                FirstHalfCommentsBox.ClearValue(TextBox.TextProperty);

                SecondHalfTeamScoreBox.ClearValue(NumberBox.ValueProperty);
                SecondHalfOpponentScoreBox.ClearValue(NumberBox.ValueProperty);
                SecondHalfCommentsBox.ClearValue(TextBox.TextProperty);

                // Disable the input fields for scores and comments if the date is in the future.
                FirstHalfTeamScoreBox.IsEnabled = false;
                FirstHalfOpponentScoreBox.IsEnabled = false;
                FirstHalfCommentsBox.IsEnabled = false;

                SecondHalfTeamScoreBox.IsEnabled = false;
                SecondHalfOpponentScoreBox.IsEnabled = false;
                SecondHalfCommentsBox.IsEnabled = false;
            }
            else
            {
                // Enable the input fields for scores and comments if the date is in the past.
                FirstHalfTeamScoreBox.IsEnabled = true;
                FirstHalfOpponentScoreBox.IsEnabled = true;
                FirstHalfCommentsBox.IsEnabled = true;

                SecondHalfTeamScoreBox.IsEnabled = true;
                SecondHalfOpponentScoreBox.IsEnabled = true;
                SecondHalfCommentsBox.IsEnabled = true;
            }
        }
    }
}
