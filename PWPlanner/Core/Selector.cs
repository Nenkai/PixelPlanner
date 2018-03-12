using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using PWPlanner.TileTypes;

namespace PWPlanner
{
    public partial class MainWindow : Window
    {

        private Tile _selectedTile;
        public Border selectBorder = new Border();
        public bool FirstSelected = false;
        public int index;

        /// <summary>
        /// (Re)Generates the selector by loading all the tiles present in the resources.
        /// </summary>
        private void GenerateSelector()
        {
            ComboTypes.Items.Add("Backgrounds");
            ComboTypes.Items.Add("Blocks");
            ComboTypes.Items.Add("Special");

            //Load tile database
            LoadResourcesIntoTileMap();

            ComboTypes.SelectedIndex = 1;
            //LoadTilesForSelector --> SelectionChanged

        }

        private void TileSelect_OnClick(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(TileCanvas);
            int X = (int)Math.Floor(p.X / 32);
            int Y = (int)Math.Floor(p.Y / 32);
            
            index = GetIndexFromPosition(X, Y);

            try
            {
                Image image = new Image()
                {
                    Height = 32,
                    Width = 32,

                    Source = selectableTiles[index].Image.Source as BitmapImage
                };

                Foreground.ForegroundName blockName;
                Background.BackgroundName backgroundName;
                Special.SpecialName specialName;

                switch (ComboTypes.SelectedIndex)
                {
                    case 0:
                        backgroundName = TileTypes.Background.GetBackgroundNameByString(selectableTiles[index].TileName);

                        //So we can use it for the prev. tiles later
                        _selectedTile = new Background(image);
                        _selectedTile.TileName = selectableTiles[index].TileName;

                        TileHover.Content = selectableTiles[index].TileName;
                        break;
                    case 1:
                        blockName = TileTypes.Foreground.GetForegroundNameByString(selectableTiles[index].TileName);

                        _selectedTile = new Foreground(image);
                        _selectedTile.TileName = selectableTiles[index].TileName;

                        TileHover.Content = selectableTiles[index].TileName;
                        break;
                    case 2:
                        specialName = TileTypes.Special.GetSpecialNameByString(selectableTiles[index].TileName);

                        _selectedTile = new Special(image);
                        _selectedTile.TileName = selectableTiles[index].TileName;

                        TileHover.Content = selectableTiles[index].TileName;
                        break;
                }

                TileCanvas.Children.Remove(selectBorder);

                //Create the blue border
                selectBorder = new Border()
                {
                    BorderBrush = Brushes.SkyBlue,
                    BorderThickness = new Thickness(2),
                    Width = 32,
                    Height = 32
                };
                Canvas.SetTop(selectBorder, Y * 32);
                Canvas.SetLeft(selectBorder, X * 32);
                TileCanvas.Children.Add(selectBorder);

                FirstSelected = true;
                LabelImg.Source = image.Source;

            } catch (IndexOutOfRangeException)  { /* ;-; */ }
        }

        private void ComboTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FirstSelected = false;
            TileCanvas.Children.Clear();
            selectableTiles.Clear();
            if (ComboTypes.SelectedIndex == 0)
            {
                MakeSpriteSheetForType(new Background());
            }
            else if (ComboTypes.SelectedIndex == 1)
            {
                MakeSpriteSheetForType(new Foreground());
            }
            else if (ComboTypes.SelectedIndex == 2)
            {
                MakeSpriteSheetForType(new Special());
            }
        }

        public void SelectTile(string tileNameToSelect)
        {
            TileCanvas.Children.Remove(selectBorder);

            int searchIndex;

            foreach (var i in tileMap)
            {
                if (i.Key.Equals(tileNameToSelect))
                {
                    if (i.Value is Background)
                    {
                        ComboTypes.SelectedIndex = 0;
                    }
                    else if (i.Value is Foreground)
                    {
                        ComboTypes.SelectedIndex = 1;
                    }
                    else
                    {
                        ComboTypes.SelectedIndex = 2;
                    }
                }
            }

            searchIndex = selectableTiles.FindIndex(prop => prop.TileName == tileNameToSelect);
            BitmapImage source = selectableTiles[searchIndex].Image.Source as BitmapImage;
            selectBorder = new Border()
            {
                BorderBrush = Brushes.SkyBlue,
                BorderThickness = new Thickness(2),
                Width = 32,
                Height = 32
            };

            Canvas.SetTop(selectBorder, GetYFromIndex(searchIndex) * 32);
            Canvas.SetLeft(selectBorder, GetXFromIndex(searchIndex) * 32);

            _selectedTile = new Background(new Image() { Source = source });
            _selectedTile.TileName = tileNameToSelect;

            TileCanvas.Children.Add(selectBorder);
            index = searchIndex;

        }

        public static int GetIndexFromPosition(int X, int Y)
        {
            return Y * columns + X;
        }

        public static int GetXFromIndex(int index)
        {
            return index % columns;
        }

        public static int GetYFromIndex(int index)
        {
            return (index - GetXFromIndex(index)) / columns;
        }
    }
}

