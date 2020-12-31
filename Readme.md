# Plant Picker Addin

This is an ArcMap 10.x Addin that was developed for Carl Roland
(Denali Plant Ecologist) to support creating ArcMap layers on the
fly from pick lists presented in a toolbar in the Addin.  The
items in the picklists, and the data for the layers is in a
file geodatabase on the GIS server on the NPS network.

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

Install the ArcObjects SDK (comes with ArcGIS Desktop 10.x) Open the solution in the version Visual Studio supported by your version of ArcGIS. Select Build Solution from the VS menu.

## Deploy

After building a release version, copy the file
`bin/release/DenaPlantPicker.esriAddin` to `X:\GIS\Addins\10.X`
where it will automatically be installed for all Alaska GIS users. Users without network access can get a copy from someone who does, and then double click the Addin file to launch the esri Addin install tool.

The tool requires a file geodatabase and layer files to be
deployed to a GIS data server available to all tool users.
The path to the database and layer files is hardcoded as
`X:\AKR\DENA\biologic\Plants` in
[`SpeciesLayerFactory.cs`](https://github.com/AKROGIS/PlantPickerAddin/blob/7f91919b9a6f47a1e44f55791d66bc3863f60f17/SpeciesLayerFactory.cs#L16).  The layer files are also in
the `Data` folder of this repo.  The file geodatabase is
created from a MS Access database maintained by Carl.  See
the folder `Scripts` for instructions and scripts for 
creating a new geodatabase if Carl provides an updated
plants database.

The database can be update independent of the Addin provided there
are no schema changes.  If schema changes are needed code changes
may be required.  The file
[`SpeciesLayerFactory.cs`](https://github.com/AKROGIS/PlantPickerAddin/blob/7f91919b9a6f47a1e44f55791d66bc3863f60f17/SpeciesLayerFactory.cs#L18-L20)
assumes a table named `taxonPicklist` with a field named
`Taxon_txtLocalAcceptedName`.  If this
changes in the database, the code will need to be updated and
a new version deployed.  Other database changes may require
changes to the layer files deployed with the database.  The
network location and layer and database filenames should not
need to be changed, code changes will be required if they are.

## Using

1) You need to be on the NPS network with your X drive
   mapped to the GIS data server.

   * If you need to use the tool off the NPS network, you will
     need a copy of the GIS network server on an external drive
     mounted as your X drive.  The external drive will need at
     least the `X:\GIS\Addins` and `X:\AKR\DENA\biologic\Plants`
     folders.

2) Open a new or existing ArcMap document.
3) Use the `Customize -> Toolbars` menu to select the
   `Denali Plant Species Selector` toolbar.

   * If the toolbar is not available, see step 1 and make sure
   that `X:\GIS\ADDINS\10.x` is in the addin folder list in the
   AddIn Manager.

4) Select a species or state rank from one of the picklist
   in the toolbar to add that layer to your map.
5) Adjust the layer properties as desired for aesthetic visual
   presentation.
6) Repeat.
