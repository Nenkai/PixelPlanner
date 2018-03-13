using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Generic;

using PWPlanner.TileTypes;

using MaterialDesignThemes.Wpf;

namespace PWPlanner
{
    public partial class MainWindow : Window
    {
        public static PlannerSettings DB;
        public static Line[] vertLines;
        public static Line[] horiLines;

        public static string[] blacklist = new string[] { "Bedrock", "BedrockFlat", "BedrockLava" };

        #region WPF Handlers
        //Disable/Enable Grid
        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            if (gridButton.IsChecked)
            {
                RemoveGrid();
                gridButton.Icon = PackIconKind.GridOff;
            }
            else
            {
                DrawGrid(DB.WorldWidth, DB.WorldHeight);
            }
        }

        //Painter
        public void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {

            int X = (int)(e.GetPosition(MainCanvas).X) / 32;
            int Y = (int)(e.GetPosition(MainCanvas).Y) / 32;
            int realY = 60 - Y - 1;

            PosLabel.Content = $"X = {X} | Y = {realY}";

            //Last pixel crashes the entire thing. Why? No idea.
            if (Y == 60 || X == 80)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Check if a tile has been selected before doing anything first.
                if (FirstSelected)
                {
                    //Check if the tile selected can be placed at a position
                    if (!HasSameTypeAt(X, Y, _selectedTile))
                    {
                        //So we can make sure to put a confirm box if the user makes a new world or opens one
                        if (!firstPlaced) firstPlaced = true;

                        PlaceAt(X, Y, _selectedTile);

                    }
                }
            }

            //Delete Tiles
            if (e.RightButton == MouseButtonState.Pressed)
            {
                DeleteAt(X, Y, _selectedTile);
            }

            //Pan with mouse wheel click
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                scrollMousePoint = e.GetPosition(sv);
                hOff = sv.HorizontalOffset;
                vOff = sv.VerticalOffset;
                sv.CaptureMouse();
            }
        }

        //Pass Click to Move + Tile Selector
        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                int X = (int)(e.GetPosition(MainCanvas).X) / 32;
                int Y = (int)(e.GetPosition(MainCanvas).Y) / 32;

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (HasBackgroundAt(DB, X, Y))
                    {
                        Background bg = GetBackgroundAt(X, Y);
                        tileMap.TryGetValue(bg.TileName, out Tile src);
                        Image image = new Image()
                        {
                            Source = src.Image.Source
                        };
                        _selectedTile = new Background(image);
                        _selectedTile.TileName = bg.ToString();
                        SelectTile(bg.ToString());
                        LabelImg.Source = src.Image.Source;
                        TileHover.Content = bg;

                    }
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (HasForegroundAt(DB, X, Y))
                    {
                        Foreground block = GetForegroundAt(DB, X, Y);
                        tileMap.TryGetValue(block.TileName, out Tile src);
                        Image image = new Image()
                        {
                            Source = src.Image.Source
                        };

                        _selectedTile = new Foreground(image);
                        _selectedTile.TileName = block.TileName;
                        SelectTile(block.TileName);
                        LabelImg.Source = src.Image.Source;
                        TileHover.Content = block.TileName;
                    }
                }

            }
            else
            {
                MainCanvas_MouseMove(sender, e);
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
                }
                else
                {
                    scale = new ScaleTransform(MainCanvas.LayoutTransform.Value.M11 - 0.13, MainCanvas.LayoutTransform.Value.M22 - 0.13);
                }
                MainCanvas.LayoutTransform = scale;
                MainCanvas.InvalidateVisual();
            }
        }

        private void DrawGrid(int width, int height)
        {
            MainCanvas.Height = height * 32;
            MainCanvas.Width = width * 32;

            vertLines = new Line[width];
            for (int i = 32; i <= MainCanvas.Width; i += 32)
            {
                int count = i / 32 - 1;
                vertLines[count] = new Line()
                {
                    Stroke = Brushes.Gray,
                    X1 = i,
                    X2 = i,
                    Y1 = 0,
                    Y2 = MainCanvas.Height,
                    StrokeThickness = 1
                };
                MainCanvas.Children.Add(vertLines[count]);
            }

            horiLines = new Line[height];
            for (int i = 32; i <= MainCanvas.Height; i += 32)
            {
                int count = i / 32 - 1;
                horiLines[count] = new Line()
                {
                    Stroke = Brushes.Gray,
                    X1 = 0,
                    X2 = MainCanvas.Width,
                    Y1 = i,
                    Y2 = i,
                    StrokeThickness = 1
                };
                MainCanvas.Children.Add(horiLines[count]);
            }

            gridButton.IsChecked = true;
        }

        private void RemoveGrid()
        {
            //Temporarily get rid of cache, so the lines doesn't stay until visual update
            CacheMode cm = MainCanvas.CacheMode;
            MainCanvas.CacheMode = null;

            for (int i = 0; i < horiLines.Length - 1; i++)
            {
                MainCanvas.Children.Remove(horiLines[i]);
            }
            for (int i = 0; i < vertLines.Length - 1; i++)
            {
                MainCanvas.Children.Remove(vertLines[i]);
            }
            MainCanvas.CacheMode = cm;

            gridButton.IsChecked = false;
        }
        #endregion

        private void GenerateTileMap(short Width, short Height)
        {
            DB = new PlannerSettings(Width, Height);
        }

        private void DrawBedrock()
        {
            int WorldWidth = DB.TileMap.GetLength(0);
            int WorldHeight = DB.TileMap.GetLength(1);

            int location = 2;
            for (int y = WorldHeight - 1; y > WorldHeight - 4; y--)
            {
                for (int x = WorldWidth - 1; x >= 0; x--)
                {
                    string tileName;
                    if (y == WorldHeight - 1) tileName = "BedrockLava";
                    else if (y == WorldHeight - 2) tileName = "Bedrock";
                    else tileName = "BedrockFlat";

                    Image image = new Image();
                    tileMap.TryGetValue(tileName, out Tile src);
                    image.Source = src.Image.Source;
                    Canvas.SetTop(image, y * 32);
                    Canvas.SetLeft(image, x * 32);
                    image.SetValue(Canvas.ZIndexProperty, 1);
                    MainCanvas.Children.Add(image);

                    if (!AnyTileAt(DB, x, y))
                    {
                        DB.TileMap[x, y].Tiles.Add(new Foreground(image) { TileName = tileName });
                    }
                }
                location--;
            }
        }

        private Tile GetTileFromName(string tileName)
        {
            tileMap.TryGetValue(tileName, out Tile tmp);
            tmp.Image = new Image() { Source = tmp.Image.Source };
            tmp.TileName = tileName;
            return tmp;
        }

        private void PlaceAt(int X, int Y, Tile tile)
        {
            Image image = new Image()
            {
                Source = selectableTiles[index].Image.Source
            };

            Canvas.SetTop(image, Y * 32);
            Canvas.SetLeft(image, X * 32);
            MainCanvas.Children.Add(image);
            
            image.SetValue(Canvas.ZIndexProperty, tile.ZIndex);

            //Make a new object, type based on input tile
            Tile tempTile = tile.Clone(image);


            DB.TileMap[X, Y].Tiles.Add(tempTile);
            AddToPreviousTilesSelected(tile);
        }


        public void DeleteAt(int X, int Y, Tile tile)
        {
            if (AnyTileAt(DB, X, Y))
            {
                foreach (var t in DB.TileMap[X, Y].Tiles.ToList())
                {
                    if (Object.ReferenceEquals(t.GetType(), tile.GetType()))
                    {
                        MainCanvas.Children.Remove(t.Image);
                        DB.TileMap[X, Y].Tiles.Remove(t);
                    }
                }
            }
        }

        public List<Tile> GetTilesAt(int X, int Y)
        {
            return DB.TileMap[X, Y].Tiles;
        }

        public bool HasSameTypeAt(int X, int Y, Tile tile)
        {
            if (AnyTileAt(DB, X, Y))
            {
                foreach (var t in GetTilesAt(X, Y).ToList())
                {
                    if (Object.ReferenceEquals(t.GetType(), tile.GetType())) return true;
                }
                return false;
            }
            return false;
        }

        public Foreground GetForegroundAt(PlannerSettings s, int X, int Y)
        {
            if (HasForegroundAt(s, X, Y))
            {
                return DB.TileMap[X, Y].Tiles.OfType<Foreground>().ToArray()[0];
            }
            return null;
        }

        public Background GetBackgroundAt(int X, int Y)
        {
            if (HasBackgroundAt(DB, X, Y))
            {
                return DB.TileMap[X, Y].Tiles.OfType<Background>().ToArray()[0];
            }
            return null;
        }

        public Special GetSpecialAt(int X, int Y)
        {
            if (HasBackgroundAt(DB, X, Y))
            {
                return DB.TileMap[X, Y].Tiles.OfType<Special>().ToArray()[0];
            }
            return null;
        }

        public static bool AnyTileAt(PlannerSettings s, int X, int Y)
        {
            if (s.TileMap[X, Y] != null)
            {
                if (s.TileMap[X, Y].Tiles.Count > 0)
                {
                    return true;
                }
                return false;
            }
            s.TileMap[X, Y] = new TileData();
            return false;
        }

        public bool HasBackgroundAt(PlannerSettings s, int X, int Y)
        {
            if (AnyTileAt(s, X, Y))
            {
                return s.TileMap[X, Y].Tiles.OfType<Background>().Any();
            }
            return false;
        }

        public bool HasForegroundAt(PlannerSettings s, int X, int Y)
        {
            if (AnyTileAt(s, X, Y))
            {
                return DB.TileMap[X, Y].Tiles.OfType<Foreground>().Any();
            }
            return false;
        }

        public bool HasSpecialAt(PlannerSettings s, int X, int Y)
        {
            if (AnyTileAt(s, X, Y))
            {
                return DB.TileMap[X, Y].Tiles.OfType<Special>().Any();
            }
            return false;
        }

    }

}
