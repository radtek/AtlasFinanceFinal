﻿SELECT 	
	ALC."AllocatedUserId",
	PSN."Firstname" || ' ' || PSN."Lastname" AS "AllocatedUser",
	CAST(CST."CreateDate" AS date) AS "Date",
	
	COUNT(DISTINCT CASE 
		WHEN 
			CST."CreateDate" BETWEEN '{0}' AND '{1}' 
			AND CSA."ActionTypeId" = CASE WHEN CST."StreamId" = 2 THEN 2 ELSE 3 END
			AND CST."StreamId" IN (2, 3) -- PTP, PTC
		THEN CST."CaseStreamId"
		ELSE NULL
		END) AS "PtpPtcObtained", 	
	COUNT(DISTINCT CASE 
		WHEN 
			CSA."ActionDate" BETWEEN '{0}' AND '{1}'
			AND "StreamId" = 4 -- Follow Up
		THEN CST."CaseStreamId"
		ELSE NULL
	END) AS "FollowUps",
	COUNT(DISTINCT CASE 
		WHEN 
			CST."EscalationId"> 1
		THEN CST."CaseStreamId"
		ELSE NULL
	END) AS "Escalations",
	SUM(CAL."NoActionCount") AS "NoActionCount"
FROM "STR_Case" CSE
LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
LEFT JOIN "CPY_Company" CPY ON BRN."Company" = CPY."CompanyId"
LEFT JOIN "Region" RGN ON BRN."Region" = RGN."RegionId"
LEFT JOIN "STR_SubCategory" SBC ON CSE."SubCategoryId" = SBC."SubCategoryId"
LEFT JOIN "STR_Category" CTG ON SBC."CategoryId" = CTG."CategoryId"
LEFT JOIN "STR_CaseStream" CST ON CSE."CaseId" = CST."CaseId"
LEFT JOIN "STR_Account" ACC ON CSE."DebtorId" = ACC."DebtorId" 
LEFT JOIN "STR_CaseStreamAction" CSA ON CST."CaseStreamId" = CSA."CaseStreamId"
LEFT JOIN "STR_CaseStreamAllocation" ALC ON CST."CaseStreamId" = ALC."CaseStreamId"
LEFT JOIN "PER_Person" PSN ON ALC."AllocatedUserId" = PSN."PersonId"
LEFT JOIN (
	SELECT 
		CSA."BranchId", CAL."CaseStreamAllocationId", 
		SUM(CAL."NoActionCount") AS "NoActionCount",
		SUM(CAL."SMSCount") AS "SMSCount",
		SUM(
			CASE 
				WHEN CAL."TransferredIn" AND CAL."AllocatedDate" BETWEEN '{0}' AND '{1}' THEN 1
				ELSE 0
			END) AS "TransferredIn",
		SUM(
			CASE 
				WHEN CAL."TransferredOut" AND CAL."TransferredOutDate" BETWEEN '{0}' AND '{1}' THEN 1
				ELSE 0
			END) AS "TransferredOut"
	FROM (
		SELECT DISTINCT CSE."BranchId", CAL."CaseStreamAllocationId"
			FROM "STR_Case" CSE
			LEFT JOIN "STR_CaseStream" CST ON CSE."CaseId" = CST."CaseId"
			LEFT JOIN "STR_CaseStreamAllocation" CAL ON CST."CaseStreamId" = CAL."CaseStreamId"
			WHERE (CSE."CaseStatusId" IN (1,2,3)
				OR CSE."LastStatusDate" BETWEEN '{0}' AND '{1}')
				AND CSE."BranchId" IN ({2})
				AND CSE."GroupId" IN ({3})) CSA
	LEFT JOIN "STR_CaseStreamAllocation" CAL ON CSA."CaseStreamAllocationId" = CAL."CaseStreamAllocationId"
	GROUP BY CSA."BranchId", CAL."CaseStreamAllocationId"
	) CAL ON ALC."CaseStreamAllocationId" = CAL."CaseStreamAllocationId" 
WHERE (CSE."CaseStatusId" IN (1,2,3)
	OR CSE."LastStatusDate" BETWEEN '{0}' AND '{1}')
	AND CSE."BranchId" IN ({2})
	AND CSE."GroupId" IN ({3})
GROUP BY CAST(CST."CreateDate" AS date), ALC."AllocatedUserId", PSN."Firstname" || ' ' || PSN."Lastname", SBC."CategoryId"
	