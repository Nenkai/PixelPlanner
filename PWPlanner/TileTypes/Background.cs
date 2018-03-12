using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace PWPlanner.TileTypes
{
    public class Background : Tile
    {
        public override int ZIndex
        {
            get { return 1; }
        }

        public Background() { }

        public Background(Image image)
        {
            Image = image;
        }

        public static BackgroundName GetBackgroundNameByString(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            if (Enum.TryParse(fileName, out BackgroundName name))
            {
                return name;
            }
            else
            {
                return BackgroundName.NONE;
            }

        }

        public override Tile Clone(Image image)
        {
            var bg = new Background();
            bg.Image = image;
            bg.TileName = this.TileName;
            return bg;
        }

        public enum BackgroundName
        {
            ArmoredBackground,
            BambooWall,
            BlackDiagonalChecker,
            BlackTile,
            BlueDiagonalChecker,
            BlueTile,
            BlueXmasWallpaper,
            BrownBrickWallpaper,
            CandyBackground,
            CastleWallBackground,
            CastleWallpaper,
            CastleWindow,
            CaveWallpaper,
            ChocolateBackground,
            CloverleafWindow,
            CloverWallpaper,
            ClownWallpaper,
            DirtyHerringbone,
            DollarBackground,
            DungeonBars,
            DungeonWall,
            EctoplasmBackground,
            FlowerWallpaper,
            GhostBackground,
            GlassTile,
            GreenGiftwrap,
            GreenScreen,
            GreenTile,
            GreyBrickBackground,
            GreyHerringbone,
            IceBackground,
            JailBackground,
            LavaBackground,
            LightBlueWallpaper,
            LightWoodenBackground,
            MagicBackground,
            MeltedChocolate,
            MetalBackground1,
            MetalBackground2,
            MetalBackground3,
            MoneyBackground,
            NonSlipMetal,
            OrangeTile,
            PinkTile,
            RainbowWallpaper,
            RedBrickWallpaper,
            RedDiagonalChecker,
            RedDotIllusion,
            RedTile,
            RedVelvet,
            RedWallpaper,
            RedXmasWallpaper,
            RoundMoire,
            RuneBackground,
            SamuraiBackground,
            SandyCaveWall,
            ScifiBackground1,
            ScifiBackground2,
            ScifiBackground3,
            ScifiBackground4,
            Shoji,
            SlimyBackground,
            SpheresWallpaper,
            SpiralMosaic,
            SquareMoire,
            StarryWallpaper,
            StoneBackground,
            StripesWallpaper,
            TilesWallpaper,
            TintedWindowFrame,
            TornWallpaper,
            VikingStoneBackground,
            WhiteTile,
            WindowFrame,
            WoodenBackground,
            WoodenPolesBackground,
            WoodenWindowFrame,
            YellowBrickWallpaper,
            YellowTile,
            NONE
        }
    }
}
