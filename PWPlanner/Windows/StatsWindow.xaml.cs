using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace PWPlanner.Windows
{
    public partial class StatsWindow : Window
    {
        public StatsWindow(SortedList<string, int> list)
        {
            InitializeComponent();

            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Found {list.Count} different tiles");
                foreach (KeyValuePair<string, int> entry in list)
                {
                    sb.AppendLine($"-{entry.Key} [x{entry.Value}]");
                }
                Stats.Text = sb.ToString();
            }
            else
            {
                Stats.Text = "No tiles placed!";
            }
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
