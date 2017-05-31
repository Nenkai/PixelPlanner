using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PWPlanner
{
    public partial class MainWindow : Window
    {

        public Tile _selectedTile = new Tile();
        public Border selectBorder = new Border();
        public bool FirstSelected = false;
        public int index;

        private void GenerateSelector()
        {
            ComboTypes.Items.Add("Backgrounds");
            ComboTypes.Items.Add("Blocks");
            ComboTypes.SelectedIndex = 0;
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

                    Source = selectableTiles[index].source
                };

                TileType tt = TileType.None;
                BlockName blockName;
                BackgroundName backgroundName;
                switch (ComboTypes.SelectedIndex)
                {
                    case 0:
                        backgroundName = SelectableTile.GetBackgroundNameByString(selectableTiles[index].bgName);
                        tt = TileType.Background;
                        TileHover.Content = selectableTiles[index].bgName;
                        break;
                    case 1:
                        blockName = SelectableTile.GetBlockNameByString(selectableTiles[index].blName);
                        tt = TileType.Foreground;
                        TileHover.Content = selectableTiles[index].blName;
                        break;
                }

                TileCanvas.Children.Remove(selectBorder);

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
                _selectedTile = new Tile(tt, image);
            } catch (IndexOutOfRangeException index) { }
        }

        private void ComboTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FirstSelected = false;
            TileCanvas.Children.Clear();

            LoadMap(TileType.Background);
            LoadMap(TileType.Foreground);

            if (ComboTypes.SelectedIndex == 0) {
                LoadTilesForSelector(TileType.Background);
            }
            else if (ComboTypes.SelectedIndex == 1) {
                LoadTilesForSelector(TileType.Foreground);
            }
        }

        public void SelectTile(TileType type, string bgOrblName)
        {
            TileCanvas.Children.Remove(selectBorder);

            int searchIndex;
            if (type == TileType.Background)
            {
                ComboTypes.SelectedIndex = 0;
                searchIndex = Array.FindIndex(selectableTiles, prop => prop.bgName == bgOrblName);
            }
            else
            {
                ComboTypes.SelectedIndex = 1;
                searchIndex = Array.FindIndex(selectableTiles, prop => prop.blName == bgOrblName);
            }

            BitmapImage source = selectableTiles[searchIndex].source;
            selectBorder = new Border()
            {
                BorderBrush = Brushes.SkyBlue,
                BorderThickness = new Thickness(2),
                Width = 32,
                Height = 32
            };

            Canvas.SetTop(selectBorder, GetYFromIndex(searchIndex) * 32);
            Canvas.SetLeft(selectBorder, GetXFromIndex(searchIndex) * 32);
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

