
	SELECT "GroupId", 
		"RegionId", 
		"BranchId", 
		"AllocatedUserId", 
		"CategoryId", 
		"SubCategoryId", 
		"CaseId", 
		"SMSCount" AS "SMSSent", 
		"NoActionCount" as "NoActions"
	FROM (
	SELECT DISTINCT CSE."GroupId", BRN."Region" AS "RegionId", BRN."BranchId", CAL."AllocatedUserId", SCT."CategoryId", SCT."SubCategoryId",CSE."CaseId", CST."CaseStreamId", 
		CAL."SMSCount", CAL."NoActionCount"
	FROM "STR_CaseStreamAction" CSA
	LEFT JOIN "STR_CaseStream" CST ON CSA."CaseStreamId" = CST."CaseStreamId"
	LEFT JOIN "STR_CaseStreamAllocation" CAL ON CST."CaseStreamId" = CAL."CaseStreamId"
	LEFT JOIN "STR_Case" CSE ON CST."CaseId" = CSE."CaseId"
	LEFT JOIN "STR_Account" ACC ON CSE."DebtorId" = ACC."DebtorId"
	LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
	LEFT JOIN "STR_SubCategory" SCT ON CSE."SubCategoryId" = SCT."SubCategoryId"
	WHERE (CSA."ActionDate" BETWEEN '{0}' AND '{1}'
		OR CST."CreateDate" BETWEEN '{0}' AND '{1}'
		OR CSE."CaseStatusId" = 1
		OR CSA."DateActioned" BETWEEN '{0}' AND '{1}')
		AND CSE."GroupId" = {2}
		AND BRN."BranchId" IN ({3})
		AND SCT."CategoryId" IN ({4})
		AND CAL."AllocatedUserId" IN ({5})
		AND (CAL."SMSCount" > 0 OR CAL."NoActionCount" > 0)
	) AL
	--GROUP BY "GroupId", "RegionId", "BranchId", "AllocatedUserId", "CategoryId", "SubCategoryId"