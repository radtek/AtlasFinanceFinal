WITH CTE_Total AS 
(
	SELECT {3}, 
		0 AS "PayNo", 
		CASE WHEN SUM("Cheque") = 0 
			THEN 0 
			ELSE SUM("ChargesExclVAT") / SUM("Cheque") 
		END AS "ChargesPercent",
		CASE WHEN SUM("Cheque") = 0 
			THEN 0 
			ELSE SUM("CreditLife") / SUM("Cheque") 
		END AS "CreditLifePercent",
		CASE WHEN SUM("Cheque") = 0 
			THEN 0 
			ELSE SUM("LoanFeeExclVAT") / SUM("Cheque") 
		END AS "LoanFeePercent",
		CASE WHEN SUM("Cheque") = 0 
			THEN 0 
			ELSE SUM("TotFeeExcl") / SUM("Cheque") 
		END AS "TotFeePercent",
		CASE WHEN SUM("Cheque") = 0 
			THEN 0 
			ELSE SUM("FuneralAddOn" + "AgeAddOn" + "VAPExcl") / SUM("Cheque") 
		END AS "AddOnPercent",
		SUM("Cheque") AS "Cheque", 		
		SUM("ChargesExclVAT") AS "ChargesExclVAT", 
		SUM("ChargesVAT") AS "ChargesVAT", 
		SUM("TotalCharges") AS "TotalCharges", 
		SUM("CreditLife") AS "CreditLife", 
		SUM("LoanFeeExclVAT") AS "LoanFeeExclVAT", 
		SUM("LoanFeeVAT") AS "LoanFeeVAT", 
		SUM("LoanFeeInclVAT") AS "LoanFeeInclVAT", 
		SUM("FuneralAddOn") AS "FuneralAddOn", 
		SUM("AgeAddOn") AS "AgeAddOn", 
		SUM("VAPExcl") AS "VAPExcl", 
		SUM("VAPVAT") AS "VAPVAT", 
		SUM("VAPIncl") AS "VAPIncl", 
		SUM("TotalAddOn") AS "TotalAddOn", 
		SUM("TotFeeExcl") AS "TotFeeExcl", 
		SUM("TotFeeVAT") AS "TotFeeVAT", 
		SUM("TotFeeIncl") AS "TotFeeIncl",
		TEXT('M') AS "LoanMeth", TEXT('') AS "QtySfee",
		SUM(HI."HandedOverLoansQuantity") AS "HandedOverLoansQuantity", 
		SUM(HI."HandedOverClientQuantity") AS "HandedOverClientQuantity", 
		SUM(HI."HandedOverLoansAmount") AS "HandedOverLoansAmount",
		CASE WHEN SUM(CI."Cheque" + CI."Fees" + CI."Insurance") = 0 
			THEN 0
			ELSE SUM(HI."HandedOverLoansAmount") / SUM(CI."Cheque" + CI."Fees" + CI."Insurance")
		END AS "HandedOverLoanPercent",
		SUM("NoOfLoans") AS "Loans", 1 AS "LoanMix",
		SUM("VapLinkedLoansValue") AS "VapValueLinked", SUM("VapLinkedLoans") AS "VapLinked", SUM("VapDeniedByConWithAuth") AS "VapDeniedByConWithAuth", 
		SUM("VapDeniedByConWithOutAuth") AS "VapDeniedByConWithOutAuth", SUM("VapExcludedLoans") AS "VapExcludedLoans", 
		SUM("Collections") as "Collections", SUM("RollbackValue") as "RolledValue", 
		CASE WHEN SUM(CI."Collections") = 0
			THEN 0
			ELSE SUM(CI."RollbackValue") / SUM(CI."Collections") 
		END AS "RolledPercent", 
		SUM("Refunds") AS "Refunds", SUM("NewClientAmount") AS "NewClientAmount", SUM(CI."NewClientNoOfLoans") AS "NewClientNoOfLoans", 
		SUM("BranchLoans") as ClientsBranch, SUM("SalesRepLoans") as SalesRepLoans, SUM("ExistingClientCount") as CurrentClient, SUM("RevivedClientCount") as RevivedClient, 
		1 AS "NewClientMix", 
		CASE WHEN SUM(CI."NoOfLoans") = 0 
			THEN 0
			ELSE SUM(CI."Cheque") / SUM(CI."NoOfLoans")
		END AS "AverageLoan", 
		CASE WHEN SUM(CI."NewClientNoOfLoans") = 0 
			THEN 0
			ELSE SUM(CI."NewClientAmount") / SUM(CI."NewClientNoOfLoans")
		END AS "NewClientAverageLoan", 
		CASE WHEN SUM("RevivedClientCount") = 0 
			THEN 0
			ELSE SUM(CI."RevivedClientAmount") / SUM("RevivedClientCount")
		END AS "AverageRevivedClientAmount", 
		SUM("ReswipeBankChange") as ReswipeBankChange, SUM("ReswipeLoanTermChange") as ReswipeLoanTermChange, SUM("ReswipeInstalmentChange") as ReswipeInstalmentChange
	FROM "ASS_CiReport" CI
	LEFT JOIN "BRN_Branch" BR ON CI."BranchId" = BR."BranchId"
	LEFT JOIN "CPY_Company" CP ON BR."Company" = CP."CompanyId"
	LEFT JOIN "Region" RG ON BR."Region" = RG."RegionId"
	LEFT JOIN "ASS_CiReportHandoverInfo" HI ON CI."BranchId" = HI."BranchId"
		AND CI."Date" = HI."Date"
		AND CI."PayNo" = HI."PayNo"
	WHERE CI."Date" BETWEEN '{0}' AND '{1}'
		AND CI."BranchId" IN ({2}) 
	GROUP BY 1, 2, 3--, 4, 5
)

SELECT {3}, 
	CI."PayNo", 
	CASE WHEN SUM(CI."Cheque") = 0 
		THEN 0 
		ELSE SUM(CI."ChargesExclVAT") / SUM(CI."Cheque") 
	END AS "ChargesPercent",
	CASE WHEN SUM(CI."Cheque") = 0 
		THEN 0 
		ELSE SUM(CI."CreditLife") / SUM(CI."Cheque") 
	END AS "CreditLifePercent",
	CASE WHEN SUM(CI."Cheque") = 0 
		THEN 0 
		ELSE SUM(CI."LoanFeeExclVAT") / SUM(CI."Cheque") 
	END AS "LoanFeePercent",
	CASE WHEN SUM(CI."Cheque") = 0 
		THEN 0 
		ELSE SUM(CI."TotFeeExcl") / SUM(CI."Cheque") 
	END AS "TotFeePercent",
	CASE WHEN SUM(CI."Cheque") = 0 
		THEN 0 
		ELSE SUM(CI."FuneralAddOn" + CI."AgeAddOn" + CI."VAPExcl") / SUM(CI."Cheque") 
	END AS "AddOnPercent",
	SUM(CI."Cheque") AS Cheque, 
	SUM(CI."ChargesExclVAT") AS "ChargesExclVAT", 
	SUM(CI."ChargesVAT") AS "ChargesVAT", 
	SUM(CI."TotalCharges") AS "TotalCharges", 
	SUM(CI."CreditLife") AS "CreditLife", 
	SUM(CI."LoanFeeExclVAT") AS "LoanFeeExclVAT", 
	SUM(CI."LoanFeeVAT") AS "LoanFeeVAT", 
	SUM(CI."LoanFeeInclVAT") AS "LoanFeeInclVAT", 
	SUM(CI."FuneralAddOn") AS "FuneralAddOn", 
	SUM(CI."AgeAddOn") AS "AgeAddOn", 
	SUM(CI."VAPExcl") AS "VAPExcl", 
	SUM(CI."VAPVAT") AS "VAPVAT", 
	SUM(CI."VAPIncl") AS "VAPIncl", 
	SUM(CI."TotalAddOn") AS "TotalAddOn", 
	SUM(CI."TotFeeExcl") AS "TotFeeExcl", 
	SUM(CI."TotFeeVAT") AS "TotFeeVAT", 
	SUM(CI."TotFeeIncl") AS "TotFeeIncl",
	'M' AS "LoanMeth", '' AS "QtySfee",
	SUM(HI."HandedOverLoansQuantity") AS "HandedOverLoansQuantity", 
	SUM(HI."HandedOverClientQuantity") AS "HandedOverClientQuantity", 
	SUM(HI."HandedOverLoansAmount") AS "HandedOverLoansAmount",
	CASE WHEN SUM(CI."Cheque" + CI."Fees" + CI."Insurance") = 0 
		THEN 0
		ELSE SUM(HI."HandedOverLoansAmount") / SUM(CI."Cheque" + CI."Fees" + CI."Insurance")
	END AS "HandedOverLoanPercent",
	SUM(CI."NoOfLoans") AS Loans, 
	CASE WHEN AVG(TT."Loans") = 0 
		THEN 0
		ELSE SUM(CI."NoOfLoans") / AVG(TT."Loans")
	END AS "ActualPercent", 
	CASE WHEN SUM(TT."Loans") = 0 
		THEN 0
		ELSE SUM(CI."NoOfLoans") / AVG(TT."Loans")
	END AS "LoanMix", 
	SUM(CI."VapLinkedLoansValue") AS VapValueLinked, SUM(CI."VapLinkedLoans") AS VapLinked, SUM(CI."VapDeniedByConWithAuth") AS VapDeniedByConWithAuth, 
	SUM(CI."VapDeniedByConWithOutAuth") AS VapDeniedByConWithOutAuth, SUM(CI."VapExcludedLoans") AS VapExcludedLoans, 
	SUM(CI."Collections") AS "Collections", SUM(CI."RollbackValue") AS RolledValue,
	CASE WHEN SUM(CI."Collections") = 0
		THEN 0
		ELSE SUM(CI."RollbackValue") / SUM(CI."Collections") 
	END AS "RolledPercent", 
	SUM(CI."Refunds") AS Refunds, SUM(CI."NewClientAmount") AS "NewClientAmount", SUM(CI."NewClientNoOfLoans") AS "NewClientNoOfLoans", 
	SUM("BranchLoans") as ClientsBranch, SUM("SalesRepLoans") as SalesRepLoans, SUM("ExistingClientCount") as CurrentClient, SUM("RevivedClientCount") as RevivedClient, 
	(CASE WHEN SUM(TT."NewClientNoOfLoans") = 0 
		THEN 0
		ELSE SUM(CI."NewClientNoOfLoans") / SUM(TT."NewClientNoOfLoans")
	END) * 100 AS "NewClientMix", 
	CASE WHEN SUM(CI."NoOfLoans") = 0 
		THEN 0
		ELSE SUM(CI."Cheque") / SUM(CI."NoOfLoans")
	END AS "AverageLoan",  
	CASE WHEN SUM(CI."NewClientNoOfLoans") = 0 
		THEN 0
		ELSE SUM(CI."NewClientAmount") / SUM(CI."NewClientNoOfLoans")
	END AS "NewClientAverageLoan", 
	CASE WHEN SUM(CI."RevivedClientCount") = 0 
		THEN 0
		ELSE SUM(CI."RevivedClientAmount") / SUM(CI."RevivedClientCount")
	END AS "AverageRevivedClientAmount", 
	SUM("ReswipeBankChange") as ReswipeBankChange, SUM("ReswipeLoanTermChange") as ReswipeLoanTermChange, SUM("ReswipeInstalmentChange") as ReswipeInstalmentChange,
	SUM(PR."OneMonth") AS "OneMonth",
	SUM(PR."OneMonthThin") AS "OneMonthThin",
	SUM(PR."OneMonthCapped") AS "OneMonthCapped",
	SUM(PR."TwoToFourMonth") AS "TwoToFourMonth",
	SUM(PR."FiveToSixMonth") AS "FiveToSixMonth",
	SUM(PR."TwelveMonth") AS "TwelveMonth",
	SUM(PR."Declined") AS "Declined",
	SUM(PR."OneMonth") + SUM(PR."OneMonthThin") + SUM(PR."OneMonthCapped") + SUM(PR."TwoToFourMonth") + SUM(PR."FiveToSixMonth") + SUM(PR."TwelveMonth") + SUM(PR."Declined") AS "TotalProducts"
FROM "ASS_CiReport" CI
LEFT JOIN "BRN_Branch" BR ON CI."BranchId" = BR."BranchId"
LEFT JOIN "CPY_Company" CP ON BR."Company" = CP."CompanyId"
LEFT JOIN "Region" RG ON BR."Region" = RG."RegionId"
LEFT JOIN CTE_Total TT ON BR."{4}" = TT."Id" -- Region or Branch, depending on grouping
LEFT JOIN "ASS_CiReportHandoverInfo" HI ON CI."BranchId" = HI."BranchId"
	AND CI."Date" = HI."Date"
	AND CI."PayNo" = HI."PayNo"
LEFT JOIN "ASS_CiReportCompuscanProduct" PR ON BR."BranchId" = PR."BranchId" 
	AND PR."Date" = CI."Date"
	AND CI."PayNo" = 1
WHERE CI."Date" BETWEEN '{0}' AND '{1}'
 	AND CI."BranchId" IN ({2}) 
GROUP BY 1, 2, 3 

UNION 

SELECT 	"Id", 
	"Name", 
	"PayNo", 
	"ChargesPercent",
	"CreditLifePercent",
	"LoanFeePercent",
	"TotFeePercent",
	"AddOnPercent",
 	"Cheque", 
	"ChargesExclVAT", 
	"ChargesVAT", 
	"TotalCharges", 
	"CreditLife", 
	"LoanFeeExclVAT", 
	"LoanFeeVAT", 
	"LoanFeeInclVAT", 
	"FuneralAddOn", 
	"AgeAddOn", 
	"VAPExcl", 
	"VAPVAT", 
	"VAPIncl", 
	"TotalAddOn", 
	"TotFeeExcl", 
	"TotFeeVAT", 
	"TotFeeIncl",
 	"LoanMeth", "QtySfee",
 	"HandedOverLoansQuantity", 
 	"HandedOverClientQuantity", 
 	"HandedOverLoansAmount", 
 	"HandedOverLoanPercent",
 	"Loans", 
	"LoanMix" AS "ActualPercent", 
	"LoanMix", 
	"VapValueLinked", "VapLinked", "VapDeniedByConWithAuth", 
	"VapDeniedByConWithOutAuth", "VapExcludedLoans", "Collections", "RolledValue", "RolledPercent", 
	"Refunds", "NewClientAmount", "NewClientNoOfLoans", 
	ClientsBranch, SalesRepLoans, CurrentClient, RevivedClient, 
	"NewClientMix", "AverageLoan", "NewClientAverageLoan","AverageRevivedClientAmount",
	ReswipeBankChange, ReswipeLoanTermChange, ReswipeInstalmentChange,
	0 AS "OneMonth",
	0 AS "OneMonthThin",
	0 AS "OneMonthCapped",
	0 AS "TwoToFourMonth",
	0 AS "FiveToSixMonth",
	0 AS "TwelveMonth",
	0 AS "Declined",
	0 AS "TotalProducts"
FROM CTE_Total