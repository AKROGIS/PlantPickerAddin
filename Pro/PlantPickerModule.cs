using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlantPickerAddin
{
    /// <summary>
    /// A ArcGIS Pro Add-In module for displaying plant picker combo boxes
    /// which will add layer files to the current map based on the combo box selections.
    /// </summary>
    internal class PlantPickerModule : Module
    {
        private static readonly string _folder = @"C:\tmp\AddinTesting\PlantPicker\Plants";
        //private static readonly string _folder = @"X:\AKR\DENA\biologic\Plants";
        private static readonly string _fgdb = @"Plants.gdb";
        private static PlantPickerModule _this = null;

        /// <summary>
        /// Retrieves the singleton instance to this module.
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

        /// <summary>
        /// The Plant Picker Module Constructor.  Configures picklists for the combo boxes.
        /// </summary>
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

        /// <summary>
        /// Creates an asynchronous task to load a picklist from an external database.
        /// This is created in the module level so it can be used by multiple Combo Boxes, but loaded only once.
        /// </summary>
        /// <returns>A <see cref="Picklist"/> with "Species" values.</returns>
        public async Task<PickList> LoadSpeciesPickList()
        {
            await _speciesPickList.LoadAsync();
            return _speciesPickList;
        }

        /// <summary>
        /// Creates an asynchronous task to load a picklist from an external database.
        /// This is created in the module level so it can be used by multiple Combo Boxes, but loaded only once.
        /// </summary>
        /// <returns>A <see cref="Picklist"/> with "Rank" values.</returns>
        public async Task<PickList> LoadRankPickList()
        {
            await _rankPickList.LoadAsync();
            return _rankPickList;
        }

        /// <summary>
        /// The folder that will contain the picklist database and the layer files
        /// used by this add in.  It should be a stable, static network location available to
        /// all Add-In users.
        /// </summary>
        public string Folder
        {
            get { return _folder; }
        }

    }

    /// <summary>
    /// An exception due to a change in the external state for the configured options.
    /// Examples include deleting, moving, or altering the layer files or databases this tool has
    /// been configured to look for in the file system.
    /// 
    /// The message must be set by the programmer.  It will be displayed to the user and should be
    /// clear enough to guide the user to a solution.
    /// </summary>
    public class ConfigurationException : ApplicationException
    {
        public ConfigurationException(string message) : base(message) { }
    }
}
