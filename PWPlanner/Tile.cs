using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PWPlanner
{
    public abstract class Tile
    {
        abstract public int ZIndex { get; }
        public string TileName;
        public Image Image;

        public void Reset()
        {
            TileName = null;
            Image = null;
        }

        public abstract Tile Clone(Image image);
    }

    public enum TileType
    {
        Background = 1,
        Foreground = 5,
        Special = 10
    }

}
