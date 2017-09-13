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

#region Tile Names
    [Serializable()]
    #region Background Names
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
        DungeonWall,
        FlowerWallpaper,
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
        SamuraiBackground,
        SandyCaveWall,
        ScifiBackground1,
        ScifiBackground2,
        Shoji,
        SpheresWallpaper,
        SpiralMosaic,
        SquareMoire,
        StarryWallpaper,
        StoneBackground,
        StripesWallpaper,
        TilesWallpaper,
        TintedWindowFrame,
        TornWallpaper,
        WhiteTile,
        WindowFrame,
        WoodenBackground,
        WoodenWindowFrame,
        YellowBrickWallpaper,
        YellowTile,
        NONE
    }
    #endregion

    [Serializable()]
    #region Block Names
    public enum BlockName
    {
        AquaCandyBlock,
        ArmChair,
        ArrowSign1,
        ArrowSign2,
        ArrowSign3,
        ArrowSign4,
        Balloons,
        Bamboo,
        BarStool,
        Bat,
        Bathtub,
        BattleGateDisabled,
        BattleGateEnabled,
        BattleScoreboard,
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
        BlueGlowBlock,
        BlueJelly,
        BlueMetalChair,
        BlueSunUmbrella,
        BookPodium,
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
        CheeseBlock,
        CherryBonsai,
        CircuitBoard,
        ClassicPainting,
        ClassicSculpture,
        ClassicSculpture2,
        ClayPot,
        ClearJelly,
        CloudPlatform,
        CloverleafBlock,
        Coffin,
        ConcreteBlock1x1,
        ConcreteBlock2x1,
        ConcreteBlock2x2,
        Corn,
        DaHoodSign,
        DarkChocolateBlock,
        DarkWorldLock,
        DeathCounter,
        DecorativeSword,
        DesktopPC,
        DiscoBall,
        Door,
        DottedPinkBlock,
        DungeonDoor,
        EasterBasket,
        Evolverator,
        FamiliarFoodMachine,
        FinishLine,
        Fireplace,
        FireTrap,
        FireTrap2,
        FireTrap3,
        FireTrap4,
        Fishbowl,
        FishTank,
        Flame1,
        Flame2,
        Flame3,
        Flame4,
        FlatScreenTV,
        FloorLamp,
        Fridge,
        FruitTray,
        GardenGnome,
        Gargoyle,
        GiantEasterEgg,
        GingerbreadBlock,
        GlassBlock,
        GlassCabinet,
        GlassDoor,
        GlowingContainer,
        GlueBlock,
        GlueBlock2,
        GoldenConeStatue,
        GoldenHorseshoe,
        GoldenToilet,
        Gramophone,
        Granite,
        Grass,
        Gravestone,
        GreenBlock,
        GreenCandyBlock,
        GreenEasterBlock,
        GreenGlowBlock,
        GreenGummyBear,
        GreenJelly,
        GreenMetalChair,
        GreenMushroom,
        GreenPennant,
        GreenRibbon,
        GreyBlock,
        GreyBrick,
        KatanaDecoration,
        HamRadio,
        HazardBlock,
        HeartDecoration,
        HeavyMetalPoster,
        Hokora,
        HollyWreath,
        HousePlant,
        HugeMetalFan,
        HugeMetalFan2,
        IceBlock,
        Icicles,
        IrishString,
        IronBlock,
        IronFence,
        JungleGrass,
        KiddieRide,
        KiwiBlock,
        Lantern,
        LargeLock,
        LargeSandCastle,
        Lava,
        LavaLamp,
        LEDSign,
        LegendarySoilBlock,
        LeopardArmchair,
        LeprechaunGnome,
        Lifebuoy,
        LightBlueBlock,
        LightBlueJelly,
        Lily,
        LuckyCloverleaf,
        LuckyHorseshoe,
        MagicStuff,
        MagLift,
        MakeupTable,
        ManekiNekoL,
        Marble,
        MarbleFireplace,
        MediumLock,
        MediumSandCastle,
        MetalBarrel,
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
        OldWallLamp,
        OrangeBlock,
        OrangeEasterEgg,
        OrangeGlowBlock,
        Oven,
        PileOfMoney,
        Pineapple,
        Pineapple2,
        PinkBlock,
        PinkCandyBlock,
        PinkJelly,
        PinkMetalChair,
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
        PottedPlant,
        PrizeBox,
        PurpleBlock,
        PurpleEasterBlock,
        PurpleJelly,
        PurpleSunUmbrella,
        Quicksand,
        RatingBoard,
        RedBlock,
        RedBrick,
        RedCandyBlock,
        RedEasterBlock,
        RedGlowBlock,
        RedGummyBear,
        RedJelly,
        RedMetalChair,
        RedRibbon,
        RedSunUmbrella,
        ReindeerLights,
        Replicator,
        Rose,
        RoundMetalTable,
        RubberDuck,
        SamuraiBlock,
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
        SmallSandCastle,
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
        StuddedMetal,
        Sunflower,
        Sushi,
        TaikoDrum,
        Tapestry,
        TeaSet,
        TeslaSphere,
        Throne,
        TintedGlassDoor,
        ToiletSeat,
        Torch,
        Torii,
        ToyBunny,
        ToyChick,
        TVChair,
        Vine,
        VioletEasterEgg,
        VortexPortal,
        WallTorch,
        Wardrobe,
        Water,
        Water2,
        WaterBed,
        WatercolorBlock,
        WaterMelonBlock,
        WhiteBlocks,
        WhiteBrick,
        WinterBells,
        WireFence,
        WoodenBarrel,
        WoodBlock,
        WoodenChair,
        WoodenFence,
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
        YellowMetalChair,
        YinYangBlock,
        NONE
    }
    #endregion
#endregion
}
