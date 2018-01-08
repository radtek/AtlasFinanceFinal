
WITH CTE_BankChange AS
(
	SELECT LN.loan, LN.client, LN.brnum, PH.bank as currentbank, PH.bankacc as currentbankacc, CB.bank as initialbank, CB.bankacc as initialbankacc
	FROM company.PayplanH PH
	LEFT JOIN company.loans LN ON PH.loan = LN.loan
		AND PH.client = LN.client
		AND PH.brnum = LN.brnum
	LEFT JOIN company.claud CB ON PH.client = CB.client
		AND PH.brnum = CB.brnum
		AND LN.userdate <= CB.date
		AND LN.usertime <= CB.au_time
		AND CB.chg IN ('B')
		AND NULLIF(TRIM(CB.bank), '') IS NOT NULL
	INNER JOIN company.claud CN ON PH.client = CN.client
		AND PH.brnum = CN.brnum
		AND CB.date = CN.date
		AND CB.au_time = CN.au_time
		AND CN.chg IN ('N')
		AND NULLIF(TRIM(CN.bank), '') IS NOT NULL
	WHERE PH.userdate BETWEEN '{0}' AND '{1}'
		AND PH.brnum IN ('{2}')
)

 SELECT brnum AS "LegacyBranchNumber", 
	CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END as "PayNo", 
	SUM(BankChange) AS "BankChange", 
	SUM(LoanTermChange) AS "LoanTermChange", 
	SUM(InstalmentChange) AS "InstalmentChange"
 FROM (
	SELECT PP.brnum,
		CASE 
			WHEN PP.loanmeth = 'W' THEN ceiling(CAST(PP.payno AS numeric) / 4) 
			WHEN PP.loanmeth = 'B' THEN ceiling(CAST(PP.payno AS numeric) / 2) 
			WHEN PP.loanmeth = 'M' THEN CAST(PP.payno AS numeric) 
			ELSE 0 
		END as payno,
		CASE 
			WHEN currentbank <> initialbank OR currentbankacc <> initialbankacc THEN 1
			ELSE 0
		END AS BankChange,
		CASE 
			WHEN currentbank <> initialbank OR currentbankacc <> initialbankacc THEN 0
			WHEN enddate < lasttrdate THEN 1
			ELSE 0
		END AS LoanTermChange,
		CASE 
			WHEN currentbank <> initialbank OR currentbankacc <> initialbankacc THEN 0
			WHEN enddate < lasttrdate THEN 0
			ELSE 1
		END AS InstalmentChange
	FROM (
		SELECT DISTINCT Ph.brnum, PH.client, PH.loan, PH.payno, PH.loanmeth, PH.instalamt, LN.enddate, LN.tramount, PD.trdate as lasttrdate, RANK() OVER (PARTITION BY Ph.brnum, PH.client, PH.loan ORDER BY PD.trdate DESC) AS Rank
		FROM company.PayplanH PH
		LEFT JOIN company.paypland PD ON PH.brnum = PD.brnum 
			AND PH.loan = PD.loan
			AND PH.client = PD.client
			AND PH.plan_num = PD.plan_num
		LEFT JOIN company.loans LN ON PH.loan = LN.loan
			AND PH.client = LN.client
			AND PH.brnum = LN.brnum
		WHERE PH.userdate BETWEEN '{0}' AND '{1}'
		  	AND PH.brnum IN ('{2}')
		) PP
	LEFT JOIN CTE_BankChange BC ON PP.brnum = BC.brnum 
		AND PP.client = BC.client
		AND PP.loan = BC.loan
	WHERE PP.Rank = 1
)RS
WHERE payno > 0
GROUP BY brnum, CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END