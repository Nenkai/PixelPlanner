using System;
using System.Diagnostics;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Input;

using PWPlanner.Core;

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

            index = Y * columns + X;

            if (p.X % 32 == 0 || p.Y % 32 == 0)
            {
                return;
            }

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
            _selectedTile = new Tile(tt, image)
            {
                X = (int)p.X / 32,
                Y = (int)p.Y / 32
            };
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
    }
}

