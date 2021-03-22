using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace PlantPickerAddin
{
    internal class PlantPickerModule : Module
    {
        private static readonly string _folder = @"C:\tmp\AddinTesting\PlantPicker\Plants";
        //private static readonly string _folder = @"X:\AKR\DENA\biologic\Plants";
        private static readonly string _fgdb = @"Plants.gdb";
        private static PlantPickerModule _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static PlantPickerModule Current
        {
            get
            {
                return _this ?? (_this = (PlantPickerModule)FrameworkApplication.FindModule("PlantPickerAddin_Module"));
            }
        }

        private readonly PickList _speciesPickList;
        private readonly PickList _rankPickList;

        public PlantPickerModule()
        {
            string fgdbPath = Path.Combine(_folder, _fgdb);
            string tableName = "taxonPicklist";
            string fieldName = "txtLocalAcceptedName";
            _speciesPickList = new PickList(fgdbPath, tableName, fieldName);
            tableName = "akhpRankPicklist";
            fieldName = "vasc_aknhp_s_rank";
            _rankPickList = new PickList(fgdbPath, tableName, fieldName);
        }

        public async Task<PickList> LoadSpeciesPickList()
        {
            await _speciesPickList.LoadAsync();
            return _speciesPickList;
        }

        public async Task<PickList> LoadRankPickList()
        {
            await _rankPickList.LoadAsync();
            return _rankPickList;
        }

        public string Folder
        {
            get { return _folder; }
        }

    }
}
