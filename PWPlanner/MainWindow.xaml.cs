using PWPlanner;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Windows;
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
                                }
                                else
                                {
                                    TileData[pos.X, pos.Y] = new Tile(TileType.Background, image);
                                }
                                break;
                            case TileType.Foreground:
                                image.SetValue(Canvas.ZIndexProperty, 20);
                                if (HasBackgroundAt(pos.X, pos.Y))
                                {
                                    TileData[pos.X, pos.Y].Type = TileType.Both;
                                    TileData[pos.X, pos.Y].Foreground = image;
                                }
                                else
                                {
                                    TileData[pos.X, pos.Y] = new Tile(TileType.Foreground, image);
                                }
                                break;
                        }
                    }
                } 
            }
        }

        //Delete Tiles
        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                CanvasPos pos = new CanvasPos(e.GetPosition(MainCanvas));
                PosLabel.Content = $"({pos.X},{pos.Y})";
                RemoveSameTileTypeAt(_selectedTile, pos.X, pos.Y);
            }
        }
    }
}

