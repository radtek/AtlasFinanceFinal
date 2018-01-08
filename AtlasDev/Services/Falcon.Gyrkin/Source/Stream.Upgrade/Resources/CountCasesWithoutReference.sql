SELECT COUNT(CSE."CaseId")
FROM "STR_Case" CSE
LEFT JOIN "BRN_Branch" BRN ON CSE."BranchId" = BRN."BranchId"
LEFT JOIN "STR_Debtor" DBT ON CSE."DebtorId" = DBT."DebtorId"
WHERE CSE."Reference" IS NULL