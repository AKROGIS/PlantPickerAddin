using System;
using System.Windows.Forms;

namespace PlantPickerAddIn
{
    public class SpeciesRank : ESRI.ArcGIS.Desktop.AddIns.ComboBox
    {
        private readonly SpeciesLayerFactory _layerBuilder;
        
        public SpeciesRank()
        {
            try
            {
                _layerBuilder = new SpeciesLayerFactory
                                    {
                                        LayerFileName = "Plant Species by State Rank.lyr",
                                        PickListTableName = "akhpRankPicklist",
                                        FieldName = "Taxon_vasc_aknhp_s_rank",
                                        LayerNameFormat = "State Rank of {0}",
                                        LayerFixer = LayerUtilities.RemoveUnusedItemsFromUniqueValueRenderer,
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
