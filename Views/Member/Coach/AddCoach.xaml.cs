using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Member.Coach
{
    /// <summary>
    /// Page for adding a coach.
    /// </summary>
    public sealed partial class AddCoach : Page
    {
        private Models.Team.Team selectedTeam;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCoach"/> class.
        /// </summary>
        public AddCoach()
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
            TeamService teamService = new TeamService();
            // Admin accounts can select a team.
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
            {
                TeamSelect.Visibility = Visibility.Visible;
                TeamSelectTitle.Visibility = Visibility.Visible;

                TeamSelect.ItemsSource = teamService.GetAllTeams();
            }
            // Coach accounts are assigned to a team.
            else if (AccountManager.Instance.CurrentAccount is Models.Account.CoachAccount)
            {
                TeamSelect.Visibility = Visibility.Collapsed;
                TeamSelectTitle.Visibility = Visibility.Collapsed;

                // Get the coach's team.
                var coachAccount = (Models.Account.CoachAccount)AccountManager.Instance.CurrentAccount;
                selectedTeam = teamService.GetTeam(coachAccount.TeamID.Value);
            }
        }

        /// <summary>
        /// Event handler for the team selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearInputFields();
            selectedTeam = (Models.Team.Team)TeamSelect.SelectedItem;
            ErrorText.Text = string.Empty;  // Clear any previous errors
        }

        /// <summary>
        /// Event handler for the add coach button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void AddCoachButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Check if the team is selected
            if (selectedTeam == null)
            {
                ErrorText.Text = "Please select a team.";
                return;
            }

            // Check if either the telephone number or mobile phone number is entered
            if (MobilePhoneBox.Text == string.Empty && TelephoneBox.Text == string.Empty)
            {
                ErrorText.Text = "Please enter a telephone number or mobile phone number.";
                return;
            }

            try
            {
                // Add the coach
                await new MemberService().AddCoachAsync(CreateCoach());
                ErrorText.Text = "Coach added successfully.";
                ClearInputFields();
            }
            catch (System.Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Creates a coach from the input fields.
        /// </summary>
        /// <returns>Coach from input fields.</returns>
        private Models.Member.Coach CreateCoach()
        {
            return new Models.Member.Coach
            {
                FirstName = FirstNameBox.Text,
                LastName = LastNameBox.Text,
                TeamID = selectedTeam.TeamID,
                DateOfBirth = DOBBox.Date.Date,
                SRUNumber = (int)SRUBox.Value,
                Address = AddressBox.Text,
                Postcode = PostcodeBox.Text,
                TelephoneNumber = TelephoneBox.Text,
                MobileNumber = MobilePhoneBox.Text,
                Email = EmailBox.Text
            };
        }

        /// <summary>
        /// Clears the input fields.
        /// </summary>
        private void ClearInputFields()
        {
            FirstNameBox.ClearValue(TextBox.TextProperty);
            LastNameBox.ClearValue(TextBox.TextProperty);
            DOBBox.ClearValue(DatePicker.DateProperty);
            SRUBox.ClearValue(NumberBox.ValueProperty);
            AddressBox.ClearValue(TextBox.TextProperty);
            PostcodeBox.ClearValue(TextBox.TextProperty);
            TelephoneBox.ClearValue(TextBox.TextProperty);
            MobilePhoneBox.ClearValue(TextBox.TextProperty);
            EmailBox.ClearValue(TextBox.TextProperty);
        }
    }
}
