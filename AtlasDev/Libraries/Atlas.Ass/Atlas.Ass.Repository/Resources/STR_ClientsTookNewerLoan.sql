SELECT DISTINCT LN.recid as loanreference, LN.oper as OperatorCode, CL.recid as clientreference, LN.client, LN.loan, LN.brnum as LegacyBranchNumber, LN.loandate, 
	LN.enddate, LN.cheque, LN.outamnt as OutstandingAmount, CL.surname, CL.firstname, CL.othname as OtherName, CL.title, CL.identno as IdentityNo, 
		CL.hometel, CL.worktel, CL.workfax, CL.spwrktel as SpouseWorkTel, CL.cell, CL.email_addr as Email, 
		CL.haddr1 as ResidentialAddress1, CL.haddr2 as ResidentialAddress2, CL.haddr3 as ResidentialAddress3, CL.hpostcode as ResidentialAddressPostalCode, 
		CL.waddr1 as WorkAddress1, CL.waddr2 as WorkAddress2, CL.waddr3 as WorkAddress3, CL.waddr4 as WorkAddress4, CL.waddr5 as WorkAddress5, CL.waddr6 as WorkAddress6, CL.wpostcode as WorkAddressPostalCode, 
		CL.emp_no as EmployerNo, CL.birthdate as DateOfBirth, LN.loanmeth as LoanMethod, LN.payno as LoanTerm
FROM (
	SELECT LN.recid, RANK() OVER (PARTITION BY LN.brnum, LN.client ORDER BY LN.enddate DESC) AS Rank
	FROM company.loans LN
	LEFT JOIN company.client CL ON LN.client = CL.client
		AND LN.brnum = CL.brnum
	INNER JOIN company.trans TR ON LN.loan = TR.loan 
		AND TR.brnum = LN.brnum 
		AND TR.client = LN.client 
		AND TR.order = 1
		AND TR.seqno = 1 
	WHERE LN.nctrantype IN ('USE', 'N/A') 
	AND CL.recid IN ({0})) NL
LEFT JOIN company.loans LN ON NL.recid = LN.recid
LEFT JOIN company.client CL ON LN.client = Cl.client
	AND LN.brnum = CL.brnum
WHERE NL.Rank = 1
	AND 
	LN.loandate > CASE 
		{1}
	END;