﻿
SELECT COUNT(CSE."CaseId")
FROM "STR_Case" CSE
LEFT JOIN "STR_Account" ACC ON CSE."AccountId" = ACC."AccountId"
WHERE CSE."BranchId" IS NULL
AND ACC."BranchId" IS NOT NULL