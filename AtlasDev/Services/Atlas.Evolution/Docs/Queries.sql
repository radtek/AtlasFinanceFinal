SELECT LN.client, LN.loan, LN.brnum as LegacyBranchNumber, LN.status,
	CASE 
		WHEN LN.status = 'H' THEN 'Handover'
		WHEN LN.status = 'E' THEN 'Early Settlement'
		WHEN LN.status = 'N' THEN 'Newly created loan'
		WHEN LN.status = 'C' THEN 'Cancelled'
		WHEN LN.status = 'W' THEN 'Write Off'
		WHEN LN.status = 'J' THEN 'Journalised'
		WHEN LN.status = 'D' THEN 'Discounted'
		WHEN LN.status = 'F' THEN 'Finished/Closed'
		WHEN LN.status = 'P' THEN 'Part Paid'
		ELSE 'Unknown'
	END AS statusdescription, LN.paidate as PaidDate, LN.recid as LoanReference
FROM company.loans LN
WHERE LN.recid IN ({0})
	AND Ln.outamnt <= 0
	
	

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
	LEFT JOIN company.loans HL ON CL.client = Hl.client
 		AND Cl.brnum = HL.brnum
		AND HL.status = 'H'
	WHERE SL.Rank = 1
		AND SL.enddate <= (CURRENT_DATE + INTERVAL '90 DAYS')
		AND CL.recid NOT IN ({1})
		AND TT.client IS NULL
		AND HL.loan IS NULL
		AND TP.client IS NOT NULL) SA
WHERE enddate <= 
	(CASE 
		WHEN LoanMethod = 'W' THEN (CURRENT_DATE + INTERVAL '14 DAYS')
		WHEN LoanMethod = 'B' THEN (CURRENT_DATE + INTERVAL '30 DAYS')
		ELSE (CURRENT_DATE + INTERVAL '90 DAYS')
	END)
	
	
Deceased:
================
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
	


	
Arrears:
==============
SELECT TR.brnum, TR.client, TR.loan, SUM(TR.tramount) AS "ArrearsValue"
FROM company.trans TR
INNER JOIN company.loans LN ON TR.loan = LN.loan
	AND TR.client = LN.client
	AND TR.brnum = LN.brnum
	 AND LN.nctrantype IN ('USE', 'N/A') --, 'SIMSALEC', 'VAP')  KB 2015-07-15: To match Ass arrears reports used by Norman, Celia says include all ?!?
WHERE 
	TR.trdate <= CURRENT_DATE
	AND TR.trtype = 'R' 
	AND NULLIF(TRIM(TR.trstat), '') IS NULL 
	AND LN.outamnt > 0	
GROUP BY TR.brnum, TR.client, TR.loan




SELECT LN.brnum AS "LegacyBranchNumber", 
	SUM(outamnt) AS "DebtorsBookValue"
FROM company.loans LN 
WHERE LN.brnum IN ('{0}')
GROUP BY brnum