using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace PWPlanner.TileTypes
{
    public class Special : Tile
    {
        public override int ZIndex
        {
            get { return 10; }
        }

        public Special() { }

        public Special(Image image)
        {
            Image = image;
        }

        public static SpecialName GetSpecialNameByString(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);

            if (Enum.TryParse(fileName, out SpecialName name))
            {
                return name;
            }
            else
            {
                return SpecialName.NONE;
            }

        }

        public override Tile Clone(Image image)
        {
            var fg = new Special();
            fg.Image = image;
            fg.TileName = this.TileName;
            return fg;
        }

        public enum SpecialName
        {
            Acid,
            FakeBlood,
            Fog,
            Quicksand,
            Water,
            Water2,
            NONE
        }
    }
}
