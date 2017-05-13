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

namespace PWPlanner
{
    public partial class MainWindow : Window
    {
        public const int columns = 8;
        public SelectableTile[] selectableTiles;

        private Dictionary<BackgroundName, BitmapImage> backgroundMap = new Dictionary<BackgroundName, BitmapImage>();
        private Dictionary<BlockName, BitmapImage> blockMap = new Dictionary<BlockName, BitmapImage>();

        public void LoadTilesForSelector(TileType tt)
        {
            if (tt == TileType.Background)
            {
                selectableTiles = new SelectableTile[backgroundMap.Count];
                MakeBackgroundSpriteSheet();
            }
            else
            {
                selectableTiles = new SelectableTile[blockMap.Count];
                MakeBlockSpriteSheet();
            }
        }

        public void MakeBackgroundSpriteSheet()
        {
            int x = 0;
            int y = 0;
            int count = 0;

            foreach (KeyValuePair<BackgroundName, BitmapImage> entry in backgroundMap)
            {
                selectableTiles[count] = new SelectableTile(TileType.Background, entry.Key.ToString(), entry.Value);
                Image image = new Image
                {
                    Width = 32,
                    Height = 32
                };
                image.Source = selectableTiles[count].source;

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

                Canvas.SetTop(image, y);
                Canvas.SetLeft(image, x);
                TileCanvas.Children.Add(image);
                TileCanvas.MinWidth = columns * 32;
                TileCanvas.MinHeight = count / columns * 32;

                count++;
            }
        }

        public void MakeBlockSpriteSheet()
        {
            int x = 0;
            int y = 0;
            int count = 0;

            foreach (KeyValuePair<BlockName, BitmapImage> entry in blockMap)
            {
                selectableTiles[count] = new SelectableTile(TileType.Foreground, entry.Key.ToString(), entry.Value);
                Image image = new Image
                {
                    Width = 32,
                    Height = 32
                };
                image.Source = selectableTiles[count].source;

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

                Canvas.SetTop(image, y);
                Canvas.SetLeft(image, x);
                TileCanvas.Children.Add(image);
                TileCanvas.MinWidth = columns * 32;
                TileCanvas.MinHeight = count / columns * 32;

                count++;
            }
        }

        public void LoadMap(TileType tt)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            string folderName;
            if (tt == TileType.Background && backgroundMap.Count <= 0)
            {
                folderName = string.Format("{0}.Resources.Tiles.Backgrounds", executingAssembly.GetName().Name);
            }
            else if (tt == TileType.Foreground && blockMap.Count <= 0)
            {
                folderName = string.Format("{0}.Resources.Tiles.Blocks", executingAssembly.GetName().Name);
            }
            else
            {
                return;
            }

            string[] files = executingAssembly.GetManifestResourceNames().Where(r => r.StartsWith(folderName) && r.EndsWith(".png")).ToArray();

            for (int count = 0; count < files.Length; count++)
            {
                using (Stream file = executingAssembly.GetManifestResourceStream(files[count].ToString()))
                {
                    string fileName = files[count].ToString().Substring(folderName.Length + 1);
                    var src = new BitmapImage();
                    src.BeginInit();
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.StreamSource = file;
                    src.EndInit();

                    if (tt == TileType.Background)
                    {
                        backgroundMap.Add(SelectableTile.GetBackgroundNameByString(fileName), src);
                    }
                    else if (tt == TileType.Foreground)
                    {
                        blockMap.Add(SelectableTile.GetBlockNameByString(fileName), src);
                    }

                }
            }
        }
    }

    public class SelectableTile
    {
        public BitmapImage source;
        public string bgName;
        public string blName;
        public TileType type;

        public SelectableTile(TileType tt, string bgOrBlName, BitmapImage source)
        {
            if (tt == TileType.Background)
            {
                this.type = TileType.Background;
                this.bgName = bgOrBlName;
            }
            else
            {
                this.type = TileType.Foreground;
                this.blName = bgOrBlName;
            }
            this.source = source;
        }


        public static BackgroundName GetBackgroundNameByString(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            if (Enum.TryParse(fileName, out BackgroundName name))
            {
                return name;
            }
            else
            {
                return BackgroundName.NONE;
            }
            
        }

        public static BlockName GetBlockNameByString(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);

            if (Enum.TryParse(fileName, out BlockName name))
            {
                return name;
            }
            else
            {
                return BlockName.NONE;
            }

        }
    }
}
