using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlantPickerAddin
{
    /// <summary>
    /// Represents the ComboBox
    /// </summary>
    internal class SpeciesOccurrence : ComboBox
    {

        private readonly SpeciesLayerFactory _layerBuilder;
        private readonly string _firstItem = "Select a Species";

        /// <summary>
        /// Combo Box constructor
        /// </summary>
        public SpeciesOccurrence()
        {
            _layerBuilder = new SpeciesLayerFactory("Plant Occurrence by Species.lyrx")
            {
                LayerNameFormat = "All occurrences of {0}",
                RandomizeMarkerColor = true,
            };
            Task.Run(InitAsync);
        }

        private async void InitAsync()
        {
            Clear();
            Add(new ComboBoxItem(_firstItem));

            PickList picklist = null;
            try
            {
                picklist = await PlantPickerModule.Current.LoadSpeciesPickList();
            }
            // Catch all, because an uncaught exception in an Add-In will crash ArcGIS Pro
            catch (Exception ex)
            {
                ShowError(ex);
            }
            if (picklist != null)
            {
                foreach (string rank in picklist.Names)
                {
                    Add(new ComboBoxItem(rank));
                }
            }

            SelectedItem = ItemCollection.FirstOrDefault();
            Enabled = true;
        }

        /// <summary>
        /// The on comboBox selection change event. 
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected async override void OnSelectionChange(ComboBoxItem item)
        {

            if (item == null)
                return;

            // TODO: Null/Empty text in combo box is unclear; use something more obvious
            //if (string.IsNullOrEmpty(item.Text))
            //    return;

            if (string.Equals(item.Text, _firstItem))
                return;

            try
            {
                await _layerBuilder.BuildLayerAsync(item.Text);
            }
            // Catch all, because an uncaught exception in an Add-In will crash ArcGIS Pro
            catch (Exception ex)
            {
                ShowError(ex);
            }

        }

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