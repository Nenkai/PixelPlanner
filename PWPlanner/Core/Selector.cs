using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace PWPlanner
{
    public partial class MainWindow : Window
    {
        public Tile _selectedTile = new Tile();
        public Image SpriteSheet;
        public bool FirstSelected = false;

        private void GenerateSelector()
        {
            ComboTypes.Items.Add("Backgrounds");
            ComboTypes.Items.Add("Blocks");
            ComboTypes.SelectedIndex = 0;

            SpriteSheet = TileTypeMethods.GetImageForType(TileType.Background);

            TileCanvas.MinWidth = SpriteSheet.Width;
            TileCanvas.MinHeight = SpriteSheet.Height;
            Canvas.SetTop(SpriteSheet, 0);
            Canvas.SetLeft(SpriteSheet, 0);
            TileCanvas.Children.Add(SpriteSheet);
        }

        private void TileSelect_OnClick(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(TileCanvas);
            int X = (int)Math.Floor(p.X / 32);
            int Y = (int)Math.Floor(p.Y / 32);

            System.Drawing.Bitmap bmp = Utils.GetCroppedBitmap(_selectedTile.Type, X * 32, Y * 32);
            Image image = Utils.BitmapToImageControl(bmp);
            
            TileType tt = TileType.None;
            switch (ComboTypes.SelectedIndex)
            {
                case 0:
                    tt = TileType.Background;
                    break;
                case 1:
                    tt = TileType.Foreground;
                    break;
            }
            FirstSelected = true;
            _selectedTile = new Tile(tt, image);
            _selectedTile.X = (int) p.X / 32;
            _selectedTile.Y = (int) p.Y / 32;
            bmp.Dispose();
        }

        private void ComboTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FirstSelected = false;
            TileCanvas.Children.Remove(SpriteSheet);

            switch (ComboTypes.SelectedIndex)
            {
                case 0:
                    SpriteSheet = TileTypeMethods.GetImageForType(TileType.Background);
                    _selectedTile.Type = TileType.Background;
                    break;
                case 1:
                    SpriteSheet = TileTypeMethods.GetImageForType(TileType.Foreground);
                    _selectedTile.Type = TileType.Foreground;
                    break;
            }

            TileCanvas.MinWidth = SpriteSheet.Width;
            TileCanvas.MinHeight = SpriteSheet.Height;
            Canvas.SetTop(SpriteSheet, 0);
            Canvas.SetLeft(SpriteSheet, 0);
            TileCanvas.Children.Add(SpriteSheet);
            
        }
    }
}

