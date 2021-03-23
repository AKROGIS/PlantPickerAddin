using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace PlantPickerAddin
{
    public class PicklistFieldWrongTypeException : ApplicationException
    {
        public PicklistFieldWrongTypeException() {}
    }

    public class PicklistFieldNotFoundException : ApplicationException
    {
        public PicklistFieldNotFoundException() {}
    }

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
        private string _errorMessage = @"Picklist is not loaded";

        public IEnumerable<string> Names
        {
            get
            {
                return _names;
            }
        }
        
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        public async Task LoadAsync()
        {
            await QueuedTask.Run(() => Load());
        }

        private void Load()
        {
            if (_loaded) { return; }
            //TODO: Handle Exceptions in Picklist similar to the layer builder; i.e. throw custom exception and catch all in UI code
            try
            {
                _names = GetNames().ToArray();
                _loaded = true;
                _errorMessage = null;
            }
            catch (GeodatabaseNotFoundOrOpenedException exception)
            {
                _errorMessage = $"Picklist database ({_fgdbFileName}) not found: {exception.Message}.";
            }
            catch (GeodatabaseCatalogDatasetException exception)
            {
                _errorMessage = $"Picklist table ({_pickListTableName}) not found: {exception.Message}.";
            }
            catch (PicklistFieldNotFoundException)
            {
                _errorMessage = $"Picklist field ({_pickListFieldName}) not found.";
            }
            catch (PicklistFieldWrongTypeException)
            {
                _errorMessage = $"Picklist field ({_pickListFieldName}) is not a text field.";
            }
            // An uncaught exception in an addin will crash ArcGIS Pro
            catch (Exception ex)
            {
                _errorMessage = $"Unexpected error: {ex}.";
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
                        throw new PicklistFieldNotFoundException();
                    }
                   if (tableDefinition.GetFields()[index].FieldType != FieldType.String)
                    {
                        throw new PicklistFieldWrongTypeException();
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
