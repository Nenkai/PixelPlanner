using System;
using PWPlanner;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace PWPlanner
{
    public partial class MainWindow : Window
    {
        private bool isRendered = false;
        private Tile LastPlacedOrRemoved;
        
        public MainWindow()
        {
            InitializeComponent();
            DrawGrid(TileDB.Height, TileDB.Width);
            DrawBedrock();
            MainCanvas.Background = new SolidColorBrush(Color.FromRgb(140, 226, 249));
            _selectedTile.Type = TileType.Background;
            GenerateSelector();
            ComboTypes.SelectedIndex = 0;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            isRendered = true;
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
                        PlaceAt(pos.X, pos.Y, _selectedTile);
                    }
                } 
            }

            //Delete Tiles
            if (e.RightButton == MouseButtonState.Pressed)
            {
                DeleteAt(pos.X, pos.Y, _selectedTile);
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
                MessageBox.Show("Image saved successfully at\n" + path, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //World Load
        private void LoadWorld_Click(object sender, RoutedEventArgs e)
        {
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
                }
            }
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

        private void Undo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {

        }

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
    }
}

