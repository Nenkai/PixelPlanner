using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PWPlanner
{
    public partial class MainWindow
    {
        //Logic is similar to search.
        private void PreviousTiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isRendered && PreviousTiles.SelectedItem != null)
            {
                object selectedItem = PreviousTiles.SelectedItem;
                ComboBoxItem selectedPrevTile = PreviousTiles.ItemContainerGenerator.ContainerFromItem(selectedItem) as ComboBoxItem;
                PreviousTile entry = selectedPrevTile.Content as PreviousTile;

                TileCanvas.Children.Remove(selectBorder);

                SelectTile(entry.type, entry.Name);

                //Index is changed in SelectTile.
                BitmapImage source = selectableTiles[index].source;

                TileHover.Content = entry.Name;
                LabelImg.Source = source;

                FirstSelected = true;
            }
        }

        private void AddToPreviousTilesSelected(Tile tile, TileType type)
        {
            foreach (var previous in PreviousTiles.Items)
            {
                var prev = previous as PreviousTile;

                //Check if the tile placed is different, so we don't add duplicates
                if (prev.Name == _selectedTile.bgName || prev.Name == _selectedTile.blName)
                {
                    return;
                }
            }

            PreviousTile se = new PreviousTile();
            if (type == TileType.Background)
            {
                se.Name = tile.bgName;
                se.Source = tile.Background.Source as BitmapImage;
            }
            else
            {
                se.Name = tile.blName;
                se.Source = tile.Foreground.Source as BitmapImage;
            }
            se.type = type;
            PreviousTiles.Items.Add(se);
        }

        public class PreviousTile
        {
            public string Name { get; set; }
            public BitmapSource Source { get; set; }
            public TileType type;
        }
    }
}
