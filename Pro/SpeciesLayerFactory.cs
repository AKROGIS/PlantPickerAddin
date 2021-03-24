using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping; //For Layer
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PlantPickerAddin
{
    class SpeciesLayerFactory
    {
        private readonly string _layerFilePath;

        public SpeciesLayerFactory(string layerfile)
        {
            _layerFilePath = Path.IsPathRooted(layerfile) ? layerfile
                                                          : Path.Combine(PlantPickerModule.Current.Folder, layerfile);
            FieldName = "Taxon_txtLocalAcceptedName";
            LayerNameFormat = "{0}";
            RandomizeMarkerColor = false;
        }
        public string FieldName { get; set; }
        public string LayerNameFormat { get; set; }
        public bool RandomizeMarkerColor { get; set; }
        public string LayerFilePath
        {
            get { return _layerFilePath; }
        }

        public async Task BuildLayerAsync(string plant)
        {
            await QueuedTask.Run(() => BuildLayer(plant));
        }

        private void BuildLayer(string plant)
        {
            //MessageBox.Show($"Build Layer for {plant}");

            string definitionQuery;
            if (string.IsNullOrEmpty(plant))
            {
                plant = "unspecified";
                definitionQuery = "\"" + FieldName + "\" = '' OR \"" + FieldName + "\" is null";
            }
            else
            {
                definitionQuery = "\"" + FieldName + "\" = '" + plant.Replace("'", "''") + "'";
            }

            var layerName = string.Format(LayerNameFormat, plant);

            if (MapView.Active == null)
            {
                throw new ConfigurationException("No map available.");
            }
            var map = MapView.Active.Map;

            var layerDocument = new LayerDocument(LayerFilePath);
            var cimLayerDocument = layerDocument.GetCIMLayerDocument();
            if (cimLayerDocument == null)
            {
                var msg = $"Layer File ({LayerFilePath}) not found or invalid.";
                throw new ConfigurationException(msg);
            }

            if (!(cimLayerDocument.LayerDefinitions[0] is CIMFeatureLayer cimFeatureLayer))
            {
                throw new ConfigurationException("Layer file is not a feature layer.");
            }
            cimFeatureLayer.Name = layerName;
            var featureTable = cimFeatureLayer.FeatureTable;
            if (featureTable == null)
            {
                throw new ConfigurationException("Feature layer has no data source.");
            }
            featureTable.DefinitionExpression = definitionQuery;

            if (RandomizeMarkerColor)
            {
                if (!(cimFeatureLayer.Renderer is CIMSimpleRenderer renderer))
                {
                    throw new ConfigurationException("Feature layer is not using a simple renderer.");
                }
                renderer.Symbol.Symbol.SetColor(RandomColor());
            }

            var layerParameters = new LayerCreationParams(cimLayerDocument);
            LayerFactory.Instance.CreateLayer<FeatureLayer>(layerParameters, map, LayerPosition.AutoArrange);
        }
        private static CIMRGBColor RandomColor()
        {
            var color = new CIMRGBColor();
            var rand = new Random();
            color.R = rand.Next(256);
            color.B = rand.Next(256);
            color.G = rand.Next(256);
            return color;
        }

    }
}
