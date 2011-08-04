using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;       //For ILayer
using ESRI.ArcGIS.Display;     //For IMarkerSymbol and IRgbColor
using ESRI.ArcGIS.Geodatabase; //For IFeature, IFeatureCursor, and IQueryFilter
using ESRI.ArcGIS.ADF;         //for ComReleaser, requires ESRI.ArcGIS.ADF.Connection.Local.dll


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

        internal static void RemoveUnusedItemsFromUniqueValueRenderer(ILayer layer)
        {
            var geoLayer = layer as IGeoFeatureLayer;
            if (geoLayer == null)
                return;
            var renderer = geoLayer.Renderer as IUniqueValueRenderer;
            if (renderer == null)
                return;

            //I need to create a unique list of all values in the field
            //I will check every value in the renderer to see if it is in the valid values list
            //I use Dictionary because it enforces uniqueness and is O(1) for contains, while List is O(n).

            Dictionary<string, int> validValues = GetAllValues(renderer.Field[0], geoLayer);

            var valuesToRemove = new List<string>();
            for (int i = 0; i < renderer.ValueCount; i++)
            {
                string value = renderer.Value[i];
                if (!validValues.ContainsKey(value))
                    valuesToRemove.Add(value);
            }
            foreach (var unused in valuesToRemove)
            {
                renderer.RemoveValue(unused);
            }
        }

        private static Dictionary<string, int> GetAllValues(string fieldName, IGeoFeatureLayer layer)
        {
            var results = new Dictionary<string, int>();
            IQueryFilter query = new QueryFilter { SubFields = fieldName };
            using (var comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = layer.Search(query, true);
                comReleaser.ManageLifetime(cursor);
                IFeature feature;
                int fieldIndex = cursor.FindField(fieldName);
                while ((feature = cursor.NextFeature()) != null)
                {
                    results[feature.Value[fieldIndex].ToString()] = 1;
                }
            }
            return results;
        }
    }
}
