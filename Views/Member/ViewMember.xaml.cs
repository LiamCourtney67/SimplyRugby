using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SimplyRugby.Services;
using System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Member
{
    /// <summary>
    /// Page for viewing and editing member details.
    /// </summary>
    public sealed partial class ViewMember : Page
    {
        private Models.Member.Member selectedMember;

        // UI elements
        private List<UIElement> titles = new List<UIElement>();
        private List<UIElement> viewFields = new List<UIElement>();
        private List<UIElement> editFields = new List<UIElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMember"/> class.
        /// </summary>
        public ViewMember()
        {
            this.InitializeComponent();

            InitializeUILists();
            HideMemberDetails();

            SetMemberSelect();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getters and Setters

        /// <summary>
        /// Sets the member select.
        /// </summary>
        private void SetMemberSelect() => MemberSelect.ItemsSource = new MemberService().GetAllMembers();

        /// <summary>
        /// Gets the updated member from the UI.
        /// </summary>
        /// <returns>Updated member from UI.</returns>
        private Models.Member.Member GetUpdatedMember()
        {
            selectedMember.FirstName = FirstNameBox.Text;
            selectedMember.LastName = LastNameBox.Text;
            selectedMember.SRUNumber = (int)SRUBox.Value;
            selectedMember.DateOfBirth = DOBBox.Date.Date;
            selectedMember.Address = AddressBox.Text;
            selectedMember.Postcode = PostcodeBox.Text;
            selectedMember.Email = EmailBox.Text;
            selectedMember.TelephoneNumber = TelephoneBox.Text;
            selectedMember.MobileNumber = MobilePhoneBox.Text;

            return selectedMember;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Event Handlers

        /// <summary>
        /// Event handler for the member select.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void MemberSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MemberSelect.SelectedItem == null) return; // This is to prevent the event from firing when the page is first loaded (before a member is selected)

            try
            {
                // Get the coach
                var selectedMemberID = ((Models.Member.Member)MemberSelect.SelectedItem).MemberID;
                selectedMember = new MemberService().GetMember(selectedMemberID);
                PopulateMemberDetails(selectedMember);
                ErrorText.Text = string.Empty;  // Clear any previous errors
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

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
            contentText.Inlines.Add(new Run { Text = selectedMember.Name, FontWeight = FontWeights.Bold });
            contentText.Inlines.Add(new Run { Text = ", this cannot be undone?" });

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

            // Event handler for the delete dialog primary button click
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
                // Delete the member
                await new MemberService().DeleteMemberAsync(selectedMember.MemberID);

                ErrorText.Text = "Member deleted.";
                selectedMember = null;

                HideMemberDetails();
                SetMemberSelect();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ChangeViewToEdit();

        /// <summary>
        /// Event handler for the update button.
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
                // Update the member
                await new MemberService().UpdateMemberAsync(GetUpdatedMember());

                selectedMember = GetUpdatedMember();
                SetMemberSelect();

                ChangeViewToView();
                ErrorText.Text = "Member updated.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Management

        /// <summary>
        /// Populates the member details.
        /// </summary>
        /// <param name="selectedMember">Selected member.</param>
        private void PopulateMemberDetails(Models.Member.Member selectedMember)
        {
            ShowMemberDetails();

            // Populate the view details
            FirstNameBlock.Text = selectedMember.FirstName;
            LastNameBlock.Text = selectedMember.LastName;
            SRUBlock.Text = selectedMember.SRUNumber.ToString();
            DOBBlock.Text = selectedMember.DateOfBirth.ToShortDateString();
            AddressBlock.Text = selectedMember.Address;
            PostcodeBlock.Text = selectedMember.Postcode;
            EmailBlock.Text = selectedMember.Email;
            TelephoneBlock.Text = selectedMember.TelephoneNumber;
            MobilePhoneBlock.Text = selectedMember.MobileNumber;

            // Populate the edit details
            MemberSelectBlock.Text = selectedMember.Name;
            FirstNameBox.Text = selectedMember.FirstName;
            LastNameBox.Text = selectedMember.LastName;
            DOBBox.Date = selectedMember.DateOfBirth;
            SRUBox.Value = selectedMember.SRUNumber;
            AddressBox.Text = selectedMember.Address;
            PostcodeBox.Text = selectedMember.Postcode;
            EmailBox.Text = selectedMember.Email;
            TelephoneBox.Text = selectedMember.TelephoneNumber;
            MobilePhoneBox.Text = selectedMember.MobileNumber;
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
        /// Shows the member details.
        /// </summary>
        private void ShowMemberDetails()
        {
            ChangeVisibility(titles, Visibility.Visible);
            ChangeVisibility(viewFields, Visibility.Visible);
        }

        /// <summary>
        /// Hides the member details.
        /// </summary>
        private void HideMemberDetails()
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
            MemberSelect.Visibility = Visibility.Collapsed;
            ChangeVisibility(editFields, Visibility.Visible);
        }

        /// <summary>
        /// Changes the view to view mode.
        /// </summary>
        private void ChangeViewToView()
        {
            ChangeVisibility(editFields, Visibility.Collapsed);
            MemberSelect.Visibility = Visibility.Visible;
            PopulateMemberDetails(selectedMember);
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
            editFields.Add(MemberSelectBlock);
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
