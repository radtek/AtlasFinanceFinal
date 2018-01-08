SELECT LN.client, LN.loan, LN.brnum as LegacyBranchNumber, LN.status,
	CASE 
		WHEN LN.status = 'H' THEN 'Handover'
		WHEN LN.status = 'E' THEN 'Early Settlement'
		WHEN LN.status = 'N' THEN 'Newly created loan'
		WHEN LN.status = 'C' THEN 'Cancelled'
		WHEN LN.status = 'W' THEN 'Write Off'
		WHEN LN.status = 'J' THEN 'Journalised'
		WHEN LN.status = 'D' THEN 'Discounted'
		WHEN LN.status = 'F' THEN 'Finished/Closed'
		WHEN LN.status = 'P' THEN 'Part Paid'
		ELSE 'Unknown'
	END AS statusdescription, LN.paidate as PaidDate, LN.recid as LoanReference
FROM company.loans LN
WHERE LN.recid IN ({0})
	AND (Ln.outamnt <= 0 OR LN.outamnt IS NULL)