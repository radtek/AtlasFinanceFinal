	SELECT DISTINCT LN.recid as loanreference, LN.oper as OperatorCode, CL.recid as clientreference, LN.client, LN.loan, LN.brnum as LegacyBranchNumber, LN.loandate, LN.enddate, LN.cheque, LN.outamnt as OutstandingAmount, 
		CL.surname, CL.firstname, CL.othname as OtherName, CL.title, CL.identno as IdentityNo, CL.hometel, CL.worktel, CL.workfax, CL.spwrktel as SpouseWorkTel, CL.cell, CL.email_addr as Email, 
		CL.haddr1 as ResidentialAddress1, CL.haddr2 as ResidentialAddress2, CL.haddr3 as ResidentialAddress3, CL.hpostcode as ResidentialAddressPostalCode, 
		CL.waddr1 as WorkAddress1, CL.waddr2 as WorkAddress2, CL.waddr3 as WorkAddress3, CL.waddr4 as WorkAddress4, CL.waddr5 as WorkAddress5, CL.waddr6 as WorkAddress6, CL.wpostcode as WorkAddressPostalCode, 
		CL.emp_no as EmployerNo, CL.birthdate as DateOfBirth, LN.loanmeth as LoanMethod,
		CASE 
			WHEN LN.loanmeth = 'W' THEN ceiling(LN.payno / 4) 
			WHEN LN.loanmeth = 'B' THEN ceiling(LN.payno / 2) 
			WHEN LN.loanmeth = 'M' THEN LN.payno 
			ELSE 0 
		END as LoanTerm
	FROM company.loans LN
	INNER JOIN company.client CL ON LN.client = CL.client
		AND LN.brnum = CL.brnum
	INNER JOIN company.trans TR ON LN.loan = TR.loan 
		AND LN.client = TR.client
		AND LN.brnum = TR.brnum
		AND TR.trtype = 'R' 
		AND NULLIF(TR.trstat, '') IS NULL 
		AND TR.tramount IS NOT NULL
	WHERE TR.trdate <= (CURRENT_DATE - INTERVAL '1 DAYS')
		AND TRIM(LN.nctrantype) IN ('USE', 'N/A', 'SIMSALEC', 'VAP')
		AND LN.recid NOT IN ({0})
		--AND LN.brnum IN ('60', 'K7', 'C6', '33', '42', '50', '80', 'F9', '57', 'L8', 'H9', 'J9', '13', '69', '04', '97', '54', '94', 'D3', '18', '46', '66', '03', 'G4', '45', 'G8', 'J1', 'K6',
	 --           'L1', 'K3', '07', '85', '96', 'C1', 'C7', '59', 'F7', '25', '43', '68', '74', 'A7', '02', '08', 'I2', 'I6', 'I8', 'G5', 'G7', '88', 'F1', '14', '48',
	 --           'K8', 'D8', 'E3', '10', '30', '52', '64', '86', 'D9', '34', '44', '51', 'F5', '23', '27', 'H7', 'I4', 'J4', 'J5', 'J7', 'K1', 'K2', 'K5', 'D5', 'H3', 'I3', '47',
	 --           'D7', 'H4', 'F3', 'A5', '55', 'E6', '01', '41', '56', 'E5', 'I1', 'K6', '82', 'A8',
	 --           'G1', 'J2', '19', 'F6', '95', '71', '78', '15', '77', 'E2', 'H8', '61',
	 --           'D7', '58', '67', 
	 --           '26', 'E9', '62', 'D4', '90', '91', 'H1', 'L2', '35', 'A2', '87',
	 --           '22', '65', 'L3',
	 --           'A4', '93', 'A9', 'F8', '28', 'E1', 'L5', 'L7', 'C3', '16', '98', 'H2', '81', 'K9', 'L6', '05', '11', 'J6', 'L4',
		--		'32', '38', '63', 'C4', 'E8', 'K4', 'H5',
	 --           '40', '29', '11', 'I9', 'J3', 'L9',
		--		'D6', 'I7', 'D1',
		--		'M1'
		--)	
		AND LN.outamnt > 0