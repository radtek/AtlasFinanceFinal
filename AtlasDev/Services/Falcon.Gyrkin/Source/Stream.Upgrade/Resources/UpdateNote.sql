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