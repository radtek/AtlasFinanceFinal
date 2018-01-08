UPDATE "STR_Case" CSE
SET "DebtorId" = UP."DebtorId"
FROM (
	SELECT CSE."CaseId", ACC."DebtorId"
	FROM "STR_Case" CSE
	LEFT JOIN "STR_Account" ACC ON CSE."AccountId" = ACC."AccountId"
	WHERE CSE."DebtorId" IS NULL
	AND ACC."DebtorId" IS NOT NULL
	LIMIT 10000) UP
WHERE CSE."CaseId" = UP."CaseId";