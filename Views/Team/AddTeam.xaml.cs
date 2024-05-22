using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Services;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Team
{
    /// <summary>
    /// Page for adding a team.
    /// </summary>
    public sealed partial class AddTeam : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddTeam"/> class.
        /// </summary>
        public AddTeam()
        {
            this.InitializeComponent();

            // Add the levels to the level select.
            LevelSelect.ItemsSource = new string[2] { "Junior", "Senior" };
        }

        /// <summary>
        /// Event handler for the add team button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void AddTeamButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Check if the level is selected.
            if (LevelSelect.SelectedItem == null)
            {
                ErrorText.Text = "Please select a level.";
                return;
            }

            // Add the team.
            try
            {
                await new TeamService().AddTeamAsync(CreateTeam());
                ClearInputFields();
                ErrorText.Text = "Team added successfully.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Creates a team from the input fields.
        /// </summary>
        /// <returns>Team from the inputs.</returns>
        private Models.Team.Team CreateTeam()
        {
            return new Models.Team.Team
            {
                Name = NameBox.Text,
                Level = LevelSelect.SelectedItem?.ToString()
            };
        }

        /// <summary>
        /// Clears the input fields.
        /// </summary>
        private void ClearInputFields()
        {
            NameBox.ClearValue(TextBox.TextProperty);
            LevelSelect.SelectedItem = null;
        }
    }
}
