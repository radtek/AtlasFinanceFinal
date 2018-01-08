SELECT {3}, 
	CI."PayNo", 
	SUM("ScoreAboveXWeekly") AS "Weekly615", SUM("ScoreAboveXBiWeekly") AS "BiWeekly615", SUM("ScoreAboveXMonthly") AS "Monthly615", 
	SUM("ScoreAboveXWeekly" + "ScoreAboveXBiWeekly" + "ScoreAboveXMonthly") AS "Total615"
FROM "ASS_CiReportScore" CI
LEFT JOIN "BRN_Branch" BR ON CI."BranchId" = BR."BranchId"
LEFT JOIN "CPY_Company" CP ON BR."Company" = CP."CompanyId"
LEFT JOIN "Region" RG ON BR."Region" = RG."RegionId"
WHERE "Date" BETWEEN '{0}' AND '{1}'
	AND CI."BranchId" IN ({2}) 
GROUP BY 1, 2, 3 

UNION 

SELECT {3}, 
	0 AS "PayNo", 
	SUM("ScoreAboveXWeekly") AS "Weekly615", SUM("ScoreAboveXBiWeekly") AS "BiWeekly615", SUM("ScoreAboveXMonthly") AS "Monthly615", 
	SUM("ScoreAboveXWeekly" + "ScoreAboveXBiWeekly" + "ScoreAboveXMonthly") AS "Total615"
FROM "ASS_CiReportScore" CI
LEFT JOIN "BRN_Branch" BR ON CI."BranchId" = BR."BranchId"
LEFT JOIN "CPY_Company" CP ON BR."Company" = CP."CompanyId"
LEFT JOIN "Region" RG ON BR."Region" = RG."RegionId"
WHERE "Date" BETWEEN '{0}' AND '{1}'
	AND CI."BranchId" IN ({2}) 
GROUP BY 1, 2, 3 