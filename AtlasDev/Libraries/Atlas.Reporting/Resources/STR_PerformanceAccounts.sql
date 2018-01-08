SELECT CSE."CaseId",
		CASE 
			WHEN CSE."CaseStatusId" IN (4, 5) AND CSE."CreateDate"::date = CSE."LastStatusDate"::date  -- "Handed Over", "Closed"
				THEN  0 
			ELSE ACC."AccountId" 
		END AS "TotalAccounts",
		-- NEW Accounts
		CASE 
			WHEN CST."StreamId" = 1 -- NEW
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "NewAccountsThisMonth",
		CASE 
			WHEN CST."StreamId" = 1 -- NEW
				AND CST."CreateDate" < '{0}'
				THEN CST."CaseStreamId" 
			ELSE 0
		END AS "NewAccountsFromPast",
		CASE 
			WHEN CST."StreamId" = 1 -- NEW
				AND CST."CreateDate" < '{1}'
				THEN CST."CaseStreamId" 
			ELSE 0
		END AS "NewAccountsTotal",
		CASE 
			WHEN CST."StreamId" = 1 -- NEW
				AND CST."CreateDate" < '{1}'
				THEN ACC."AccountId" 
			ELSE 0
		END AS "NewAccountsDistinct",
		-- PTP/PTC
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "PTPsPTCsMadeThisMonth",
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				THEN ACC."AccountId"
			ELSE 0
		END AS "PTPsPTCsMadeThisMonthAccountsAffected",
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				AND CSA."ActionDate" > '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "PTPsPTCsDueFuture",
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "PTPsPTCsMadeAndDueThisMonth",
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" < '{0}'
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "PTPsPTCsDueFromPast",
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CST."CreateDate" < '{0}'
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				THEN ACC."AccountId"
			ELSE 0
		END AS "PTPsPTCsDueFromPastAccountsAffected", 
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				AND CSA."CompleteDate" IS NULL
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "PTPsPTCsDueThisMonthWaiting", 
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				AND CSA."IsSuccess" = false
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "PTPsPTCsDueThisMonthBroken", 
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				AND CSA."IsSuccess" = true
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "PTPsPTCsDueThisMonthSuccessful", 
		CASE 
			WHEN CST."StreamId" IN (2, 3) -- PTP, PTC
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionTypeId" = 2
				AND CSA."CompleteDate" IS NOT NULL
				AND CSA."IsSuccess" IS NULL
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "PTPsPTCsDueThisMonthRescheduled",
		-- Follow Ups
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "FollowUpsMadeThisMonth",
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				THEN ACC."AccountId"
			ELSE 0
		END AS "FollowUpsMadeThisMonthAccountsAffected",
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionDate" > '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "FollowUpsMadeThisMonthAndDueFuture",
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" BETWEEN '{0}' AND '{1}'
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "FollowUpsMadeAndDueThisMonth",
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" < '{0}' 
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "FollowUpsMadeFromPastAndDueThisMonth",
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CST."CreateDate" < '{0}' 
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				THEN ACC."AccountId"
			ELSE 0
		END AS "FollowUpsMadeFromPastAndDueThisMonthAccountsAffected",
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."CompleteDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "FollowUpsDueAndCompletedThisMonth",
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CSA."ActionDate" BETWEEN '{0}' AND '{1}'
				AND CSA."CompleteDate" IS NULL
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "FollowUpsDueAndIncompleteThisMonth",
		CASE 
			WHEN CST."StreamId" = 4 -- Follow Up
				AND CSA."ActionDate" < '{0}' 
				AND CSA."CompleteDate" BETWEEN '{0}' AND '{1}'
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "FollowUpsIncompleteFromPastAndCompletedThisMonth",
		CASE 
			WHEN CST."StreamId" = 1 -- New
				AND CSA."CompleteDate" IS NULL
				THEN CST."CaseStreamId"
			ELSE 0
		END AS "UnActioned"
FROM "STR_CaseStreamAction" CSA
LEFT JOIN "STR_CaseStream" CST ON CSA."CaseStreamId" = CST."CaseStreamId"
LEFT JOIN "STR_CaseStreamAllocation" CAL ON CST."CaseStreamId" = CAL."CaseStreamId"
LEFT JOIN "STR_CaseStreamAllocation" HES ON CAL."CaseStreamId" = HES."CaseStreamId"
	AND HES."EscalationId" > CAL."EscalationId"
LEFT JOIN "STR_Case" CSE ON CST."CaseId" = CSE."CaseId"
LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
LEFT JOIN "STR_Account" ACC ON CSE."DebtorId" = ACC."DebtorId"
LEFT JOIN "STR_SubCategory" SCT ON CSE."SubCategoryId" = SCT."SubCategoryId"
WHERE (CSA."ActionDate" BETWEEN '{0}' AND '{1}'
	OR CST."CreateDate" BETWEEN '{0}' AND '{1}'
	OR CSE."CaseStatusId" = 1
	OR CSA."DateActioned" BETWEEN '{0}' AND '{1}')
	AND CSE."GroupId" = {2}
	AND BRN."BranchId" IN ({3})
	AND SCT."CategoryId" IN ({4})
	AND CAL."AllocatedUserId" IN ({5})
	AND (CSE."CaseStatusId" IN (4, 5) AND CSE."CreateDate"::date = CSE."LastStatusDate"::date) = false;