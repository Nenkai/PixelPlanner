using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWPlanner
{
    public class PlannerSettings
    {
        public static byte[] SaveMagic = new byte[] { 0x50, 0x57, 0x57, 0x4F, 0x52, 0x4C, 0x44 };
        public const short CurrentVersion = 1;

        public TileData[,] TileMap;
        public BackgroundData.MainBackgroundType MainBackground;
        public bool hasMainBackground;
        public bool hasCustomMainBackgroundColor = false;
        public int ARGBBackgroundColor;

        public const short DefaultWorldWidth = 80;
        public const short DefaultWorldHeight = 60;

        public SortedList<string, int> InvalidTiles = new SortedList<string, int>();

        //Incase. So we already have custom world dimensions supported
        public short WorldWidth;
        public short WorldHeight;

        public PlannerSettings(short Width, short Height)
        {
            TileMap = new TileData[Width, Height];
            MainBackground = BackgroundData.MainBackgroundType.Forest;
            hasMainBackground = true;
            this.WorldWidth = Width;
            this.WorldHeight = Height;
        }

        /// <summary>
        /// Clone settings from old ones
        /// </summary>
        /// <param name="old"></param>
        public PlannerSettings(PlannerSettings old)
        {
            this.hasMainBackground = old.hasMainBackground;
            this.MainBackground = old.MainBackground;
            this.hasCustomMainBackgroundColor = old.hasCustomMainBackgroundColor;
            this.ARGBBackgroundColor = old.ARGBBackgroundColor;
            this.InvalidTiles = old.InvalidTiles;

            this.WorldWidth = old.WorldWidth;
            this.WorldHeight = old.WorldHeight;
            this.TileMap = new TileData[WorldWidth, WorldHeight];
        }
    }
}
