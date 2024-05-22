using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SimplyRugby.Data;
using SimplyRugby.Models.Account;
using SimplyRugby.Services;
using System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Member.Coach
{
    /// <summary>
    /// Page for viewing a coach.
    /// </summary>
    public sealed partial class ViewCoach : Page
    {
        private Models.Team.Team selectedTeam;
        private Models.Member.Coach selectedCoach;

        // UI elements
        private List<UIElement> titles = new List<UIElement>();
        private List<UIElement> viewFields = new List<UIElement>();
        private List<UIElement> editFields = new List<UIElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCoach"/> class.
        /// </summary>
        public ViewCoach()
        {
            this.InitializeComponent();

            InitializeUILists();
            HideCoachDetails();

            TeamSelectionFromAccount();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getters and Setters

        /// <summary>
        /// Team selection from account.
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
                // Coaches can only select their own team
                else if (AccountManager.Instance.CurrentAccount is Models.Account.CoachAccount)
                {
                    TeamSelect.Visibility = Visibility.Collapsed;
                    TeamSelectTitle.Visibility = Visibility.Collapsed;

                    var coachAccount = (Models.Account.CoachAccount)AccountManager.Instance.CurrentAccount;
                    selectedTeam = new TeamService().GetTeam(coachAccount.TeamID.Value);

                    SetCoachSelect(selectedTeam.TeamID);
                }
            }
        }

        /// <summary>
        /// Sets the coach selection based on the team ID.
        /// </summary>
        /// <param name="teamID">Team ID to select from.</param>
        private void SetCoachSelect(int teamID) => CoachSelect.ItemsSource = new MemberService().GetAllCoachesForTeam(teamID);

        /// <summary>
        /// Gets the updated coach from the input fields.
        /// </summary>
        /// <returns>Updated coach from input fields.</returns>
        private Models.Member.Coach GetUpdatedCoach()
        {
            selectedCoach.FirstName = FirstNameBox.Text;
            selectedCoach.LastName = LastNameBox.Text;
            selectedCoach.SRUNumber = (int)SRUBox.Value;
            selectedCoach.DateOfBirth = DOBBox.Date.Date;
            selectedCoach.Address = AddressBox.Text;
            selectedCoach.Postcode = PostcodeBox.Text;
            selectedCoach.Email = EmailBox.Text;
            selectedCoach.TelephoneNumber = TelephoneBox.Text;
            selectedCoach.MobileNumber = MobilePhoneBox.Text;

            return selectedCoach;
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
                SetCoachSelect(selectedTeam.TeamID);
                HideCoachDetails();
                ErrorText.Text = string.Empty;  // Clear any previous errors
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Event handler for when a coach is selected.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void CoachSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoachSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a member is selected)

            try
            {
                // Get the coach
                var selectedCoachID = ((Models.Member.Member)CoachSelect.SelectedItem).MemberID;
                selectedCoach = new MemberService().GetCoach(selectedCoachID);
                PopulateCoachDetails(selectedCoach);
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
            contentText.Inlines.Add(new Run { Text = selectedCoach.Name, FontWeight = FontWeights.Bold });
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

            // Event handler for when the primary button is clicked
            deleteDialog.PrimaryButtonClick += DeleteDialog_PrimaryButtonClick;

            await deleteDialog.ShowAsync();
        }

        /// <summary>
        /// Event handler for when the primary button is clicked on the delete dialog.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the click event.</param>
        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                MemberService memberService = new MemberService();

                if (AccountManager.Instance.CurrentAccount is CoachAccount coachAccount && coachAccount.MemberID.Equals(selectedCoach.MemberID))
                {
                    await memberService.DeleteMemberAsync(selectedCoach.MemberID);
                    AccountManager.Instance.Logout();
                }
                else
                {
                    await memberService.DeleteMemberAsync(selectedCoach.MemberID);
                }

                ErrorText.Text = "Coach deleted.";
                selectedCoach = null;
                HideCoachDetails();
                SetCoachSelect(selectedTeam.TeamID);
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
                await new MemberService().UpdateMemberAsync(GetUpdatedCoach());
                SetCoachSelect(selectedTeam.TeamID);
                ChangeViewToView();
                ErrorText.Text = "Coach updated.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred. {ex.Message}";
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Management

        /// <summary>
        /// Populates the coach details.
        /// </summary>
        /// <param name="selectedCoach">Coach to get details from.</param>
        private void PopulateCoachDetails(Models.Member.Coach selectedCoach)
        {
            ShowCoachDetails();

            // Populate the view details
            FirstNameBlock.Text = selectedCoach.FirstName;
            LastNameBlock.Text = selectedCoach.LastName;
            SRUBlock.Text = selectedCoach.SRUNumber.ToString();
            DOBBlock.Text = selectedCoach.DateOfBirth.ToShortDateString();
            AddressBlock.Text = selectedCoach.Address;
            PostcodeBlock.Text = selectedCoach.Postcode;
            EmailBlock.Text = selectedCoach.Email;
            TelephoneBlock.Text = selectedCoach.TelephoneNumber;
            MobilePhoneBlock.Text = selectedCoach.MobileNumber;

            // Populate the edit details
            TeamBlock.Text = selectedTeam.Name;
            CoachBlock.Text = selectedCoach.Name;
            FirstNameBox.Text = selectedCoach.FirstName;
            LastNameBox.Text = selectedCoach.LastName;
            DOBBox.Date = selectedCoach.DateOfBirth;
            SRUBox.Value = selectedCoach.SRUNumber;
            AddressBox.Text = selectedCoach.Address;
            PostcodeBox.Text = selectedCoach.Postcode;
            EmailBox.Text = selectedCoach.Email;
            TelephoneBox.Text = selectedCoach.TelephoneNumber;
            MobilePhoneBox.Text = selectedCoach.MobileNumber;
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
        /// Shows the coach details.
        /// </summary>
        private void ShowCoachDetails()
        {
            ChangeVisibility(titles, Visibility.Visible);
            ChangeVisibility(viewFields, Visibility.Visible);
        }

        /// <summary>
        /// Hides the coach details.
        /// </summary>
        private void HideCoachDetails()
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

            // Hide the team select if the current account is an admin
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Collapsed;

            CoachSelect.Visibility = Visibility.Collapsed;

            ChangeVisibility(editFields, Visibility.Visible);
        }

        /// <summary>
        /// Changes the view to view mode.
        /// </summary>
        private void ChangeViewToView()
        {
            ChangeVisibility(editFields, Visibility.Collapsed);

            // Show the team select if the current account is an admin
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                TeamSelect.Visibility = Visibility.Visible;

            CoachSelect.Visibility = Visibility.Visible;

            PopulateCoachDetails(selectedCoach);
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
            titles.Add(FirstNameTitle);
            titles.Add(LastNameTitle);
            titles.Add(SRUTitle);
            titles.Add(DOBTitle);
            titles.Add(AddressTitle);
            titles.Add(PostcodeTitle);
            titles.Add(EmailTitle);
            titles.Add(TelephoneTitle);
            titles.Add(MobilePhoneTitle);
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
            viewFields.Add(EditButton);
            viewFields.Add(DeleteButton);
        }

        /// <summary>
        /// Initializes the edit fields list.
        /// </summary>
        private void InitializeEditFieldsList()
        {
            // Admins accounts only
            if (AccountManager.Instance.CurrentAccount is Models.Account.AdminAccount)
                editFields.Add(TeamBlock);

            editFields.Add(CoachBlock);
            editFields.Add(FirstNameBox);
            editFields.Add(LastNameBox);
            editFields.Add(SRUBox);
            editFields.Add(DOBBox);
            editFields.Add(AddressBox);
            editFields.Add(PostcodeBox);
            editFields.Add(EmailBox);
            editFields.Add(TelephoneBox);
            editFields.Add(MobilePhoneBox);
            editFields.Add(UpdateButton);
            editFields.Add(CancelButton);
        }
    }
}
