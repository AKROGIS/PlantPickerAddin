namespace PlantPickerAddIn
{
    class SpeciesObservationLayerFactory : SpeciesLayerFactory
    {
        public SpeciesObservationLayerFactory()
        {
            LayerFileName = Directory + @"\Plant Observation by Species.lyr";
            LayerNameFormat = "Observation types for {0}";
        }
    }
}
