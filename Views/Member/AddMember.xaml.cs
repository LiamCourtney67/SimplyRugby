using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Member
{
    /// <summary>
    /// Page for adding a member.
    /// </summary>
    public sealed partial class AddMember : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddMember"/> class.
        /// </summary>
        public AddMember()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event handler for the add member button.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void AddMemberButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Check if either the telephone number or mobile phone number is entered
            if (MobilePhoneBox.Text == string.Empty && TelephoneBox.Text == string.Empty)
            {
                ErrorText.Text = "Please enter a telephone number or mobile phone number.";
                return;
            }

            try
            {
                // Add the player
                await new MemberService().AddMemberAsync(CreateMember());
                ErrorText.Text = "Member added successfully.";
                ClearInputFields();
            }
            catch (System.Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Creates a member from the input fields.
        /// </summary>
        /// <returns>Member from input fields.</returns>
        private Models.Member.Member CreateMember()
        {
            return new Models.Member.Member
            {
                FirstName = FirstNameBox.Text,
                LastName = LastNameBox.Text,
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
