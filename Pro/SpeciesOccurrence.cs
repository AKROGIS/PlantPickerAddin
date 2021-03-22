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
        private readonly string _fisrtItem = "Select a Species";

        /// <summary>
        /// Combo Box constructor
        /// </summary>
        public SpeciesOccurrence()
        {
            _layerBuilder = new SpeciesLayerFactory("Plant Occurrence by Species.lyr")
            {
                LayerNameFormat = "All occurrences of {0}",
                //LayerFixer = LayerUtilities.RandomizeMarkerColor,
            };
            Task.Run(InitAsync);
        }

        private async void InitAsync()
        {
            PickList picklist = await PlantPickerModule.Current.LoadSpeciesPickList();
            if (!string.IsNullOrEmpty(picklist.ErrorMessage))
            {
                ShowError("The data required for this tool is missing or invalid.\n" + picklist.ErrorMessage, "Configuration Error");
            }
            Clear();
            Add(new ComboBoxItem(_fisrtItem));
            foreach (string rank in picklist.Names)
            {
                Add(new ComboBoxItem(rank));
            }
            SelectedItem = ItemCollection.FirstOrDefault();
            Enabled = true;
        }

        /// <summary>
        /// The on comboBox selection change event. 
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected override void OnSelectionChange(ComboBoxItem item)
        {

            if (item == null)
                return;

            if (string.IsNullOrEmpty(item.Text))
                return;

            if (string.Equals(item.Text, _fisrtItem))
                return;

            try
            {
                _layerBuilder.BuildLayer(item.Text);
            }
            // Catch all, because an uncaught exception in an Add-In will crash ArcGIS Pro
            catch (Exception ex)
            {
                ShowError(GetType() + " encountered a problem.\n\n" + ex);
            }

        }

        private static void ShowError(string msg, string title = "Unhandled Exception")
        {
            MessageBox.Show(msg, title,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);

        }

    }
}