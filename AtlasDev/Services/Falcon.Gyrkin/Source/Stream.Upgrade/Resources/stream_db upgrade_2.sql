-- ALTER TABLE "STR_CaseStream" DROP CONSTRAINT "FK_STR_CaseStream_CompleteAccountNoteId";
-- ALTER TABLE "STR_AccountNote" DROP CONSTRAINT "FK_STR_AccountNote_NoteId"; 

 
-- update cases with debtor
UPDATE "STR_Case" CSE
SET "DebtorId" = UP."DebtorId"
FROM (
	SELECT CSE."CaseId", ACC."DebtorId"
	FROM "STR_Case" CSE
	LEFT JOIN "STR_Account" ACC ON CSE."AccountId" = ACC."AccountId"
	WHERE CSE."DebtorId" IS NULL
	AND ACC."DebtorId" IS NOT NULL
	LIMIT 100000) UP
WHERE CSE."CaseId" = UP."CaseId";



-- update cases with host
UPDATE "STR_Case" CSE
SET "HostId" = UP."HostId"
FROM (
	SELECT CSE."CaseId", ACC."HostId"
	FROM "STR_Case" CSE
	LEFT JOIN "STR_Account" ACC ON CSE."AccountId" = ACC."AccountId"
	WHERE CSE."HostId" IS NULL
	AND ACC."HostId" IS NOT NULL
	LIMIT 100000) UP
WHERE CSE."CaseId" = UP."CaseId";

-- update cases with branch
UPDATE "STR_Case" CSE
SET "BranchId" = UP."BranchId"
FROM (
	SELECT CSE."CaseId", ACC."BranchId"
	FROM "STR_Case" CSE
	LEFT JOIN "STR_Account" ACC ON CSE."AccountId" = ACC."AccountId"
	WHERE CSE."BranchId" IS NULL
	AND ACC."BranchId" IS NOT NULL
	LIMIT 100000) UP
WHERE CSE."CaseId" = UP."CaseId";


-- update debtors with contact
UPDATE "STR_DebtorContact" DB
SET "Value" = UP."Value", "ContactTypeId" = UP."ContactTypeId", "IsActive" = UP."IsActive"
FROM (
	SELECT DC."DebtorContactId", CT."Value", CT."ContactTypeId", CT."IsActive"
	FROM "STR_DebtorContact" DC
	LEFT JOIN "Contact" CT ON DC."ContactId" = CT."ContactId"
	WHERE DC."Value" IS NULL
	LIMIT 100000) UP
WHERE DB."DebtorContactId" = UP."DebtorContactId";


-- UPDATE Note
INSERT INTO "STR_Note" ("Note", "CaseId", "AccountNoteTypeId", "CreateUserId", "CreateDate")
SELECT NTE."Note", CSE."CaseId", NT."AccountNoteTypeId", NT."CreateUserId", NT."CreateDate"
FROM "STR_AccountNote" NT
INNER JOIN "STR_Case" CSE ON NT."AccountId" = CSE."AccountId"
INNER JOIN "NTE_Note" NTE ON NT."NoteId" = NTE."NoteId"
ORDER BY NT."NoteId"
LIMIT 100000;

DELETE FROM "NTE_Note" WHERE "ParentNoteId" IN (
SELECT "NoteId"
FROM "STR_AccountNote"
ORDER BY "NoteId"
LIMIT 100000);

DELETE FROM "NTE_Note" WHERE "NoteId" IN (
SELECT "NoteId"
FROM "STR_AccountNote"
ORDER BY "NoteId"
LIMIT 100000);

DELETE FROM "STR_AccountNote" WHERE "NoteId" IN (
SELECT "NoteId"
FROM "STR_AccountNote"
ORDER BY "NoteId"
LIMIT 100000);