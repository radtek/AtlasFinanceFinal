SELECT LN.recid as loanreference, TR.recid AS transactionreference,
	TR.tramount as Amount, TR.order, TR.seqno, TR.trdate as TransactionDate, TR.trtype as TransactionType, TR.trstat as TranasctionStatus, TR.statdate as StatusDate, TR.backupdate
FROM company.loans LN
INNER JOIN company.trans TR ON LN.loan = TR.loan 
	AND LN.client = TR.client
	AND LN.brnum = TR.brnum
	AND TR.tramount != 0
WHERE LN.recid IN ({0})
	AND 
	TR.backupdate > '{1}'