SELECT TR.brnum AS "LegacyBranchNumber",
	CASE 
		WHEN loanmeth = 'W' THEN ceiling(payno / 4) 
		WHEN loanmeth = 'B' THEN ceiling(payno / 2) 
		WHEN loanmeth = 'M' THEN payno 
		ELSE 0 
	END as "PayNo", 
	SUM(TR.tramount) AS "RollbackValue"
FROM company.loans LN 
INNER JOIN company.trans TR ON TR.loan  = LN.loan 
	AND TR.client = LN.client 
	AND TR.brnum = LN.brnum 
	AND TR.order >= 1 
	AND TR.seqno >= 1 
	AND TR.glbank = 'ROL'
WHERE LN.nctrantype IN ('USE', 'N/A') -- NUCARD, EFT OR CASH 
	AND LN.brnum IN ('{0}')
	AND TR.trdate BETWEEN '{1}' AND '{2}'
GROUP BY TR.brnum, 
	CASE 
		WHEN loanmeth = 'W' THEN ceiling(payno / 4) 
		WHEN loanmeth = 'B' THEN ceiling(payno / 2) 
		WHEN loanmeth = 'M' THEN payno 
		ELSE 0 
	END