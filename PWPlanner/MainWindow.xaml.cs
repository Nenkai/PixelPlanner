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

using PWPlanner.TileTypes;
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
            DrawGrid(PlannerSettings.DefaultWorldWidth, PlannerSettings.DefaultWorldHeight);

            //Used for zooming stuff.
            defaultMatrix = MainCanvas.LayoutTransform.Value;

            //We have block type selected by default.
            _selectedTile = new Foreground();

            //Load Images, searching and tile selection box.
            GenerateSelector();

            //Initialize the map used to store tiles, normal world size by default
            GenerateTileMap(PlannerSettings.DefaultWorldWidth, PlannerSettings.DefaultWorldHeight);

            //Set Background to forest.
            MainCanvas.Background = BackgroundData.GetBackground(BackgroundData.MainBackgroundType.Forest);
            DB.MainBackground = BackgroundData.MainBackgroundType.Forest;
            DB.hasMainBackground = true;

            //Draw Bedrock by default.
            DrawBedrock();
            
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

            for (int i = 0; i < DB.TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < DB.TileMap.GetLength(1); j++)
                {
                    if (AnyTileAt(DB, i, j))
                    {
                        foreach (var tile in DB.TileMap[i, j].Tiles)
                        {
                            if (Array.IndexOf(blacklist, tile.TileName) > -1)
                            {
                                continue;
                            }
                            else if (placed.ContainsKey(tile.TileName))
                            {
                                placed[tile.TileName]++;
                            }
                            else
                            {
                                placed.Add(tile.TileName, 1);
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
                DB = new PlannerSettings(80, 60);
                DrawGrid(DB.WorldWidth, DB.WorldHeight);
                DrawBedrock();
                MainCanvas.Background = BackgroundData.GetBackground(DB.MainBackground);
                defaultMatrix = MainCanvas.LayoutTransform.Value;
                //_selectedTile.TileName
                ComboTypes.SelectedIndex = 1;
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

            if (DB.MainBackground != BackgroundData.MainBackgroundType.None)
            {
                DB.hasMainBackground = false;
                DB.MainBackground = BackgroundData.MainBackgroundType.None;
            }

            MainCanvas.Background = new SolidColorBrush(e.NewValue.Value);
            DB.ARGBBackgroundColor = Utils.ARGBColortoInt(e.NewValue.Value);
            DB.hasCustomMainBackgroundColor = true;
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

            BackgroundData.MainBackgroundType bt;
            switch (mi.Name)
            {
                case "Forest":
                    bt = BackgroundData.MainBackgroundType.Forest;
                    break;
                case "Night":
                    bt = BackgroundData.MainBackgroundType.Night;
                    break;
                case "Star":
                    bt = BackgroundData.MainBackgroundType.Star;
                    break;
                case "Candy":
                    bt = BackgroundData.MainBackgroundType.Candy;
                    break;
                case "Winter":
                    bt = BackgroundData.MainBackgroundType.Winter;
                    break;
                case "Alien":
                    bt = BackgroundData.MainBackgroundType.Alien;
                    break;
                case "Desert":
                    bt = BackgroundData.MainBackgroundType.Desert;
                    break;
                case "Cemetery":
                    bt = BackgroundData.MainBackgroundType.Cemetery;
                    break;

                default:
                    bt = BackgroundData.MainBackgroundType.Forest;
                    break;
            }

            MainCanvas.Background = BackgroundData.GetBackground(bt);
            DB.hasMainBackground = true;
            DB.MainBackground = bt;

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

        Point scrollMousePoint = new Point();
        double hOff = 1;
        double vOff = 1;
        private void sv_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sv.IsMouseCaptured)
            {
                Mouse.OverrideCursor = Cursors.Hand;
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
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

        private void sv_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                sv.ReleaseMouseCapture();
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }
    }
}

