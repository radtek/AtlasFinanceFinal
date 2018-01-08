SELECT LN.brnum AS "LegacyBranchNumber", 
	SUM(outamnt) AS "DebtorsBookValue"
FROM company.loans LN 
WHERE LN.brnum IN ('{0}')
GROUP BY brnum