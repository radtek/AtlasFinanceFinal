SELECT INST.brnum AS "LegacyBranchNum", INST.instal_value - ADJ.instal_value AS "Receivable", PAID.instal_value AS "Received"
FROM (
	SELECT tr.brnum, SUM(tr.tramount) AS instal_value
	FROM company.trans tr, company.loans ln
	WHERE tr.trtype = 'R' 
	AND (tr.trdate BETWEEN '{1}' AND '{2}') 
	  AND  tr."order" = tr.seqno
	  AND COALESCE(tr.tramount, 0) != 0
	  AND ln.nctrantype IN ('USE', 'N/A', 'VAP')   
	  AND tr.brnum = ln.brnum 
	  AND tr.client = ln.client 
	  AND tr.loan = ln.loan
	  AND tr.brnum = '{0}'
	GROUP BY tr.brnum) INST
LEFT JOIN (
	SELECT tr.brnum, SUM(CASE WHEN tr.trtype = 'A' THEN tr.tramount * -1 ELSE tr.tramount END) AS instal_value
	FROM company.trans tr, company.loans ln
	WHERE(tr.trtype IN('E', 'C', 'H', 'W', 'A'))
	AND EXISTS (select TR1.trdate FROM company.trans TR1
	    WHERE TR1.brnum = TR.brnum AND TR1.client = TR.client AND TR1.loan = TR.loan
	    AND TR1."order" = TR."order"
	    AND TR1.seqno::bigint = TR."order"::bigint
	    AND TR1.trtype = 'R'
	  AND(TR1.trdate BETWEEN '{1}' AND '{2}')) 
	  AND COALESCE(tr.tramount, 0) != 0
	  AND ln.nctrantype IN ('USE', 'N/A', 'VAP')
	  AND tr.brnum = ln.brnum 
	  AND tr.client = ln.client 
	  AND tr.loan = ln.loan
	  AND tr.brnum = '{0}'
	GROUP BY tr.brnum) ADJ ON INST.brnum = ADJ.brnum
LEFT JOIN (
	SELECT tr.brnum, SUM(tr.tramount) AS instal_value
	FROM company.trans tr, company.loans ln
	WHERE tr.trtype IN('P', 'F', 'G')
	AND EXISTS(SELECT TR1.trdate FROM company.trans TR1
		WHERE TR1.brnum = TR.brnum
			AND TR1.client = TR.client
			AND TR1.loan = TR.loan
			AND TR1."order" = TR."order"
			AND TR1.seqno::bigint = TR."order"::bigint
			AND TR1.trtype = 'R'
	  AND(TR1.trdate BETWEEN '{1}' AND '{2}')) 
	  AND   COALESCE(tr.tramount, 0) != 0
	  AND   ln.nctrantype IN('USE', 'N/A', 'VAP')
	  AND   tr.brnum = ln.brnum
	  AND   tr.client = ln.client
	  AND   tr.loan = ln.loan
	  AND tr.brnum = '{0}'
	  GROUP BY tr.brnum) AS PAID ON INST.brnum = PAID.brnum