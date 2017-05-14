using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PWPlanner
{
    public partial class MainWindow
    {
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isRendered)
            {
                TextBox objTextBox = (TextBox)sender;
                string text = objTextBox.Text;
                SearchAllTilesAndRender(text);
            }
        }

        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchResultList.Visibility == Visibility.Visible && SearchResultList.IsFocused)
            {
                SearchResultList.Visibility = Visibility.Hidden;
            }
        }

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox objTextBox = (TextBox)sender;
            string text = objTextBox.Text;
            if (SearchResultList.Visibility == Visibility.Hidden && text.Length > 0)
            {
                SearchResultList.Visibility = Visibility.Visible;
            }
        }

        private void SearchEntry_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                object selectedItem = SearchResultList.SelectedItem;
                ListBoxItem selectedListBoxItem = SearchResultList.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
                SearchEntry entry = selectedListBoxItem.Content as SearchEntry;

                TileCanvas.Children.Remove(selectBorder);

                int searchIndex;
                if (entry.type == TileType.Background)
                {
                    ComboTypes.SelectedIndex = 0;
                    searchIndex = Array.FindIndex(selectableTiles, prop => prop.bgName == entry.Name);

                }
                else
                {
                    ComboTypes.SelectedIndex = 1;
                    searchIndex = Array.FindIndex(selectableTiles, prop => prop.blName == entry.Name);
                }
                Debug.WriteLine(searchIndex);
                BitmapImage source = selectableTiles[searchIndex].source;
                selectBorder = new Border()
                {
                    BorderBrush = Brushes.SkyBlue,
                    BorderThickness = new Thickness(2),
                    Width = 32,
                    Height = 32
                };

                Canvas.SetTop(selectBorder, GetYFromIndex(searchIndex) * 32);
                Canvas.SetLeft(selectBorder, GetXFromIndex(searchIndex) * 32);
                TileCanvas.Children.Add(selectBorder);

                index = searchIndex;
                _selectedTile = new Tile(entry.type, new Image() { Source = source });
                TileHover.Content = entry.Name;
                FirstSelected = true;
                SearchResultList.Visibility = Visibility.Hidden;
            }
        }

        private void SearchList_LostFocus(object sender, RoutedEventArgs e)
        {
            var focused = FocusManager.GetFocusedElement(this);

            var item = focused as ListBoxItem;
            if (item == null)
            {
                SearchResultList.Visibility = Visibility.Hidden;
            }
        }

        public void SearchAllTilesAndRender(string name)
        {
            SearchResultList.Visibility = Visibility.Visible;
            if (name.Length > 0 && SearchResultList.Visibility == Visibility.Hidden)
            {
                SearchBackgrounds(name);
                SearchBlocks(name);
                SearchResultList.Items.Refresh();
            }
            else if (name.Length == 0)
            {
                SearchResultList.Visibility = Visibility.Hidden;
                SearchResultList.Items.Clear();
            }
            else
            {
                SearchResultList.Items.Clear();
                SearchBackgrounds(name);
                SearchBlocks(name);
                SearchResultList.Items.Refresh();
            }
        }

        public void SearchBackgrounds(string name)
        {
            foreach (KeyValuePair<BackgroundName, BitmapImage> entry in backgroundMap)
            {
                if (entry.Key.ToString().IndexOf(name, 0, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    SearchEntry se = new SearchEntry();
                    se.Name = entry.Key.ToString();
                    se.Source = entry.Value;
                    se.type = TileType.Background;
                    SearchResultList.Items.Add(se);
                }
            }
        }

        public void SearchBlocks(string name)
        {
            foreach (KeyValuePair<BlockName, BitmapImage> entry in blockMap)
            {
                if (entry.Key.ToString().IndexOf(name, 0, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    SearchEntry se = new SearchEntry();
                    se.Name = entry.Key.ToString();
                    se.Source = entry.Value;
                    se.type = TileType.Foreground;
                    SearchResultList.Items.Add(se);
                }
            }
        }
    }

    public class SearchEntry
    {
        public string Name { get; set; }
        public BitmapSource Source { get; set; }
        public TileType type;
    }
}
