namespace PlantPickerAddIn
{
    class SpeciesCoverageLayerFactory : SpeciesLayerFactory
    {
        public SpeciesCoverageLayerFactory()
        {
            LayerFileName = Directory + @"\Plant Cover by Species.lyr";
            LayerNameFormat = "Estimated abundance of {0}";
        }
    }
}
