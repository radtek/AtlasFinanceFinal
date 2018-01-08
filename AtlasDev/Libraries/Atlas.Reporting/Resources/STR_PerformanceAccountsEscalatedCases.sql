
	SELECT  "GroupId", 
		"RegionId", 
		"BranchId", 
		"AllocatedUserId", 
		"CategoryId", 
		"SubCategoryId", 
		"CaseId"
		--COUNT(*) AS "EscalatedCases"
	FROM (
		SELECT CSE."GroupId", 
			BRN."Region" AS "RegionId", 
			BRN."BranchId", 
			CAL."AllocatedUserId", 
			SCT."CategoryId", 
			SCT."SubCategoryId", 
			CSE."CaseId",
			RANK() OVER (PARTITION BY CAL."AllocatedUserId", CAL."CaseStreamId" ORDER BY CES."CreateDate") AS Rank
		FROM "STR_CaseStreamAllocation" CAL
		INNER JOIN "STR_CaseStreamEscalation" CES ON CAL."CaseStreamId" = CES."CaseStreamId"
			AND CES."EscalationId" > CAL."EscalationId"
		LEFT JOIN "STR_CaseStream" CST ON CAL."CaseStreamId" = CST."CaseStreamId"
		LEFT JOIN "STR_Case" CSE ON CST."CaseId" = CSE."CaseId"
		LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
		LEFT JOIN "STR_SubCategory" SCT ON CSE."SubCategoryId" = SCT."SubCategoryId"
		WHERE CES."CreateDate" BETWEEN '{0}' AND '{1}'
			AND (CAL."TransferredOut" = false
				OR (CAL."TransferredOut" = true 
					AND CAL."TransferredOutDate" > CES."CreateDate"))
			AND CSE."GroupId" = {2}
			AND BRN."BranchId" IN ({3})
			AND SCT."CategoryId" IN ({4})
			AND CAL."AllocatedUserId" IN ({5})) ESC
	WHERE Rank = 1
	--GROUP BY "GroupId", "RegionId", "BranchId", "AllocatedUserId", "CategoryId", "SubCategoryId" 