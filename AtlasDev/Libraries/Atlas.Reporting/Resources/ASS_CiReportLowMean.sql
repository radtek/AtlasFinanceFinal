--SELECT CP."Name", TG."Target", TG."Target" - CH."Cheque" AS "Achieved", (TG."Target" - CH."Cheque") / TG."Target" AS "AchievedPercent", CAST((TG."Target" - CH."Cheque") / 3300 AS INT) AS "NoOfLoansRequired"
--FROM (
--	SELECT CI."BranchId", SUM(CI."Cheque") AS "Cheque"
--	FROM "ASS_CiReport" CI
--	WHERE CI."BranchId" IN ({2}) 
--		AND CI."Date" BETWEEN '{0}' AND '{1}'
--	GROUP BY CI."BranchId") CH
--INNER JOIN (SELECT BR."BranchId", BR."Company", SUM((BM."Amount" * TP."DailyPhase" / 100) * (LM."Percent"/100)) AS "Target"
--	FROM "BRN_Branch" BR 
--	INNER JOIN "TAR_BranchCIMonthly" BM ON BR."BranchId" = BM."BranchId"
--		AND EXTRACT(MONTH FROM BM."TargetDate") = EXTRACT(MONTH FROM to_date('{0}', 'yyyy-MM-dd'))
--		AND EXTRACT(YEAR FROM BM."TargetDate") = EXTRACT(YEAR FROM to_date('{0}', 'yyyy-MM-dd'))
--	CROSS JOIN (SELECT SUM("Percent") AS "DailyPhase"
--		FROM "TAR_DailySale"
--		WHERE "TargetDate" BETWEEN '{0}' AND '{1}') TP
--	INNER JOIN "TAR_LoanMix" LM ON EXTRACT(MONTH FROM BM."TargetDate") = EXTRACT(MONTH FROM LM."TargetDate")
--		AND EXTRACT(YEAR FROM BM."TargetDate") = EXTRACT(YEAR FROM LM."TargetDate")
--	WHERE BR."BranchId" IN ({2}) 
--		AND LM."PayNo" = 0
--	GROUP BY BR."BranchId", BR."Company") TG ON CH."BranchId" = TG."BranchId"
--LEFT JOIN "CPY_Company" CP ON TG."Company" = CP."CompanyId"
--WHERE ((TG."Target" - CH."Cheque") / TG."Target" * 100) < {3}
--	AND CH."Cheque" < TG."Target"


SELECT CH."BranchId", CP."Name", TG."Target", CH."Cheque" AS "Achieved", 
	(TG."Target" - CH."Cheque") AS "Variance",
	(100 - (((TG."Target" - CH."Cheque") / NULLIF(TG."Target", 0)) * 100)) / 100 "AchievedPercent", 
	CAST((TG."Target" - CH."Cheque") / 3300 AS INT) AS "NoOfLoansRequired"
FROM (
	SELECT CI."BranchId", SUM(CI."Cheque") AS "Cheque"
	FROM "ASS_CiReport" CI
	WHERE CI."BranchId" IN ({2}) 
		AND CI."Date" BETWEEN '{0}' AND '{1}'
	GROUP BY CI."BranchId") CH
INNER JOIN (SELECT BR."BranchId", BR."Company", SUM(BM."Amount" * TP."DailyPhase" / 100) AS "Target"
	FROM "BRN_Branch" BR 
	INNER JOIN "TAR_BranchCIMonthly" BM ON BR."BranchId" = BM."BranchId"
		AND EXTRACT(MONTH FROM BM."TargetDate") = EXTRACT(MONTH FROM to_date('{0}', 'yyyy-MM-dd'))
		AND EXTRACT(YEAR FROM BM."TargetDate") = EXTRACT(YEAR FROM to_date('{0}', 'yyyy-MM-dd'))
	CROSS JOIN (SELECT SUM("Percent") AS "DailyPhase"
		FROM "TAR_DailySale"
		WHERE "TargetDate" BETWEEN '{0}' AND '{1}'
			AND "DisableDate" IS NULL) TP
	WHERE BR."BranchId" IN ({2}) 
	GROUP BY BR."BranchId", BR."Company") TG ON CH."BranchId" = TG."BranchId"
LEFT JOIN "CPY_Company" CP ON TG."Company" = CP."CompanyId"
--WHERE (100 - (((TG."Target" - CH."Cheque") / TG."Target") * 100)) < {3}
--	AND CH."Cheque" < TG."Target"

