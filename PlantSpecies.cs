using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto; //For ILayer
using ESRI.ArcGIS.Display; // for ISymbol

namespace PlantPickerAddIn
{
    public class PlantSpecies : ESRI.ArcGIS.Desktop.AddIns.ComboBox
    {
        //private const string Directory =
        //    @"C:\Users\resarwas\Documents\Visual Studio 2010\Projects\PlantPickerAddIn\Data";
        private const string Directory =
            @"C:\Plants";
        private const string LayerName = Directory + @"\Plant Cover by Species.lyr";
        private const string FgdbName = Directory + @"\plants.gdb";
        private const string PickListTableName = "taxonPicklist";
        private const string FieldName = "Taxon_txtLocalAcceptedName";

        public PlantSpecies()
        {
            foreach (string plant in Names)
                Add(plant);
        }

        protected override void OnSelChange(int cookie)
        {
            base.OnSelChange(cookie);
            BuildPlantLayer(Value);
        }

        private static void BuildPlantLayer(string plant)
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
                MessageBox.Show("Could not load '" + LayerName + "'\n"+ ex.Message, "No Layer File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                layer = null;
            }

            if (layer != null && layer is IFeatureLayerDefinition2)
            {
                if (string.IsNullOrEmpty(plant))
                {
                    layer.Name = "Species unspecified";
                    const string definitionQuery = "\"" + FieldName + "\" = '' OR \"" + FieldName + "\" is null";
                    ((IFeatureLayerDefinition2)layer).DefinitionExpression = definitionQuery;
                }
                else
                {
                    layer.Name = plant;
                    string definitionQuery = "\"" + FieldName + "\" = '" + plant.Replace("'", "''") + "'";
                    ((IFeatureLayerDefinition2) layer).DefinitionExpression = definitionQuery;
                }
                RandomizeMarkerColor(layer);
                ArcMap.Document.AddLayer(layer);
            }
        }

        private static void RandomizeMarkerColor(ILayer layer)
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

        private static void RandomizeMarkerColor(IClassBreaksRenderer renderer)
        {
            IMarkerSymbol symbol;
            IRgbColor color = RandomColor();
            for (int i = 0; i < renderer.BreakCount; i++)
            {
                symbol = renderer.Symbol[i] as IMarkerSymbol;
                if (symbol != null)
                    symbol.Color = color;
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

        public IEnumerable<string> Names
        {
            get
            {
                if (_names == null)
                    _names = GetNames().ToArray();
                return _names;
            }
        }

        private string[] _names;

        static List<string> GetNames()
        {
            return new PickList(FgdbName, PickListTableName);
        }
    }
}
