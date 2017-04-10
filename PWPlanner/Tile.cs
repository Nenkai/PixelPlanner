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
    public class Tile
    {
        public TileType Type;
        public Image Foreground;
        public Image Background;

        public Tile(TileType Type, Image image)
        {

            this.Type = Type;
            switch (Type)
            {
                case TileType.Background:
                    Background = image;
                    break;
                case TileType.Foreground:
                    Foreground = image;
                    break;
            }
        }

        public Tile()
        {
            this.Type = TileType.None;
        }

    }

    public static class TileTypeMethods
    {
        public static System.Drawing.Bitmap GetResourceForType(TileType tt)
        {
            if (tt == TileType.Background)
            {
                return Properties.Resources.Backgrounds as System.Drawing.Bitmap;
            }
            else if (tt == TileType.Foreground)
            {
                return Properties.Resources.Blocks as System.Drawing.Bitmap;
            }

            return null;
        }

        public static Image GetImageForType(TileType tt)
        {
            System.Drawing.Bitmap image = GetResourceForType(tt);
            Image SpriteSheet = new Image();
            switch (tt)
            {
                case TileType.Background:
                    SpriteSheet = new Image
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Source = new BitmapImage(new Uri(@"/Resources/Backgrounds.png", UriKind.Relative)),
                    };
                    break;
                case TileType.Foreground:
                    SpriteSheet = new Image
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Source = new BitmapImage(new Uri(@"/Resources/Blocks.png", UriKind.Relative)),
                    };
                    break;
            }
            return SpriteSheet;
        }
    }

    public enum TileType
    {
        None,
        Background,
        Foreground,
        Both
    }
}
