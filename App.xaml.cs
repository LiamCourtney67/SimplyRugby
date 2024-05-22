using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SimplyRugby.Data;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimplyRugby
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Warms up the database by executing a query.
        /// </summary>
        private async Task WarmUpDatabaseAsync()
        {
            using (var context = new SimplyRugbyContext())
            {
                try
                {
                    var result = await context.Teams.FirstOrDefaultAsync();
                }
                catch (Exception)
                {
                    // No need to do anything here, just catch the exception
                }
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Warm up the database
            Task.Run(async () => await WarmUpDatabaseAsync()).Wait();

            m_window = new MainWindow();

            // Set the title of the window
            m_window.Title = "Simply Rugby";

            // Maximize the window
            if (m_window.AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }

            m_window.Activate();
        }

        private Window m_window;
    }
}
