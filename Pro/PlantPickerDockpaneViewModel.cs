using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PlantPickerAddin
{
    internal class PlantPickerDockpaneViewModel : DockPane
    {
        private const string _dockPaneID = "PlantPickerAddin_PlantPickerDockpane";

        protected PlantPickerDockpaneViewModel() { }

        /// <summary>
        /// Show the DockPane.
        /// </summary>

        protected override Task InitializeAsync()
        {
            return LoadPicklist();
        }

        private async Task LoadPicklist()
        {
            var speciesPicklist = await PlantPickerModule.Current.LoadSpeciesPickList();
            _speciesList = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>(speciesPicklist.Names));
            _selectedSpecies = AllSpecies.FirstOrDefault();
            return;
        }
        ReadOnlyObservableCollection<string> _speciesList;

        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Create Plant Layers";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        public ReadOnlyObservableCollection<string> AllSpecies => _speciesList;
        public string SelectedSpecies
        {
            get
            {
                return _selectedSpecies;
            }
            set
            {
                SetProperty(ref _selectedSpecies, value, () => SelectedSpecies);
                System.Diagnostics.Debug.WriteLine("selected");
            }
        }
        string _selectedSpecies = "Loading...";
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class PlantPickerDockpane_ShowButton : Button
    {
        protected override void OnClick()
        {
            PlantPickerDockpaneViewModel.Show();
        }
    }
}
