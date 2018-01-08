
SELECT 
  brnum as "LegacyBranchNumber",
  CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END as "PayNo", 
  COUNT(*) as "Quantity", 
  SUM(cheque) as "Cheque", 
  SUM(CASE 
   WHEN trdate IS  NUlL THEN 1 
    ELSE 0 
  END) AS "NewClientQuantity", 
  SUM(CASE 
    WHEN trdate IS  NUlL THEN cheque 
    ELSE 0 
  END) AS "NewClientAmount", 
  SUM(CASE 
    WHEN trdate IS NOT NUlL 
      AND DATE_PART('day', loandate::timestamp - trdate::timestamp) <= 17 THEN 1 
    ELSE 0 
  END) AS "ExistingClientCount", 
  SUM(CASE 
    WHEN trdate IS NOT NUlL 
      AND DATE_PART('day', loandate::timestamp - trdate::timestamp) > 17 THEN 1 
    ELSE 0	
  END) AS "RevivedClientCount",
  SUM(CASE 
    WHEN trdate IS NOT NUlL 
      AND DATE_PART('day', loandate::timestamp - trdate::timestamp) > 17 THEN cheque 
    ELSE 0	
  END) AS "RevivedClientAmount" 
FROM (
  SELECT DISTINCT
      CASE 
        WHEN LN.loanmeth = 'W' THEN ceiling(LN.payno / 4) 
        WHEN LN.loanmeth = 'B' THEN ceiling(LN.payno / 2) 
        WHEN LN.loanmeth = 'M' THEN LN.payno 
        ELSE 0 
      END as payno, 
      LN.cheque, 
      LN.loandate, 
      LN.brnum, LN.client, LN.loan, 
      TR.trdate,
      RANK() OVER (PARTITION BY LN.loan, LN.client, LN.brnum ORDER BY TR.trdate DESC) as lasttransaction 
  FROM company.loans LN 
  LEFT JOIN company.loans LH ON LN.client = LH.client 
    AND LN.brnum = LH.brnum 
    AND LN.loandate > LH.loandate  
    AND LH.nctrantype IN ('USE', 'N/A') -- NUCARD, EFT OR CASH
  LEFT JOIN company.trans TR ON LH.loan = TR.loan 
    AND TR.brnum = LH.brnum 
    AND TR.client = LH.client 
    AND (TR.trtype NOT IN ('R', 'A')
	OR TR.trdate> CURRENT_DATE)
    AND TR.trtype <> CASE WHEN tr.trstat IS NULL THEN 'asd' ELSE tr.trstat END
  WHERE LN.loandate BETWEEN '{0}' AND '{1}'
    AND LN.nctrantype IN ('USE', 'N/A') -- NUCARD, EFT OR CASH 
    AND LN.brnum IN ('{2}')
	AND LN.status <> 'C'
    ) CL 
WHERE lasttransaction = 1 
GROUP BY brnum, CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END;