using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Dialogs;
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
        }
        public string FieldName { get; set; }
        public string LayerNameFormat { get; set; }
        public Action<Layer> LayerFixer { get; set; }
        public string LayerFileName
        {
            get { return _layerFilePath; }
        }

        //internal async Task<string> Validate()
        //{
        //    string result = ""; //ValidateLayerFile();

        //    //if (String.IsNullOrEmpty(result))
        //    //    result = ValidateFieldName();

        //    return result;
        //}

        //private string ValidateLayerFile()
        //{
        //    if (string.IsNullOrEmpty(LayerFileName))
        //        return "No layer file template is defined.";

        //    if (!File.Exists(LayerFileName))
        //        return "layer file '" + LayerFileName + "' does not exist.";

        //    try
        //    {
        //        var layerFile = new LayerFileClass();
        //        layerFile.Open(LayerFileName);
        //        _layer = layerFile.Layer;
        //        layerFile.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        _layer = null;
        //        return "Could not load layer file '" + LayerFileName + "'\n" + ex.Message;
        //    }

        //    if (_layer == null)
        //        return "layer file '" + LayerFileName + "' is empty.";

        //    if (!(_layer is IFeatureLayerDefinition2))
        //    {
        //        _layer = null;
        //        return "layer file '" + LayerFileName + "' does not support definition queries.";
        //    }

        //    return null;
        //}

        //private string ValidateFieldName()
        //{
        //    if (_layer == null)
        //        throw new NullReferenceException("_layer");

        //    var featureLayer = _layer as IFeatureLayer;
        //    if (featureLayer == null || 
        //        featureLayer.FeatureClass == null ||
        //        featureLayer.FeatureClass.Fields == null)
        //    {
        //        _layer = null;
        //        return "layer file '" + LayerFileName + "' does not have a feature class with fields.";
        //    }
        //    if (featureLayer.FeatureClass.Fields.FindField(FieldName) < 0)
        //    {
        //        _layer = null;
        //        return "layer file '" + LayerFileName + "' does not have a field named '" + FieldName + "'.";
        //    }

        //    return null;
        //}

        internal void BuildLayer(string plant)
        {
            MessageBox.Show($"Build Layer for {plant}");
            //Validate must be called by the user before calling BuildLayers
            //    if (_layer == null)
            //        throw new Exception("Unable to build this layer due to invalid (or unvalidated) configuration properties.");

            //    //reload the layer file to create a new layer (we may get called many times)
            //    var layerFile = new LayerFileClass();
            //    layerFile.Open(LayerFileName);
            //    _layer = layerFile.Layer;
            //    layerFile.Close();

            //    string definitionQuery;
            //    if (string.IsNullOrEmpty(plant))
            //    {
            //        plant = "unspecified";
            //        definitionQuery = "\"" + FieldName + "\" = '' OR \"" + FieldName + "\" is null";
            //    }
            //    else
            //    {
            //        definitionQuery = "\"" + FieldName + "\" = '" + plant.Replace("'", "''") + "'";
            //    }
            //    _layer.Name = string.Format(LayerNameFormat, plant);
            //    ((IFeatureLayerDefinition2)_layer).DefinitionExpression = definitionQuery;
            //    // Call the layer fixer delegate
            //    var layerFixer = LayerFixer;
            //    if (layerFixer != null)
            //        layerFixer(_layer);
            //    ArcMap.Document.AddLayer(_layer);

            //    var lyrDocFromLyrxFile = new LayerDocument(LayerFileName);
            //    var cimLyrDoc = lyrDocFromLyrxFile.GetCIMLayerDocument();
            //    //modifying its renderer symbol to red
            //    var r = ((CIMFeatureLayer)cimLyrDoc.LayerDefinitions[0]).Renderer as CIMSimpleRenderer;
            //    r.Symbol.Symbol.SetColor(new CIMRGBColor() { R = 255 });
            //    //create a layer and add it to a map
            //    var lcp = new LayerCreationParams(cimLyrDoc);
            //    var lyr = LayerFactory.Instance.CreateLayer<FeatureLayer>(lcp, map, LayerPosition.AutoArrange);
        }
    }
}
