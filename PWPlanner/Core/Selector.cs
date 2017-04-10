using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace PWPlanner2
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

            System.Drawing.Image image = Properties.Resources.Backgrounds;

            SpriteSheet = new Image
            {
                Width = image.Width,
                Height = image.Height,
                Source = new BitmapImage(new Uri(@"/Resources/Backgrounds.png", UriKind.Relative)),
            };

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

            var bi = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
            }

            Image image = new Image();
            image.Source = bi;

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
        }

        private void ComboTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FirstSelected = false;
            TileCanvas.Children.Remove(SpriteSheet);

            if (ComboTypes.SelectedIndex == 0)
            {
                _selectedTile.Type = TileType.Background;
                System.Drawing.Image image = Properties.Resources.Backgrounds;

                Image SpriteSheet = new Image
                {
                    Width = image.Width,
                    Height = image.Height,
                    Source = new BitmapImage(new Uri(@"/Resources/Backgrounds.png", UriKind.Relative)),
                };

                TileCanvas.MinWidth = SpriteSheet.Width;
                TileCanvas.MinHeight = SpriteSheet.Height;
                Canvas.SetTop(SpriteSheet, 0);
                Canvas.SetLeft(SpriteSheet, 0);
                TileCanvas.Children.Add(SpriteSheet);
            }
            else if (ComboTypes.SelectedIndex == 1)
            {
                _selectedTile.Type = TileType.Foreground;
                System.Drawing.Image image = Properties.Resources.Blocks;

                SpriteSheet = new Image
                {
                    Width = image.Width,
                    Height = image.Height,
                    Source = new BitmapImage(new Uri(@"/Resources/Blocks.png", UriKind.Relative)),
                };

                TileCanvas.MinWidth = SpriteSheet.Width;
                TileCanvas.MinHeight = SpriteSheet.Height;
                Canvas.SetTop(SpriteSheet, 0);
                Canvas.SetLeft(SpriteSheet, 0);
                TileCanvas.Children.Add(SpriteSheet);
            }

        }
    }
}

