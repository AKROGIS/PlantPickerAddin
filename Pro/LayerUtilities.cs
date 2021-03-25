using ArcGIS.Core.CIM;
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

        internal static void RemoveUnusedItemsFromUniqueValueRenderer(CIMFeatureLayer geoLayer)
        {
            if (geoLayer == null)
                return;
            var renderer = geoLayer.Renderer as CIMUniqueValueRenderer;
            if (renderer == null)
                throw new ConfigurationException("Feature layer does not have a unique value renderer.");

            //I need to create a unique list of all values in the field
            //I will check every value in the renderer to see if it is in the valid values list
            //I use Dictionary because it enforces uniqueness and is O(1) for contains, while List is O(n).

            Dictionary<string, int> validValues = GetAllValues(renderer.Fields[0], geoLayer);

            foreach (var group in renderer.Groups)
            {
                foreach (var rendererClass in group.Classes)
                {
                    foreach (var value in rendererClass.Values)
                    {
                        var valuesToKeep = new List<string>();
                        foreach (var fieldValue in value.FieldValues)
                        {
                            if (validValues.ContainsKey(fieldValue))
                                valuesToKeep.Add(fieldValue);
                        }
                        value.FieldValues = valuesToKeep.ToArray();
                    }
                }
            }
        }

        private static Dictionary<string, int> GetAllValues(string fieldName, CIMFeatureLayer layer)
        {
            var results = new Dictionary<string, int>();
            // TODO: Get list of values in field `fieldName` in layer.FeatureTable
            // create a ArcGIS.Core.Data.Table from layer.FeatureTable
            // See PickList.GetNames for the rest of the details.
            return results;
        }
    }
}
