using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWPlanner
{
    public class Utils
    {
        public static Bitmap GetCroppedBitmap(TileType type, int X, int Y)
        {

            Rectangle cropRect = new Rectangle(X, Y, 32, 32);
            Bitmap src = TileTypeMethods.GetResourceForType(type);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }
            return target;
        }
        

    }
}
