SQL to create minimal picklist tables

SELECT DISTINCT ref_taxon.txtLocalAcceptedName INTO taxonPicklist
FROM ref_taxon LEFT JOIN tbl_vascular_plant_species_occurrences ON ref_taxon.species_code = tbl_vascular_plant_species_occurrences.species_code
WHERE (((tbl_vascular_plant_species_occurrences.species_code) Is Not Null))
ORDER BY ref_taxon.txtLocalAcceptedName;


SELECT DISTINCT ref_taxon.vasc_aknhp_s_rank INTO akhpRankPicklist
FROM ref_taxon LEFT JOIN tbl_vascular_plant_species_occurrences ON ref_taxon.species_code = tbl_vascular_plant_species_occurrences.species_code
WHERE (((tbl_vascular_plant_species_occurrences.species_code) Is Not Null))
ORDER BY ref_taxon.vasc_aknhp_s_rank;
