using System;
using System.Diagnostics;
using System.Linq;
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
        public static TileData TileDB = new TileData(80, 60);

        private void DrawGrid(int height, int width)
        {
            MainCanvas.Height = height * 32;
            MainCanvas.Width = width * 32;
            for (int i = 32; i <= MainCanvas.Width; i += 32)
            {
                MainCanvas.Children.Add(new Line()
                {
                    Stroke = Brushes.Gray,
                    X1 = i,
                    X2 = i,
                    Y1 = 0,
                    Y2 = MainCanvas.Height,
                    StrokeThickness = 1
                });             
            }

            for (int i = 32; i <= MainCanvas.Height; i += 32)
            {
                MainCanvas.Children.Add(new Line()
                {
                    Stroke = Brushes.Gray,
                    X1 = 0,
                    X2 = MainCanvas.Width,
                    Y1 = i,
                    Y2 = i,
                    StrokeThickness = 1
                });
            }
        }

        private void DrawBedrock()
        {
            if (TileDB.Width != 80 || TileDB.Height != 60)
            {
                return;
            }
            int location = 2;
            for (int y = TileDB.Height - 1; y > TileDB.Height - 4; y--)
            {
                for (int x = TileDB.Width - 1; x >= 0 ; x--)
                {
                    System.Drawing.Bitmap bmp = Utils.GetCroppedBitmap(TileType.Foreground, location * 32, 0 * 32);
                    Image image = Utils.BitmapToImageControl(bmp);
                    Canvas.SetTop(image, y * 32);
                    Canvas.SetLeft(image, x * 32);
                    image.SetValue(Canvas.ZIndexProperty, 10);
                    MainCanvas.Children.Add(image);
                    TilePosition Position = new TilePosition(TileType.Foreground, x, y ,location, 0);
                    TileDB.Tiles[x, y] = new Tile(TileType.Foreground, image, Position);
                    
                    bmp.Dispose();
                }
                location--;
            }
        }

        public bool Exists(int X, int Y)
        {
            if (TileDB.Tiles[X, Y] != null)
            {
                return true;
            }
            return false;
        }

        public bool HasBackgroundAt(int X, int Y)
        {
            if (Exists(X, Y))
            {
                if (TileDB.Tiles[X, Y].Background != null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool HasForegroundAt(int X, int Y)
        {
            if (Exists(X, Y))
            {
                if (TileDB.Tiles[X, Y].Foreground != null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool AlreadyHasBothTypes(int X, int Y)
        {
            if (Exists(X, Y))
            {
                if (TileDB.Tiles[X, Y].Type == TileType.Both)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool SameTypeAt(Tile tile, int X, int Y)
        {
            if (TileDB.Tiles[X, Y] == null)
            {
                return false;
            }
            if (tile.Type == TileDB.Tiles[X, Y].Type)
            {
                return true;
            }
            return false;
        }

        public void RemoveSameTileTypeAt(Tile tile, int X, int Y)
        {
            if (TileDB.Tiles[X, Y] != null)
            {
                //If there are both types on 1 tile, and the current selected is a background, leave the foreground only
                if ((TileDB.Tiles[X, Y].Type == TileType.Both) && (tile.Type == TileType.Background))
                {
                    MainCanvas.Children.Remove(TileDB.Tiles[X, Y].Background);
                    TileDB.Tiles[X, Y].Type = TileType.Foreground;
                    TileDB.Tiles[X, Y].Background = null;
                    TileDB.Tiles[X, Y].Positions.DeleteBackgroundPositions();
                }
                //If there are both types on 1 tile, and the current selected is a foreground, leave the background only
                else if ((TileDB.Tiles[X, Y].Type == TileType.Both) && (tile.Type == TileType.Foreground))
                {
                    MainCanvas.Children.Remove(TileDB.Tiles[X,Y].Foreground);
                    TileDB.Tiles[X, Y].Type = TileType.Background;
                    TileDB.Tiles[X, Y].Foreground = null;
                    TileDB.Tiles[X, Y].Positions.DeleteForegroundPositions();
                }
                //If there is only one type, just erase the tile.
                else if (TileDB.Tiles[X, Y].Type == tile.Type)
                {
                    switch (tile.Type)
                    {
                        case TileType.Background:
                            MainCanvas.Children.Remove(TileDB.Tiles[X, Y].Background);
                            TileDB.Tiles[X, Y].Positions.DeleteBackgroundPositions();
                            break;
                        case TileType.Foreground:
                            MainCanvas.Children.Remove(TileDB.Tiles[X, Y].Foreground);
                            TileDB.Tiles[X, Y].Positions.DeleteForegroundPositions();
                            break;
                    }
                    TileDB.Tiles[X, Y] = null;
                }
            }
        }

        public string TileInfo(int X, int Y)
        {
            if (TileDB.Tiles[X, Y] == null)
            {
                return "Empty";
            }
            return $"{X}, {Y}, Type = {TileDB.Tiles[X, Y].Type}";
        }
    }
}
