WITH CTE_Allocation AS 
(
	SELECT "GroupId", 
		"RegionId", 
		"BranchId", 
		"AllocatedUserId", 
		"CategoryId", 
		"SubCategoryId", 
		SUM("SMSCount") AS "SMSSent", 
		SUM("NoActionCount") as "NoActions"
	FROM (
	SELECT DISTINCT CSE."GroupId", BRN."Region" AS "RegionId", BRN."BranchId", CAL."AllocatedUserId", SCT."CategoryId", SCT."SubCategoryId",CSE."CaseId", CST."CaseStreamId", 
		CAL."SMSCount", CAL."NoActionCount"
	FROM "STR_CaseStreamAction" CSA
	LEFT JOIN "STR_CaseStream" CST ON CSA."CaseStreamId" = CST."CaseStreamId"
	LEFT JOIN "STR_CaseStreamAllocation" CAL ON CST."CaseStreamId" = CAL."CaseStreamId"
	LEFT JOIN "STR_Case" CSE ON CST."CaseId" = CSE."CaseId"
	LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
	LEFT JOIN "STR_SubCategory" SCT ON CSE."SubCategoryId" = SCT."SubCategoryId"
	WHERE (CSA."ActionDate" BETWEEN '{0}' AND '{1}'
		OR CST."CreateDate" BETWEEN '{0}' AND '{1}'
		OR CSE."CaseStatusId" = 1
		OR CSA."DateActioned" BETWEEN '{0}' AND '{1}')
		AND CSE."GroupId" = {2}
		AND BRN."BranchId" IN ({3})
	) AL
	GROUP BY "GroupId", "RegionId", "BranchId", "AllocatedUserId", "CategoryId", "SubCategoryId"
),
CTE_Transfers AS 
(
	SELECT CSE."GroupId", 
		BRN."Region" AS "RegionId", 
		BRN."BranchId", 
		CAL."AllocatedUserId", 
		SCT."CategoryId", 
		SCT."SubCategoryId", 
		SUM(CASE 
			WHEN CAL."AllocatedDate" BETWEEN '{0}' AND '{1}' AND CAL."TransferredIn" = true THEN 1
			ELSE 0
		END) AS "TransfersIn", 
		SUM(CASE 
			WHEN CAL."TransferredOutDate" BETWEEN '{0}' AND '{1}' AND CAL."TransferredOut" = true THEN 1
			ELSE 0
		END) AS "TransfersOut"
	FROM "STR_CaseStreamAllocation" CAL
	LEFT JOIN "STR_CaseStream" CST ON CAL."CaseStreamId" = CST."CaseStreamId"
	LEFT JOIN "STR_Case" CSE ON CST."CaseId" = CSE."CaseId"
	LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
	LEFT JOIN "STR_SubCategory" SCT ON CSE."SubCategoryId" = SCT."SubCategoryId"
	WHERE (CAL."AllocatedDate" BETWEEN '{0}' AND '{1}'
		OR CAL."TransferredOutDate" BETWEEN '{0}' AND '{1}')
		AND (CAL."TransferredIn" = true 
			OR CAL."TransferredOut" = true)
		AND CSE."GroupId" = {2}
		AND BRN."BranchId" IN ({3})
	GROUP BY CSE."GroupId", BRN."Region", BRN."BranchId", CAL."AllocatedUserId", SCT."CategoryId", SCT."SubCategoryId"
),
CTE_Esclations AS 
(
	SELECT  "GroupId", 
		"RegionId", 
		"BranchId", 
		"AllocatedUserId", 
		"CategoryId", 
		"SubCategoryId", 
		COUNT(*) AS "EscalatedCases"
	FROM (
		SELECT CSE."GroupId", 
			BRN."Region" AS "RegionId", 
			BRN."BranchId", 
			CAL."AllocatedUserId", 
			SCT."CategoryId", 
			SCT."SubCategoryId", 
			RANK() OVER (PARTITION BY CAL."AllocatedUserId", CAL."CaseStreamId" ORDER BY CES."CreateDate") AS Rank
		FROM "STR_CaseStreamAllocation" CAL
		INNER JOIN "STR_CaseStreamEscalation" CES ON CAL."CaseStreamId" = CES."CaseStreamId"
			AND CES."EscalationId" > CAL."EscalationId"
		LEFT JOIN "STR_CaseStream" CST ON CAL."CaseStreamId" = CST."CaseStreamId"
		LEFT JOIN "STR_Case" CSE ON CST."CaseId" = CSE."CaseId"
		LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
		LEFT JOIN "STR_SubCategory" SCT ON CSE."SubCategoryId" = SCT."SubCategoryId"
		WHERE CES."CreateDate" BETWEEN '{0}' AND '{1}'
			AND (CAL."TransferredOut" = false
				OR (CAL."TransferredOut" = true 
					AND CAL."TransferredOutDate" > CES."CreateDate"))
			AND CSE."GroupId" = {2}
			AND BRN."BranchId" IN ({3})) ESC
	WHERE Rank = 1
	GROUP BY "GroupId", "RegionId", "BranchId", "AllocatedUserId", "CategoryId", "SubCategoryId" 
)

SELECT RPT."GroupId", 
	RPT."RegionId", 
 	RGN."Description" AS "Region", 
	RPT."BranchId", 
 	CPY."Name" AS "Branch", 
	RPT."AllocatedUserId", 
	PSN."Firstname" || ' ' || PSN."Lastname" AS "AllocatedUser",
	PSN."Firstname" || ' ' || PSN."Lastname" AS "Description",
	RPT."CategoryId", 
	CTG."Description" AS "Category",
	RPT."SubCategoryId", 
	SCT."Description" AS "SubCategory",
	RPT."TotalAccounts",
	RPT."NewAccountsThisMonth",
	RPT."NewAccountsFromPast",
	RPT."NewAccountsThisMonth" + RPT."NewAccountsFromPast" AS "NewAccountsTotal",
	RPT."NewAccountsDistinct",
	RPT."PTPsPTCsMadeThisMonth",
	RPT."PTPsPTCsMadeThisMonthAccountsAffected",
	RPT."PTPsPTCsDueFuture",
	RPT."PTPsPTCsMadeAndDueThisMonth",
	RPT."PTPsPTCsDueFromPast",
	RPT."PTPsPTCsDueFromPast" + RPT."PTPsPTCsMadeAndDueThisMonth" AS "PTPsPTCsDueThisMonthTotal",
	RPT."PTPsPTCsDueFromPastAccountsAffected", 
	RPT."PTPsPTCsDueThisMonthWaiting", 
	RPT."PTPsPTCsDueThisMonthBroken", 
	RPT."PTPsPTCsDueThisMonthSuccessful", 
	RPT."PTPsPTCsDueThisMonthRescheduled",
	RPT."FollowUpsMadeThisMonth",
	RPT."FollowUpsMadeThisMonthAccountsAffected",
	RPT."FollowUpsMadeThisMonthAndDueFuture",
	RPT."FollowUpsMadeAndDueThisMonth",
	RPT."FollowUpsMadeFromPastAndDueThisMonth",
	RPT."FollowUpsMadeFromPastAndDueThisMonthAccountsAffected",
	RPT."FollowUpsMadeFromPastAndDueThisMonth" + RPT."FollowUpsMadeAndDueThisMonth" AS "FollowUpsDueThisMonthTotal",
	RPT."FollowUpsDueAndCompletedThisMonth",
	RPT."FollowUpsDueAndIncompleteThisMonth",
	RPT."FollowUpsIncompleteFromPastAndCompletedThisMonth", 
	RPT."UnActioned",
	CAT."SMSSent", 
	CAT."NoActions", 
	COALESCE(ESC."EscalatedCases", 0) AS "EscalatedCases",
	COALESCE(TRF."TransfersIn", 0) AS "TransfersIn", 
	COALESCE(TRF."TransfersOut", 0) AS "TransfersOut"
FROM (
	SELECT CSE."GroupId", 
		BRN."Region" AS "RegionId", 
		BRN."BranchId", 
		BRN."Company" AS "CompanyId",
		CAL."AllocatedUserId", 
		SCT."CategoryId", 
		SCT."SubCategoryId", 
		COUNT(DISTINCT CASE 
			WHEN CSE."CaseStatusId" IN (4, 5) AND CSE."CreateDate"::date = CSE."LastStatusDate"::date  -- "Handed Over", "Closed"
				THEN  0 
			ELSE ACC."AccountId" 
		END) - 1 AS "TotalAccounts",
		-- NEW Accounts
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 1 -- NEW
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "NewAccountsThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 1 -- NEW
				AND CST."CreateDate" < '{0}'
				THEN CST."CaseStreamId" 
			ELSE 0
		END) - 1 AS "NewAccountsFromPast",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 1 -- NEW
				AND CST."CreateDate" < '{1}'
				THEN ACC."AccountId" 
			ELSE 0
		END) - 1 AS "NewAccountsDistinct",
		-- PTP/PTC
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "PTPsPTCsMadeThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				THEN ACC."AccountId"
			ELSE 0
		END) - 1 AS "PTPsPTCsMadeThisMonthAccountsAffected",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				AND CSA."ActionDate" > '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "PTPsPTCsDueFuture",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "PTPsPTCsMadeAndDueThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" < '{0}'
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "PTPsPTCsDueFromPast",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" < '{0}'
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				THEN ACC."AccountId"
			ELSE 0
		END) - 1 AS "PTPsPTCsDueFromPastAccountsAffected", 
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				AND CSA."CompleteDate" IS NULL
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "PTPsPTCsDueThisMonthWaiting", 
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				AND CSA."IsSuccess" = false
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "PTPsPTCsDueThisMonthBroken", 
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				AND CSA."IsSuccess" = true
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "PTPsPTCsDueThisMonthSuccessful", 
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = (CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END)
				AND CSA."CompleteDate" IS NOT NULL
				AND CSA."IsSuccess" IS NULL
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "PTPsPTCsDueThisMonthRescheduled",
		-- Follow Ups
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "FollowUpsMadeThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				THEN ACC."AccountId"
			ELSE 0
		END) - 1 AS "FollowUpsMadeThisMonthAccountsAffected",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionDate" > '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "FollowUpsMadeThisMonthAndDueFuture",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "FollowUpsMadeAndDueThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" < '{0}' 
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "FollowUpsMadeFromPastAndDueThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" < '{0}' 
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				THEN ACC."AccountId"
			ELSE 0
		END) - 1 AS "FollowUpsMadeFromPastAndDueThisMonthAccountsAffected",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."CompleteDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "FollowUpsDueAndCompletedThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."CompleteDate" IS NULL
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "FollowUpsDueAndIncompleteThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CSA."ActionDate" < '{0}' 
				AND CSA."CompleteDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "FollowUpsIncompleteFromPastAndCompletedThisMonth",
		COUNT(DISTINCT CASE 
			WHEN CST."StreamId" = 1 -- New
				AND CSA."CompleteDate" IS NULL
				THEN CST."CaseStreamId"
			ELSE 0
		END) - 1 AS "UnActioned"
	FROM "STR_CaseStreamAction" CSA
	LEFT JOIN "STR_CaseStream" CST ON CSA."CaseStreamId" = CST."CaseStreamId"
	LEFT JOIN "STR_CaseStreamAllocation" CAL ON CST."CaseStreamId" = CAL."CaseStreamId"
	LEFT JOIN "STR_CaseStreamAllocation" HES ON CAL."CaseStreamId" = HES."CaseStreamId"
		AND HES."EscalationId" > CAL."EscalationId"
	LEFT JOIN "STR_Case" CSE ON CST."CaseId" = CSE."CaseId"
	LEFT JOIN "STR_Debtor" DBT ON CSE."DebtorId" = DBT."DebtorId"
	LEFT JOIN "STR_Account" ACC ON DBT."DebtorId" = ACC."DebtorId"
	LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
	LEFT JOIN "STR_SubCategory" SCT ON CSE."SubCategoryId" = SCT."SubCategoryId"
	WHERE (CSA."ActionDate" BETWEEN '{0}' AND '{1}'
		OR CST."CreateDate" BETWEEN '{0}' AND '{1}'
		OR CSE."CaseStatusId" = 1
		OR CSA."DateActioned" BETWEEN '{0}' AND '{1}')
		AND CSE."GroupId" = {2}
		AND BRN."BranchId" IN ({3})
	GROUP BY CSE."GroupId", BRN."Region", BRN."BranchId", CAL."AllocatedUserId", SCT."CategoryId", SCT."SubCategoryId") RPT
LEFT JOIN CTE_Allocation CAT ON RPT."GroupId" = CAT."GroupId"
	AND RPT."RegionId" = CAT."RegionId"
	AND RPT."BranchId" = CAT."BranchId"
	AND RPT."AllocatedUserId" = CAT."AllocatedUserId"
	AND RPT."CategoryId" = CAT."CategoryId"
	AND RPT."SubCategoryId" = CAT."SubCategoryId"
LEFT JOIN CTE_Transfers TRF ON RPT."GroupId" = TRF."GroupId"
	AND RPT."RegionId" = TRF."RegionId"
	AND RPT."BranchId" = TRF."BranchId"
	AND RPT."AllocatedUserId" = TRF."AllocatedUserId"
	AND RPT."CategoryId" = TRF."CategoryId"
	AND RPT."SubCategoryId" = TRF."SubCategoryId"
LEFT JOIN CTE_Esclations ESC ON RPT."GroupId" = ESC."GroupId"
	AND RPT."RegionId" = ESC."RegionId"
	AND RPT."BranchId" = ESC."BranchId"
	AND RPT."AllocatedUserId" = ESC."AllocatedUserId"
	AND RPT."CategoryId" = ESC."CategoryId"
	AND RPT."SubCategoryId" = ESC."SubCategoryId"
LEFT JOIN "Region" RGN ON RPT."RegionId" = RGN."RegionId"
LEFT JOIN "CPY_Company" CPY ON RPT."CompanyId" = CPY."CompanyId"
LEFT JOIN "STR_SubCategory" SCT ON RPT."SubCategoryId" = SCT."SubCategoryId"
LEFT JOIN "STR_Category" CTG ON RPT."CategoryId" = CTG."CategoryId"
LEFT JOIN "PER_Person" PSN ON RPT."AllocatedUserId" = PSN."PersonId"
ORDER BY RPT."RegionId", RPT."BranchId", 7, RPT."CategoryId", RPT."SubCategoryId"


