using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantPickerAddIn
{
    class SpeciesOccurrenceLayerFactory
    {
        //private const string Directory =
        //    @"C:\Users\resarwas\Documents\Visual Studio 2010\Projects\PlantPickerAddIn\Data";
        private const string Directory =
            @"C:\Plants";
        private const string LayerName = Directory + @"\Plant Cover by Species.lyr";
        private const string FgdbName = Directory + @"\plants.gdb";
        private const string PickListTableName = "taxonPicklist";
        private const string FieldName = "Taxon_txtLocalAcceptedName";
        
        private readonly PickList _picklist;

        public SpeciesOccurrenceLayerFactory()
        {
            _picklist = new PickList(FgdbName, PickListTableName);
        }

        internal string Validate()
        {
            string result = null;
            //FIXME - implement
            result = "SpeciesOccurrenceLayerFactory is not implemented"; 
            return result;
        }

        internal IEnumerable<string> PicklistNames
        {
            get { return _picklist.Names; }
        }

        internal void BuildLayer(string plant)
        {
            throw new NotImplementedException();
        }
    }
}
