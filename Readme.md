# Plant Picker Addin

ArcGIS Addins (one for 10.x and one for Pro) developed for Carl Roland
(Denali Plant Ecologist) to support creating map layers on the
fly from selections in pick lists presented by the Addin.  The
items in the picklists, and the data for the layers is in a
file geodatabase on the GIS server on the NPS network.

## Picklists

The following pick lists are available in the toolbar:

### Species Occurrence

Create a layer of plant occurrences by selecting the species name

### Species Observation

Create a layer of plant observations by selecting the species name

### Species Coverage

Create a layer of plant coverage by selecting the species name

### SpeciesRank

Create a layer of plant species with the selected state rank

## Build

### ArcGIS 10.x

* Install ArcObjects SDK (comes with ArcGIS Desktop 10.x).
* Open `10.x\PlantPickerAddin.sln` in the version Visual Studio supported by
  your version of ArcGIS.
* Ensure `Release` and not `Debug` is selected on the Visual Studio toolbar.
* Select `Build` -> `Build Solution` from the Visual Studio menu.

### ArcGIS Pro

* Install the Visual Studio extension `ArcGIS Pro SDK for .Net` (from the
  Visual Studio menu `Extensions` -> `Manage Extensions`) that matches your
  version of ArcGIS Pro.
* Open `Pro\PlantPickerAddin.sln` in the version Visual Studio supported by
  your version of ArcGIS.
* Ensure `Release` and not `Debug` is selected on the Visual Studio toolbar.
* Select `Build` -> `Build Solution` from the Visual Studio menu.

## Deploy

After building a release version, copy
`10.x/bin/x86/release/DenaPlantPicker.esriAddin` to `X:\GIS\Addins\10.X`
and copy
`Pro/bin/release/PlantPickerAddin.esriAddinX` to `X:\GIS\Addins\Pro`.
Alaska GIS users should be configured to automatically load Add-Ins from these
folders. Users without network access can get a copy from someone who does,
and then double click the Addin file to launch the esri Addin install tool.

The tool requires a file geodatabase and layer files to be
deployed to a GIS data server available to all tool users.
The path to the database and layer files is hardcoded as
`X:\AKR\DENA\biologic\Plants` in
`SpeciesLayerFactory.cs` ([10.x](10.x/SpeciesLayerFactory.cs),
[Pro](Pro/SpeciesLayerFactory.cs)).
.  The layer files are also in
the `Data` folder of this repo.  The file geodatabase is
created from a MS Access database maintained by Carl.  See
the folder `Scripts` for instructions and scripts for
creating a new geodatabase if Carl provides an updated
plants database.

The database can be update independent of the Addin provided there
are no schema changes.  If schema changes are needed code changes
may be required.  The file `SpeciesLayerFactory.cs`
assumes a table named `taxonPicklist` with a field named
`Taxon_txtLocalAcceptedName`.  If this
changes in the database, the code will need to be updated and
a new version deployed.  Other database changes may require
changes to the layer files deployed with the database.  The
network location and layer and database filenames should not
need to be changed, code changes will be required if they are.

## Using

You need to be on the NPS network with your X drive
mapped to the GIS data server.

* If you need to use the tool off the NPS network, you will
   need a copy of the GIS network server on an external drive
   mounted as your X drive.  The external drive will need at
   least the `X:\GIS\Addins` and `X:\AKR\DENA\biologic\Plants`
   folders.

### ArcGIS 10.x

1) Open a new or existing ArcMap document.
2) Use the `Customize -> Toolbars` menu to select the
   `Denali Plant Species Selector` toolbar.

   * If the toolbar is not available, see step 1 and make sure
   that `X:\GIS\ADDINS\10.x` is in the addin folder list in the
   AddIn Manager.

3) Select a species or state rank from one of the picklist
   in the toolbar to add that layer to your map.
4) Adjust the layer properties as desired for aesthetic visual
   presentation.
5) Repeat.

### ArcGIS Pro

1) Open a new or existing ArcGIS Pro document.
2) Click on the `Add-In` tab in the Ribbon.

   * If there is no group called `Denali Plant Picker` in the Add-In
   ribbon, then see step 1 above and make sure that `X:\GIS\ADDINS\Pro` is in
   the box titled `Search for additional Add-Ins in these folders` in the
   `Options` tab in the `Add-In Manager` on the `Project` properties tab.

3) Select a species or state rank from one of the pick lists
   in the `Denali Plant Picker` group to add that layer to your map.
4) Adjust the layer properties as desired for aesthetic visual
   presentation.
5) Repeat.
