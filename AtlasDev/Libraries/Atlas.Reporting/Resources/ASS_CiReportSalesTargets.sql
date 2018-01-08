SELECT {3}, LM."PayNo", SUM((BM."Amount" * TP."DailyPhase" / 100) * (LM."Percent"/100)) AS "Target", (LM."Percent"/100) AS "TargetPercent"
FROM "BRN_Branch" BR
LEFT JOIN "CPY_Company" CP ON BR."Company" = CP."CompanyId"
LEFT JOIN "Region" RG ON BR."Region" = RG."RegionId"
LEFT JOIN "TAR_BranchCIMonthly" BM ON BR."BranchId" = BM."BranchId"
	AND EXTRACT(MONTH FROM BM."TargetDate") = EXTRACT(MONTH FROM to_date('{0}', 'yyyy-MM-dd'))
	AND EXTRACT(YEAR FROM BM."TargetDate") = EXTRACT(YEAR FROM to_date('{0}', 'yyyy-MM-dd'))
CROSS JOIN (SELECT SUM("Percent") AS "DailyPhase"
	FROM "TAR_DailySale"
	WHERE "TargetDate" BETWEEN '{0}' AND '{1}'
		AND "DisableDate" IS NULL) TP
INNER JOIN "TAR_LoanMix" LM ON EXTRACT(MONTH FROM BM."TargetDate") = EXTRACT(MONTH FROM LM."TargetDate")
	AND EXTRACT(YEAR FROM BM."TargetDate") = EXTRACT(YEAR FROM LM."TargetDate")
WHERE BR."BranchId" IN ({2}) 
GROUP BY 1, 2, 3, LM."Percent"

UNION 

SELECT {3}, 0, SUM((BM."Amount" * TP."DailyPhase" / 100) * (LM."Percent"/100)) AS "Target", (SUM(LM."Percent")/100/COUNT(DISTINCT BR."BranchId")) AS "TargetPercent"
FROM "BRN_Branch" BR
LEFT JOIN "CPY_Company" CP ON BR."Company" = CP."CompanyId"
LEFT JOIN "Region" RG ON BR."Region" = RG."RegionId"
LEFT JOIN "TAR_BranchCIMonthly" BM ON BR."BranchId" = BM."BranchId"
	AND EXTRACT(MONTH FROM BM."TargetDate") = EXTRACT(MONTH FROM to_date('{0}', 'yyyy-MM-dd'))
	AND EXTRACT(YEAR FROM BM."TargetDate") = EXTRACT(YEAR FROM to_date('{0}', 'yyyy-MM-dd'))
CROSS JOIN (SELECT SUM("Percent") AS "DailyPhase"
	FROM "TAR_DailySale"
	WHERE "TargetDate" BETWEEN '{0}' AND '{1}'
		AND "DisableDate" IS NULL) TP
INNER JOIN "TAR_LoanMix" LM ON EXTRACT(MONTH FROM BM."TargetDate") = EXTRACT(MONTH FROM LM."TargetDate")
	AND EXTRACT(YEAR FROM BM."TargetDate") = EXTRACT(YEAR FROM LM."TargetDate")
WHERE BR."BranchId" IN ({2}) 
GROUP BY 1, 2, 3 
ORDER BY 1, 2, 3 