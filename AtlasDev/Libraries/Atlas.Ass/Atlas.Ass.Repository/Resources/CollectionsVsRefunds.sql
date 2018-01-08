SELECT 
  brnum AS "LegacyBranchNumber",
  CASE 
    WHEN payno < 1 THEN 1
    WHEN payno > 6 AND payno < 12 THEN 6 
    WHEN payno > 12 AND payno < 24 THEN 12 
    WHEN payno > 24 AND payno < 36 THEN 24 
    ELSE payno 
  END AS "PayNo", 
  SUM(collections) AS "Collections", 
  SUM(refunds) AS "Refunds" 
FROM ( 
  SELECT 
    CASE 
      WHEN LN.loanmeth = 'W' THEN ceiling(payno / 4) 
      WHEN LN.loanmeth = 'B' THEN ceiling(payno / 2) 
      WHEN LN.loanmeth = 'M' THEN payno 
      ELSE 0 
    END as payno, 
    LN.Brnum,
    CASE 
      WHEN TR.trtype = 'P' THEN TR.tramount 
      ELSE 0 
    END AS collections, 
    CASE 
      WHEN TR.trtype IN ('F', 'G', 'B') THEN TR.tramount 
      ELSE 0 
    END AS refunds 
  FROM company.trans TR 
  INNER JOIN company.loans LN ON TR.loan = LN.loan 
    AND TR.client = LN.client 
    AND TR.brnum = LN.brnum 
  WHERE TR.trdate BETWEEN '{0}' AND '{1}' 
    AND TR.trtype IN ('P', 'F', 'G', 'B')
    AND TR.brnum IN ('{2}')) LS 
GROUP BY 
  brnum,
  CASE 
    WHEN payno < 1 THEN 1
    WHEN payno > 6 AND payno < 12 THEN 6 
    WHEN payno > 12 AND payno < 24 THEN 12 
    WHEN payno > 24 AND payno < 36 THEN 24 
    ELSE payno 
  END;