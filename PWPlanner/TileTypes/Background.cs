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
            CaveWallpaper,
            WoodenBackground,
            WindowFrame,
            SandyCaveWall,
            RedWallpaper,
            StoneBackground,
            FlowerWallpaper,
            Shoji,
            DungeonWall,
            RedBrickWallpaper,
            BrownBrickWallpaper,
            YellowBrickWallpaper,
            ClownWallpaper,
            CastleWallpaper,
            WhiteTile,
            ImperialWallpaper,
            LightBlueWallpaper,
            ScifiBackground1,
            ScifiBackground2,
            WoodenWindowFrame,
            GreyBrickWallpaper,
            StarryWallpaper,
            TintedWindowFrame,
            TornWallpaper,
            LightWoodenBackground,
            RedTile,
            OrangeTile,
            YellowTile,
            PinkTile,
            BlueTile,
            GreenTile,
            GlassTile,
            CastleWallTile,
            DollarBackground,
            MoneyBackground,
            RedVelvetBackground,
            RedXmasWallpaper,
            BlueXmasWallpaper,
            GhostBackground,
            EctoplasmBackground,
            CandyLaceBackground,
            ChocolateBackground,
            CandyBackground,
            HeartWallpaper,
            CloverleafWallpaper,
            GreenGiftwrapWallpaper,
            RainbowWallpaper,
            CloverleafWindow,
            SpheresWallpaper,
            StripesWallpaper,
            TilesWallpaper,
            SamuraiBackground,
            BambooWall,
            CastleWindow,
            ArmoredBackground,
            BlackDiagonalChecker,
            BlueDiagonalChecker,
            DirtyHerringbone,
            GreyHerringbone,
            GreyDotIllusion,
            RedDotIllusion,
            JailBackground,
            LavaBackground,
            MetalBackground1,
            MetalBackground2,
            MetalBackground3,
            SquareMoire,
            SpiralMosaic,
            BlackTile,
            NonSlipMetal,
            RoundMore,
            GreenScreen,
            WoodenPolesBackground,
            VikingStoneBackground,
            RuneBackground,
            DungeonBars,
            ScifiBackground3,
            ScifiBackground4,
            MeltedChocolate,
            OrangeNeonBackground,
            GreenNeonBackground,
            BlueNeonBackground,
            VioletNeonBackground,
            BlueBigTile,
            GreyBigTile,
            BrownBigTile,
            RedBigTile,
            GreenBigTile,
            DirtyWall1,
            DirtyWall2,
            GreenGlowWire,
            OrangeGlowWire,
            RuinBackground,
            BrokenRuinBackground,
            WoodenBeamBackground,
            BlueSpheresBackground,
            RedSpheresBackground,
            GreySpheresBackground,
            GreenSpheresBackground,
            WirefenceBackground,
            NONE
        }
    }
}
