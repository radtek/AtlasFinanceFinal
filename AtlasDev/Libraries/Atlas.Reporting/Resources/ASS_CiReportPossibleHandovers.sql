SELECT {2}, 
	SUM(CI."PossibleHandOvers") AS "PossibleHandovers",
	SUM(TG."HandoverBudget") AS "HandoverBudget", 
	SUM(CI."PossibleHandOvers") - SUM(TG."HandoverBudget") AS "Variance", 
	CASE WHEN SUM(TG."HandoverBudget") = 0
		THEN 0
		ELSE SUM(CI."PossibleHandOvers") / SUM(TG."HandoverBudget")
	END AS "ActualVsBudgetPercent", 
	SUM(CI."NextPossibleHandOvers") AS "NextPossibleHandovers",
	SUM(CI."NextPossibleHandOvers") - SUM(TG."HandoverBudget") AS "NextMonthBreakEven",
	SUM(CI."DebtorsBookValue") AS "DebtorsBookValue", 
	SUM(CI."Arrears") AS "ActualArrears", 
	AVG(TG."ArrearTarget") AS "ArrearTargetPercent",
	SUM(CI."DebtorsBookValue") * AVG(TG."ArrearTarget") AS "ArrearTarget",
	CASE WHEN SUM(CI."DebtorsBookValue") = 0
		THEN 0
		ELSE SUM(CI."Arrears") / SUM(CI."DebtorsBookValue") 
	END AS "ArrearsToDebtorsBookPercent",
	CASE WHEN SUM(CI."ReceivableThisMonth") = 0
		THEN 0
		ELSE SUM(CI."ReceivedThisMonth") / SUM(CI."ReceivableThisMonth") 
	END AS "CollectionsThisMonthPercent",
	CASE WHEN SUM(CI."ReceivablePast") = 0
		THEN 0
		ELSE SUM(CI."ReceivedPast") / SUM(CI."ReceivablePast") 
	END AS "CollectionsPastPercent",
	SUM(CI."FlaggedNoOfLoans") AS "FlaggedNoOfLoans",
	SUM(CI."FlaggedOverdueValue") AS "FlaggedOverdueValue",
	MIN("OldestArrearsDate") AS "OldestArrearsDate"	
FROM "ASS_CiReportPossibleHandover" CI
LEFT JOIN "BRN_Branch" BR ON CI."BranchId" = BR."BranchId"
LEFT JOIN "CPY_Company" CP ON BR."Company" = CP."CompanyId"
LEFT JOIN "Region" RG ON BR."Region" = RG."RegionId"
LEFT JOIN "TAR_HandoverTarget" TG ON CI."BranchId" = TG."BranchId"
	AND "DisableDate" IS NULL
	AND '{0}' BETWEEN TG."StartRange" AND TG."EndRange"
WHERE "Date" = '{0}'
 	AND CI."BranchId" IN ({1}) 
GROUP BY 1, 2
ORDER BY 2, 1