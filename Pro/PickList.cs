using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace PlantPickerAddin
{
    class PickList
    {
        public PickList(string fgdb, string table, string field)
        {
            _fgdbFileName = fgdb;
            _pickListTableName = table;
            _pickListFieldName = field;
        }
        private readonly string _fgdbFileName;
        private readonly string _pickListTableName;
        private readonly string _pickListFieldName;
        private string[] _names = new string[0];
        private bool _loaded = false;

        /// <summary>
        /// An enumeration of text values for use in a picklist (combo box).
        /// The enumeration will be empty if the picklist has not been loaded (or failed to load) 
        /// </summary>
        public IEnumerable<string> Names
        {
            get
            {
                return _names;
            }
        }

        /// <summary>
        /// Populates Names with the list of text values from the field, table, and FGDB set in the constructor.
        /// Must be run on the MCT; Call within QueryTask.Run()
        /// Can be called multiple times, but it will return cached values unless the previous call failed.
        /// </summary>
        public async Task LoadAsync()
        {
            await QueuedTask.Run(() => Load());
        }

        private void Load()
        {
            if (_loaded) { return; }
            try
            {
                _names = GetNames().ToArray();
                _loaded = true;
            }
            catch (GeodatabaseNotFoundOrOpenedException exception)
            {
                throw new ConfigurationException($"Picklist database ({_fgdbFileName}) not found: {exception.Message}.");
            }
            catch (GeodatabaseTableException)
            {
                throw new ConfigurationException($"Picklist table ({_pickListTableName}) was not found.");
            }
        }

        /// <summary>
        /// Reads the list of text values from the field, table, and FGDB set in the constructor.
        /// Must be run on the MCT; Call within QueryTask.Run()
        /// </summary>
        /// <returns>A list of strings to use as picklist values.</returns>
        private List<string> GetNames()
        {
            var names = new List<string>();

            using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(_fgdbFileName))))
            // GeodatabaseNotFoundOrOpenedException if _fgdbFileName not found or can't be opened.
            {
                using (Table table = geodatabase.OpenDataset<Table>(_pickListTableName))
                //GeodatabaseCatalogDatasetException if name not found
                {
                    TableDefinition tableDefinition = table.GetDefinition();
                    var index = tableDefinition.FindField(_pickListFieldName);
                    if (index < 0)
                    {
                        throw new ConfigurationException($"Picklist field ({_pickListFieldName}) not found.");
                    }
                   if (tableDefinition.GetFields()[index].FieldType != FieldType.String)
                    {
                        throw new ConfigurationException($"Picklist field ({_pickListFieldName}) is not a text field.");
                    }
                    using (RowCursor rowCursor = table.Search(null, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            string name = (string)rowCursor.Current[index];
                            // TODO: Null/Empty text in combo box is unclear; use something more obvious
                            names.Add(name);
                            //if (!string.IsNullOrWhiteSpace(name))
                            //{
                            //    names.Add(name);
                            //}
                        }
                    }
                }
            }
            return names;
        }
    }
}
