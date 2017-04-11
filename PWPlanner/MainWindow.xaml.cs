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

        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();
            _selectedTile.Type = TileType.Background;
            GenerateSelector();
            ComboTypes.SelectedIndex = 0;
        }

        private void DrawGrid()
        {
            MainCanvas.Height = 32 * 59;
            MainCanvas.Width = 32 * 80;
            for (int i = 32; i <= MainCanvas.Width; i += 32)
            {
                Line vertLine = new Line();
                vertLine.Stroke = Brushes.Gray;
                vertLine.X1 = i;
                vertLine.X2 = i;
                vertLine.Y1 = 0;
                vertLine.Y2 = MainCanvas.Height;
                vertLine.StrokeThickness = 1;
                MainCanvas.Children.Add(vertLine);
            }

            for (int i = 32; i <= MainCanvas.Height; i += 32)
            {
                Line horiLine = new Line();
                horiLine.Stroke = Brushes.Gray;
                horiLine.X1 = 0;
                horiLine.X2 = MainCanvas.Width;
                horiLine.Y1 = i;
                horiLine.Y2 = i;
                horiLine.StrokeThickness = 1;
                MainCanvas.Children.Add(horiLine);
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
                        Image image = new Image();
                        if (_selectedTile.Type == TileType.Background)
                        {
                            image.Source = _selectedTile.Background.Source;
                        }
                        else if (_selectedTile.Type == TileType.Foreground)
                        {
                            image.Source = _selectedTile.Foreground.Source;
                        }
                        Canvas.SetTop(image, pos.Y * 32);
                        Canvas.SetLeft(image, pos.X * 32);
                        MainCanvas.Children.Add(image);

                        switch (_selectedTile.Type)
                        {
                            case TileType.Background:
                                image.SetValue(Canvas.ZIndexProperty, 10);
                                if (HasForegroundAt(pos.X, pos.Y))
                                {
                                    TileData[pos.X, pos.Y].Type = TileType.Both;
                                    TileData[pos.X, pos.Y].Background = image;
                                    if (TileData[pos.X, pos.Y].Positions == null)
                                    {
                                        TileData[pos.X, pos.Y].Positions = new TilePosition(_selectedTile.Type, pos.X, pos.Y, _selectedTile.X, _selectedTile.Y);
                                    } else
                                    {
                                        TileData[pos.X, pos.Y].Positions.SetBackgroundPositions(pos.X, pos.Y, _selectedTile.X, _selectedTile.Y);
                                    }

                                }
                                else
                                {
                                    TilePosition Position = new TilePosition(_selectedTile.Type, pos.X, pos.Y, _selectedTile.X, _selectedTile.Y);
                                    TileData[pos.X, pos.Y] = new Tile(TileType.Background, image, Position);
                                }
                                break;
                            case TileType.Foreground:
                                image.SetValue(Canvas.ZIndexProperty, 20);
                                if (HasBackgroundAt(pos.X, pos.Y))
                                {
                                    TileData[pos.X, pos.Y].Type = TileType.Both;
                                    TileData[pos.X, pos.Y].Foreground = image;
                                    if (TileData[pos.X, pos.Y].Positions == null)
                                    {
                                        TileData[pos.X, pos.Y].Positions = new TilePosition(_selectedTile.Type, pos.X, pos.Y, _selectedTile.X, _selectedTile.Y);
                                    }
                                    else
                                    {
                                        TileData[pos.X, pos.Y].Positions.SetForegroundPositions(pos.X, pos.Y, _selectedTile.X, _selectedTile.Y);
                                    }
                                }
                                else
                                {
                                    TilePosition Position = new TilePosition(_selectedTile.Type, pos.X, pos.Y, _selectedTile.X, _selectedTile.Y);
                                    TileData[pos.X, pos.Y] = new Tile(TileType.Foreground, image, Position);
                                }
                                break;
                        }
                    }
                } 
            }

            //Delete Tiles
            if (e.RightButton == MouseButtonState.Pressed)
            {
                RemoveSameTileTypeAt(_selectedTile, pos.X, pos.Y);
            }
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
                DataHandler.SaveWorld(TileData, path);
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
                DrawGrid();
                TileData = (Tile[,])DataHandler.LoadWorld(path);
                for (int i = 0; i < TileData.GetLength(0); i++)
                {
                    for (int j = 0; j < TileData.GetLength(1); j++)
                    {
                        if (TileData[i, j] != null)
                        {
                            if (TileData[i, j].Type == TileType.Background || TileData[i, j].Type == TileType.Both)
                            {
                                System.Drawing.Bitmap bmp = Utils.GetCroppedBitmap(TileType.Background, TileData[i, j].Positions.BackgroundSpriteX.Value * 32, TileData[i, j].Positions.BackgroundSpriteY.Value * 32);
                                Image image = Utils.BitmapToImageControl(bmp);
                                Canvas.SetTop(image, TileData[i, j].Positions.BackgroundY.Value * 32);
                                Canvas.SetLeft(image, TileData[i, j].Positions.BackgroundX.Value * 32);
                                image.SetValue(Canvas.ZIndexProperty, 10);
                                MainCanvas.Children.Add(image);
                                TileData[i, j].Background = image;
                            }
                            if (TileData[i, j].Type == TileType.Foreground || TileData[i, j].Type == TileType.Both)
                            {
                                System.Drawing.Bitmap bmp = Utils.GetCroppedBitmap(TileType.Foreground, TileData[i, j].Positions.ForegroundSpriteX.Value * 32, TileData[i, j].Positions.ForegroundSpriteY.Value * 32);
                                Image image = Utils.BitmapToImageControl(bmp);
                                Canvas.SetTop(image, TileData[i, j].Positions.ForegroundY.Value * 32);
                                Canvas.SetLeft(image, TileData[i, j].Positions.ForegroundX.Value * 32);
                                image.SetValue(Canvas.ZIndexProperty, 20);
                                MainCanvas.Children.Add(image);
                                TileData[i, j].Foreground = image;
                            }
                        }
                    }
                }
            }
        }
    }
}

