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
    [Serializable()]
    public class Tile
    {
        public TileType Type;
        [NonSerialized()]
        public Image Foreground;
        [NonSerialized()]
        public Image Background;
        public int X;
        public int Y;
        public TilePosition Positions = null;

        public Tile(TileType Type, Image image, TilePosition Positions)
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
            this.Positions = Positions;
        }

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

        public override string ToString()
        {
            return $"Type: {Type.ToString()}, BG? {Background != null} FG? {Foreground != null}\n" ;
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

    [Serializable()]
    public class TilePosition
    {
        public int? ForegroundX;
        public int? ForegroundY;
        public int? ForegroundSpriteX;
        public int? ForegroundSpriteY;
        public int? BackgroundX;
        public int? BackgroundY;
        public int? BackgroundSpriteX;
        public int? BackgroundSpriteY;

        public TilePosition(TileType tt, int TileX, int TileY, int SpriteX, int SpriteY)
        {
            switch (tt)
            {
                case TileType.Background:
                    this.BackgroundX = TileX;
                    this.BackgroundY = TileY;
                    this.BackgroundSpriteX = SpriteX;
                    this.BackgroundSpriteY = SpriteY;
                    break;
                case TileType.Foreground:
                    this.ForegroundX = TileX;
                    this.ForegroundY = TileY;
                    this.ForegroundSpriteX = SpriteX;
                    this.ForegroundSpriteY = SpriteY;
                    break;
            }
        }

        public void SetForegroundPositions(int ForegroundX, int ForegroundY, int SpriteX, int SpriteY)
        {
            this.ForegroundX = ForegroundX;
            this.ForegroundY = ForegroundY;
            this.ForegroundSpriteX = SpriteX;
            this.ForegroundSpriteY = SpriteY;
        }

        public void SetBackgroundPositions(int BackgroundX, int BackgroundY, int SpriteX, int SpriteY)
        {
            this.BackgroundX = BackgroundX;
            this.BackgroundY = BackgroundY;
            this.BackgroundSpriteX = SpriteX;
            this.BackgroundSpriteY = SpriteY;
        }

        public void DeleteForegroundPositions()
        {
            this.ForegroundX = null;
            this.ForegroundY = null;
            this.ForegroundSpriteX = null;
            this.ForegroundSpriteY = null;
        }

        public void DeleteBackgroundPositions()
        {
            this.BackgroundX = null;
            this.BackgroundY = null;
            this.BackgroundSpriteX = null;
            this.BackgroundSpriteY = null;
        }

    }

    [Serializable()]
    public enum TileType
    {
        None,
        Background,
        Foreground,
        Both
    }
}
