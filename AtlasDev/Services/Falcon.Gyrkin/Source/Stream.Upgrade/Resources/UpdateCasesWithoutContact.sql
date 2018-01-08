
UPDATE "STR_DebtorContact" DB
SET "Value" = UP."Value", "ContactTypeId" = UP."ContactTypeId", "IsActive" = UP."IsActive"
FROM (
	SELECT DC."DebtorContactId", CT."Value", CT."ContactTypeId", CT."IsActive"
	FROM "STR_DebtorContact" DC
	LEFT JOIN "Contact" CT ON DC."ContactId" = CT."ContactId"
	WHERE DC."Value" IS NULL
	LIMIT 10000) UP
WHERE DB."DebtorContactId" = UP."DebtorContactId";