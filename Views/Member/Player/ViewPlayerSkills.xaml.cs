using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SimplyRugby.Models.Member.Skills;
using SimplyRugby.Services;
using System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby.Views.Member.Player
{
    /// <summary>
    /// Page for viewing and editing a player's skills.
    /// </summary>
    public sealed partial class ViewPlayerSkills : Page
    {
        private Models.Member.Player player;
        private Models.Member.Skills.Skills skills;

        // UI elements
        private List<UIElement> viewFields = new List<UIElement>();
        private List<UIElement> editFields = new List<UIElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPlayerSkills"/> class.
        /// </summary>
        public ViewPlayerSkills()
        {
            this.InitializeComponent();
            InitializeUILists();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getters and Setters

        /// <summary>
        /// Event handler for when the page is navigated to, setting the player and skills.
        /// </summary>
        /// <param name="e">Event data that provides information about the navigated to event.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get the player and skills from the navigation parameter
            var player = e.Parameter as Models.Member.Player;
            if (player != null)
            {
                this.player = player;
                this.skills = player.Skills;
                PopulateSkillsDetails();
            }
        }

        /// <summary>
        /// Gets the updated skills from the UI.
        /// </summary>
        /// <returns>Updated skills from the UI.</returns>
        private Skills GetUpdatedSkills()
        {
            // Update the skills with the new values
            skills.Kicking.Drop = (int)DropKickingBox.Value;
            skills.Kicking.Punt = (int)PuntKickingBox.Value;
            skills.Kicking.Grubber = (int)GrubberKickingBox.Value;
            skills.Kicking.Goal = (int)GoalKickingBox.Value;
            skills.Passing.Pop = (int)PopPassingBox.Value;
            skills.Passing.Spin = (int)SpinPassingBox.Value;
            skills.Passing.Standard = (int)StandardPassingBox.Value;
            skills.Tackling.Front = (int)FrontTacklingBox.Value;
            skills.Tackling.Rear = (int)RearTacklingBox.Value;
            skills.Tackling.Side = (int)SideTacklingBox.Value;
            skills.Tackling.Scramble = (int)ScrambleTacklingBox.Value;

            // Update the skills with the new comments
            skills.Kicking.DropComments = DropKickingCommentsBox.Text;
            skills.Kicking.PuntComments = PuntKickingCommentsBox.Text;
            skills.Kicking.GrubberComments = GrubberKickingCommentsBox.Text;
            skills.Kicking.GoalComments = GoalKickingCommentsBox.Text;
            skills.Passing.PopComments = PopPassingCommentsBox.Text;
            skills.Passing.SpinComments = SpinPassingCommentsBox.Text;
            skills.Passing.StandardComments = StandardPassingCommentsBox.Text;
            skills.Tackling.FrontComments = FrontTacklingCommentsBox.Text;
            skills.Tackling.RearComments = RearTacklingCommentsBox.Text;
            skills.Tackling.SideComments = SideTacklingCommentsBox.Text;
            skills.Tackling.ScrambleComments = ScrambleTacklingCommentsBox.Text;

            return skills;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Event Handlers

        /// <summary>
        /// Event handler for when the update button is clicked, updating the player's skills.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private async void UpdateButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                // Update the player's skills
                PlayerService playerService = new PlayerService();
                await playerService.UpdateSkillsAsync(GetUpdatedSkills());
                player = playerService.GetPlayer(player.MemberID);
                skills = player.Skills;

                ChangeViewToView();
                PopulateSkillsDetails();
                ErrorText.Text = "Skills updated.";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"An error occurred: {ex.Message}.";

            }
        }

        /// <summary>
        /// Event handler for when the cancel button is clicked, changing the view back to view mode.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeViewToView();
            ErrorText.Text = string.Empty;  // Clear any previous errors
        }

        /// <summary>
        /// Event handler for when the edit button is clicked, changing the view to edit mode.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => ChangeViewToEdit();

        /// <summary>
        /// Event handler for when the return button is clicked, returning to the previous page.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void ReturnButton_Click(object sender, RoutedEventArgs e) => Frame.GoBack();

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Management

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
        /// Populates the skills details in the UI.
        /// </summary>
        private void PopulateSkillsDetails()
        {
            PlayerBlock.Text = player.Name;

            // Populate the skills details
            DropKickingBlock.Text = $"{skills.Kicking.Drop.ToString()}/5";
            PuntKickingBlock.Text = $"{skills.Kicking.Punt.ToString()}/5";
            GrubberKickingBlock.Text = $"{skills.Kicking.Grubber.ToString()}/5";
            GoalKickingBlock.Text = $"{skills.Kicking.Goal.ToString()}/5";
            AverageKickingBlock.Text = $"{skills.Kicking.Average.ToString("0.#")}/5";

            PopPassingBlock.Text = $"{skills.Passing.Pop.ToString()}/5";
            SpinPassingBlock.Text = $"{skills.Passing.Spin.ToString()}/5";
            StandardPassingBlock.Text = $"{skills.Passing.Standard.ToString()}/5";
            AveragePassingBlock.Text = $"{skills.Passing.Average.ToString("0.#")}/5";

            FrontTacklingBlock.Text = $"{skills.Tackling.Front.ToString()}/5";
            RearTacklingBlock.Text = $"{skills.Tackling.Rear.ToString()}/5";
            SideTacklingBlock.Text = $"{skills.Tackling.Side.ToString()}/5";
            ScrambleTacklingBlock.Text = $"{skills.Tackling.Scramble.ToString()}/5";
            AverageTacklingBlock.Text = $"{skills.Tackling.Average.ToString("0.#")}/5";

            AverageBlock.Text = $"{skills.Average.ToString("0.#")}/5";

            // Populate the skills in the edit fields
            DropKickingBox.Value = skills.Kicking.Drop;
            PuntKickingBox.Value = skills.Kicking.Punt;
            GrubberKickingBox.Value = skills.Kicking.Grubber;
            GoalKickingBox.Value = skills.Kicking.Goal;

            PopPassingBox.Value = skills.Passing.Pop;
            SpinPassingBox.Value = skills.Passing.Spin;
            StandardPassingBox.Value = skills.Passing.Standard;

            FrontTacklingBox.Value = skills.Tackling.Front;
            RearTacklingBox.Value = skills.Tackling.Rear;
            SideTacklingBox.Value = skills.Tackling.Side;
            ScrambleTacklingBox.Value = skills.Tackling.Scramble;

            PopulateComments();
        }

        /// <summary>
        /// Populates the comments in the UI.
        /// </summary>
        private void PopulateComments()
        {
            DropKickingCommentsBox.Text = skills.Kicking.DropComments ?? null;
            PuntKickingCommentsBox.Text = skills.Kicking.PuntComments ?? null;
            GrubberKickingCommentsBox.Text = skills.Kicking.GrubberComments ?? null;
            GoalKickingCommentsBox.Text = skills.Kicking.GoalComments ?? null;

            PopPassingCommentsBox.Text = skills.Passing.PopComments ?? null;
            SpinPassingCommentsBox.Text = skills.Passing.SpinComments ?? null;
            StandardPassingCommentsBox.Text = skills.Passing.StandardComments ?? null;

            FrontTacklingCommentsBox.Text = skills.Tackling.FrontComments ?? null;
            RearTacklingCommentsBox.Text = skills.Tackling.RearComments ?? null;
            SideTacklingCommentsBox.Text = skills.Tackling.SideComments ?? null;
            ScrambleTacklingCommentsBox.Text = skills.Tackling.ScrambleComments ?? null;
        }

        /// <summary>
        /// Changes the view to edit mode.
        /// </summary>
        private void ChangeViewToEdit()
        {
            ChangeVisibility(viewFields, Visibility.Collapsed);
            ChangeVisibility(editFields, Visibility.Visible);
            ChangeCommentsToEdit();
        }

        /// <summary>
        /// Changes the view to view mode.
        /// </summary>
        private void ChangeViewToView()
        {
            ChangeVisibility(viewFields, Visibility.Visible);
            ChangeVisibility(editFields, Visibility.Collapsed);
            ChangeCommentsToView();
        }

        /// <summary>
        /// Changes the comments to edit mode.
        /// </summary>
        private void ChangeCommentsToEdit()
        {
            PopulateComments();

            DropKickingCommentsBox.IsEnabled = true;
            PuntKickingCommentsBox.IsEnabled = true;
            GrubberKickingCommentsBox.IsEnabled = true;
            GoalKickingCommentsBox.IsEnabled = true;

            PopPassingCommentsBox.IsEnabled = true;
            SpinPassingCommentsBox.IsEnabled = true;
            StandardPassingCommentsBox.IsEnabled = true;

            FrontTacklingCommentsBox.IsEnabled = true;
            RearTacklingCommentsBox.IsEnabled = true;
            SideTacklingCommentsBox.IsEnabled = true;
            ScrambleTacklingCommentsBox.IsEnabled = true;
        }

        /// <summary>
        /// Changes the comments to view mode.
        /// </summary>
        private void ChangeCommentsToView()
        {
            PopulateComments();

            DropKickingCommentsBox.IsEnabled = false;
            PuntKickingCommentsBox.IsEnabled = false;
            GrubberKickingCommentsBox.IsEnabled = false;
            GoalKickingCommentsBox.IsEnabled = false;

            PopPassingCommentsBox.IsEnabled = false;
            SpinPassingCommentsBox.IsEnabled = false;
            StandardPassingCommentsBox.IsEnabled = false;

            FrontTacklingCommentsBox.IsEnabled = false;
            RearTacklingCommentsBox.IsEnabled = false;
            SideTacklingCommentsBox.IsEnabled = false;
            ScrambleTacklingCommentsBox.IsEnabled = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Initialization (keeping UI elements in lists for easier management)

        /// <summary>
        /// Initializes the UI lists.
        /// </summary>
        private void InitializeUILists()
        {
            viewFields.Add(DropKickingBlock);
            viewFields.Add(PuntKickingBlock);
            viewFields.Add(GrubberKickingBlock);
            viewFields.Add(GoalKickingBlock);
            viewFields.Add(PopPassingBlock);
            viewFields.Add(SpinPassingBlock);
            viewFields.Add(StandardPassingBlock);
            viewFields.Add(FrontTacklingBlock);
            viewFields.Add(RearTacklingBlock);
            viewFields.Add(SideTacklingBlock);
            viewFields.Add(ScrambleTacklingBlock);

            viewFields.Add(EditButton);
            viewFields.Add(ReturnButton);

            editFields.Add(DropKickingBox);
            editFields.Add(PuntKickingBox);
            editFields.Add(GrubberKickingBox);
            editFields.Add(GoalKickingBox);
            editFields.Add(PopPassingBox);
            editFields.Add(SpinPassingBox);
            editFields.Add(StandardPassingBox);
            editFields.Add(FrontTacklingBox);
            editFields.Add(RearTacklingBox);
            editFields.Add(SideTacklingBox);
            editFields.Add(ScrambleTacklingBox);

            editFields.Add(UpdateButton);
            editFields.Add(CancelButton);
        }
    }
}
