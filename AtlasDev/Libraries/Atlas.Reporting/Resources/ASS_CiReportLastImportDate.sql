SELECT "BranchId", MIN("Date") AS "LastImportDate"
FROM (
	SELECT BR."BranchId", MAX(CI."Date") AS "Date"
	FROM "BRN_Branch" BR
	LEFT JOIN "ASS_CiReport" CI ON BR."BranchId" = CI."BranchId"
	GROUP BY BR."BranchId" 

	UNION 

	SELECT BR."BranchId", MAX(CS."Date")
	FROM "BRN_Branch" BR
	LEFT JOIN "ASS_CiReportScore" CS ON BR."BranchId"  = CS."BranchId"
	GROUP BY BR."BranchId" 

	UNION 

	SELECT BR."BranchId", MAX(PH."Date")
	FROM "BRN_Branch" BR
	LEFT JOIN "ASS_CiReportPossibleHandover" PH ON BR."BranchId" = PH."BranchId"
	GROUP BY BR."BranchId") CIR
GROUP BY "BranchId"