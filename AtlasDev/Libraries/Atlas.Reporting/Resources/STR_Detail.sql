SELECT 
	CSE."CaseId" AS "CaseNo",
	STA."Description" AS "CaseStatus",
	STR."Description" AS "Stream",
	CTG."Description" || ' - ' || SBC."Description" AS "Category",
	DB."IdNumber", DB."FirstName", DB."LastName"
FROM "STR_Case" CSE
INNER JOIN "STR_CaseStream" CST ON CSE."CaseId" = CST."CaseId"
	AND CST."CompleteDate" IS NULL
LEFT JOIN "STR_Debtor" DB ON CSE."DebtorId" = DB."DebtorId"
LEFT JOIN "STR_CaseStatus" STA ON CSE."CaseStatusId" = STA."CaseStatusId"
LEFT JOIN "STR_SubCategory" SBC ON CSE."SubCategoryId" = SBC."SubCategoryId"
LEFT JOIN "STR_Category" CTG ON SBC."CategoryId" = CTG."CategoryId"
LEFT JOIN "STR_Stream" STR ON CST."StreamId" = STR."StreamId"
WHERE CSE."CaseStatusId" IN ({0})
AND CSE."GroupId" IN ({1})
AND CSE."BranchId" IN ({2})
AND CST."StreamId" IN ({3})

