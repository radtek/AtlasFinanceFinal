UPDATE "STR_Account"
SET "LastImportReference" = NULL
WHERE "AccountId" IN (SELECT "AccountId" FROM "STR_Account" 
WHERE "LastImportReference" IS NOT NULL
LIMIT 10000);