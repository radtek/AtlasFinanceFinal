SELECT DISTINCT CL.recid
FROM company.client CL
LEFT JOIN company.loans LN ON CL.client = LN.client
	AND CL.brnum = LN.brnum 
WHERE CL.recid IN ({0})
	AND LN.loandate > CURRENT_DATE - INTERVAL '12 months'
	AND LN.status = 'H'