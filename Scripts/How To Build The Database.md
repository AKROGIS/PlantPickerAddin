# Scripts/How To Build The Database

Carl will maintain the master database in MS Access. When Carl
provides an updated database, do the following:

1) Open the DB in MS Access.
2) Run the queries in `code.sql` to create the picklist tables.
3) Export the database to Access 2003 format and call it
   `C:\tmp\Plants.mdb` (or edit 
   [`CreatePlantFGDB.py`](https://github.com/AKROGIS/PlantPickerAddin/blob/3c9567839d4f4df5d5a0cf1ec38c6937821bf076/Scripts/CreatePlantFGDB.py#L12)
   to match your preferred path/name.)
4) Run the `CreatePlantFGDB.py` script. If it fails:

   * Check the directory path in the script.
   * Check for database schema changes that would effect the table
     to table copies.
   * Check that an ArcGIS upgrade has not changed commands in the
     script.
   * The script can be recreated from the model in `DENA_Plants.tbx`.
     Check the input parameters in edit mode before running.

5) The FGDB plants.gdb should now be up to date.
6) Delete the plants.mdb
7) The unique values symbology in `Plant Species by State Rank.lyr`
   should be updated by using 'Add All Values' to capture any changes
   to the locally accepted name (`Taxon_txtLocalAcceptedName`).
8) Provide the updated layer file and `Plants.gdb` to the PDS Data
   Manager and request that she replace the files at
   `X:\AKR\DENA\biologic\Plants`  (or the directory path in
   [`SpeciesLayerFactory.cs`](https://github.com/AKROGIS/PlantPickerAddin/blob/7f91919b9a6f47a1e44f55791d66bc3863f60f17/SpeciesLayerFactory.cs#L16).
