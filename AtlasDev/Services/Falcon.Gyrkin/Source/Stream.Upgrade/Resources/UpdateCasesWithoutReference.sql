UPDATE "STR_Case" CSE
SET "Reference" = UP."CaseReference"
FROM (
	SELECT CSE."CaseId", CSE."Reference", BRN."LegacyBranchNum", DBT."Reference", BRN."LegacyBranchNum" || 'X' || DBT."Reference" AS "CaseReference"
	FROM "STR_Case" CSE
	LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
	LEFT JOIN "STR_Debtor" DBT ON CSE."DebtorId" = DBT."DebtorId"
	WHERE CSE."Reference" IS NULL
	ORDER BY 1 desc
	LIMIT 1000) UP
WHERE CSE."CaseId" = UP."CaseId";

