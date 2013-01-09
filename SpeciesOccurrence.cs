using System;
using System.Windows.Forms;

namespace DenaPlantPicker
{
    public class SpeciesOccurrence : ESRI.ArcGIS.Desktop.AddIns.ComboBox
    {
        private readonly SpeciesLayerFactory _layerBuilder;

        public SpeciesOccurrence()
        {
            try
            {
                _layerBuilder = new SpeciesLayerFactory
                                    {
                                        LayerFileName = "Plant Occurrence by Species.lyr",
                                        LayerNameFormat = "All occurrences of {0}",
                                        LayerFixer = LayerUtilities.RandomizeMarkerColor,
                                    };
                string msg = _layerBuilder.Validate();
                if (string.IsNullOrEmpty(msg))
                {
                    foreach (string rank in _layerBuilder.PicklistNames)
                        Add(rank);
                }
                else
                {
                    MessageBox.Show("The data required for this tool is missing or invalid.\n" + msg,
                                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetType() + " encountered a problem.\n\n" + ex,
                                "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnSelChange(int cookie)
        {
            try
            {
                base.OnSelChange(cookie);
                _layerBuilder.BuildLayer(Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetType() + " encountered a problem.\n\n" + ex,
                                "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
