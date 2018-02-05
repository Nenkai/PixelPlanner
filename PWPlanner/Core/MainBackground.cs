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
        public static ImageBrush GetBackground(BackgroundType bt)
        {
            ImageBrush imagebrush = new ImageBrush();

            switch (bt)
            {
                case BackgroundType.Forest:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Forest.png"));
                    break;
                case BackgroundType.Night:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Night.png"));
                    break;
                case BackgroundType.Star:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Star.png"));
                    break;
                case BackgroundType.Candy:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Candy.png"));
                    break;
                case BackgroundType.Winter:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Winter.png"));
                    break;
                case BackgroundType.Alien:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Alien.png"));
                    break;
                case BackgroundType.Desert:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Desert.png"));
                    break;
                case BackgroundType.Cemetery:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Cemetery.png"));
                    break;

                default:
                    imagebrush.ImageSource = new BitmapImage(new Uri(Resources + "/Backgrounds/Forest.png"));
                    break;
            }
            return imagebrush;
        }

        [Serializable()]
        public enum BackgroundType
        {
            Forest,
            Night,
            Star,
            Candy,
            Winter,
            Alien,
            Desert,
            Cemetery,
            None
        }
    }
}
