SELECT 
	"LegacyBranchNumber",
	"HandoverDate",
	"PayNo", 
	"Quantity", 
	"Amount",
	"ClientQuantity"
FROM (
  SELECT 
        brnum AS "LegacyBranchNumber", hovrdate AS "HandoverDate",
		CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END AS "PayNo", 
		COUNT(*) AS "Quantity", 
		SUM(hovramt) AS "Amount"
  FROM ( 
    SELECT 
          CASE 
            WHEN loanmeth = 'W' THEN ceiling(payno / 4) 
            WHEN loanmeth = 'B' THEN ceiling(payno / 2) 
            WHEN loanmeth = 'M' THEN payno 
            ELSE 0 
          END as payno, brnum, LN.hovramt, LN.hovrdate
    FROM company.loans LN 
    WHERE LN.hovrdate BETWEEN '{0}' AND '{1}' 
      AND LN.brnum IN ('{2}')) LS 
  GROUP BY brnum, LS.hovrdate, CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END) HO
 LEFT JOIN (
    SELECT 1 as payno, brnum, COUNT(DISTINCT CONCAT(client,brnum)) AS "ClientQuantity"
    FROM company.loans LN 
    WHERE LN.hovrdate BETWEEN '{0}' AND '{1}' 
      AND LN.brnum IN ('{2}')
    GROUP BY 1, 2) CH ON HO."PayNo" = CH.payno AND HO."LegacyBranchNumber" = CH.brnum