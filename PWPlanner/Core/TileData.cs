using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PWPlanner
{
    [Serializable()]
    public class TileData
    {
        public Tile[,] Tiles;
        public BackgroundData.BackgroundType MainBackground;
        public bool hasMainBackground;
        public int ARGBBackgroundColor;
        public int Width;
        public int Height;

        public TileData(int Width, int Height)
        {
            Tiles = new Tile[Width, Height];
            MainBackground = BackgroundData.BackgroundType.Forest;
            hasMainBackground = true;
            this.Width = Width;
            this.Height = Height;
        }
    }
}
