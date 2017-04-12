using System;
using System.Windows;

namespace PWPlanner
{
    public class CanvasPos
    {
        public int X;
        public int Y;

        public CanvasPos(Point p)
        {
            X = (int) Math.Floor(p.X / 32);
            Y = (int) Math.Floor(p.Y / 32);
        }

    }
}
