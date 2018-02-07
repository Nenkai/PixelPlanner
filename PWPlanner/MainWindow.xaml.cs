using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using PWPlanner.Windows;

namespace PWPlanner
{
    public partial class MainWindow : Window
    {
        private bool isRendered = false;
        private bool firstPlaced = false;
        private bool isLoading = false;
        public static bool isBackgroundUpdateChecking = false;
        private Matrix defaultMatrix;

        public MainWindow()
        {

            InitializeComponent();

            //Didn't know it was this easy to get raw quality...

            RenderOptions.SetBitmapScalingMode(MainCanvas, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(TileCanvas, BitmapScalingMode.NearestNeighbor);
            //Draw Grid by default.
            DrawGrid(TileDB.Height, TileDB.Width);

            //Set Background to forest.
            MainCanvas.Background = BackgroundData.GetBackground(BackgroundData.BackgroundType.Forest);
            TileDB.MainBackground = BackgroundData.BackgroundType.Forest;
            TileDB.hasMainBackground = true;

            defaultMatrix = MainCanvas.LayoutTransform.Value;
            _selectedTile.Type = TileType.Background;

            //Load Images, searching and tile selection box.
            GenerateSelector();

            //Draw Bedrock by default.
            DrawBedrock();
            ComboTypes.SelectedIndex = 0;
            
            this.Title = $"{this.Title} ({UpdateChecker.current})";
        }

        /// <summary>
        /// This is where we check for updates, once the window is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            isRendered = true;

            UpdateCheckProgressBar.Visibility = Visibility.Visible;
            UpdateCheckLabel.Visibility = Visibility.Visible;
            
            bool updateAvailable = await Task.Run(() => UpdateChecker.CheckForUpdates());
            isBackgroundUpdateChecking = true;

            if (updateAvailable)
            {
                this.Title = $"{this.Title} [Outdated])";
                string content = $"Found a new version ({UpdateChecker.latest}), currently using {UpdateChecker.current}. Check it out?\n\nChangelog ({UpdateChecker.latest}): {UpdateChecker.changelog}";

                MessageBoxResult result = MessageBox.Show(content, "Update", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    UpdateWindow uw = new UpdateWindow(UpdateChecker.latest);
                    if (!uw.isClosing)
                    {
                        uw.Show();
                    }
                }
            }
            isBackgroundUpdateChecking = false;

            UpdateCheckProgressBar.Visibility = Visibility.Hidden;
            UpdateCheckLabel.Visibility = Visibility.Hidden;

        }

        
        /// <summary>
        /// Save/Render a world to PNG.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Shows Shortcut MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shortcuts_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("- Ctrl + S -> Save");
            sb.AppendLine("- Ctrl + N -> New World");
            sb.AppendLine("- Ctrl + Mouse Wheel -> Canvas Zoom");
            sb.AppendLine("- Shift + Mouse Wheel -> Scroll sideways");
            sb.AppendLine("- Shift + Left Click -> Pick Background from canvas");
            sb.AppendLine("- Shift + Right Click -> Pick Foreground from canvas");

            MessageBox.Show(sb.ToString(), "Shortcuts", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows About Window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        /// <summary>
        /// Shows Stats Window (Total tiles used in a world.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stats_Click(object sender, RoutedEventArgs e)
        {
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
                                placed.Add(itemName, 1);
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
                                placed.Add(itemName, 1);
                            }
                        }
                    }
                }
            }
            StatsWindow statsWindow = new StatsWindow(placed);
            statsWindow.ShowDialog();

        }

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                FirstSelected = false;
                SaveButton.IsEnabled = false;
                PreviousTiles.Items.Clear();
                PreviousTiles.SelectedItem = null;
                SavedPath = String.Empty;
                _selectedTile.Reset();
            }
        }

        /// <summary>
        /// Custom background color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Orb Picker. Changes the entire background of a world.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                case "Desert":
                    bt = BackgroundData.BackgroundType.Desert;
                    break;
                case "Cemetery":
                    bt = BackgroundData.BackgroundType.Cemetery;
                    break;

                default:
                    bt = BackgroundData.BackgroundType.Forest;
                    break;
            }

            MainCanvas.Background = BackgroundData.GetBackground(bt);
            TileDB.hasMainBackground = true;
            TileDB.MainBackground = bt;

        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            //Scroll Horizontally. Can't be bothered to change the default scrolling value, so let's just do it twice y'not.
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (e.Delta > 0)
                {
                    scrollviewer.LineLeft();
                    scrollviewer.LineLeft();
                }
                else
                {
                    scrollviewer.LineRight();
                    scrollviewer.LineRight();
                }
            }
            //Zoom
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta > 0)
                {
                    zoomSlider.Value += 10;
                }
                else
                {
                    zoomSlider.Value -= 10;
                }
            }
            //Scroll Vertically
            else
            {
                if (e.Delta > 0)
                {
                    scrollviewer.LineUp();
                    scrollviewer.LineUp();
                }
                else
                {
                    scrollviewer.LineDown();
                    scrollviewer.LineDown();
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Exit menu button. Points to <see cref="Window_Closing"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Window_Closing(sender, new CancelEventArgs());
        }

        /// <summary>
        /// Main exit handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

