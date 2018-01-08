SELECT DISTINCT CL.recid
FROM company.client CL 
LEFT JOIN company.loans LN ON LN.client = CL.client
	AND LN.brnum = CL.brnum
LEFT JOIN company.trans TR ON LN.loan = TR.loan
	AND LN.client = TR.client
	AND LN.brnum = TR.brnum
	AND TR.trtype = 'W'
	AND TR.reason = '09'
WHERE (claimcode = 'D' OR 
	TR.trdate IS NOT NULL)
	AND CL.recid IN ({0});
	

