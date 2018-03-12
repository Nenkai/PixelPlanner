using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Threading;

using PWPlanner.TileTypes;

namespace PWPlanner
{
    public partial class MainWindow
    {
        private static string SavedFileName;
        private static string SavedPath = "";

        //Save Shortcut
        private void SaveBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(SavedPath))
            {
                SaveWorld_Click(sender, e);
            }
            else
            {
                DirectWorldSave_Click(sender, e);
            }
        }

        //New Shortcut
        private void NewBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewWorld_Click(sender, e);
        }

        //World Save
        private void SaveWorld_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Pixel Worlds World (*.pww)|*.pww",
                DefaultExt = "pww",
                FileName = "world.pww",
            };
            Nullable<bool> Selected = dialog.ShowDialog();
            string path = dialog.FileName;
            if (Selected == true)
            {
                DataHandler.SaveWorld(path);
                SavedFileName = dialog.SafeFileName;
                SavedPath = Path.GetDirectoryName(path);
                MessageBox.Show("World saved successfully at\n" + path, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                SaveButton.IsEnabled = true;
            }
        }

        private void DirectWorldSave_Click(object sender, RoutedEventArgs e)
        {
            //Prompt user to actually select a file
            if (String.IsNullOrEmpty(SavedFileName) || String.IsNullOrEmpty(SavedPath))
            {
                SaveWorld_Click(sender, e);
            }
            else
            {
                string path = SavedPath + @"\" + SavedFileName;
                DataHandler.SaveWorld(path);
                MessageBox.Show("World saved successfully at\n" + path, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }

        //World Load
        private void LoadWorld_Click(object sender, RoutedEventArgs e)
        {
            if (firstPlaced && MessageBox.Show("Are you sure to load a new world? You may lose all your unsaved progress!", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            isLoading = true;

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Pixel Worlds World (*.pww)|*.pww",
                DefaultExt = "pww",
                RestoreDirectory = true
            };
            Nullable<bool> Selected = dialog.ShowDialog();
            string path = dialog.FileName;

            if (Selected == true)
            {

                PlannerSettings newWorldDB;
                DataHandler.LoadStatus status = DataHandler.TryLoadWorld(path, out newWorldDB);

                if (status == DataHandler.LoadStatus.VersionMismatch)
                {
                    MessageBox.Show("This world is not compatible with this planner version.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (status == DataHandler.LoadStatus.InvalidMagic)
                {
                    MessageBox.Show("This is not a world file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else //Load-able
                {

                    MainCanvas.Children.Clear();
                    DB = new PlannerSettings(newWorldDB);
                    DrawGrid(DB.WorldWidth, DB.WorldHeight);

                    if (status == DataHandler.LoadStatus.TileNotFound)
                    {
                        if (newWorldDB.InvalidTiles.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"Could not load {newWorldDB.InvalidTiles.Count} tiles (Using an older version?)");
                            foreach (KeyValuePair<string, int> entry in newWorldDB.InvalidTiles)
                            {
                                sb.AppendLine($"-{entry.Key} [x{entry.Value}]");
                            }
                            MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    for (int y = 0; y < DB.WorldHeight; y++)
                    {
                        for (int x = 0; x < DB.WorldWidth; x++)
                        {
                            if (!AnyTileAt(newWorldDB, x, y)) continue;

                            foreach (Tile t in newWorldDB.TileMap[x, y].Tiles.ToList())
                            {
                                Image image = new Image()
                                {
                                    Source = t.Image.Source
                                };
                                Canvas.SetTop(image, y * 32);
                                Canvas.SetLeft(image, x * 32);
                                image.SetValue(Canvas.ZIndexProperty, t.ZIndex);
                                MainCanvas.Children.Add(image);
                                if (DB.TileMap[x, y] == null) DB.TileMap[x, y] = new TileData();

                                
                                if (t is Foreground)
                                {
                                    DB.TileMap[x, y].Tiles.Add(new Foreground(image) { TileName = t.TileName });
                                }
                                else if (t is Background)
                                {
                                    DB.TileMap[x, y].Tiles.Add(new Background(image) { TileName = t.TileName });
                                }
                                else
                                {
                                    DB.TileMap[x, y].Tiles.Add(new Special(image) { TileName = t.TileName });
                                }
                                
                            }
                        }
                    }

                    if (!DB.hasMainBackground)
                    {
                        MainCanvas.Background = new SolidColorBrush(Utils.IntToARGBColor(DB.ARGBBackgroundColor));
                    }
                    else
                    {
                        MainCanvas.Background = BackgroundData.GetBackground(DB.MainBackground);
                    }

                    ColorSelector.SelectedColor = Utils.IntToARGBColor(DB.ARGBBackgroundColor);
                    firstPlaced = false;
                    FirstSelected = false;
                    SaveButton.IsEnabled = false;
                    SavedPath = String.Empty;
                    PreviousTiles.Items.Clear();
                    PreviousTiles.SelectedItem = null;
                    _selectedTile.Reset();

                }
            }
            isLoading = false;

        }

        public class DataHandler
        {
            public static void SaveWorld(string path)
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
                {
                    //Header (60 bytes)
                    bw.Write(PlannerSettings.SaveMagic); //PWWORLD magic
                    bw.Write(PlannerSettings.CurrentVersion); //2 bytes
                    bw.Write(DB.WorldWidth); // 2 bytes
                    bw.Write(DB.WorldHeight); //2 bytes
                    bw.Write(DB.hasMainBackground); // 1 byte
                    bw.Write((byte)DB.MainBackground); // 1 byte
                    bw.Write(DB.hasCustomMainBackgroundColor); // 1 byte
                    bw.Write(DB.ARGBBackgroundColor); // 4 bytes
                    bw.Write(new byte[40]); // Reserved

                    for (int y = 0; y < DB.WorldHeight; y++)
                    {
                        for (int x = 0; x < DB.WorldWidth; x++)
                        {
                            if (AnyTileAt(DB, x, y))
                            {
                                bw.Write((short)x); // 2 bytes X
                                bw.Write((short)y); // 2 bytes Y
                                bw.Write((byte)DB.TileMap[x, y].Tiles.Count); // 1 byte Tile Count 
                                foreach (var tile in DB.TileMap[x, y].Tiles)
                                {
                                    bw.Write((byte)tile.ZIndex); // 1 byte Defines tile type using zindex
                                    bw.Write(tile.TileName); // Write the tile name so we can load by name later
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Loads a world from path. Returns an error code.
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static LoadStatus TryLoadWorld(string path, out PlannerSettings db)
            {
                using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    //Header (60 bytes)
                    db = new PlannerSettings(PlannerSettings.DefaultWorldWidth, PlannerSettings.DefaultWorldHeight);
                    if (Encoding.ASCII.GetString(br.ReadBytes(7)) != Encoding.ASCII.GetString(PlannerSettings.SaveMagic)) return LoadStatus.InvalidMagic; //Invalid file magic
                    if (br.ReadInt16() != PlannerSettings.CurrentVersion) return LoadStatus.VersionMismatch; //Different version
                    db.WorldWidth = br.ReadInt16();
                    db.WorldHeight = br.ReadInt16();
                    db.hasMainBackground = br.ReadByte() == 1;
                    db.MainBackground = (BackgroundData.MainBackgroundType)br.ReadByte();
                    db.hasCustomMainBackgroundColor = br.ReadByte() == 1;
                    db.ARGBBackgroundColor = br.ReadInt32();
                    br.BaseStream.Seek(40, SeekOrigin.Current); //Skip reserved
                    
                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        int X = br.ReadInt16();
                        int Y = br.ReadInt16();
                        int tileCountAtPos = br.ReadByte();
                        for (int i = 0; i < tileCountAtPos; i++)
                        {
                            byte index = br.ReadByte();
                            string tileName = br.ReadString();
                            bool existsInDatabase = tileMap.TryGetValue(tileName, out Tile t);
                            if (existsInDatabase)
                            {
                                if (db.TileMap[X, Y] == null) db.TileMap[X, Y] = new TileData();
                                db.TileMap[X, Y].Tiles.Add(t);
                            }
                            else
                            {
                                if (db.InvalidTiles.ContainsKey(tileName))
                                {
                                    db.InvalidTiles[tileName]++;
                                }
                                else
                                {
                                    db.InvalidTiles.Add(tileName, 0);
                                }
                            }
                        }
                    }
                    if (db.InvalidTiles.Count > 0) return LoadStatus.TileNotFound; //At least one tile is not present in the current version

                    return LoadStatus.OK;
                }
            }

            public enum LoadStatus
            {
                OK,
                InvalidMagic,
                VersionMismatch,
                TileNotFound
            }
        }
    }
}
