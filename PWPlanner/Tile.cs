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
        //Determines which layers are in
        public TileType Type;

        //Save used tiles as sprite name
        public string bgName;
        public string blName;

        [NonSerialized]
        public Image Foreground;
        [NonSerialized]
        public Image Background;

        public int X;
        public int Y;
        public TilePosition Positions = null;

        public Tile(TileType Type, string bgOrBlName, Image image, TilePosition Positions)
        {

            this.Type = Type;
            switch (Type)
            {
                case TileType.Background:
                    this.bgName = bgOrBlName;
                    Background = image;
                    break;
                case TileType.Foreground:
                    this.blName = bgOrBlName;
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
            return $"Type: {Type.ToString()}, BG Name? {bgName} FG Name? {blName}\n" ;
        }

    }

    [Serializable()]
    public class TilePosition
    {
        public int? ForegroundX;
        public int? ForegroundY;
        public int? BackgroundX;
        public int? BackgroundY;


        public TilePosition(TileType tt, int TileX, int TileY)
        {
            switch (tt)
            {
                case TileType.Background:
                    this.BackgroundX = TileX;
                    this.BackgroundY = TileY;
                    break;
                case TileType.Foreground:
                    this.ForegroundX = TileX;
                    this.ForegroundY = TileY;
                    break;
            }
        }

        public void SetForegroundPositions(int ForegroundX, int ForegroundY)
        {
            this.ForegroundX = ForegroundX;
            this.ForegroundY = ForegroundY;
        }

        public void SetBackgroundPositions(int BackgroundX, int BackgroundY)
        {
            this.BackgroundX = BackgroundX;
            this.BackgroundY = BackgroundY;
        }

        public void DeleteForegroundPositions()
        {
            this.ForegroundX = null;
            this.ForegroundY = null;
        }

        public void DeleteBackgroundPositions()
        {
            this.BackgroundX = null;
            this.BackgroundY = null;
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

    [Serializable()]
    public enum BackgroundName
    {
        BlueTile,
        BlueXmasWallpaper,
        BrownBrickWallpaper,
        CandyBackground,
        CastleWallBackground,
        CastleWallpaper,
        CaveWallpaper,
        ChocolateBackground,
        CloverleafWindow,
        CloverWallpaper,
        ClownWallpaper,
        DollarBackground,
        DungeonWall,
        FlowerWallpaper,
        GlassTile,
        GreenGiftwrap,
        GreenTile,
        GreyBrickBackground,
        IceBackground,
        LightWoodenBackground,
        MagicBackground,
        MoneyBackground,
        OrangeTile,
        PinkTile,
        RainbowWallpaper,
        RedBrickWallpaper,
        RedTile,
        RedVelvet,
        RedWallpaper,
        RedXmasWallpaper,
        SandyCaveWall,
        ScifiBackground1,
        ScifiBackground2,
        Shoji,
        SpheresWallpaper,
        StarryWallpaper,
        StoneBackground,
        StripesWallpaper,
        TilesWallpaper,
        TintedWindowFrame,
        TornWallpaper,
        WhiteTile,
        WindowFrame,
        WoodenBackground,
        YellowBrickWallpaper,
        YellowTile,
        NONE
    }
    
    [Serializable()]
    public enum BlockName
    {
        AquaCandyBlock,
        ArmChair,
        Balloons,
        Bat,
        Bathtub,
        Bed,
        Bedrock,
        BedrockFlat,
        BedrockLava,
        BlackBlock,
        BlackBrick,
        BlackPennant,
        BlingBlingBlock,
        BlueBlock,
        BlueCandyBlock,
        BlueEasterBlock,
        BlueEasterEgg,
        BlueJelly,
        Bookshelf,
        Brazier,
        BrownBlock,
        Bumper,
        Bushes,
        Buzzsaw,
        Cabinet,
        Cactus,
        Cake,
        CandyCaneBlock,
        CandyPipe,
        CandySiphon,
        CandyWatermelonBlock,
        CastleBrick,
        CastleDoor,
        Chandelier,
        Checkpoint,
        CircuitBoard,
        ClassicPainting,
        ClassicSculpture,
        ClassicSculpture2,
        ClayPot,
        ClearJelly,
        CloudPlatform,
        CloverleafBlock,
        Corn,
        DaHoodSign,
        DarkChocolateBlock,
        DarkWorldLock,
        DeathCounter,
        Door,
        DottedPinkBlock,
        DungeonDoor,
        EasterBasket,
        FinishLine,
        Fireplace,
        FireTrap,
        FireTrap2,
        FireTrap3,
        FireTrap4,
        Fishbowl,
        Flame1,
        Flame2,
        Flame3,
        Flame4,
        FloorLamp,
        FruitTray,
        GardenGnome,
        Gargoyle,
        GiantEasterEgg,
        GingerbreadBlock,
        GlassBlock,
        GlassDoor,
        GlowingContainer,
        GlueBlock,
        GlueBlock2,
        GoldenHorseshoe,
        GoldenToilet,
        Gramophone,
        Granite,
        Grass,
        Gravestone,
        GreenBlock,
        GreenCandyBlock,
        GreenEasterBlock,
        GreenGummyBear,
        GreenJelly,
        GreenMushroom,
        GreenPennant,
        GreenRibbon,
        GreyBlock,
        GreyBrick,
        HollyWreath,
        HugeMetalFan,
        HugeMetalFan2,
        IceBlock,
        Icicles,
        IrishString,
        IronBlock,
        JungleGrass,
        KiwiBlock,
        Lantern,
        LargeLock,
        Lava,
        LavaLamp,
        LEDSign,
        LeopardArmchair,
        LeprechaunGnome,
        LightBlueBlock,
        LightBlueJelly,
        Lily,
        LuckyCloverleaf,
        LuckyHorseshoe,
        MagicStuff,
        MagLift,
        MakeupTable,
        Marble,
        MediumLock,
        MetalPlate,
        MetalPlatform,
        MetalTrapDoor,
        Microwave,
        MilkChocolateBlock,
        MinatureSpaceship,
        ModernPainting,
        Mushroom,
        NoteBoard,
        Obisidian,
        OldTV,
        OrangeBlock,
        OrangeEasterEgg,
        Oven,
        PileOfMoney,
        Pineapple,
        Pineapple2,
        PinkBlock,
        PinkCandyBlock,
        PinkJelly,
        PlatinumLock,
        PoisonGas1,
        PoisonGas2,
        PoisonGas3,
        PoisonGas4,
        PoisonTrap,
        PoisonTrap1,
        PoisonTrap2,
        PoisonTrap3,
        Portal,
        PortalDoor,
        PotofGold,
        PrizeBox,
        PurpleBlock,
        PurpleEasterBlock,
        PurpleJelly,
        Quicksand,
        RatingBoard,
        RedBlock,
        RedBrick,
        RedCandyBlock,
        RedEasterBlock,
        RedGummyBear,
        RedJelly,
        RedRibbon,
        ReindeerLights,
        Replicator,
        Rose,
        RoundMetalTable,
        RubberDuck,
        Sand,
        Sand2,
        Sandstone,
        ScifiComputer,
        ScifiCrate,
        ScifiCratePile,
        ScifiDoor,
        ScifiGenerator,
        ScifiGenerator2,
        ScifiLights,
        ScifiPanel1,
        ScifiPanel2,
        ScifiPanel3,
        ScifiTable,
        Scoreboard,
        Serpentine,
        SerpentineAndEggs,
        Sign,
        SmallChest,
        SmallLock,
        SnowBlock,
        Snowman,
        Soil,
        Soil2,
        Spike1,
        Spike2,
        Spike3,
        Spike4,
        SpikeBall,
        SpikeTrap,
        SpikeTrap2,
        SpikeTrap3,
        SpikeTrap4,
        SpringBoard,
        SpringBoard2,
        SpringBoard3,
        SpringBoard4,
        StartLine,
        Stereo,
        Strawberry,
        Strawberry2,
        Sunflower,
        Sushi,
        Tapestry,
        Throne,
        TintedGlassDoor,
        Torch,
        ToyBunny,
        ToyChick,
        VioletEasterEgg,
        VortexPortal,
        WallTorch,
        Wardrobe,
        Water,
        Water2,
        WatercolorBlock,
        WhiteBlocks,
        WhiteBrick,
        WinterBells,
        WoodBlock,
        WoodenChair,
        WoodenHatch,
        WoodenPlatform,
        WoodenTable,
        WoodWall,
        WorldLock,
        XmasLights,
        XmasTree,
        YardLamp,
        YellowBlock,
        YellowBrick,
        YellowCandyBlock,
        YellowEasterBlock,
        YellowGummyBear,
        YellowJelly,
        NONE
    }
}
