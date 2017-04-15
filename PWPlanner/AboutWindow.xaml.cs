using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;


namespace PWPlanner
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
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
    }

}
