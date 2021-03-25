using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlantPickerAddin
{
    /// <summary>
    /// A combo box for selecting a state rank and layer with all species in that rank
    /// to the current map.
    /// </summary>
    internal class SpeciesRank : ComboBox
    {
        /// <summary>
        /// Combo Box constructor
        /// </summary>
        public SpeciesRank()
        {
            _layerBuilder = new SpeciesLayerFactory("Plant Species by State Rank.lyrx")
            {
                LayerNameFormat = "State Rank of {0}",
                LayerFixer = LayerUtilities.RemoveUnusedItemsFromUniqueValueRenderer,
            };
            Task.Run(InitAsync);
        }

        private readonly SpeciesLayerFactory _layerBuilder;
        private readonly string _firstItem = "Select a Rank";
        private readonly string _nullMarker = "* Not Specified *";

        /// <summary>
        /// Asynchronously load the combo box data.
        /// </summary>
        private async void InitAsync()
        {
            Clear();
            Add(new ComboBoxItem(_firstItem));

            PickList picklist = null;
            try
            {
                picklist = await PlantPickerModule.Current.LoadRankPickList();
            }
            // Catch all, because an uncaught exception in an Add-In will crash ArcGIS Pro
            catch (Exception ex)
            {
                ShowError(ex);
            }
            if (picklist != null)
            {
                foreach (string item in picklist.Names)
                {
                    var item2 = (string.IsNullOrEmpty(item)) ? _nullMarker : item;
                    Add(new ComboBoxItem(item2));
                }
            }

            SelectedItem = ItemCollection.FirstOrDefault();
            Enabled = true;
        }

        /// <summary>
        /// Called by the framework when the selection in the combo box is changed.
        /// (Also called when the `SelectedItem` is changed programatically.
        /// When called it will load the layer file and add it to the current map.
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected async override void OnSelectionChange(ComboBoxItem item)
        {

            if (item == null)
                return;

            if (string.IsNullOrEmpty(item.Text))
                return;

            if (string.Equals(item.Text, _firstItem))
                return;

            var text = string.Equals(item.Text, _nullMarker) ? null : item.Text;
            try
            {
                await _layerBuilder.BuildLayerAsync(text);
            }
            // Catch all, because an uncaught exception in an Add-In will crash ArcGIS Pro
            catch (Exception ex)
            {
                ShowError(ex);
            }

        }

        /// <summary>
        /// Display any (and all) excpetions to the user.
        /// </summary>
        /// <param name="ex">The exception to display to the user.</param>
        private void ShowError(Exception ex)
        {
            var title = (ex is ConfigurationException) ? "Configuration Error" : "Unexpected Error";
            var msg = GetType() + " encountered a problem.\n\n" + ex.Message;
            MessageBox.Show(msg, title,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }

    }
}
