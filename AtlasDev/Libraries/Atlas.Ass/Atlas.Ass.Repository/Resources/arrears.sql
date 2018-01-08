	SELECT TR.brnum AS "LegacyBranchNumber", 
		SUM(TR.tramount) AS "ArrearsValue"
	FROM company.trans TR
	INNER JOIN company.loans LN ON TR.loan = LN.loan
		AND TR.client = LN.client
		AND TR.brnum = LN.brnum
		-- AND LN.nctrantype IN ('USE', 'N/A', 'SIMSALEC', 'VAP')  KB 2015-07-15: To match Ass arrears reports used by Norman, Celia says include all ?!?
	WHERE 
		TR.trdate <= CURRENT_DATE
		AND TR.trtype = 'R' 
		AND NULLIF(TRIM(TR.trstat), '') IS NULL 
		-- AND LN.outamnt > 0                                      KB 2015-07-15: To match Ass arrears reports used by Norman, Celia says include all ?!?
		AND TR.brnum IN ('{0}') 
	GROUP BY TR.brnum 