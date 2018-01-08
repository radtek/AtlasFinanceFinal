SELECT *
FROM (
	SELECT DISTINCT LN.recid as loanreference, LN.oper as OperatorCode, CL.recid as clientreference, LN.client, LN.loan, LN.brnum as LegacyBranchNumber, LN.loandate, 
	SL.enddate, LN.cheque, LN.outamnt as OutstandingAmount, CL.surname, CL.firstname, CL.othname as OtherName, CL.title, CL.identno as IdentityNo, 
		CL.hometel, CL.worktel, CL.workfax, CL.spwrktel as SpouseWorkTel, CL.cell, CL.email_addr as Email, 
		CL.haddr1 as ResidentialAddress1, CL.haddr2 as ResidentialAddress2, CL.haddr3 as ResidentialAddress3, CL.hpostcode as ResidentialAddressPostalCode, 
		CL.waddr1 as WorkAddress1, CL.waddr2 as WorkAddress2, CL.waddr3 as WorkAddress3, CL.waddr4 as WorkAddress4, CL.waddr5 as WorkAddress5, CL.waddr6 as WorkAddress6, CL.wpostcode as WorkAddressPostalCode, 
		CL.emp_no as EmployerNo, CL.birthdate as DateOfBirth, LN.loanmeth as LoanMethod, LN.payno as LoanTerm
		-- CASE 
-- 			WHEN LN.loanmeth = 'W' THEN ceiling(LN.payno / 4) 
-- 			WHEN LN.loanmeth = 'B' THEN ceiling(LN.payno / 2) 
-- 			WHEN LN.loanmeth = 'M' THEN LN.payno 
-- 			ELSE 0 
-- 		END as LoanTerm
	FROM (
		SELECT LN.recid as loanreference, COALESCE(MAX(PD.trdate), LN.enddate) AS enddate, RANK() OVER (PARTITION BY LN.brnum, LN.client ORDER BY COALESCE(MAX(PD.trdate), LN.enddate) DESC) AS Rank
		FROM company.client CL 
		INNER JOIN company.loans LN ON CL.client = LN.client
			AND CL.brnum = LN.brnum
		LEFT JOIN company.payplanh PH ON LN.loan = PH.loan 
			AND LN.client = PH.client 
			AND LN.brnum = PH.brnum 
		LEFT JOIN company.payplanD PD ON PH.loan = PD.loan 
			AND PH.client = PD.client 
			AND PH.brnum = PD.brnum 
			AND PH.plan_num = PD.plan_num
		INNER JOIN company.trans TR ON LN.loan = TR.loan 
			AND TR.brnum = LN.brnum 
			AND TR.client = LN.client 
			AND TR.order = 1
			AND TR.seqno = 1 
		WHERE LN.nctrantype IN ('USE', 'N/A') 
			AND LN.status <> 'H'
			AND LN.enddate >= (CURRENT_DATE - INTERVAL '13 MONTHS') -- AND (CURRENT_DATE + INTERVAL '1 WEEK')
			AND LN.brnum IN ('{0}')
		GROUP BY LN.brnum, LN.client, LN.recid, LN.enddate) SL
	LEFT JOIN company.loans LN ON SL.loanreference = LN.recid
	INNER JOIN company.client CL ON LN.client = CL.client
		AND LN.brnum = CL.brnum
	LEFT JOIN company.trans TT ON LN.loan = TT.loan -- check to make sure they dont have unpaids
		AND LN.client = TT.client
		AND LN.brnum = TT.brnum
		AND TT.trdate <= CURRENT_DATE
		AND TT.trtype = 'R'
		AND NULLIF(TRIM(TT.trstat), '') IS NULL
	LEFT JOIN company.trans TP ON LN.loan = TP.loan -- check to make sure they had at least 1 payment
		AND LN.client = TP.client
		AND LN.brnum = TP.brnum
		AND TP.trdate <= CURRENT_DATE
		AND TP.trtype = 'R'
		AND TP.trstat = 'P'
	--LEFT JOIN company.loans HL ON CL.client = Hl.client  -- removed as per phillips and joggies request on 2016/07/15
	--	AND Cl.brnum = HL.brnum
	--	AND HL.status = 'H'
	WHERE SL.Rank = 1
		AND SL.enddate <= (CURRENT_DATE + INTERVAL '153 DAYS')
		AND CL.recid NOT IN ({1})
		AND TT.client IS NULL
		-- AND HL.loan IS NULL -- removed as per phillips and joggies request on 2016/07/15
		AND TP.client IS NOT NULL) SA
WHERE enddate <= 
	(CASE 
		WHEN LoanMethod = 'W' THEN (CURRENT_DATE + INTERVAL '365 DAYS')
		WHEN LoanMethod = 'B' THEN (CURRENT_DATE + INTERVAL '365 DAYS')
		ELSE (CURRENT_DATE + INTERVAL '365 DAYS')
	END) 