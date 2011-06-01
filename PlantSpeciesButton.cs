namespace PlantPickerAddIn
{
    public class PlantSpeciesButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            ArcMap.Application.CurrentTool = null;
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }
}
