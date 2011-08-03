using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;

namespace PlantPickerAddIn
{
    class PickList
    {
        public PickList(string fgdb, string table)
        {
            if (string.IsNullOrEmpty(fgdb))
                throw new ArgumentNullException("fgdb");
            if (string.IsNullOrEmpty(table))
                throw new ArgumentNullException("table");
            FgdbName = fgdb;
            PickListTableName = table;
            //FIXME - validate FgdbName and PickListTableName - used in GetNames()
        }

        public string FgdbName { get; private set; }

        public string PickListTableName { get; private set; }

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

        private List<string> GetNames()
        {
            var results = new List<string>();
            var workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(
                    Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")
                );
            var featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(FgdbName, 0);
            //FIXME - manage cursor lifetime
            ITable data = featureWorkspace.OpenTable(PickListTableName);
            ICursor cursor = data.Search(null, true);
            IRow row = cursor.NextRow();
            while (row != null)
            {
                //FIXME - throw an exception if this assumption is not valid
                // index 0 has the OID, index 1 has the picklist value
                results.Add(row.Value[1].ToString());
                row = cursor.NextRow();
            }
            return results;
        }
    }
}
