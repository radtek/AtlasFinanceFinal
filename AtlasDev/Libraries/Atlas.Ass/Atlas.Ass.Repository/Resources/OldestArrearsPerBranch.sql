SELECT LN.brnum, MIN(TR.trdate) AS OldestArrearDate
FROM company.loans LN 
INNER JOIN company.trans TR ON TR.loan  = LN.loan 
	AND TR.client = LN.client 
	AND TR.brnum = LN.brnum 
WHERE LN.brnum IN ('{0}')
  AND LN.nctrantype IN ('USE', 'N/A', 'VAP')
	AND TR.trdate <= current_date
	AND TR.trtype IN ('R')
	AND NULLIF(TRIM(TR.trstat), '') IS NULL 
GROUP BY LN.brnum