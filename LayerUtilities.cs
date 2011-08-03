using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Carto;       //For ILayer
using ESRI.ArcGIS.Display;     //For IMarkerSymbol and IRgbColor
using ESRI.ArcGIS.Geodatabase; //For IFeature, IFeatureCursor, and IQueryFilter


namespace PlantPickerAddIn
{
    static class LayerUtilities
    {
        internal static void RandomizeMarkerColor(ILayer layer)
        {
            var geoLayer = layer as IGeoFeatureLayer;
            if (geoLayer == null)
                return;
            if (geoLayer.Renderer is ISimpleRenderer)
            {
                RandomizeMarkerColor((ISimpleRenderer)geoLayer.Renderer);
                return;
            }
            if (geoLayer.Renderer is IClassBreaksRenderer)
            {
                RandomizeMarkerColor((IClassBreaksRenderer)geoLayer.Renderer);
                return;
            }
            if (geoLayer.Renderer is IUniqueValueRenderer)
            {
                RandomizeMarkerColor((IUniqueValueRenderer)geoLayer.Renderer);
                return;
            }
            if (geoLayer.Renderer is IProportionalSymbolRenderer)
            {
                RandomizeMarkerColor((IProportionalSymbolRenderer)geoLayer.Renderer);
                return;
            }
            if (geoLayer.Renderer is IBivariateRenderer)
            {
                RandomizeMarkerColor((IBivariateRenderer)geoLayer.Renderer);
                return;
            }
        }

        private static void RandomizeMarkerColor(ISimpleRenderer renderer)
        {
            var symbol = renderer.Symbol as IMarkerSymbol;
            if (symbol != null)
                symbol.Color = RandomColor();
        }

        private static void RandomizeMarkerColor(IProportionalSymbolRenderer renderer)
        {
            var symbol = renderer.MinSymbol as IMarkerSymbol;
            if (symbol != null)
            {
                symbol.Color = RandomColor();
                renderer.MinSymbol = (ISymbol) symbol;
            }
            renderer.CreateLegendSymbols();
        }

        private static void RandomizeMarkerColor(IClassBreaksRenderer renderer)
        {
            IMarkerSymbol symbol;
            IRgbColor color = RandomColor();
            for (int i = 0; i < renderer.BreakCount; i++)
            {
                symbol = renderer.Symbol[i] as IMarkerSymbol;
                if (symbol != null)
                {
                    symbol.Color = color;
                    renderer.Symbol[i] = (ISymbol)symbol;
                }
            }
        }

        private static void RandomizeMarkerColor(IUniqueValueRenderer renderer)
        {
            IMarkerSymbol symbol;
            IRgbColor color = RandomColor();
            for (int i = 0; i < renderer.ValueCount; i++)
            {
                symbol = renderer.Symbol[renderer.Value[i]] as IMarkerSymbol;
                if (symbol != null)
                    symbol.Color = color;
            }
        }

        private static void RandomizeMarkerColor(IBivariateRenderer renderer)
        {
            RandomizeMarkerColor(renderer.MainRenderer as IUniqueValueRenderer);
            //RandomizeMarkerColor(renderer.VariationRenderer as IClassBreaksRenderer);
            renderer.CreateLegend();
        }

        private static IRgbColor RandomColor()
        {
            IRgbColor color = new RgbColorClass();
            var rand = new Random();
            color.Red = rand.Next(256);
            color.Blue = rand.Next(256);
            color.Green = rand.Next(256);
            color.UseWindowsDithering = true;
            return color;
        }

        internal static void RemoveUnusedItemsFromLegend(ILayer layer)
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
