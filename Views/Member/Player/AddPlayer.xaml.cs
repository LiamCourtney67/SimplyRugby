using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Models.Member;
using SimplyRugby.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Member.Player
{
    /// <summary>
    /// Page for adding a player.
    /// </summary>
    public sealed partial class AddPlayer : Page
    {
        private Models.Team.Team selectedTeam;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPlayer"/> class.
        /// </summary>
        public AddPlayer()
        {
            this.InitializeComponent();

            // Set the positions select.
            SetPositionSelect();

            // Select the team based on the account type.
            TeamSelectionFromAccount();
        }

        /// <summary>
        /// Sets the positions for the select.
        /// </summary>
        private void SetPositionSelect() => PositionsSelect.ItemsSource = new PlayerService().GetPossiblePositions();

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

                // Show the fields based on the team level.
                ShowFieldsFromTeamLevel(selectedTeam.Level);
            }
        }

        /// <summary>
        /// Event handler for the team selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearInputFields();
            selectedTeam = (Models.Team.Team)TeamSelect.SelectedItem;
            ShowFieldsFromTeamLevel(selectedTeam.Level);
            ErrorText.Text = string.Empty;  // Clear any previous errors
        }

        /// <summary>
        /// Event handler for the positions selection.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void PositionsSelect_SelectionChanged(object sender, SelectionChangedEventArgs e) => PositionsSelect.SelectedItem = null;

        /// <summary>
        /// Event handler for the add player button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void AddPlayerButton_ClickAsync(object sender, RoutedEventArgs e)
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
                // Add the player
                await CreatePlayer();
                ClearInputFields();
                ErrorText.Text = "Player added successfully.";
            }
            catch (System.Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets the selected positions from the combobox.
        /// </summary>
        /// <returns>List of selected positions.</returns>
        private List<Position> GetPositions()
        {
            List<Position> positions = new List<Position>();

            // Add selected positions from the combobox to the list
            // This code is a bit more nested than it should be, but it's necessary to get a ListView style combobox to work
            foreach (var item in PositionsSelect.Items)
            {
                // Get the container for the item
                ComboBoxItem container = PositionsSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the content template root
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;
                    if (checkBox.IsChecked == true)
                    {
                        // Get the position from the tag
                        Position position = checkBox.Tag as Position;
                        if (position != null)
                        {
                            // Add the position to the list
                            positions.Add(position);
                        }
                    }
                }
            }

            // Return the list of positions
            return positions;
        }

        /// <summary>
        /// Creates a senior player from the input fields.
        /// </summary>
        /// <returns>Player from input fields.</returns>
        private Models.Member.Player CreateSeniorPlayer()
        {
            return new Models.Member.Player
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
                Email = EmailBox.Text,
                Positions = GetPositions(),
                HealthConditions = HealthConditionsBox.Text,
                Doctor = new Doctor
                {
                    Name = DoctorNameBox.Text,
                    TelephoneNumber = DoctorTelephoneBox.Text,
                    Address = DoctorAddressBox.Text,
                },
                NextOfKin = new NextOfKin
                {
                    FirstName = KinFirstNameBox.Text,
                    LastName = KinLastNameBox.Text,
                    TelephoneNumber = KinTelephoneBox.Text,
                },
            };
        }

        /// <summary>
        /// Creates a junior player from the input fields.
        /// </summary>
        /// <returns>Junior player from input fields.</returns>
        private JuniorPlayer CreateJuniorPlayer()
        {
            // Create the player
            JuniorPlayer player = new JuniorPlayer
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
                Email = EmailBox.Text,
                Positions = GetPositions(),
                HealthConditions = HealthConditionsBox.Text,
                Doctor = new Doctor
                {
                    Name = DoctorNameBox.Text,
                    TelephoneNumber = DoctorTelephoneBox.Text,
                    Address = DoctorAddressBox.Text
                },
                HasConsentForm = HasConsentForm(),
            };

            // Add guardians
            return AddGuardians(player);
        }

        /// <summary>
        /// Adds guardians to the junior player.
        /// </summary>
        /// <param name="player">Junior player to add guardians to.</param>
        /// <returns>Junior player with guardians.</returns>
        private JuniorPlayer AddGuardians(Models.Member.JuniorPlayer player)
        {
            // Add the first guardian
            player.Guardians.Add(new Guardian
            {
                FirstName = GuardianFirstNameBox.Text,
                LastName = GuardianLastNameBox.Text,
                Relationship = GuardianRelationshipBox.Text,
                TelephoneNumber = GuardianTelephoneBox.Text,
                Address = GuardianAddressBox.Text,
                PostCode = GuardianPostcodeBox.Text,
            });

            // Add the second guardian
            player.Guardians.Add(new Guardian
            {
                FirstName = Guardian2FirstNameBox.Text,
                LastName = Guardian2LastNameBox.Text,
                Relationship = Guardian2RelationshipBox.Text,
                TelephoneNumber = Guardian2TelephoneBox.Text,
                Address = Guardian2AddressBox.Text,
                PostCode = Guardian2PostcodeBox.Text,
            });

            return player;
        }

        /// <summary>
        /// Checks if the player has a consent form.
        /// </summary>
        /// <returns>True if the player has a consent form, otherwise an exception.</returns>
        /// <exception cref="Exception">Player does not have a consent form.</exception>
        private bool HasConsentForm()
        {
            if (HasConsentFormBox.SelectedItem != null && HasConsentFormBox.SelectedIndex == 0)
                return true;

            throw new Exception("Player must have a signed consent form before being added to the system.");
        }

        /// <summary>
        /// Creates either a player or a junior player from the input fields.
        /// </summary>
        /// <returns>Player or junior player from the input fields.</returns>
        private async Task CreatePlayer()
        {
            PlayerService playerService = new PlayerService();

            // Add the player based on the team level
            if (selectedTeam.Level.Equals("Junior"))
                await playerService.AddJuniorPlayerAsync(CreateJuniorPlayer());
            else
                await playerService.AddPlayerAsync(CreateSeniorPlayer());
        }

        /// <summary>
        /// Shows the fields based on the team level.
        /// </summary>
        /// <param name="level">Level for UI to view from.</param>
        private void ShowFieldsFromTeamLevel(string level)
        {
            if (level.Equals("Junior"))
            {
                SeniorPlayerFields.Visibility = Visibility.Collapsed;
                JuniorPlayerFields.Visibility = Visibility.Visible;
            }
            else
            {
                SeniorPlayerFields.Visibility = Visibility.Visible;
                JuniorPlayerFields.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Clears the input fields.
        /// </summary>
        private void ClearInputFields()
        {
            // Clear the fields
            FirstNameBox.ClearValue(TextBox.TextProperty);
            LastNameBox.ClearValue(TextBox.TextProperty);
            DOBBox.ClearValue(DatePicker.DateProperty);
            SRUBox.ClearValue(NumberBox.ValueProperty);
            AddressBox.ClearValue(TextBox.TextProperty);
            PostcodeBox.ClearValue(TextBox.TextProperty);
            TelephoneBox.ClearValue(TextBox.TextProperty);
            MobilePhoneBox.ClearValue(TextBox.TextProperty);
            EmailBox.ClearValue(TextBox.TextProperty);

            // Clear medical fields
            HealthConditionsBox.ClearValue(TextBox.TextProperty);
            DoctorNameBox.ClearValue(TextBox.TextProperty);
            DoctorTelephoneBox.ClearValue(TextBox.TextProperty);
            DoctorAddressBox.ClearValue(TextBox.TextProperty);

            // Clear next of kin fields
            KinFirstNameBox.ClearValue(TextBox.TextProperty);
            KinLastNameBox.ClearValue(TextBox.TextProperty);
            KinTelephoneBox.ClearValue(TextBox.TextProperty);

            // Clear guardian fields
            GuardianFirstNameBox.ClearValue(TextBox.TextProperty);
            GuardianLastNameBox.ClearValue(TextBox.TextProperty);
            GuardianRelationshipBox.ClearValue(TextBox.TextProperty);
            GuardianTelephoneBox.ClearValue(TextBox.TextProperty);
            GuardianAddressBox.ClearValue(TextBox.TextProperty);
            GuardianPostcodeBox.ClearValue(TextBox.TextProperty);
            Guardian2FirstNameBox.ClearValue(TextBox.TextProperty);
            Guardian2LastNameBox.ClearValue(TextBox.TextProperty);
            Guardian2RelationshipBox.ClearValue(TextBox.TextProperty);
            Guardian2TelephoneBox.ClearValue(TextBox.TextProperty);
            Guardian2AddressBox.ClearValue(TextBox.TextProperty);
            Guardian2PostcodeBox.ClearValue(TextBox.TextProperty);

            HasConsentFormBox.SelectedItem = null;

            // Clear the positions
            foreach (var item in PositionsSelect.Items)
            {
                ComboBoxItem container = PositionsSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;
                    checkBox.IsChecked = false;
                }
            }

            PositionsSelect.PlaceholderText = "Select positions (multiple or single)...";
        }

        /// <summary>
        /// Event handler for the position checkbox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the checked changed event.</param>
        private void PositionCheckBox_Checked(object sender, RoutedEventArgs e) => PositionsSelect.PlaceholderText = "Edit selected positions...";
    }
}
