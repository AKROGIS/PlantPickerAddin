using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;


namespace PlantPickerAddin
{
    static class LayerUtilities
    {
        internal static void RandomizeMarkerColor(CIMFeatureLayer geoLayer)
        {
            if (geoLayer == null)
                return;
            if (geoLayer.Renderer is CIMSimpleRenderer)
            {
                RandomizeMarkerColor((CIMSimpleRenderer)geoLayer.Renderer);
                return;
            }
            if (geoLayer.Renderer is CIMClassBreaksRenderer)
            {
                RandomizeMarkerColor((CIMClassBreaksRenderer)geoLayer.Renderer);
                return;
            }
            if (geoLayer.Renderer is CIMUniqueValueRenderer)
            {
                RandomizeMarkerColor((CIMUniqueValueRenderer)geoLayer.Renderer);
                return;
            }
            throw new ConfigurationException("Unsupported renderer in feature layer.");
        }

        private static void RandomizeMarkerColor(CIMSimpleRenderer renderer)
        {
            renderer.Symbol.Symbol.SetColor(RandomColor());
        }

        private static void RandomizeMarkerColor(CIMClassBreaksRenderer renderer)
        {
            CIMRGBColor color = RandomColor();
            foreach (var rendererBreak in renderer.Breaks)
            {
                rendererBreak.Symbol.Symbol.SetColor(color);
            }
        }

        private static void RandomizeMarkerColor(CIMUniqueValueRenderer renderer)
        {
            CIMRGBColor color = RandomColor();
            foreach (var group in renderer.Groups)
            {
                foreach (var rendererClass in group.Classes)
                {
                    rendererClass.Symbol.Symbol.SetColor(color);
                }
            }
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

        internal static void RemoveUnusedItemsFromUniqueValueRenderer(FeatureLayer geoLayer)
        {
            if (geoLayer == null)
                return;
            var cimLayer = geoLayer.GetDefinition() as CIMFeatureLayer;
            if (cimLayer == null)
                return;
            var renderer = cimLayer.Renderer as CIMUniqueValueRenderer;
            if (renderer == null)
                throw new ConfigurationException("Feature layer does not have a unique value renderer.");

            //I need to create a unique list of all values in the field
            //I will check every value in the renderer to see if it is in the valid values list
            HashSet<string> validValues = GetAllValues(renderer.Fields[0], geoLayer, cimLayer.FeatureTable.DefinitionExpression);

            //Assume 1 Unique Value Group, remove all un-needed classes
            // Remove classes when class.Values[0].FieldValues[0] is not in validValues; DO NOT modify list during enumeration
            var classesToKeep = new List<CIMUniqueValueClass>();
            foreach (var rendererClass in renderer.Groups[0].Classes)
            {
                // Assumes there is only 1 field in the unique value class
                if (validValues.Contains(rendererClass.Values[0].FieldValues[0])) {
                        classesToKeep.Add(rendererClass);
                }
            }
            renderer.Groups[0].Classes = classesToKeep.ToArray();
            geoLayer.SetDefinition(cimLayer);
        }

        private static HashSet<string> GetAllValues(string fieldName, FeatureLayer layer, string queryExpression)
        {
            var values = new HashSet<string>();
            var table = layer.GetTable();
            if (table == null)
            {
                throw new ConfigurationException("Feature layer has no data source.");
            }
            TableDefinition tableDefinition = table.GetDefinition();
            var index = tableDefinition.FindField(fieldName);
            if (index < 0)
            {
                throw new ConfigurationException($"Feature layer field ({fieldName}) not found in data source.");
            }
            if (tableDefinition.GetFields()[index].FieldType != FieldType.String)
            {
                throw new ConfigurationException($"Feature layer field ({fieldName}) is not a text field.");
            }
            var query = new QueryFilter {
                WhereClause = queryExpression,
            };
            using (RowCursor rowCursor = table.Search(query, false))
            {
                while (rowCursor.MoveNext())
                {
                    string name = (string)rowCursor.Current[index];
                    values.Add(name);
                }
            }
            return values;
        }

    }
}
