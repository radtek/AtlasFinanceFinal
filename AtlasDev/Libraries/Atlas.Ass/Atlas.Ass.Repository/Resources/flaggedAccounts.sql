SELECT TR.brnum AS "LegacyBranchNumber", 
	SUM(TR.tramount) AS "OverdueValue", 
	COUNT(DISTINCT (LN.client, LN.loan, LN.brnum)) AS "NoOfLoans" 
FROM company.loans LN 
INNER JOIN company.trans TR ON TR.loan  = LN.loan 
	AND TR.client = LN.client 
	AND TR.brnum = LN.brnum 
	AND TR.order >= 1 
	AND TR.seqno >= 1 
 	AND TR.trtype IN ('R')
	AND TR.trstat NOT IN ('P','E', 'T')
WHERE 
	LN.nctrantype IN ('USE', 'N/A', 'SIMSALEC') -- NUCARD, EFT OR CASH 
	AND LN.brnum IN ('{0}')
	AND NULLIF(TRIM(LN.claimcode), '') IS NOT NULL
	AND TR.trdate <= CURRENT_DATE
	AND LN.outamnt > 0
GROUP BY TR.brnum