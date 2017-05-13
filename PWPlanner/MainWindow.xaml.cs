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

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            isRendered = true;
            if (UpdateChecker.CheckForUpdates())
            {
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

        //Painter
        public void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            CanvasPos pos = new CanvasPos(e.GetPosition(MainCanvas));
            PosLabel.Content = $"({pos.X},{pos.Y})";
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Check if a tile has been selected before doing anything first.
                if (FirstSelected)
                {
                    //Check if the tile selected can be placed at a position
                    if (!SameTypeAt(_selectedTile, pos.X, pos.Y) && !AlreadyHasBothTypes(pos.X, pos.Y))
                    {
                        //So we can make sure to put a confirm box if the user makes a new world or opens one
                        if (!firstPlaced) firstPlaced = true;

                        PlaceAt(pos.X, pos.Y, _selectedTile);

                    }
                }
            }

            //Delete Tiles
            if (e.RightButton == MouseButtonState.Pressed)
            {
                DeleteAt(pos.X, pos.Y, _selectedTile);
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (TileDB.Tiles[pos.X, pos.Y] != null) Debug.WriteLine(TileDB.Tiles[pos.X, pos.Y].ToString());
            }
        }

        //Pass Click to Move
        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainCanvas_MouseMove(sender, e);
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

        //World Save
        private void SaveWorld_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Pixel Worlds World (*.pww)|*.pww",
                DefaultExt = "pww",
                FileName = "world.pww",
                RestoreDirectory = true
            };
            Nullable<bool> Selected = dialog.ShowDialog();
            string path = dialog.FileName;
            if (Selected == true)
            {
                DataHandler.SaveWorld(TileDB, path);
                MessageBox.Show("World saved successfully at\n" + path, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //World Load
        private void LoadWorld_Click(object sender, RoutedEventArgs e)
        {
            if (firstPlaced && MessageBox.Show("Are you sure to load a new world? You may lose all your unsaved progress!", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            isLoading = true;

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Pixel Worlds World (*.pww)|*.pww",
                DefaultExt = "pww",
                RestoreDirectory = true
            };
            Nullable<bool> Selected = dialog.ShowDialog();
            string path = dialog.FileName;

            if (Selected == true)
            {

                MainCanvas.Children.Clear();
                TileDB = (TileData)DataHandler.LoadWorld(path);
                DrawGrid(TileDB.Height, TileDB.Width);

                if (!TileDB.hasMainBackground)
                {
                    MainCanvas.Background = new SolidColorBrush(Utils.IntToARGBColor(TileDB.ARGBBackgroundColor));
                }
                else
                {
                    MainCanvas.Background = BackgroundData.GetBackground(TileDB.MainBackground);
                }

                SortedList<string, int> invalids = new SortedList<string, int>();
                for (int i = 0; i < TileDB.Tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < TileDB.Tiles.GetLength(1); j++)
                    {
                        if (TileDB.Tiles[i, j] != null)
                        {
                            if (TileDB.Tiles[i, j].Type == TileType.Background || TileDB.Tiles[i, j].Type == TileType.Both)
                            {
                                Image image = new Image()
                                {
                                    Height = 32,
                                    Width = 32
                                };
                                string dataName = TileDB.Tiles[i, j].bgName;
                                BackgroundName name = SelectableTile.GetBackgroundNameByString(TileDB.Tiles[i, j].bgName);

                                //If the sprite does not exist.
                                bool exists = backgroundMap.TryGetValue(name, out BitmapImage src);
                                if (!exists && dataName != null)
                                {
                                    if (invalids.ContainsKey(dataName))
                                    {
                                        invalids[dataName]++;
                                    }
                                    else
                                    {
                                        invalids.Add(dataName, 0);
                                    }
                                    TileDB.Tiles[i, j] = null;
                                }
                                else
                                {
                                    image.Source = src;

                                    Canvas.SetTop(image, TileDB.Tiles[i, j].Positions.BackgroundY.Value * 32);
                                    Canvas.SetLeft(image, TileDB.Tiles[i, j].Positions.BackgroundX.Value * 32);
                                    image.SetValue(Canvas.ZIndexProperty, 10);
                                    MainCanvas.Children.Add(image);
                                    TileDB.Tiles[i, j].Background = image;
                                }
                            }

                            if (TileDB.Tiles[i, j].Type == TileType.Foreground || TileDB.Tiles[i, j].Type == TileType.Both)
                            {
                                Image image = new Image()
                                {
                                    Height = 32,
                                    Width = 32
                                };
                                string dataName = TileDB.Tiles[i, j].blName;
                                BlockName name = SelectableTile.GetBlockNameByString(TileDB.Tiles[i, j].blName);

                                bool exists = blockMap.TryGetValue(name, out BitmapImage src);
                                if (!exists && dataName != null)
                                {
                                    if (invalids.ContainsKey(dataName))
                                    {
                                        invalids[dataName]++;
                                    }
                                    else
                                    {
                                        invalids.Add(dataName, 0);
                                    }
                                    TileDB.Tiles[i, j] = null;
                                }
                                else
                                {
                                    image.Source = src;

                                    Canvas.SetTop(image, TileDB.Tiles[i, j].Positions.ForegroundY.Value * 32);
                                    Canvas.SetLeft(image, TileDB.Tiles[i, j].Positions.ForegroundX.Value * 32);
                                    image.SetValue(Canvas.ZIndexProperty, 20);
                                    MainCanvas.Children.Add(image);
                                    TileDB.Tiles[i, j].Foreground = image;
                                }
                            }
                        }
                    }
                    ColorSelector.SelectedColor = Utils.IntToARGBColor(TileDB.ARGBBackgroundColor);
                }
                if (invalids.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Could not load {invalids.Count} tiles (Using an older version?)");
                    foreach(KeyValuePair<string, int> entry in invalids)
                    {
                        sb.AppendLine($"-{entry.Key} [x{entry.Value}]");
                    }
                    MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            isLoading = false;
            
        }

        //Zoom
        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isRendered)
            {
                ScaleTransform scale;
                if (e.NewValue > e.OldValue)
                {
                    scale = new ScaleTransform(MainCanvas.LayoutTransform.Value.M11 + 0.13, MainCanvas.LayoutTransform.Value.M22 + 0.13);
                } else
                {
                    scale = new ScaleTransform(MainCanvas.LayoutTransform.Value.M11 - 0.13, MainCanvas.LayoutTransform.Value.M22 -0.13);
                }
                MainCanvas.LayoutTransform = scale;
                MainCanvas.UpdateLayout();
            }
        }

        //Zoom on Shift+Wheel
        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
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
        }

        //Disable/Enable Grid
        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            if (gridButton.IsChecked)
            {
                RemoveGrid();
            }
            else
            {
                DrawGrid(TileDB.Height, TileDB.Width);
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
        private void OrbsRadioButton_Click(object sender, System.Windows.RoutedEventArgs e)
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
        }

    }
}

