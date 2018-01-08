
WITH CTE_VAP AS
(
	SELECT brnum, CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END AS payno, SUM(cheque) AS VapLinkedLoansValue, COUNT(*) AS quantity
	FROM (
	SELECT LN.brnum,
		CASE 
			WHEN LN.loanmeth = 'W' THEN ceiling(LN.payno / 4) 
			WHEN LN.loanmeth = 'B' THEN ceiling(LN.payno / 2) 
			WHEN LN.loanmeth = 'M' THEN LN.payno 
		ELSE 0 END as payno, LN.cheque
	FROM company.loans LN
	INNER JOIN company.trans TR ON LN.loan = TR.loan
		AND LN.brnum = TR.brnum
		AND LN.client = TR.client
		AND TR.order = 1
		AND TR.seqno = 1
	WHERE LN.nctrantype = 'VAP'
		AND LN.brnum IN ('{0}')
		AND LN.loandate BETWEEN '{1}' AND '{2}') VP
	GROUP BY brnum, CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END
)


SELECT VS.brnum AS "LegacyBranchNumber", 
	VS.payno AS "PayNo", 
	VS.VapLinkedLoans AS "VapLinkedLoans", 
	VS.VapDeniedByConWithAuth AS "VapDeniedByConWithAuth", 
	VS.VapDeniedByConWithOutAuth AS "VapDeniedByConWithOutAuth", 
	VS.VapExcludedLoans AS "VapExcludedLoans", 
	CP.VapLinkedLoansValue AS "VapLinkedLoansValue"
FROM (
	SELECT VP.brnum, CASE WHEN VP.payno > 6 AND VP.payno < 12 THEN 6 ELSE VP.payno END AS payno, SUM(VapLinkedLoans) AS VapLinkedLoans, SUM(VapDeniedByConWithAuth) AS VapDeniedByConWithAuth, 
		SUM(VapDeniedByConWithOutAuth) AS VapDeniedByConWithOutAuth, SUM(VapExcludedLoans) AS VapExcludedLoans
	FROM (
		SELECT brnum, payno, 
		CASE 
			WHEN VapOption = 1 THEN 1 
			ELSE 0
		END AS VapLinkedLoans,
		CASE 
			WHEN VapOption = 2 THEN 1 
			ELSE 0
		END AS VapDeniedByConWithAuth,
		CASE 
			WHEN VapOption = 3 THEN 1 
			ELSE 0
		END AS VapDeniedByConWithOutAuth,
		CASE 
			WHEN VapOption = 4 THEN 1 
			ELSE 0
		END AS VapExcludedLoans, RANK() OVER (PARTITION BY OL.brnum, OL.client, OL.loan ORDER BY VapOption) AS rank
		FROM (
			SELECT LN.brnum, LN.client, LN.loan,  
				CASE 
					WHEN LN.loanmeth = 'W' THEN ceiling(LN.payno / 4) 
					WHEN LN.loanmeth = 'B' THEN ceiling(LN.payno / 2) 
					WHEN LN.loanmeth = 'M' THEN LN.payno 
				ELSE 0 END as payno, 
				CASE 
					WHEN LN.mths_cover > 0 THEN 1
					WHEN NULLIF(LN.mths_cover, 0) IS NULL AND AU.detail = 'No VAP' THEN 2
					WHEN NULLIF(LN.mths_cover, 0) IS NULL AND AU.detail = 'No VAP, No Auth' THEN 3
					ELSE 4
				END AS VapOption
			FROM company.loans LN
			INNER JOIN company.trans TR ON LN.loan = TR.loan
				AND LN.brnum = TR.brnum
				AND LN.client = TR.client
				AND TR.order = 1
				AND TR.seqno = 1
			LEFT JOIN company.authorit AU ON LN.loan = AU.loan
				AND LN.brnum = AU.brnum
				AND LN.client = AU.client
			WHERE LN.nctrantype IN ('USE', 'N/A')
				AND LN.brnum IN ('{0}')
				AND LN.loandate BETWEEN '{1}' AND '{2}'
				) OL)VP
	WHERE rank = 1
	GROUP BY VP.brnum, CASE WHEN VP.payno > 6 AND VP.payno < 12 THEN 6 ELSE VP.payno END) VS
LEFT JOIN CTE_VAP CP ON VS.brnum = CP.brnum
	AND CP.payno = VS.payno