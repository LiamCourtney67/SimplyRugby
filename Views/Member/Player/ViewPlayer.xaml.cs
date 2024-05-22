using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SimplyRugby.Models.Member;
using SimplyRugby.Services;
using System;
using System.Collections.Generic;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Member.Player
{
    /// <summary>
    /// Page for viewing and editing player details
    /// </summary>
    public sealed partial class ViewPlayer : Page
    {
        private Models.Team.Team selectedTeam;
        private Models.Member.Player selectedPlayer;

        // Dictionary to store the selection state of each position
        private Dictionary<string, bool> positionSelections = new Dictionary<string, bool>();

        // Lists to store the positions that have been added and removed
        private List<Models.Member.Position> addedPositions = new List<Models.Member.Position>();
        private List<Models.Member.Position> removedPositions = new List<Models.Member.Position>();

        // Lists to store the UI elements for each section
        private List<UIElement> titles = new List<UIElement>();
        private List<UIElement> viewFields = new List<UIElement>();
        private List<UIElement> seniorViewFields = new List<UIElement>();
        private List<UIElement> juniorViewFields = new List<UIElement>();
        private List<UIElement> editFields = new List<UIElement>();
        private List<UIElement> seniorEditFields = new List<UIElement>();
        private List<UIElement> juniorEditFields = new List<UIElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPlayer"/> class.
        /// </summary>
        public ViewPlayer()
        {
            this.InitializeComponent();

            InitializeUILists();

            TeamSelectionFromAccount();
            HidePlayerDetails();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getters and Setters

        /// <summary>
        /// Sets the selected team from the account.
        /// </summary>
        private void TeamSelectionFromAccount()
        {
            TeamService teamService = new TeamService();
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
            {
                TeamSelect.Visibility = Visibility.Visible;

                TeamSelect.ItemsSource = teamService.GetAllTeams();
            }
            else if (AccountManager.Instance.CurrentAccount is Models.Account.CoachAccount)
            {
                TeamSelect.Visibility = Visibility.Collapsed;
                TeamSelectTitle.Visibility = Visibility.Collapsed;

                // Get the team for the coach
                var coachAccount = (Models.Account.CoachAccount)AccountManager.Instance.CurrentAccount;
                selectedTeam = teamService.GetTeam(coachAccount.TeamID.Value);

                SetPlayerSelect(selectedTeam.TeamID);
            }
        }

        /// <summary>
        /// Populates the position selections.
        /// </summary>
        private void PopulatePositionSelections()
        {
            // Get the possible positions
            var positions = new PlayerService().GetPossiblePositions();

            // Set the items source for the combobox
            PositionsSelect.ItemsSource = positions;

            // Clear the selection state dictionary
            positionSelections.Clear();

            // Set selection state based on whether they are part of the current session
            foreach (var position in positions)
                positionSelections[position.Name] = selectedPlayer.Positions.Any(p => p.Name == position.Name);
        }

        /// <summary>
        /// Sets the player select.
        /// </summary>
        /// <param name="teamID">ID of team to select from.</param>
        private void SetPlayerSelect(int teamID)
        {
            if (selectedTeam == null) return; // This is to prevent the event from firing when the page is first loaded (before a team is selected)

            // If the team is a junior team
            if (selectedTeam.Level.Equals("Junior"))
                PlayerSelect.ItemsSource = new PlayerService().GetAllJuniorPlayersForTeam(teamID);
            // If the team is a senior team
            else
                PlayerSelect.ItemsSource = new PlayerService().GetAllPlayersForTeam(teamID);
        }

        /// <summary>
        /// Gets the updated player from the UI.
        /// </summary>
        /// <returns>Updated player from the UI</returns>
        private Models.Member.Player GetUpdatedPlayer()
        {
            // If the player is a junior player
            if (selectedPlayer is JuniorPlayer player)
            {
                player.FirstName = FirstNameBox.Text;
                player.LastName = LastNameBox.Text;
                player.SRUNumber = (int)SRUBox.Value;
                player.DateOfBirth = DOBBox.Date.Date;
                player.Address = AddressBox.Text;
                player.Postcode = PostcodeBox.Text;
                player.Email = EmailBox.Text;
                player.TelephoneNumber = TelephoneBox.Text;
                player.MobileNumber = MobilePhoneBox.Text;
                player.HealthConditions = HealthConditionsBox.Text;
                player.Doctor.Name = DoctorNameBox.Text;
                player.Doctor.TelephoneNumber = DoctorTelephoneBox.Text;
                player.Doctor.Address = DoctorAddressBox.Text;

                // Update the guardian details
                player.Guardians.ElementAt(0).FirstName = GuardianFirstNameBox.Text;
                player.Guardians.ElementAt(0).LastName = GuardianLastNameBox.Text;
                player.Guardians.ElementAt(0).TelephoneNumber = GuardianTelephoneBox.Text;
                player.Guardians.ElementAt(0).Relationship = GuardianRelationshipBox.Text;
                player.Guardians.ElementAt(0).Address = GuardianAddressBox.Text;
                player.Guardians.ElementAt(0).PostCode = GuardianPostcodeBox.Text;

                player.Guardians.ElementAt(1).FirstName = Guardian2FirstNameBox.Text;
                player.Guardians.ElementAt(1).LastName = Guardian2LastNameBox.Text;
                player.Guardians.ElementAt(1).TelephoneNumber = Guardian2TelephoneBox.Text;
                player.Guardians.ElementAt(1).Relationship = Guardian2RelationshipBox.Text;
                player.Guardians.ElementAt(1).Address = Guardian2AddressBox.Text;
                player.Guardians.ElementAt(1).PostCode = Guardian2PostcodeBox.Text;

                player.HasConsentForm = HasConsentFormBox.SelectedIndex == 0;

                selectedPlayer = player;

                return player;
            }
            // If the player is a senior player
            else
            {
                selectedPlayer.FirstName = FirstNameBox.Text;
                selectedPlayer.LastName = LastNameBox.Text;
                selectedPlayer.SRUNumber = (int)SRUBox.Value;
                selectedPlayer.DateOfBirth = DOBBox.Date.Date;
                selectedPlayer.Address = AddressBox.Text;
                selectedPlayer.Postcode = PostcodeBox.Text;
                selectedPlayer.Email = EmailBox.Text;
                selectedPlayer.TelephoneNumber = TelephoneBox.Text;
                selectedPlayer.MobileNumber = MobilePhoneBox.Text;
                selectedPlayer.HealthConditions = HealthConditionsBox.Text;
                selectedPlayer.Doctor.Name = DoctorNameBox.Text;
                selectedPlayer.Doctor.TelephoneNumber = DoctorTelephoneBox.Text;
                selectedPlayer.Doctor.Address = DoctorAddressBox.Text;
                selectedPlayer.NextOfKin.FirstName = KinFirstNameBox.Text;
                selectedPlayer.NextOfKin.LastName = KinLastNameBox.Text;
                selectedPlayer.NextOfKin.TelephoneNumber = KinTelephoneBox.Text;

                return selectedPlayer;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Event Handlers

        /// <summary>
        /// Event handler for when the team selection is changed.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void TeamSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeamSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a team is selected)

            try
            {
                selectedTeam = (Models.Team.Team)TeamSelect.SelectedItem;
                HidePlayerDetails();
                SetPlayerSelect(selectedTeam.TeamID);
                ErrorText.Text = string.Empty;  // Clear any previous errors
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for when the player selection is changed.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void PlayerSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayerSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a Training Session is selected)

            try
            {
                // Get the Player
                var selectedPlayerID = ((Models.Member.Player)PlayerSelect.SelectedItem).MemberID;

                // If the team is a junior team
                if (selectedTeam.Level.Equals("Junior"))
                    selectedPlayer = new PlayerService().GetJuniorPlayer(selectedPlayerID);
                // If the team is a senior team
                else
                    selectedPlayer = new PlayerService().GetPlayer(selectedPlayerID);

                // Update the UI
                PopulatePositionSelections();
                UpdateCheckboxesState();
                PopulatePlayerDetails(selectedPlayer);
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
            contentText.Inlines.Add(new Run { Text = selectedPlayer.Name, FontWeight = FontWeights.Bold });
            contentText.Inlines.Add(new Run { Text = ", this cannot be undone?" });
            contentText.TextWrapping = TextWrapping.WrapWholeWords;

            // Create a ContentDialog for the delete confirmation
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
        /// Event handler for when the delete dialog primary button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await new PlayerService().DeletePlayerAsync(selectedPlayer);

                ErrorText.Text = "Player deleted.";
                selectedPlayer = null;
                HidePlayerDetails();
                SetPlayerSelect(selectedTeam.TeamID);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for when the edit button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ChangeViewToEdit();

        /// <summary>
        /// Event handler for when the update button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void UpdateButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Check if either the telephone number or mobile phone number is entered
            if (MobilePhoneBox.Text == string.Empty && TelephoneBox.Text == string.Empty)
            {
                ErrorText.Text = "Please enter a telephone number or mobile phone number.";
                return;
            }

            try
            {
                PlayerService playerService = new PlayerService();

                // Update the player based on the type
                if (selectedPlayer is JuniorPlayer juniorPlayer)
                {
                    await playerService.UpdateJuniorPlayerAsync((JuniorPlayer)GetUpdatedPlayer(), addedPositions, removedPositions);
                    selectedPlayer = playerService.GetJuniorPlayer(selectedPlayer.MemberID);
                }
                else
                {
                    await playerService.UpdatePlayerAsync(GetUpdatedPlayer(), addedPositions, removedPositions);
                    selectedPlayer = playerService.GetPlayer(selectedPlayer.MemberID);
                }

                // Clear the added and removed positions
                addedPositions.Clear();
                removedPositions.Clear();

                // Update the UI
                SetPlayerSelect(selectedTeam.TeamID);
                ChangeViewToView();
                PopulatePositionSelections();
                UpdateCheckboxesState();
                PopulatePlayerDetails(selectedPlayer);
                ErrorText.Text = "Player updated.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

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
        /// Event handler for when the position checkbox is checked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the checked event.</param>
        private void PositionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Get the checkbox from the sender
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.Tag is Position)
            {
                // Get the position from the checkbox's tag
                Position position = checkBox.Tag as Position;

                // Update the selection state of the position
                positionSelections[position.Name] = true;

                if (!addedPositions.Any(p => p.Name == position.Name))
                {
                    addedPositions.Add(position);

                    // Remove the position from the removed positions if it was there
                    if (removedPositions.Any(p => p.Name == position.Name))
                    {
                        removedPositions.Remove(position);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the position checkbox is unchecked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the unchecked event.</param>
        private void PositionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Get the checkbox from the sender
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.Tag is Position)
            {
                // Get the position from the checkbox's tag
                Position position = checkBox.Tag as Position;

                // Update the selection state of the position
                positionSelections[position.Name] = false;

                if (!removedPositions.Any(p => p.Name == position.Name))
                {
                    removedPositions.Add(position);

                    // Remove the position from the added positions if it was there
                    if (addedPositions.Any(p => p.Name == position.Name))
                    {
                        addedPositions.Remove(position);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the position checkbox is loaded to set the selection state.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the loaded event.</param>
        private void PositionCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the selection state of the checkbox
            if (sender is CheckBox checkBox && checkBox.Tag is Position position)
                checkBox.IsChecked = positionSelections[position.Name];
        }

        /// <summary>
        /// Event handler for when the positions select selection is changed to deselect the item.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void PositionsSelect_SelectionChanged(object sender, SelectionChangedEventArgs e) => PositionsSelect.SelectedItem = null;

        /// <summary>
        /// Event handler for when the view skills button is clicked and navigates to the view player skills page.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void ViewSkillsButton_Click(object sender, RoutedEventArgs e) => base.Frame.Navigate(typeof(ViewPlayerSkills), selectedPlayer);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Management

        /// <summary>
        /// Populates the player details.
        /// </summary>
        /// <param name="selectedPlayer">Player to populate from.</param>
        private void PopulatePlayerDetails(Models.Member.Player selectedPlayer)
        {
            // Populate the view details
            FirstNameBlock.Text = selectedPlayer.FirstName;
            LastNameBlock.Text = selectedPlayer.LastName;
            SRUBlock.Text = selectedPlayer.SRUNumber.ToString();
            DOBBlock.Text = selectedPlayer.DateOfBirth.ToShortDateString();
            AddressBlock.Text = selectedPlayer.Address;
            PostcodeBlock.Text = selectedPlayer.Postcode;
            EmailBlock.Text = selectedPlayer.Email;
            TelephoneBlock.Text = selectedPlayer.TelephoneNumber;
            MobilePhoneBlock.Text = selectedPlayer.MobileNumber;
            DoctorNameBlock.Text = selectedPlayer.Doctor.Name;
            DoctorTelephoneBlock.Text = selectedPlayer.Doctor.TelephoneNumber;
            HealthConditionsBlock.Text = selectedPlayer.HealthConditions;
            DoctorNameBlock.Text = selectedPlayer.Doctor.Name;
            DoctorTelephoneBlock.Text = selectedPlayer.Doctor.TelephoneNumber;
            DoctorAddressBlock.Text = selectedPlayer.Doctor.Address;

            // Populate the edit details
            TeamBlock.Text = selectedTeam.Name;
            PlayerBlock.Text = selectedPlayer.Name;
            FirstNameBox.Text = selectedPlayer.FirstName;
            LastNameBox.Text = selectedPlayer.LastName;
            SRUBox.Value = selectedPlayer.SRUNumber;
            DOBBox.Date = selectedPlayer.DateOfBirth;
            AddressBox.Text = selectedPlayer.Address;
            PostcodeBox.Text = selectedPlayer.Postcode;
            EmailBox.Text = selectedPlayer.Email;
            TelephoneBox.Text = selectedPlayer.TelephoneNumber;
            MobilePhoneBox.Text = selectedPlayer.MobileNumber;
            HealthConditionsBox.Text = selectedPlayer.HealthConditions;
            DoctorNameBox.Text = selectedPlayer.Doctor.Name;
            DoctorTelephoneBox.Text = selectedPlayer.Doctor.TelephoneNumber;
            DoctorAddressBox.Text = selectedPlayer.Doctor.Address;

            // Populate the next of kin or guardian details
            PopulateKinDetails();

            // Add selected players from the combobox to the session
            // This code is a bit more nested than it should be, but it's necessary to get a ListView style combobox to work
            foreach (var item in PositionsSelect.Items)
            {
                // Get the container for the item
                ComboBoxItem container = PositionsSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox from the container
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;

                    // Get the position from the checkbox's tag
                    Position position = checkBox.Tag as Position;

                    if (selectedPlayer.Positions.Any(p => p.Name == position.Name))
                    {
                        // Set the selection state of the position
                        positionSelections[position.Name] = true;
                        checkBox.IsChecked = true;
                    }
                }
            }

            ShowPlayerDetails();
        }

        /// <summary>
        /// Populates the next of kin or guardian details.
        /// </summary>
        private void PopulateKinDetails()
        {
            // Populate the guardian details
            if (selectedPlayer is JuniorPlayer juniorPlayer)
            {
                GuardianFirstNameBlock.Text = juniorPlayer.Guardians.ElementAt(0).FirstName;
                GuardianLastNameBlock.Text = juniorPlayer.Guardians.ElementAt(0).LastName;
                GuardianTelephoneBlock.Text = juniorPlayer.Guardians.ElementAt(0).TelephoneNumber;
                GuardianRelationshipBlock.Text = juniorPlayer.Guardians.ElementAt(0).Relationship;
                GuardianAddressBlock.Text = juniorPlayer.Guardians.ElementAt(0).Address;
                GuardianPostcodeBlock.Text = juniorPlayer.Guardians.ElementAt(0).PostCode;

                Guardian2FirstNameBlock.Text = juniorPlayer.Guardians.ElementAt(1).FirstName;
                Guardian2LastNameBlock.Text = juniorPlayer.Guardians.ElementAt(1).LastName;
                Guardian2TelephoneBlock.Text = juniorPlayer.Guardians.ElementAt(1).TelephoneNumber;
                Guardian2RelationshipBlock.Text = juniorPlayer.Guardians.ElementAt(1).Relationship;
                Guardian2AddressBlock.Text = juniorPlayer.Guardians.ElementAt(1).Address;
                Guardian2PostcodeBlock.Text = juniorPlayer.Guardians.ElementAt(1).PostCode;

                GuardianFirstNameBox.Text = juniorPlayer.Guardians.ElementAt(0).FirstName;
                GuardianLastNameBox.Text = juniorPlayer.Guardians.ElementAt(0).LastName;
                GuardianTelephoneBox.Text = juniorPlayer.Guardians.ElementAt(0).TelephoneNumber;
                GuardianRelationshipBox.Text = juniorPlayer.Guardians.ElementAt(0).Relationship;
                GuardianAddressBox.Text = juniorPlayer.Guardians.ElementAt(0).Address;
                GuardianPostcodeBox.Text = juniorPlayer.Guardians.ElementAt(0).PostCode;

                Guardian2FirstNameBox.Text = juniorPlayer.Guardians.ElementAt(1).FirstName;
                Guardian2LastNameBox.Text = juniorPlayer.Guardians.ElementAt(1).LastName;
                Guardian2TelephoneBox.Text = juniorPlayer.Guardians.ElementAt(1).TelephoneNumber;
                Guardian2RelationshipBox.Text = juniorPlayer.Guardians.ElementAt(1).Relationship;
                Guardian2AddressBox.Text = juniorPlayer.Guardians.ElementAt(1).Address;
                Guardian2PostcodeBox.Text = juniorPlayer.Guardians.ElementAt(1).PostCode;

                if (juniorPlayer.HasConsentForm) { HasConsentFormBox.SelectedIndex = 0; }
                else { HasConsentFormBox.SelectedIndex = 1; }

                JuniorPlayerFields.Visibility = Visibility.Visible;
                SeniorPlayerFields.Visibility = Visibility.Collapsed;
                ChangeVisibility(juniorViewFields, Visibility.Visible);
                ChangeVisibility(seniorViewFields, Visibility.Collapsed);
            }
            // Populate the next of kin details
            else
            {
                KinFirstNameBlock.Text = selectedPlayer.NextOfKin.FirstName;
                KinLastNameBlock.Text = selectedPlayer.NextOfKin.LastName;
                KinTelephoneBlock.Text = selectedPlayer.NextOfKin.TelephoneNumber;

                KinFirstNameBox.Text = selectedPlayer.NextOfKin.FirstName;
                KinLastNameBox.Text = selectedPlayer.NextOfKin.LastName;
                KinTelephoneBox.Text = selectedPlayer.NextOfKin.TelephoneNumber;

                JuniorPlayerFields.Visibility = Visibility.Collapsed;
                SeniorPlayerFields.Visibility = Visibility.Visible;
                ChangeVisibility(juniorViewFields, Visibility.Collapsed);
                ChangeVisibility(seniorViewFields, Visibility.Visible);
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
        /// Shows the player details.
        /// </summary>
        private void ShowPlayerDetails()
        {
            ChangeVisibility(titles, Visibility.Visible);
            ChangeVisibility(viewFields, Visibility.Visible);
            MedicalFields.Visibility = Visibility.Visible;

            // Show the correct fields based on the player type
            if (selectedPlayer is JuniorPlayer)
            {
                ChangeVisibility(juniorViewFields, Visibility.Visible);
                ChangeVisibility(seniorViewFields, Visibility.Collapsed);
            }
            else
            {
                ChangeVisibility(juniorViewFields, Visibility.Collapsed);
                ChangeVisibility(seniorViewFields, Visibility.Visible);
            }
        }

        /// <summary>
        /// Hides the player details.
        /// </summary>
        private void HidePlayerDetails()
        {
            ChangeVisibility(titles, Visibility.Collapsed);
            ChangeVisibility(viewFields, Visibility.Collapsed);
            MedicalFields.Visibility = Visibility.Collapsed;
            JuniorPlayerFields.Visibility = Visibility.Collapsed;
            SeniorPlayerFields.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Changes the view to the edit state.
        /// </summary>
        private void ChangeViewToEdit()
        {
            ChangeVisibility(viewFields, Visibility.Collapsed);

            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Collapsed;

            // Change the visibility of the UI elements
            PlayerSelect.Visibility = Visibility.Collapsed;
            ChangeVisibility(editFields, Visibility.Visible);

            PositionsSelect.PlaceholderText = "Edit positions...";

            // Show the correct fields based on the player type
            if (selectedPlayer is JuniorPlayer)
            {
                ChangeVisibility(juniorViewFields, Visibility.Collapsed);
                ChangeVisibility(juniorEditFields, Visibility.Visible);
            }
            else
            {
                ChangeVisibility(seniorViewFields, Visibility.Collapsed);
                ChangeVisibility(seniorEditFields, Visibility.Visible);
            }

            HasConsentFormBox.IsEnabled = true;

            foreach (var item in PositionsSelect.Items)
            {
                // Get the container for the item
                ComboBoxItem container = PositionsSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox from the container
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;

                    // Enable the checkbox
                    checkBox.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Changes the view to the view state.
        /// </summary>
        private void ChangeViewToView()
        {
            ChangeVisibility(editFields, Visibility.Collapsed);

            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Visible;

            // Change the visibility of the UI elements
            PlayerSelect.Visibility = Visibility.Visible;
            PopulatePlayerDetails(selectedPlayer);              // Repopulate the player details
            ChangeVisibility(viewFields, Visibility.Visible);

            PositionsSelect.PlaceholderText = "View positions...";

            // Show the correct fields based on the player type
            if (selectedPlayer is JuniorPlayer)
            {
                ChangeVisibility(juniorEditFields, Visibility.Collapsed);
                ChangeVisibility(juniorViewFields, Visibility.Visible);
            }
            else
            {
                ChangeVisibility(seniorEditFields, Visibility.Collapsed);
                ChangeVisibility(seniorViewFields, Visibility.Visible);
            }

            HasConsentFormBox.IsEnabled = false;

            foreach (var item in PositionsSelect.Items)
            {
                // Get the container for the item
                ComboBoxItem container = PositionsSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox from the container
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;

                    // Disable the checkbox
                    checkBox.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Updates the state of the checkboxes based on the selection state of each position.
        /// </summary>
        private void UpdateCheckboxesState()
        {
            foreach (var item in PositionsSelect.Items)
            {
                // Get the container for the item
                var container = PositionsSelect.ContainerFromItem(item) as ComboBoxItem;
                if (container != null)
                {
                    // Get the checkbox from the container
                    CheckBox checkBox = container.ContentTemplateRoot as CheckBox;

                    // Get the position from the checkbox's tag
                    Position position = checkBox.Tag as Position;

                    // Get the selection state of the position
                    bool isSelected = positionSelections.TryGetValue(position.Name, out isSelected) && isSelected;

                    // Update the checkbox state
                    checkBox.IsChecked = isSelected;
                }
            }
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
            InitializeSeniorViewFieldsList();
            InitializeJuniorViewFieldsList();
            InitializeEditFieldsList();
            InitializeSeniorEditFieldsList();
            InitializeJuniorEditFieldsList();
        }

        /// <summary>
        /// Initializes the title list.
        /// </summary>
        private void InitializeTitleList()
        {
            titles.Add(FirstNameTitle);
            titles.Add(LastNameTitle);
            titles.Add(SRUTitle);
            titles.Add(DOBTitle);
            titles.Add(AddressTitle);
            titles.Add(PostcodeTitle);
            titles.Add(EmailTitle);
            titles.Add(TelephoneTitle);
            titles.Add(MobilePhoneTitle);
            titles.Add(PositionsSelectTitle);
        }

        /// <summary>
        /// Initializes the view fields list.
        /// </summary>
        private void InitializeViewFieldsList()
        {
            viewFields.Add(FirstNameBlock);
            viewFields.Add(LastNameBlock);
            viewFields.Add(SRUBlock);
            viewFields.Add(DOBBlock);
            viewFields.Add(AddressBlock);
            viewFields.Add(PostcodeBlock);
            viewFields.Add(EmailBlock);
            viewFields.Add(TelephoneBlock);
            viewFields.Add(MobilePhoneBlock);
            viewFields.Add(PositionsSelect);
            viewFields.Add(PostcodeBlock);
            viewFields.Add(DoctorNameBlock);
            viewFields.Add(DoctorTelephoneBlock);
            viewFields.Add(DoctorAddressBlock);
            viewFields.Add(HealthConditionsBlock);

            viewFields.Add(EditButton);
            viewFields.Add(DeleteButton);
            viewFields.Add(ViewSkillsButton);
        }

        /// <summary>
        /// Initializes the senior view fields list.
        /// </summary>
        private void InitializeSeniorViewFieldsList()
        {
            seniorViewFields.Add(KinFirstNameBlock);
            seniorViewFields.Add(KinLastNameBlock);
            seniorViewFields.Add(KinTelephoneBlock);
        }

        /// <summary>
        /// Initializes the junior view fields list.
        /// </summary>
        private void InitializeJuniorViewFieldsList()
        {
            juniorViewFields.Add(GuardianFirstNameBlock);
            juniorViewFields.Add(GuardianLastNameBlock);
            juniorViewFields.Add(GuardianTelephoneBlock);
            juniorViewFields.Add(GuardianRelationshipBlock);
            juniorViewFields.Add(GuardianAddressBlock);
            juniorViewFields.Add(GuardianPostcodeBlock);

            juniorViewFields.Add(Guardian2FirstNameBlock);
            juniorViewFields.Add(Guardian2LastNameBlock);
            juniorViewFields.Add(Guardian2TelephoneBlock);
            juniorViewFields.Add(Guardian2RelationshipBlock);
            juniorViewFields.Add(Guardian2AddressBlock);
            juniorViewFields.Add(Guardian2PostcodeBlock);
        }

        /// <summary>
        /// Initializes the edit fields list.
        /// </summary>
        private void InitializeEditFieldsList()
        {
            // Admin accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                editFields.Add(TeamBlock);

            editFields.Add(PlayerBlock);
            editFields.Add(FirstNameBox);
            editFields.Add(LastNameBox);
            editFields.Add(SRUBox);
            editFields.Add(DOBBox);
            editFields.Add(AddressBox);
            editFields.Add(PostcodeBox);
            editFields.Add(EmailBox);
            editFields.Add(TelephoneBox);
            editFields.Add(MobilePhoneBox);
            editFields.Add(PositionsSelect);
            editFields.Add(DoctorNameBox);
            editFields.Add(DoctorTelephoneBox);
            editFields.Add(DoctorAddressBox);
            editFields.Add(HealthConditionsBox);

            editFields.Add(UpdateButton);
            editFields.Add(CancelButton);
        }

        /// <summary>
        /// Initializes the senior edit fields list.
        /// </summary>
        private void InitializeSeniorEditFieldsList()
        {
            seniorEditFields.Add(KinFirstNameBox);
            seniorEditFields.Add(KinLastNameBox);
            seniorEditFields.Add(KinTelephoneBox);
        }

        /// <summary>
        /// Initializes the junior edit fields list.
        /// </summary>
        private void InitializeJuniorEditFieldsList()
        {
            juniorEditFields.Add(GuardianFirstNameBox);
            juniorEditFields.Add(GuardianLastNameBox);
            juniorEditFields.Add(GuardianTelephoneBox);
            juniorEditFields.Add(GuardianRelationshipBox);
            juniorEditFields.Add(GuardianAddressBox);
            juniorEditFields.Add(GuardianPostcodeBox);

            juniorEditFields.Add(Guardian2FirstNameBox);
            juniorEditFields.Add(Guardian2LastNameBox);
            juniorEditFields.Add(Guardian2TelephoneBox);
            juniorEditFields.Add(Guardian2RelationshipBox);
            juniorEditFields.Add(Guardian2AddressBox);
            juniorEditFields.Add(Guardian2PostcodeBox);
        }
    }
}