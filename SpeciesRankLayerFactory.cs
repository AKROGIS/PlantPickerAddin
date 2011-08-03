using ESRI.ArcGIS.Carto; //For ILayer

namespace PlantPickerAddIn
{
    class SpeciesRankLayerFactory : SpeciesLayerFactory
    {
        public SpeciesRankLayerFactory()
        {
            LayerFileName = Directory + @"\Plant Species by State Rank.lyr";
            PickListTableName = "akhpRankPicklist";
            FieldName = "Taxon_vasc_aknhp_s_rank";
            LayerNameFormat = "State Rank of {0}";
        }

        protected override void  AdjustLayer(ILayer layer)
        {
            LayerUtilities.RemoveUnusedItemsFromLegend(layer);
        }
    }
}
