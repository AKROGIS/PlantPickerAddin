using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;

namespace PlantPickerAddIn
{
    class PickList : List<string>
    {
        public PickList(string fgdb, string table)
        {
            if (string.IsNullOrEmpty(fgdb))
                throw new ArgumentNullException("fgdb");
            if (string.IsNullOrEmpty(table))
                throw new ArgumentNullException("table");

            var workspaceFactory = (IWorkspaceFactory) Activator.CreateInstance(
                    Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")
                );
            var featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(fgdb, 0);
            ITable data = featureWorkspace.OpenTable(table);
            ICursor cursor = data.Search(null, true);
            IRow row = cursor.NextRow();
            while (row != null)
            {
                // index 0 has the OID, index 1 has the picklist value
                Add(row.get_Value(1).ToString());
                row = cursor.NextRow();
            }
        }
    }
}
