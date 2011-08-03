using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto; //For ILayer
using System.Linq;
using ESRI.ArcGIS.Geodatabase;

namespace PlantPickerAddIn
{
    class SpeciesRankLayerFactory
    {
        //private const string Directory =
        //    @"C:\Users\resarwas\Documents\Visual Studio 2010\Projects\PlantPickerAddIn\Data";
        private const string Directory =
            @"C:\Plants";
        private const string LayerName = Directory + @"\Plant Species by State Rank.lyr";
        private const string FgdbName = Directory + @"\plants.gdb";
        private const string PickListTableName = "akhpRankPicklist";
        private const string FieldName = "Taxon_vasc_aknhp_s_rank";
        
        private readonly PickList _picklist;

        public SpeciesRankLayerFactory()
        {
            _picklist = new PickList(FgdbName, PickListTableName);
        }

        internal string Validate()
        {
            string result = null;
            //FIXME - implement
            return result;
        }

        internal IEnumerable<string> PicklistNames
        {
            get { return _picklist.Names; }
        }

        internal void BuildLayer(string rank)
        {
            var layerFile = new LayerFileClass();
            ILayer layer;
            try
            {
                layerFile.Open(LayerName);
                layer = layerFile.Layer;
            }
            catch (Exception ex)
            {
                //FIXME - move exception handling and error messaging to higher level
                MessageBox.Show("Could not load '" + LayerName + "'\n" + ex.Message, "No Layer File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                layer = null;
            }

            if (layer != null && layer is IFeatureLayerDefinition2)
            {
                if (string.IsNullOrEmpty(rank))
                {
                    layer.Name = "State Rank unspecified";
                    const string definitionQuery = "\"" + FieldName + "\" = '' OR \"" + FieldName + "\" is null";
                    ((IFeatureLayerDefinition2)layer).DefinitionExpression = definitionQuery;
                }
                else
                {
                    layer.Name = "State Rank " + rank;
                    string definitionQuery = "\"" + FieldName + "\" = '" + rank.Replace("'", "''") + "'";
                    ((IFeatureLayerDefinition2)layer).DefinitionExpression = definitionQuery;
                }
                RemoveUnusedItemsFromLegend(layer);
                ArcMap.Document.AddLayer(layer);
            }
        }

        private static void RemoveUnusedItemsFromLegend(ILayer layer)
        {
            var geoLayer = layer as IGeoFeatureLayer;
            if (geoLayer == null)
                return;
            var renderer = geoLayer.Renderer as IUniqueValueRenderer;
            if (renderer == null)
                return;
            //FIXME - handle multiple unique value renderers
            List<string> validValue = GetValidValues(renderer.Field[0], geoLayer);  //A valid value is one that is used at least once

            var valuesToRemove = new List<string>();
            for (int i = 0; i < renderer.ValueCount; i++)
            {
                string value = renderer.Value[i];
                if (!validValue.Contains(value))
                    valuesToRemove.Add(value);
            }
            foreach (var unused in valuesToRemove)
            {
                renderer.RemoveValue(unused);
            }
        }

        private static Dictionary<string, List<string>> GetValidValues(IUniqueValueRenderer renderer, IGeoFeatureLayer layer)
        {
            var results = new Dictionary<string, List<string>>();
            for (int i = 0; i < renderer.FieldCount; i++)
            {
                string fieldName = renderer.Field[i];
                results[fieldName] = GetValidValues(fieldName, layer);
            }
            return results;
        }

        private static List<string> GetValidValues(string fieldName, IGeoFeatureLayer layer)
        {
            var results = new Dictionary<string, int>();
            IQueryFilter query = new QueryFilter { SubFields = fieldName };
            IFeatureCursor cursor = layer.Search(query, true);
            int fieldIndex = cursor.FindField(fieldName);
            IFeature feature = cursor.NextFeature();
            while (feature != null)
            {
                results[feature.Value[fieldIndex].ToString()] = 1;
                feature = cursor.NextFeature();
            }
            return results.Keys.ToList();
        }
    }
}
