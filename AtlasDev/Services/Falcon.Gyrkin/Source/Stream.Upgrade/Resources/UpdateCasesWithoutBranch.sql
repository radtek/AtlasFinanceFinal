
UPDATE "STR_Case" CSE
SET "BranchId" = UP."BranchId"
FROM (
	SELECT CSE."CaseId", ACC."BranchId"
	FROM "STR_Case" CSE
	LEFT JOIN "STR_Account" ACC ON CSE."AccountId" = ACC."AccountId"
	WHERE CSE."BranchId" IS NULL
	AND ACC."BranchId" IS NOT NULL
	LIMIT 10000) UP
WHERE CSE."CaseId" = UP."CaseId";