using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using System.Net;
using System.ComponentModel;

namespace PWPlanner.Core
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private string version;
        private string path;

        public UpdateWindow(string version)
        {
            InitializeComponent();
            this.version = version;
            startDownload();
        }

        private void startDownload()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "EXE Files (*.exe)|*.exe";
            dialog.DefaultExt = "exe";
            dialog.FileName = "PWPlanner.exe";
            dialog.Title = "Where to save?";
            dialog.RestoreDirectory = true;
            Nullable<bool> Selected = dialog.ShowDialog();
            string path = dialog.FileName;

            if (Selected == true)
            {
                this.path = path;
                try
                {
                    WebClient client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(new Uri($"https://github.com/Nenkai/PixelPlanner/releases/download/{version}/PWPlanner.exe"), path);
                    vers.Content = "Downloading version " + version;
                    url.Content = $"Downloading from https://github.com/Nenkai/PixelPlanner/releases/download/{version}/PWPlanner.exe";
                } catch (Exception e)
                {
                    MessageBox.Show("A handled exception just occurred: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                    return;
                }
            } else
            {
                Close();
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            progLabel.Content = "Downloaded " + String.Format("{0:n0}", e.BytesReceived/1000) + "kb of " + String.Format("{0:n0}", e.TotalBytesToReceive/1000) + "kb";
            progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
            percent.Content = $"({Math.Floor(percentage)}%)";
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            progLabel.Content = "Completed";
            if (MessageBox.Show("Close to restart?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    Process.Start(path);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A handled exception just occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                    return;
                }
            }
        }
    }
}
