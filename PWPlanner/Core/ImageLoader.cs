using System;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PWPlanner.TileTypes;

namespace PWPlanner
{
    public partial class MainWindow : Window
    {
        public const int columns = 8;
        public List<Tile> selectableTiles = new List<Tile>();

        public static Dictionary<string, Tile> tileMap = new Dictionary<string, Tile>();

        /// <summary>
        /// Sets up the tile selector for a tile type.
        /// </summary>
        /// <param name="t"></param>
        public void MakeSpriteSheetForType(Tile t)
        {
            int x = 0;
            int y = 0;
            int count = 0;

            foreach (KeyValuePair<string, Tile> entry in tileMap)
            {

                if (Object.ReferenceEquals(t.GetType(), entry.Value.GetType()))
                {
                    selectableTiles.Add(entry.Value);


                    if (count == 0) { }
                    else if (count % columns == 0)
                    {
                        y += 32;
                        x = 0;
                    }
                    else
                    {
                        x += 32;
                    }

                    Canvas.SetTop(entry.Value.Image, y);
                    Canvas.SetLeft(entry.Value.Image, x);
                    TileCanvas.Children.Add(entry.Value.Image);
                    TileCanvas.MinWidth = columns * 32;
                    TileCanvas.MinHeight = count / columns * 32;
                    count++;
                }
            }
        }

        /// <summary>
        /// Loads all the tiles images from the resources to a map.
        /// </summary>
        /// <param name="tile"></param>
        public void LoadResourcesIntoTileMap()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            string folderName = string.Format("{0}.Resources.Tiles", executingAssembly.GetName().Name);

            string[] files = executingAssembly.GetManifestResourceNames().Where(r => r.StartsWith(folderName) && r.EndsWith(".png")).ToArray();

            foreach (string name in files)
            {
                using (Stream file = executingAssembly.GetManifestResourceStream(name))
                {
                    string str = name.Substring(folderName.Length + 1);
                    string fileName = str.Split('.')[1];
                    var src = new BitmapImage();
                    src.BeginInit();
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.StreamSource = file;
                    src.EndInit();

                    Image image = new Image()
                    {
                        Source = src
                    };

                    if (name.Contains("Background"))
                    {
                        tileMap.Add(fileName, new Background(image) { TileName = fileName });
                    }
                    else if (name.Contains("Blocks"))
                    {
                        tileMap.Add(fileName, new Foreground(image) { TileName = fileName });
                    }
                    else
                    {
                        tileMap.Add(fileName, new Special(image) { TileName = fileName });
                    }
                }
            }
        }
    }
}
