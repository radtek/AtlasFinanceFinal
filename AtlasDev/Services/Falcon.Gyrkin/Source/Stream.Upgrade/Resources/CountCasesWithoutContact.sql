
	SELECT COUNT(DC."DebtorContactId")
	FROM "STR_DebtorContact" DC
	LEFT JOIN "Contact" CT ON DC."ContactId" = CT."ContactId"
	WHERE DC."Value" IS NULL