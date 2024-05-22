using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimplyRugby.Views.Account;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby
{
    /// <summary>
    /// Main window of the application.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // Static frame for navigation.
        public static Frame Frame { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();
            Frame = MainFrame;

            // Navigate to the login page.
            Frame.Navigate(typeof(Login));
        }
    }
}
