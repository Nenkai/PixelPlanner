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
        public static Tile[,] TileData = new Tile[80, 59];

        public bool Exists(int X, int Y)
        {
            if (TileData[X, Y] != null)
            {
                return true;
            }
            return false;
        }

        public bool HasBackgroundAt(int X, int Y)
        {
            if (Exists(X, Y))
            {
                if (TileData[X, Y].Background != null)
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
                if (TileData[X, Y].Foreground != null)
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
                if (TileData[X, Y].Type == TileType.Both)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool SameTypeAt(Tile tile, int X, int Y)
        {
            if (TileData[X, Y] == null)
            {
                return false;
            }
            if (tile.Type == TileData[X, Y].Type)
            {
                return true;
            }
            return false;
        }

        public void RemoveSameTileTypeAt(Tile tile, int X, int Y)
        {
            if (TileData[X, Y] != null)
            {
                //If there are both types on 1 tile, and the current selected is a background, leave the foreground only
                if ((TileData[X, Y].Type == TileType.Both) && (tile.Type == TileType.Background))
                {
                    MainCanvas.Children.Remove(TileData[X, Y].Background);
                    TileData[X, Y].Type = TileType.Foreground;
                    TileData[X, Y].Background = null;
                    TileData[X, Y].Positions.DeleteBackgroundPositions();
                }
                //If there are both types on 1 tile, and the current selected is a foreground, leave the background only
                else if ((TileData[X, Y].Type == TileType.Both) && (tile.Type == TileType.Foreground))
                {
                    MainCanvas.Children.Remove(TileData[X,Y].Foreground);
                    TileData[X, Y].Type = TileType.Background;
                    TileData[X, Y].Foreground = null;
                    TileData[X, Y].Positions.DeleteForegroundPositions();
                }
                //If there is only one type, just erase the tile.
                else if (TileData[X, Y].Type == tile.Type)
                {
                    switch (tile.Type)
                    {
                        case TileType.Background:
                            MainCanvas.Children.Remove(TileData[X, Y].Background);
                            TileData[X, Y].Positions.DeleteBackgroundPositions();
                            break;
                        case TileType.Foreground:
                            MainCanvas.Children.Remove(TileData[X, Y].Foreground);
                            TileData[X, Y].Positions.DeleteForegroundPositions();
                            break;
                    }
                    TileData[X, Y] = null;
                }
            }
        }

        public string TileInfo(int X, int Y)
        {
            if (TileData[X, Y] == null)
            {
                return "Empty";
            }
            return $"{X}, {Y}, Type = {TileData[X, Y].Type}";
        }
    }
}
