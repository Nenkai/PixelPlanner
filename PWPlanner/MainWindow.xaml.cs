using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;

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
            DrawBedrock();
            MainCanvas.Background = BackgroundData.GetBackground(BackgroundData.BackgroundType.Forest);
            TileDB.MainBackground = BackgroundData.BackgroundType.Forest;
            TileDB.hasMainBackground = true;

            defaultMatrix = MainCanvas.LayoutTransform.Value;
            _selectedTile.Type = TileType.Background;
            GenerateSelector();
            ComboTypes.SelectedIndex = 0;

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            isRendered = true;
            if (UpdateChecker.CheckForUpdates())
            {
                MessageBoxResult result = MessageBox.Show($"Found a new version ({UpdateChecker.latest}), currently using {UpdateChecker.current}. Would you like to update?", "Update", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start($"https://github.com/Nenkai/PixelPlanner/releases/download/{UpdateChecker.latest}/PWPlanner.exe");
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
                if (SameTypeAt(_selectedTile, pos.X, pos.Y))
                {
                    DeleteAt(pos.X, pos.Y, _selectedTile);
                }
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

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PNG Files (*.png)|*.png";
            dialog.DefaultExt = "png";
            dialog.FileName = "World.png";
            dialog.RestoreDirectory = true;
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
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Pixel Worlds World (*.pww)|*.pww";
            dialog.DefaultExt = "pww";
            dialog.FileName = "world.pww";
            dialog.RestoreDirectory = true;
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

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Pixel Worlds World (*.pww)|*.pww";
            dialog.DefaultExt = "pww";
            dialog.RestoreDirectory = true;
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

                for (int i = 0; i < TileDB.Tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < TileDB.Tiles.GetLength(1); j++)
                    {
                        if (TileDB.Tiles[i, j] != null)
                        {
                            if (TileDB.Tiles[i, j].Type == TileType.Background || TileDB.Tiles[i, j].Type == TileType.Both)
                            {
                                System.Drawing.Bitmap bmp = Utils.GetCroppedBitmap(TileType.Background, TileDB.Tiles[i, j].Positions.BackgroundSpriteX.Value * 32, TileDB.Tiles[i, j].Positions.BackgroundSpriteY.Value * 32);
                                Image image = Utils.BitmapToImageControl(bmp);
                                Canvas.SetTop(image, TileDB.Tiles[i, j].Positions.BackgroundY.Value * 32);
                                Canvas.SetLeft(image, TileDB.Tiles[i, j].Positions.BackgroundX.Value * 32);
                                image.SetValue(Canvas.ZIndexProperty, 10);
                                MainCanvas.Children.Add(image);
                                TileDB.Tiles[i, j].Background = image;
                                bmp.Dispose();
                            }
                            if (TileDB.Tiles[i, j].Type == TileType.Foreground || TileDB.Tiles[i, j].Type == TileType.Both)
                            {
                                System.Drawing.Bitmap bmp = Utils.GetCroppedBitmap(TileType.Foreground, TileDB.Tiles[i, j].Positions.ForegroundSpriteX.Value * 32, TileDB.Tiles[i, j].Positions.ForegroundSpriteY.Value * 32);
                                Image image = Utils.BitmapToImageControl(bmp);
                                Canvas.SetTop(image, TileDB.Tiles[i, j].Positions.ForegroundY.Value * 32);
                                Canvas.SetLeft(image, TileDB.Tiles[i, j].Positions.ForegroundX.Value * 32);
                                image.SetValue(Canvas.ZIndexProperty, 20);
                                MainCanvas.Children.Add(image);
                                TileDB.Tiles[i, j].Foreground = image;
                                bmp.Dispose();
                            }
                        }
                    }
                    ColorSelector.SelectedColor = Utils.IntToARGBColor(TileDB.ARGBBackgroundColor);
                    
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
                RadioButton rb = mi.Icon as RadioButton;
                if (rb != null)
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

    }
}

