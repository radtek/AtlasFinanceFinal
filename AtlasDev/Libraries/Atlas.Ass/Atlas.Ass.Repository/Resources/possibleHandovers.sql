SELECT brnum AS "LegacyBranchNumber", 
	SUM(PossibleHandOvers) AS "PossibleHandOvers", 
	SUM(NextPossibleHandOvers) AS "NextPossibleHandOvers"
	FROM 
		(
		SELECT brnum, PossibleHandOvers, NextPossibleHandOvers,
		     RANK() OVER (PARTITION BY client, loan, brnum ORDER BY PossibleHandOvers DESC) AS Rank
		FROM (
		  SELECT DISTINCT TR.client, TR.loan, TR.brnum, 
		    CASE 
		      WHEN trdate IS NOT NUlL 
			AND DATE_PART('day', (date_trunc('month', current_date) - INTERVAL '1 Day') - trdate::timestamp) >= DATE_PART('day', (date_trunc('month', current_date) - INTERVAL '1 Day')) THEN LN.outamnt
		      ELSE 0 
		    END AS PossibleHandOvers,
		    CASE 
		      WHEN trdate IS NOT NUlL 
			AND DATE_PART('day', (date_trunc('month', current_date) - INTERVAL '1 Day') - trdate::timestamp) >= 0 THEN LN.outamnt
		      ELSE 0 
		     END AS NextPossibleHandOvers
		  FROM company.trans TR
		  INNER JOIN company.loans LN ON TR.loan = LN.loan
		    AND TR.client = LN.client
		    AND TR.brnum = LN.brnum
		    AND LN.nctrantype IN ('USE', 'N/A', 'SIMSALEC') 
		  WHERE 
		    TR.trdate < date_trunc('month', current_date) 
		    AND TR.trtype = 'R' 
		    AND NULLIF(TRIM(TR.trstat), '') IS NULL 
		    AND TR.brnum IN ('{0}') 
		    AND LN.outamnt > 0) TM 
		) PH 
	WHERE Rank = 1 
	GROUP BY brnum 