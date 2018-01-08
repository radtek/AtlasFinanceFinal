SELECT BR."BranchId", CP."Name" AS "BranchName", 
	SUM("Cheque") AS "Cheque", 
	SUM(HI."HandedOverLoansAmount") AS "HandedOverLoansAmount",
	SUM("Collections") as "Collections"
FROM "ASS_CiReport" CI
LEFT JOIN "BRN_Branch" BR ON CI."BranchId" = BR."BranchId"
LEFT JOIN "CPY_Company" CP ON BR."Company" = CP."CompanyId"
LEFT JOIN "ASS_CiReportHandoverInfo" HI ON CI."BranchId" = HI."BranchId"
	AND CI."Date" = HI."Date"
	AND CI."PayNo" = HI."PayNo"
WHERE CI."Date" BETWEEN '{0}' AND '{1}'
	AND CI."BranchId" IN ({2}) 
GROUP BY BR."BranchId", CP."Name"
ORDER BY CP."Name"