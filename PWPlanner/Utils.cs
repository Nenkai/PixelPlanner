using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PWPlanner
{
    public class Utils
    {

        public static int ARGBColortoInt(System.Windows.Media.Color color)
        {
            return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }

        public static System.Windows.Media.Color IntToARGBColor(int value)
        {
            return System.Windows.Media.Color.FromArgb((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)(value));
        }

    }
}
