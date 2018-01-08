DELETE
FROM "STR_Note"
WHERE "NoteId" IN (
	SELECT "NoteId"
	FROM "STR_Note"
	WHERE "AccountNoteTypeId" = 1
	ORDER BY "NoteId" DESC
	LIMIT 10000)