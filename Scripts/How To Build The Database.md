Carl will maintain the master database in MS Access.
When Carl provides an updated database, do the following:
1) open the DB in MS Access
2) run the queries in code.sql to create the picklist tables
3) export the database to Access 2003 format and call it 
   C:\Users\resarwas\Documents\Visual Studio 2010\Projects\PlantPickerAddIn\Data\Plants.mdb
4) Run the CreatePlantFGDB.py script.  If it fails:
     a) Check the directory path in the script
	 b) Check for database schema changes that would effect the table to table copies 
	 c) Check that an ArcGIS upgrade has not changed the ground rules
	 d) The script can be recreated from the model in DENA_Plants.tbx
	    check the input parameters in edit mode before running.
5) The FGDB plants.gdb should now be up to date.
6) Delete the plants.mdb
7) The unique values symbology in Plant Species by State Rank.lyr should be
   updated by using 'Add All Values' to capture any changes to the
   locally accepted name (Taxon_txtLocalAcceptedName). 
8) Copy the updated layer file and Plants.gdb to T:\PDS\fgdb, and then 
   request that John P. move them to X:\Albers\parks\dena\base\biologic\statewid
   (or the directory path in SpeciesLayerFactory.cs)
9) If any source code was changed, rebuild the addin and deploy to X:\GIS\Addins\10.0
10) The tool is presented as a toolbar with several special picklists
