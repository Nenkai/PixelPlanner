using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace PWPlanner.Windows
{
    public partial class AboutWindow : Window
    {
        private Thread uChecker;

        public AboutWindow()
        {
            InitializeComponent();
            version.Content = "Checking for updates..";
            uChecker = new Thread(checkIfNew);
            uChecker.Start();
        }

        private void checkIfNew()
        {
            try
            {
                if (UpdateChecker.CheckForUpdates(out string latest))
                {
                    version.Dispatcher.BeginInvoke((Action)(() => version.Content = $"New version available! ({latest})"));
                }
                else
                {
                    version.Dispatcher.BeginInvoke((Action)(() => version.Content = $"Up-to-date({latest})"));
                }
            } catch (Exception e)
            {
                version.Dispatcher.BeginInvoke((Action)(() => version.Content = $"Could not check latest version"));
            }
        }

        public void forumsURL_Click(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            Process.Start(link.NavigateUri.ToString());
        }

        public void github_Click(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            Process.Start(link.NavigateUri.ToString());
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            uChecker.Abort();
        }
    }

}
