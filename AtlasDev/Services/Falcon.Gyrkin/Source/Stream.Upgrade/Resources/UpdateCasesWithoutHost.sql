
UPDATE "STR_Case" CSE
SET "HostId" = UP."HostId"
FROM (
	SELECT CSE."CaseId", ACC."HostId"
	FROM "STR_Case" CSE
	LEFT JOIN "STR_Account" ACC ON CSE."AccountId" = ACC."AccountId"
	WHERE CSE."HostId" IS NULL
	AND ACC."HostId" IS NOT NULL
	LIMIT 10000) UP
WHERE CSE."CaseId" = UP."CaseId";