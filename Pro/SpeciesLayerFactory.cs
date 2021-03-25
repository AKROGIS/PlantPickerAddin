using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping; //For Layer
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlantPickerAddin
{
    /// <summary>
    /// A class used to load a lyrx file template, modify it, and add it to the active map.
    /// 
    /// A layer file with a relative path will be searched for in <see cref="PlantPickerModule.Current.Folder"/>
    /// 
    /// Assumptions:
    /// The first item in the layer file must be a feature layer with a simple renderer
    /// and a feature class data source that is in a file geodatabase.
    /// </summary>
    class SpeciesLayerFactory
    {
        private readonly string _layerFilePath;

        /// <summary>
        /// Construct a class to load a lyrx file template, modify it, and add it to the active map.
        /// 
        /// Assumptions:
        /// The first item in the layer file must be a feature layer with a simple renderer
        /// and a feature class data source that is in a file geodatabase.
        /// </summary>
        /// <param name="layerfile">The layer file to be loaded.
        /// A layer file with a relative path will be searched for in <see cref="PlantPickerModule.Current.Folder"/></param>
        public SpeciesLayerFactory(string layerfile)
        {
            _layerFilePath = Path.IsPathRooted(layerfile) ? layerfile
                                                          : Path.Combine(PlantPickerModule.Current.Folder, layerfile);
            FieldName = "Taxon_txtLocalAcceptedName";
            LayerNameFormat = "{0}";
            RandomizeMarkerColor = false;
        }

        /// <summary>
        /// The name of a field in the layer files data source that will be used in the definition query
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// A format string for the name of the layer in the map table of contents.
        /// the string must contain "{0}" which will be replaced with `attributeValue` when building the layer.
        /// </summary>
        public string LayerNameFormat { get; set; }

        /// <summary>
        /// True if the layer file should have a new random color assigned to the features.
        /// Assumes a simple renderer.
        /// </summary>
        public bool RandomizeMarkerColor { get; set; }

        /// <summary>
        /// The full path of the layer file provided in the class constructor
        /// </summary>
        public string LayerFilePath
        {
            get { return _layerFilePath; }
        }

        /// <summary>
        /// Creates an asynchronous queued task to load the layer file and modify it before adding it to the map.
        /// Can be called multiple times with the same or different attribute values.
        /// 
        /// Will generate any number of exceptions.  Anything but a <see cref="ConfigurationException"/> is a programmming error.
        /// A ConfigurationException will result if the layer file or it's source data is deleted, moved, or altered.
        /// </summary>
        /// <param name="attributeValue">Only features with <see cref="FieldName"/> == attributeValue will be displayed in this layer</param>
        public async Task BuildLayerAsync(string attributeValue)
        {
            await QueuedTask.Run(() => BuildLayer(attributeValue));
        }

        /// <summary>
        /// Loads the layer file, modifies the defintion query to <see cref="FieldName"/> == attributeValue
        /// Applies any other specified transformation, then adds the layer to the map.
        /// Must be run on the MCT; Call within <see cref="ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run"/>
        /// Can be called multiple times with the same or different attribute values.
        /// </summary>
        /// <param name="attributeValue">Only features with <see cref="FieldName"/> == attributeValue will be displayed in this layer</param>
        private void BuildLayer(string attributeValue)
        {

            string definitionQuery;
            if (string.IsNullOrEmpty(attributeValue))
            {
                attributeValue = "unspecified";
                definitionQuery = "\"" + FieldName + "\" = '' OR \"" + FieldName + "\" is null";
            }
            else
            {
                definitionQuery = "\"" + FieldName + "\" = '" + attributeValue.Replace("'", "''") + "'";
            }

            var layerName = string.Format(LayerNameFormat, attributeValue);

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
                // FIXME: need to randomize UniqueValueRender and ClassBreakRenderer
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
