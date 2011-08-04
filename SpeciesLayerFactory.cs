using System;
using System.Collections.Generic;
using System.IO;
using ESRI.ArcGIS.Carto; //For ILayer

namespace PlantPickerAddIn
{
    class SpeciesLayerFactory
    {
        private PickList _picklist;
        private ILayer _layer;

        public SpeciesLayerFactory()
        {
            Directory = @"C:\Plants";
            //Directory = @"X:\get_path_from_john";
            FgdbName = Directory + @"\plants.gdb";
            PickListTableName = "taxonPicklist";
            //FieldName must be the featureclass field name (not layer alias), due to definition query restrictions
            FieldName = "Taxon_txtLocalAcceptedName";
            LayerNameFormat = "{0}";
        }

        public string Directory { get; set; }
        public string FgdbName { get; set; }
        public string PickListTableName { get; set; }
        public string FieldName { get; set; }
        public string LayerNameFormat { get; set; }
        public Action<ILayer> LayerFixer { get; set; }
        public string LayerFileName
        {
            get { return _layerFileName; }
            set {
                _layerFileName = Path.IsPathRooted(value) ? value
                                                          : Path.Combine(Directory,value);
            }
        }
        private string _layerFileName;

        internal string Validate()
        {
            string result = ValidateLayerFile();

            if (String.IsNullOrEmpty(result))
                result = ValidateFieldName();

            if (String.IsNullOrEmpty(result))
            {
                _picklist = new PickList(FgdbName, PickListTableName);
                result = _picklist.Validate();
            }
            return result;
        }

        private string ValidateLayerFile()
        {
            if (string.IsNullOrEmpty(LayerFileName))
                return "No layer file template is defined.";

            if (!File.Exists(LayerFileName))
                return "layer file '" + LayerFileName + "' does not exist.";

            try
            {
                var layerFile = new LayerFileClass();
                layerFile.Open(LayerFileName);
                _layer = layerFile.Layer;
                layerFile.Close();
            }
            catch (Exception ex)
            {
                _layer = null;
                return "Could not load layer file '" + LayerFileName + "'\n" + ex.Message;
            }

            if (_layer == null)
                return "layer file '" + LayerFileName + "' is empty.";

            if (!(_layer is IFeatureLayerDefinition2))
            {
                _layer = null;
                return "layer file '" + LayerFileName + "' does not support definition queries.";
            }

            return null;
        }

        private string ValidateFieldName()
        {
            if (_layer == null)
                throw new NullReferenceException("_layer");

            var featureLayer = _layer as IFeatureLayer;
            if (featureLayer == null || 
                featureLayer.FeatureClass == null ||
                featureLayer.FeatureClass.Fields == null)
            {
                _layer = null;
                return "layer file '" + LayerFileName + "' does not have a feature class with fields.";
            }
            if (featureLayer.FeatureClass.Fields.FindField(FieldName) < 0)
            {
                _layer = null;
                return "layer file '" + LayerFileName + "' does not have a field named '" + FieldName + "'.";
            }

            return null;
        }

        internal IEnumerable<string> PicklistNames
        {
            get { return _picklist.Names; }
        }

        internal void BuildLayer(string plant)
        {
            //Validate must be called by the user before calling BuildLayers
            if (_layer == null)
                throw new Exception("Unable to build this layer due to invalid (or unvalidated) configuration properties.");

            //reload the layer file to create a new layer (we may get called many times)
            var layerFile = new LayerFileClass();
            layerFile.Open(LayerFileName);
            _layer = layerFile.Layer;
            layerFile.Close();

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
            _layer.Name = string.Format(LayerNameFormat, plant);
            ((IFeatureLayerDefinition2)_layer).DefinitionExpression = definitionQuery;
            // Call the layer fixer delegate
            var layerFixer = LayerFixer;
            if (layerFixer != null)
                layerFixer(_layer);
            ArcMap.Document.AddLayer(_layer);
        }
    }
}
