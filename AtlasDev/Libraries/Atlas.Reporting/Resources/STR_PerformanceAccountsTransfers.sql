
	SELECT CSE."GroupId", 
		BRN."Region" AS "RegionId", 
		BRN."BranchId", 
		CAL."AllocatedUserId", 
		SCT."CategoryId", 
		SCT."SubCategoryId", 
		CSE."CaseId",
		CASE 
			WHEN CAL."AllocatedDate" BETWEEN '{0}' AND '{1}' AND CAL."TransferredIn" = true THEN 1
			ELSE 0
		END AS "TransfersIn", 
		CASE 
			WHEN CAL."TransferredOutDate" BETWEEN '{0}' AND '{1}' AND CAL."TransferredOut" = true THEN 1
			ELSE 0
		END AS "TransfersOut"
	FROM "STR_CaseStreamAllocation" CAL
	LEFT JOIN "STR_CaseStream" CST ON CAL."CaseStreamId" = CST."CaseStreamId"
	LEFT JOIN "STR_Case" CSE ON CST."CaseId" = CSE."CaseId"
	LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
	LEFT JOIN "STR_SubCategory" SCT ON CSE."SubCategoryId" = SCT."SubCategoryId"
	WHERE (CAL."AllocatedDate" BETWEEN '{0}' AND '{1}'
		OR CAL."TransferredOutDate" BETWEEN '{0}' AND '{1}')
		AND (CAL."TransferredIn" = true 
			OR CAL."TransferredOut" = true)
		AND CSE."GroupId" = {2}
		AND BRN."BranchId" IN ({3})
		AND SCT."CategoryId" IN ({4})
		AND CAL."AllocatedUserId" IN ({5})