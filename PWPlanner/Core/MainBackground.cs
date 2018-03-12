using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace PWPlanner
{
    public class BackgroundData
    {
        public static string Resources = @"pack://application:,,,/Resources";

        /// <summary>
        /// Background Image Loader.
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public static ImageBrush GetBackground(MainBackgroundType bt)
        {
            ImageBrush imagebrush = new ImageBrush();

            switch (bt)
            {
                case MainBackgroundType.Forest:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Forest.png"));
                    break;
                case MainBackgroundType.Night:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Night.png"));
                    break;
                case MainBackgroundType.Star:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Star.png"));
                    break;
                case MainBackgroundType.Candy:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Candy.png"));
                    break;
                case MainBackgroundType.Winter:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Winter.png"));
                    break;
                case MainBackgroundType.Alien:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Alien.png"));
                    break;
                case MainBackgroundType.Desert:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Desert.png"));
                    break;
                case MainBackgroundType.Cemetery:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Cemetery.png"));
                    break;

                default:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Forest.png"));
                    break;
            }
            return imagebrush;
        }

        [Serializable()]
        public enum MainBackgroundType
        {
            None,
            Forest,
            Night,
            Star,
            Candy,
            Winter,
            Alien,
            Desert,
            Cemetery,
        }
    }
}
