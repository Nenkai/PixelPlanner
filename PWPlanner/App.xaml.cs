using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime;
using System.Windows;

namespace PWPlanner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var profileRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), // LocalAppdata/PWPlanner/ProfileOptimization/Startup.profile
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, 
                "ProfileOptimization");
            Directory.CreateDirectory(profileRoot);
            // Define the folder where to save the profile files
            ProfileOptimization.SetProfileRoot(profileRoot);
            // Start profiling and save it in Startup.profile
            ProfileOptimization.StartProfile("Startup.profile");
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error occured, please report this message!\nError: " + e.Exception.Message + "\n\n" + e.Exception.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}
