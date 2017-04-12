using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWPlanner
{
    [Serializable()]
    public class TileData
    {
        public Tile[,] Tiles;
        public int Width;
        public int Height;

        public TileData(int Width, int Height)
        {
            Tiles = new Tile[Width, Height];
            this.Width = Width;
            this.Height = Height;
        }
    }
}
