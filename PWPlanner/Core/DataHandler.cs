using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Threading;

namespace PWPlanner
{
    public partial class MainWindow
    {
        private static string SavedFileName = "world.pww";
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
                FileName = SavedFileName,
            };
            Nullable<bool> Selected = dialog.ShowDialog();
            string path = dialog.FileName;
            if (Selected == true)
            {
                DataHandler.SaveWorld(TileDB, path);
                SavedFileName = dialog.SafeFileName;
                SavedPath = Path.GetDirectoryName(path);
                MessageBox.Show("World saved successfully at\n" + path, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                SaveButton.IsEnabled = true;
            }
        }

        private void DirectWorldSave_Click(object sender, RoutedEventArgs e)
        {
            string path = SavedPath + @"\" + SavedFileName;
            DataHandler.SaveWorld(TileDB, path);
            MessageBox.Show("World saved successfully at\n" + path, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

                MainCanvas.Children.Clear();
                TileDB = (TileData)DataHandler.LoadWorld(path);
                DrawGrid(TileDB.Height, TileDB.Width);

                if (!TileDB.hasMainBackground)
                {
                    MainCanvas.Background = new SolidColorBrush(Utils.IntToARGBColor(TileDB.ARGBBackgroundColor));
                }
                else
                {
                    MainCanvas.Background = BackgroundData.GetBackground(TileDB.MainBackground);
                }

                SortedList<string, int> invalids = new SortedList<string, int>();
                for (int i = 0; i < TileDB.Tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < TileDB.Tiles.GetLength(1); j++)
                    {
                        if (TileDB.Tiles[i, j] != null)
                        {
                            if (TileDB.Tiles[i, j].Type == TileType.Background || TileDB.Tiles[i, j].Type == TileType.Both)
                            {
                                Image image = new Image()
                                {
                                    Height = 32,
                                    Width = 32
                                };
                                string dataName = TileDB.Tiles[i, j].bgName;
                                BackgroundName name = SelectableTile.GetBackgroundNameByString(TileDB.Tiles[i, j].bgName);

                                //If the sprite does not exist.
                                bool exists = backgroundMap.TryGetValue(name, out BitmapImage src);
                                if (!exists && dataName != null)
                                {
                                    if (invalids.ContainsKey(dataName))
                                    {
                                        invalids[dataName]++;
                                    }
                                    else
                                    {
                                        invalids.Add(dataName, 0);
                                    }
                                    TileDB.Tiles[i, j] = null;
                                }
                                else
                                {
                                    image.Source = src;

                                    Canvas.SetTop(image, TileDB.Tiles[i, j].Positions.BackgroundY.Value * 32);
                                    Canvas.SetLeft(image, TileDB.Tiles[i, j].Positions.BackgroundX.Value * 32);
                                    image.SetValue(Canvas.ZIndexProperty, 10);
                                    MainCanvas.Children.Add(image);
                                    TileDB.Tiles[i, j].Background = image;
                                }
                            }

                            if (TileDB.Tiles[i, j].Type == TileType.Foreground || TileDB.Tiles[i, j].Type == TileType.Both)
                            {
                                Image image = new Image()
                                {
                                    Height = 32,
                                    Width = 32
                                };
                                string dataName = TileDB.Tiles[i, j].blName;
                                BlockName name = SelectableTile.GetBlockNameByString(TileDB.Tiles[i, j].blName);

                                bool exists = blockMap.TryGetValue(name, out BitmapImage src);
                                if (!exists && dataName != null)
                                {
                                    if (invalids.ContainsKey(dataName))
                                    {
                                        invalids[dataName]++;
                                    }
                                    else
                                    {
                                        invalids.Add(dataName, 0);
                                    }
                                    TileDB.Tiles[i, j] = null;
                                }
                                else
                                {
                                    image.Source = src;

                                    Canvas.SetTop(image, TileDB.Tiles[i, j].Positions.ForegroundY.Value * 32);
                                    Canvas.SetLeft(image, TileDB.Tiles[i, j].Positions.ForegroundX.Value * 32);
                                    image.SetValue(Canvas.ZIndexProperty, 20);
                                    MainCanvas.Children.Add(image);
                                    TileDB.Tiles[i, j].Foreground = image;
                                }
                            }
                        }
                    }
                    ColorSelector.SelectedColor = Utils.IntToARGBColor(TileDB.ARGBBackgroundColor);
                    firstPlaced = false;
                    SaveButton.IsEnabled = false;
                    SavedPath = String.Empty;
                }
                if (invalids.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Could not load {invalids.Count} tiles (Using an older version?)");
                    foreach (KeyValuePair<string, int> entry in invalids)
                    {
                        sb.AppendLine($"-{entry.Key} [x{entry.Value}]");
                    }
                    MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            isLoading = false;

        }
    }
    public static class DataHandler
    {
        public static void SaveWorld(object array, string path)
        {
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, array);
            }
        }

        public static object LoadWorld(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                return bformatter.Deserialize(stream);
            }
        }
    }
}
