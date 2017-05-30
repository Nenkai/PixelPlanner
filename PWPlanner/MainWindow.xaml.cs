using Microsoft.Win32;
using PWPlanner.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PWPlanner
{
    public partial class MainWindow : Window
    {
        private bool isRendered = false;
        private bool firstPlaced = false;
        private bool isLoading = false;
        private Matrix defaultMatrix;

        public MainWindow()
        {

            InitializeComponent();
            DrawGrid(TileDB.Height, TileDB.Width);
            MainCanvas.Background = BackgroundData.GetBackground(BackgroundData.BackgroundType.Forest);
            TileDB.MainBackground = BackgroundData.BackgroundType.Forest;
            TileDB.hasMainBackground = true;

            defaultMatrix = MainCanvas.LayoutTransform.Value;
            _selectedTile.Type = TileType.Background;
            GenerateSelector();
            DrawBedrock();
            ComboTypes.SelectedIndex = 0;
            this.Title = $"{this.Title} ({UpdateChecker.current})";

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            isRendered = true;
            if (UpdateChecker.CheckForUpdates())
            {
                this.Name = $"{this.Title} ({UpdateChecker.current} [Outdated])";
                MessageBoxResult result = MessageBox.Show($"Found a new version ({UpdateChecker.latest}), currently using {UpdateChecker.current}. Check it out?", "Update", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    UpdateWindow uw = new UpdateWindow(UpdateChecker.latest);
                    if (!uw.isClosing)
                    {
                        uw.Show();
                    }
                }
            }
        }

        //Save Entire Canvas to PNG
        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            //Apply default scale to allow 1:1 pixel world saving.
            MainCanvas.LayoutTransform = new ScaleTransform(defaultMatrix.M11, defaultMatrix.M22);
            MainCanvas.UpdateLayout();

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "PNG Files (*.png)|*.png",
                DefaultExt = "png",
                FileName = "World.png",
                RestoreDirectory = true
            };
            Nullable<bool> Selected = dialog.ShowDialog();
            string path = dialog.FileName;

            if (Selected == true)
            {
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                             (int)MainCanvas.ActualWidth, (int)MainCanvas.ActualHeight,
                              96d, 96d, PixelFormats.Pbgra32);

                MainCanvas.Measure(new Size((int)MainCanvas.ActualWidth, (int)MainCanvas.ActualHeight));
                MainCanvas.Arrange(new Rect(new Size((int)MainCanvas.ActualWidth, (int)MainCanvas.ActualHeight)));

                renderBitmap.Render(MainCanvas);

                PngBitmapEncoder imageEncoder = new PngBitmapEncoder();
                imageEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (var fs = File.OpenWrite(path))
                {
                    imageEncoder.Save(fs);
                }
                MessageBox.Show("Image saved successfully at\n" + path, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //World Stats
        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SortedList<string, int> placed = new SortedList<string, int>();

            for (int i = 0; i < TileDB.Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < TileDB.Tiles.GetLength(1); j++)
                {
                    if (TileDB.Tiles[i, j] != null)
                    {
                        if (TileDB.Tiles[i, j].bgName != null)
                        {
                            string itemName = TileDB.Tiles[i, j].bgName;
                            if (placed.ContainsKey(TileDB.Tiles[i, j].bgName))
                            {
                                placed[itemName]++;
                            }
                            else
                            {
                                placed.Add(itemName, 0);
                            }
                        }

                        if (TileDB.Tiles[i, j].blName != null)
                        {
                            string itemName = TileDB.Tiles[i, j].blName;

                            //Blacklist bedrock
                            if (Array.IndexOf(blacklist, TileDB.Tiles[i, j].blName) > -1)
                            {
                                continue;
                            }
                            else if (placed.ContainsKey(TileDB.Tiles[i, j].blName))
                            {
                                placed[itemName]++;
                            }
                            else
                            {
                                placed.Add(itemName, 0);
                            }
                        }
                    }
                }
            }

            if (placed.Count > 0)
            {
                sb.AppendLine($"Found {placed.Count} different tiles");
                foreach (KeyValuePair<string, int> entry in placed)
                {
                    sb.AppendLine($"-{entry.Key} [x{entry.Value}]");
                }
                MessageBox.Show(sb.ToString(), "Total Tiles", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No tiles placed!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //About Window
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        //New World
        private void NewWorld_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to create a new world? You may lose all your unsaved progress!", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                MainCanvas.Children.Clear();
                TileDB = new TileData(80, 60);
                DrawGrid(TileDB.Height, TileDB.Width);
                DrawBedrock();
                MainCanvas.Background = BackgroundData.GetBackground(TileDB.MainBackground);
                defaultMatrix = MainCanvas.LayoutTransform.Value;
                _selectedTile.Type = TileType.Background;
                ComboTypes.SelectedIndex = 0;
                firstPlaced = false;
                SaveButton.IsEnabled = false;
            }
        }

        //Background color picker
        private void OnColorSelect(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (isLoading) return;

            if (TileDB.MainBackground != BackgroundData.BackgroundType.None)
            {
                TileDB.hasMainBackground = false;
                TileDB.MainBackground = BackgroundData.BackgroundType.None;
            }

            MainCanvas.Background = new SolidColorBrush(e.NewValue.Value);
            TileDB.ARGBBackgroundColor = Utils.ARGBColortoInt(e.NewValue.Value);
        }

        //Orb Picker
        private void OrbsRadioButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                if (mi.Icon is RadioButton rb)
                {
                    rb.IsChecked = true;
                }
            }

            BackgroundData.BackgroundType bt;
            switch (mi.Name)
            {
                case "Forest":
                    bt = BackgroundData.BackgroundType.Forest;
                    break;
                case "Night":
                    bt = BackgroundData.BackgroundType.Night;
                    break;
                case "Star":
                    bt = BackgroundData.BackgroundType.Star;
                    break;
                case "Candy":
                    bt = BackgroundData.BackgroundType.Candy;
                    break;
                case "Winter":
                    bt = BackgroundData.BackgroundType.Winter;
                    break;
                case "Alien":
                    bt = BackgroundData.BackgroundType.Alien;
                    break;

                default:
                    bt = BackgroundData.BackgroundType.Forest;
                    break;
            }

            MainCanvas.Background = BackgroundData.GetBackground(bt);
            TileDB.hasMainBackground = true;
            TileDB.MainBackground = bt;

        }

        //Exit handlers
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Window_Closing(sender, new CancelEventArgs());
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit? You may lose all your unsaved progress!", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}

