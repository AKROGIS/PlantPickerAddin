using System;
using System.Collections.Generic;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;  //for ComReleaser, requires ESRI.ArcGIS.ADF.Connection.Local.dll

namespace PlantPickerAddIn
{
    class PickList
    {
        private ITable _data;

        public PickList(string fgdb, string table)
        {
            _fgdbFileName = fgdb;
            _pickListTableName = table;
        }
        private readonly string _fgdbFileName;
        private readonly string _pickListTableName;


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
            if (_data == null)
                throw new Exception("Unable to build this picklist due to invalid (or unvalidated) configuration properties.");

            var results = new List<string>();

            using(var comReleaser = new ComReleaser())
            {
                ICursor cursor = _data.Search(null, true);
                comReleaser.ManageLifetime(cursor);
                IRow row;
                // typically field 0 has the OID, and field 1 has the picklist value
                // however if there is only one field, then use it
                int index = _data.Fields.FieldCount < 2 ? 0 : 1;
                while ((row = cursor.NextRow()) != null)
                {
                    results.Add(row.Value[index].ToString());
                }
            }

            return results;
        }

        internal string Validate()
        {
            string results = null;

            if (string.IsNullOrEmpty(_fgdbFileName))
                return "Path to picklist FGDB not defined.";

            if (string.IsNullOrEmpty(_pickListTableName))
                return "Path to picklist table not defined.";

            try
            {
                string ext = Path.GetExtension(_fgdbFileName);
                if (ext == null || ext.ToLower() != ".gdb" || !Directory.Exists(_fgdbFileName))
                    return "FGDB '" + _fgdbFileName + "' not found.";
            }
            catch (ArgumentException)
            {
                return "Path to FGDB '" + _fgdbFileName + "' is not valid.";
            }

            try
            {
                var workspaceFactory = (IWorkspaceFactory) Activator.CreateInstance(
                    Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")
                                                               );
                var featureWorkspace = workspaceFactory.OpenFromFile(_fgdbFileName, 0) as IFeatureWorkspace;
                if (featureWorkspace == null)
                    return "Could not open '" + _fgdbFileName + "' as a FGDB.";
                _data = featureWorkspace.OpenTable(_pickListTableName);
                if (_data == null)
                    return "Could not find table '" + _pickListTableName + "' in FGDB '" + _fgdbFileName + "'.";
                if (_data.Fields == null || _data.Fields.FieldCount < 1)
                {
                    _data = null;
                    return "Table '" + _pickListTableName + "' in FGDB '" + _fgdbFileName + "' does not have any fields.";                    
                }
            }
            catch (Exception ex)
            {
                _data = null;
                return "Could not validate the picklist '" + _pickListTableName + "' in FGDB '" + _fgdbFileName + "'\n" + ex.Message;
            }

            return results;
        }
    }
}
