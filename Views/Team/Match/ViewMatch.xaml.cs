using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SimplyRugby.Data;
using SimplyRugby.Services;
using System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Team.Match
{
    /// <summary>
    /// Page for viewing and editing match details.
    /// </summary>
    public sealed partial class ViewMatch : Page
    {
        private Models.Team.Team selectedTeam;
        private Models.Team.Match.Match selectedMatch;

        // UI elements for easier management
        private List<UIElement> titles = new List<UIElement>();
        private List<UIElement> viewFields = new List<UIElement>();
        private List<UIElement> editFields = new List<UIElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMatch"/> class.
        /// </summary>
        public ViewMatch()
        {
            this.InitializeComponent();

            // Add the locations to the location select
            LocationSelect.ItemsSource = new string[2] { "Home", "Away" };

            // Initialize the UI elements
            InitializeUILists();

            // Select the team
            TeamSelectionFromAccount();

            // Hide the match details
            HideMatchDetails();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getters and Setters

        /// <summary>
        /// Sets the selected team.
        /// </summary>
        private void TeamSelectionFromAccount()
        {
            using (var context = new SimplyRugbyContext())
            {
                TeamService teamService = new TeamService();

                // Admins can select any team
                if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                {
                    TeamSelect.Visibility = Visibility.Visible;

                    TeamSelect.ItemsSource = teamService.GetAllTeams();
                }
                // Coaches can only select their own team
                else if (AccountManager.Instance.CurrentAccount is Models.Account.CoachAccount)
                {
                    TeamSelect.Visibility = Visibility.Collapsed;
                    TeamSelectTitle.Visibility = Visibility.Collapsed;

                    var coachAccount = (Models.Account.CoachAccount)AccountManager.Instance.CurrentAccount;
                    selectedTeam = teamService.GetTeam(coachAccount.TeamID.Value);

                    SetMatchSelect(selectedTeam.TeamID);
                }
            }
        }

        /// <summary>
        /// Sets the matches for the selected team.
        /// </summary>
        /// <param name="teamID">ID of team to select matches for.</param>
        private void SetMatchSelect(int teamID) => MatchSelect.ItemsSource = new MatchService().GetAllMatchesForTeam(teamID);

        /// <summary>
        /// Gets the updated match from the UI elements.
        /// </summary>
        /// <returns>Updated match from UI.</returns>
        private Models.Team.Match.Match GetUpdatedMatch()
        {
            selectedMatch.DatePlayed = DateBox.Date.Date + TimeBox.Time;
            selectedMatch.Opponent = OpponentBox.Text;
            selectedMatch.Location = LocationSelect.SelectedItem.ToString();

            selectedMatch.FirstHalf.DatePlayed = DateBox.Date.Date + TimeBox.Time;
            selectedMatch.FirstHalf.TeamScore = (int)FirstHalfTeamScoreBox.Value;
            selectedMatch.FirstHalf.OpponentScore = (int)FirstHalfOpponentScoreBox.Value;
            selectedMatch.FirstHalf.Comments = FirstHalfCommentsBox.Text;

            selectedMatch.SecondHalf.DatePlayed = DateBox.Date.Date + TimeBox.Time;
            selectedMatch.SecondHalf.TeamScore = (int)SecondHalfTeamScoreBox.Value;
            selectedMatch.SecondHalf.OpponentScore = (int)SecondHalfOpponentScoreBox.Value;
            selectedMatch.SecondHalf.Comments = SecondHalfCommentsBox.Text;

            return selectedMatch;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Event Handlers

        /// <summary>
        /// Event handler for when a team is selected.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeamSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a team is selected)

            try
            {
                selectedTeam = (Models.Team.Team)TeamSelect.SelectedItem;
                SetMatchSelect(selectedTeam.TeamID);
                HideMatchDetails();
                ErrorText.Text = string.Empty;  // Clear any previous errors
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for when a match is selected.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void MatchSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a match is selected)

            try
            {
                // Get the match
                var selectedMatchID = ((Models.Team.Match.Match)MatchSelect.SelectedItem).MatchID;
                selectedMatch = new MatchService().GetMatch(selectedMatchID);
                PopulateMatchDetails(selectedMatch);
                ErrorText.Text = string.Empty;  // Clear any previous errors
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for when the delete button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void DeleteButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Create a TextBlock for formatting the content text
            TextBlock contentText = new TextBlock();
            contentText.Inlines.Add(new Run { Text = "Are you sure you want to delete " });
            contentText.Inlines.Add(new Run { Text = selectedMatch.Overview, FontWeight = FontWeights.Bold });
            contentText.Inlines.Add(new Run { Text = ", this cannot be undone?" });
            contentText.TextWrapping = TextWrapping.WrapWholeWords;

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

            // Event handler for when the delete button is clicked
            deleteDialog.PrimaryButtonClick += DeleteDialog_PrimaryButtonClick;

            await deleteDialog.ShowAsync();

        }

        /// <summary>
        /// Event handler for when the delete button is clicked in the delete dialog.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await new MatchService().DeleteMatchAsync(selectedMatch.MatchID);

                ErrorText.Text = "Match deleted.";
                selectedMatch = null;

                HideMatchDetails();
                SetMatchSelect(selectedTeam.TeamID);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for when the update button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void UpdateButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                // Set the date and time
                DateTime dateTime = DateBox.Date.Date + TimeBox.Time;

                // Check if the location is selected
                if (LocationSelect.SelectedItem == null)
                {
                    ErrorText.Text = "Please select a location.";
                    return;
                }

                // Check if a future date is set for a past match
                if (selectedMatch.DatePlayed < DateTime.Now && dateTime > DateTime.Now)
                {
                    ErrorText.Text = "Cannot set a future date for a past match.";
                    return;
                }

                // Check if a past date is set for a future match and the scores are not set
                if (selectedMatch.DatePlayed > DateTime.Now && dateTime < DateTime.Now)
                {
                    // Check if the scores are set
                    if (FirstHalfTeamScoreBox.Value is double.NaN ||
                        FirstHalfOpponentScoreBox.Value is double.NaN ||
                        SecondHalfTeamScoreBox.Value is double.NaN ||
                        SecondHalfOpponentScoreBox.Value is double.NaN)
                    {
                        ErrorText.Text = "Please set the scores.";
                        return;
                    }
                }

                await new MatchService().UpdateMatchAsync(GetUpdatedMatch());

                SetMatchSelect(selectedTeam.TeamID);
                ChangeViewToView();

                ErrorText.Text = "Match updated.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred. {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for when the edit button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ChangeViewToEdit();

        /// <summary>
        /// Event handler for when the cancel button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeViewToView();
            ErrorText.Text = string.Empty;  // Clear any previous errors
        }

        /// <summary>
        /// Event handler for when the date is changed.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the date changed event.</param>
        private void DateBox_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            ChangeScoresAndCommentsDisplay();
        }

        /// <summary>
        /// Event handler for when the time is changed.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the time changed event.</param>
        private void TimeBox_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            ChangeScoresAndCommentsDisplay();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Management

        /// <summary>
        /// Changes the display of the scores and comments based on the date and time.
        /// </summary>
        private void ChangeScoresAndCommentsDisplay()
        {
            if (selectedMatch == null) return; // This is to prevent the code from running when the page is first loaded (before a match is selected)

            // Set the date and time
            DateTime dateTime = DateBox.Date.Date + TimeBox.Time;

            // If the match is in the future, and the date is changed to a past date, enable the score input
            if (selectedMatch.FutureMatch && dateTime < DateTime.Now)
            {
                // Enable the score input
                FirstHalfTeamScoreBox.IsEnabled = true;
                FirstHalfOpponentScoreBox.IsEnabled = true;
                FirstHalfCommentsBox.IsEnabled = true;

                SecondHalfTeamScoreBox.IsEnabled = true;
                SecondHalfOpponentScoreBox.IsEnabled = true;
                SecondHalfCommentsBox.IsEnabled = true;
            }

            // In case the user changes the date to a past date and back to a future
            else if (selectedMatch.FutureMatch && dateTime > DateTime.Now)
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
        }

        /// <summary>
        /// Populates the match details in the view mode.
        /// </summary>
        /// <param name="selectedMember">Selected member.</param>
        private void PopulateMatchDetails(Models.Team.Match.Match selectedMember)
        {
            ShowMatchDetails();

            // Populate the view details
            OpponentBlock.Text = selectedMatch.Opponent;
            LocationBlock.Text = selectedMatch.Location;
            DateBlock.Text = selectedMatch.DatePlayed.ToString("dd/MM/yyyy");
            TimeBlock.Text = selectedMatch.DatePlayed.ToString("HH:mm");

            // Populate the edit details
            TeamBlock.Text = selectedTeam.Name;
            MatchBlock.Text = selectedMatch.Overview;
            OpponentBox.Text = selectedMatch.Opponent;
            LocationSelect.SelectedItem = selectedMatch.Location;
            DateBox.Date = selectedMatch.DatePlayed.Date;
            TimeBox.Time = selectedMatch.DatePlayed.TimeOfDay;

            PopulateMatchDetailsForFutureMatch();
        }

        /// <summary>
        /// Populates the match details for a future match.
        /// </summary>
        private void PopulateMatchDetailsForFutureMatch()
        {
            // If the match is in the future, disable the score input
            if (selectedMatch.FutureMatch)
            {
                // Disable the score input
                FirstHalfTeamScoreBox.IsEnabled = false;
                FirstHalfOpponentScoreBox.IsEnabled = false;
                FirstHalfCommentsBox.IsEnabled = false;

                SecondHalfTeamScoreBox.IsEnabled = false;
                SecondHalfOpponentScoreBox.IsEnabled = false;
                SecondHalfCommentsBox.IsEnabled = false;

                // Set the scores to N/A
                ScoreBlock.Text = "N/A";
                FirstHalfScoreBlock.Text = "N/A";
                SecondHalfScoreBlock.Text = "N/A";

                // Set the comments to N/A
                FirstHalfCommentsBlock.Text = "N/A";
                SecondHalfCommentsBlock.Text = "N/A";
            }
            else
            {
                ScoreBlock.Text = selectedMatch.Score;

                // Enable the score input
                FirstHalfTeamScoreBox.IsEnabled = true;
                FirstHalfOpponentScoreBox.IsEnabled = true;
                FirstHalfCommentsBox.IsEnabled = true;

                SecondHalfTeamScoreBox.IsEnabled = true;
                SecondHalfOpponentScoreBox.IsEnabled = true;
                SecondHalfCommentsBox.IsEnabled = true;

                // Populate the scores view
                FirstHalfTeamScoreBox.Value = selectedMatch.FirstHalf.TeamScore;
                FirstHalfOpponentScoreBox.Value = selectedMatch.FirstHalf.OpponentScore;
                FirstHalfScoreBlock.Text = selectedMatch.FirstHalf.Score;
                FirstHalfCommentsBlock.Text = selectedMatch.FirstHalf.Comments;

                SecondHalfTeamScoreBox.Value = selectedMatch.SecondHalf.TeamScore;
                SecondHalfOpponentScoreBox.Value = selectedMatch.SecondHalf.OpponentScore;
                SecondHalfScoreBlock.Text = selectedMatch.SecondHalf.Score;
                SecondHalfCommentsBlock.Text = selectedMatch.SecondHalf.Comments;

                // Populate the scores edit
                FirstHalfTeamScoreBox.Value = selectedMatch.FirstHalf.TeamScore;
                FirstHalfOpponentScoreBox.Value = selectedMatch.FirstHalf.OpponentScore;
                FirstHalfCommentsBox.Text = selectedMatch.FirstHalf.Comments;

                SecondHalfTeamScoreBox.Value = selectedMatch.SecondHalf.TeamScore;
                SecondHalfOpponentScoreBox.Value = selectedMatch.SecondHalf.OpponentScore;
                SecondHalfCommentsBox.Text = selectedMatch.SecondHalf.Comments;

                // If the scores are not set, display N/A
                if (selectedMatch.FirstHalf.TeamScore.Equals(Int32.MinValue) ||
                    selectedMatch.FirstHalf.OpponentScore.Equals(Int32.MinValue) ||
                    selectedMatch.SecondHalf.TeamScore.Equals(Int32.MinValue) ||
                    selectedMatch.SecondHalf.OpponentScore.Equals(Int32.MinValue))
                {
                    ScoreBlock.Text = "N/A";
                    FirstHalfScoreBlock.Text = "N/A";
                    SecondHalfScoreBlock.Text = "N/A";
                }
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
        /// Shows the match details.
        /// </summary>
        private void ShowMatchDetails()
        {
            ChangeVisibility(titles, Visibility.Visible);
            ChangeVisibility(viewFields, Visibility.Visible);
            FullTimeScoreGrid.Visibility = Visibility.Visible;
            FirstHalfScoreGrid.Visibility = Visibility.Visible;
            SecondHalfScoreGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the match details.
        /// </summary>
        private void HideMatchDetails()
        {
            ChangeVisibility(titles, Visibility.Collapsed);
            ChangeVisibility(viewFields, Visibility.Collapsed);
            FullTimeScoreGrid.Visibility = Visibility.Collapsed;
            FirstHalfScoreGrid.Visibility = Visibility.Collapsed;
            SecondHalfScoreGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Changes the view from view mode to edit mode.
        /// </summary>
        private void ChangeViewToEdit()
        {
            ChangeVisibility(viewFields, Visibility.Collapsed);

            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Collapsed;

            MatchSelect.Visibility = Visibility.Collapsed;
            ChangeVisibility(editFields, Visibility.Visible);

            // Show the full time score
            ScoreBlock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Changes the view from edit mode to view mode.
        /// </summary>
        private void ChangeViewToView()
        {
            ChangeVisibility(editFields, Visibility.Collapsed);

            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Visible;

            MatchSelect.Visibility = Visibility.Visible;
            PopulateMatchDetails(selectedMatch);
            ChangeVisibility(viewFields, Visibility.Visible);

            if (selectedMatch.FirstHalf.TeamScore.Equals(Int32.MinValue) ||
                selectedMatch.FirstHalf.OpponentScore.Equals(Int32.MinValue) ||
                selectedMatch.SecondHalf.TeamScore.Equals(Int32.MinValue) ||
                selectedMatch.SecondHalf.OpponentScore.Equals(Int32.MinValue))
            {
                ScoreBlock.Text = "N/A";
                FirstHalfScoreBlock.Text = "N/A";
                SecondHalfScoreBlock.Text = "N/A";
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Initialization (keeping UI elements in lists for easier management)

        /// <summary>
        /// Initializes the lists of UI elements that are used for displaying the match details.
        /// </summary>
        private void InitializeUILists()
        {
            InitializeTitleList();
            InitializeViewFieldsList();
            InitializeEditFieldsList();
        }

        /// <summary>
        /// Initializes the list of UI elements that are used for displaying the titles of the match details.
        /// </summary>
        private void InitializeTitleList()
        {
            titles.Add(OpponentTitle);
            titles.Add(LocationTitle);
            titles.Add(DateTitle);
            titles.Add(TimeTitle);
            titles.Add(FullTimeScoreTitle);
            titles.Add(FirstHalfScoreTitle);
            titles.Add(SecondHalfScoreTitle);
        }

        /// <summary>
        /// Initializes the list of UI elements that are used for viewing the match details.
        /// </summary>
        private void InitializeViewFieldsList()
        {
            viewFields.Add(OpponentBlock);
            viewFields.Add(LocationBlock);
            viewFields.Add(DateBlock);
            viewFields.Add(TimeBlock);
            viewFields.Add(ScoreBlock);
            viewFields.Add(FirstHalfScoreBlock);
            viewFields.Add(FirstHalfCommentsBlock);
            viewFields.Add(SecondHalfScoreBlock);
            viewFields.Add(SecondHalfCommentsBlock);

            viewFields.Add(EditButton);
            viewFields.Add(DeleteButton);
        }

        /// <summary>
        /// Initializes the list of UI elements that are used for editing the match details.
        /// </summary>
        private void InitializeEditFieldsList()
        {
            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                editFields.Add(TeamBlock);

            editFields.Add(MatchBlock);
            editFields.Add(OpponentBox);
            editFields.Add(LocationSelect);
            editFields.Add(DateBox);
            editFields.Add(TimeBox);
            editFields.Add(FirstHalfTeamScoreBox);
            editFields.Add(FirstHalfOpponentScoreBox);
            editFields.Add(FirstHalfCommentsBox);
            editFields.Add(SecondHalfTeamScoreBox);
            editFields.Add(SecondHalfOpponentScoreBox);
            editFields.Add(SecondHalfCommentsBox);

            editFields.Add(UpdateButton);
            editFields.Add(CancelButton);
        }
    }
}