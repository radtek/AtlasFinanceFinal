SELECT LN.recid as loanreference, 
	TR.tramount as Amount, TR.order, TR.seqno, TR.trdate as TransactionDate, TR.trtype as TransactionType, TR.trstat as TranasctionStatus, TR.statdate as StatusDate
FROM company.loans LN
INNER JOIN company.trans TR ON LN.loan = TR.loan 
	AND LN.client = TR.client
	AND LN.brnum = TR.brnum
	AND TR.tramount > 0
WHERE TR.trdate >= CURRENT_DATE - INTERVAL '{1} Days'
	AND LN.recid IN ({0})
	AND TR.trtype IN ('P')
	AND TR.trstat IS NULL