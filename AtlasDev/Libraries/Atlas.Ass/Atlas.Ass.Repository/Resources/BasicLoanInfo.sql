
SELECT "PayNo",
	"LegacyBranchNumber", 
	"Quantity", 
	"Cheque", 
	"ChequeToday", 
	"BranchLoans", 
	"SalesRepLoans", 
	"ChargesExclVAT", 
	"ChargesVAT", 
	"TotalCharges", 
	"CreditLife",
	"LoanFeeExclVAT", 
	"LoanFeeVAT", 
	"LoanFeeInclVAT", 
	"ScoreAbove615Weekly", 
	"ScoreAbove615BiWeekly", 
	"ScoreAbove615Monthly",
	"FuneralAddOn", 
	"AgeAddOn", 
	"VAPExcl", 
	"VAPVAT",
	"VAPIncl",
	"TotalAddOn",
	("LoanFeeExclVAT" + "FuneralAddOn" + "AgeAddOn" + "VAPExcl") AS "TotFeeExcl",
	("LoanFeeInclVAT" + "VAPVAT") AS "TotFeeVAT",
	("LoanFeeInclVAT" + "FuneralAddOn" + "AgeAddOn" + "VAPIncl") AS "TotFeeIncl"
FROM (SELECT 
		CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END AS "PayNo",
		brnum AS "LegacyBranchNumber", 
		SUM(CASE WHEN cancelled = false THEN 1 ELSE 0 END) AS "Quantity", 
		SUM(cheque * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "Cheque", -- was ELSE -1, for this line and all the below lines as well
		SUM(ChequeToday * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "ChequeToday", 
		SUM(clbr * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) as "BranchLoans", 
		SUM(clsr * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) as "SalesRepLoans", 
		SUM((interest + initfee + servicefee) * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "ChargesExclVAT", 
		SUM((initvat + servicevat) * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "ChargesVAT", 
		SUM((interest + initfee + servicefee + initvat + servicevat) * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "TotalCharges", 
		SUM((inspremval + inspolival) * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "CreditLife",
		SUM((interest + initfee + servicefee + inspremval + inspolival) * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "LoanFeeExclVAT", 
		SUM((initvat + servicevat) * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "LoanFeeVAT", 
		SUM((interest + initfee + servicefee + initvat + servicevat + inspremval + inspolival) * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) AS "LoanFeeInclVAT", 
		SUM(scoreabove615Weekly * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) as "ScoreAbove615Weekly", 
		SUM(scoreabove615BiWeekly * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) as "ScoreAbove615BiWeekly", 
		SUM(scoreabove615Monthly * (CASE WHEN cancelled = false THEN 1 ELSE 0 END)) as "ScoreAbove615Monthly"
	  FROM ( 
	    SELECT DISTINCT
	      CASE 
		WHEN LN.loanmeth = 'W' THEN ceiling(LN.payno / 4) 
		WHEN LN.loanmeth = 'B' THEN ceiling(LN.payno / 2) 
		WHEN LN.loanmeth = 'M' THEN LN.payno 
		ELSE 0 
	      END as payno, 
	      CASE 
			WHEN LN.loandate < '{0}' THEN true
			ELSE false 
		END AS Cancelled,
	      CASE 
		WHEN LN.loanmeth = 'W' AND NULLIF(TRIM(LN.score_cs), '') IS NOT NULL
		  AND cast(right(LN.score_cs, length(LN.score_cs) - position(':' in LN.score_cs)) as int) >= 615 THEN 1
		ELSE 0
	      END as scoreabove615Weekly,
	      CASE 
		WHEN LN.loanmeth = 'B' AND NULLIF(TRIM(LN.score_cs), '') IS NOT NULL
		  AND cast(right(LN.score_cs, length(LN.score_cs) - position(':' in LN.score_cs)) as int) >= 615 THEN 1
		ELSE 0
	      END as scoreabove615BiWeekly,
	      CASE 
		WHEN LN.loanmeth = 'M' AND NULLIF(TRIM(LN.score_cs), '') IS NOT NULL
		  AND cast(right(LN.score_cs, length(LN.score_cs) - position(':' in LN.score_cs)) as int) >= 615 THEN 1
		ELSE 0
	      END as scoreabove615Monthly,
	      LN.loan, LN.client, LN.brnum,
	      COALESCE(LN.interest, 0) AS interest,
	      COALESCE(LN.initfee, 0) AS initfee,
	      COALESCE(LN.initvat, 0) AS initvat,
	      COALESCE(LN.servicevat, 0) AS servicevat,
	      COALESCE(LN.servicefee, 0) AS servicefee,
	      COALESCE(LN.inspremval, 0) as inspremval,
	      COALESCE(LN.inspolival, 0) as inspolival,
	      LN.cheque, 
	      CASE 
			WHEN LN.loandate = CURRENT_DATE THEN LN.cheque
			ELSE 0
	      END AS ChequeToday,
	      CASE 
		WHEN (NULLIF(LN.rep_code, '')	IS NOT NULL 
			OR LN.rep_code = BR.rep_branch)
		AND PTR.trdate IS NULL THEN 1
		ELSE 0
	      END AS clbr,
	      CASE 
		WHEN (NULLIF(LN.rep_code, '') IS NOT NULL 
			OR LN.rep_code <> BR.rep_branch)
			AND PTR.trdate IS NULL 	THEN 1
		ELSE 0
	      END AS clsr
	    FROM company.loans LN 
		LEFT JOIN company.loans LH ON LN.client = LH.client 
			AND LN.brnum = LH.brnum 
			AND LN.loandate > LH.loandate  
			AND LH.nctrantype IN ('USE', 'N/A') -- NUCARD, EFT OR CASH
		LEFT JOIN company.trans PTR ON LH.loan =PTR.loan 
			AND PTR.brnum = LH.brnum 
			AND PTR.client = LH.client 
		      AND PTR."order" = 1 
		      AND PTR.seqno = 1 
		      AND PTR.trtype = 'R' 
	    INNER JOIN company.trans TR ON TR.loan  = LN.loan 
	      AND TR.client = LN.client 
	      AND TR.brnum = LN.brnum 
	      AND TR."order" = 1 
	      AND TR.seqno = 1 
	      AND TR.trtype = 'R' 
	    LEFT JOIN company.asbranch BR ON LN.brnum = BR.brnum
	    WHERE LN.nctrantype IN ('USE', 'N/A') -- NUCARD, EFT OR CASH 
		AND LN.brnum IN ('{2}')
		AND ((LN.loandate BETWEEN '{0}' AND '{1}' 
		AND (LN.status <> 'C'
			OR (LN.status = 'C'
				AND LN.paidate > '{1}')))
		 OR (LN.loandate < '{0}'
			AND LN.nctrantype IN ('USE', 'N/A') -- NUCARD, EFT OR CASH 
			AND LN.status = 'C' 
			AND LN.paidate BETWEEN '{0}' AND '{1}'))
	      ) LS 
	  GROUP BY brnum, CASE WHEN payno > 6 AND payno < 12 THEN 6 ELSE payno END) BI
LEFT JOIN (SELECT VSL.payno, VSL.brnum, 
	coalesce(VSL.sale_funvapincl,0) - coalesce(VCN.cancel_funvapincl,0) AS "FuneralAddOn",
	coalesce(VSL.sale_agevapincl,0) - coalesce(VCN.cancel_agevapincl,0) AS "AgeAddOn", 
	coalesce(VSL.sale_vapexcl,0) - coalesce(VCN.cancel_vapexcl,0) AS "VAPExcl", 
	coalesce(VSL.sale_vapvat,0) - coalesce(VCN.cancel_vapvat,0) AS "VAPVAT", 
	coalesce(VSL.sale_vapincl,0) - coalesce(VCN.cancel_vapincl,0) AS "VAPIncl", 
	coalesce(VSL.sale_totvapincl,0) - coalesce(VCN.cancel_totvapincl,0) AS "TotalAddOn" 
FROM (SELECT 
		CASE 
			WHEN LN.loanmeth = 'W' THEN ceiling(LN.payno / 4) 
			WHEN LN.loanmeth = 'B' THEN ceiling(LN.payno / 2) 
			WHEN LN.loanmeth = 'M' THEN LN.payno 
			ELSE 0 
		END as payno, 
		LN.brnum,
		SUM(CASE WHEN TV.Vap_Code IN ('FUN') THEN TV.SP_INEXVAT * TV.payno ELSE 0 END) AS Sale_FunVapExcl, 
		SUM(CASE WHEN TV.Vap_Code IN ('FUN') THEN round((TV.SP_INEXVAT * TV.payno) * coalesce(TV.VAT_PC,0)/100 ,2) ELSE 0 END) AS Sale_FunVapVat, 
		SUM(CASE WHEN TV.Vap_Code IN ('FUN') THEN round ((TV.SP_INEXVAT * TV.payno) * ( coalesce(TV.VAT_PC,0)+100) /100,2) ELSE 0 END) AS Sale_FunVapIncl, 
		
		SUM(CASE WHEN TV.Vap_Code IN ('EDU', 'ACD', 'GRO') THEN TV.SP_INEXVAT * TV.payno ELSE 0 END) AS Sale_AgeVapExcl, 
		SUM(CASE WHEN TV.Vap_Code IN ('EDU', 'ACD', 'GRO') THEN round((TV.SP_INEXVAT * TV.payno) * coalesce(TV.VAT_PC,0)/100 ,2) ELSE 0 END) AS Sale_AgeVapVat, 
		SUM(CASE WHEN TV.Vap_Code IN ('EDU', 'ACD', 'GRO') THEN round ((TV.SP_INEXVAT * TV.payno) * ( coalesce(TV.VAT_PC,0)+100) /100,2) ELSE 0 END) AS Sale_AgeVapIncl, 
		
		SUM(CASE WHEN TV.Vap_Code IN ('VAP') THEN TV.SP_INEXVAT * TV.payno ELSE 0 END) AS Sale_VapExcl, 
		SUM(CASE WHEN TV.Vap_Code IN ('VAP') THEN round((TV.SP_INEXVAT * TV.payno) * coalesce(TV.VAT_PC,0)/100 ,2) ELSE 0 END) AS Sale_VapVat, 
		SUM(CASE WHEN TV.Vap_Code IN ('VAP') THEN round ((TV.SP_INEXVAT * TV.payno) * ( coalesce(TV.VAT_PC,0)+100) /100,2) ELSE 0 END) AS Sale_VapIncl,
		
		SUM(TV.SP_INEXVAT * TV.payno) AS Sale_TotVapExcl, 
		SUM(round((TV.SP_INEXVAT * TV.payno) * coalesce(TV.VAT_PC,0)/100 ,2)) AS Sale_TotVapVat, 
		SUM(round((TV.SP_INEXVAT * TV.payno) * (coalesce(TV.VAT_PC,0)+100) /100,2)) AS Sale_TotVapIncl
	FROM company.loans LN
	INNER JOIN company.transvap TV ON TV.loan  = LN.loan 
	      AND TV.client = LN.client 
	      AND TV.brnum = LN.brnum 
	WHERE 
		LN.brnum IN ('{2}')
		AND LN.loandate BETWEEN '{0}' AND '{1}' 
		AND LN.nctrantype IN ('VAP') 
	GROUP BY 1, 2) VSL
LEFT JOIN (SELECT
	CASE 
		WHEN LN.loanmeth = 'W' THEN ceiling(LN.payno / 4) 
		WHEN LN.loanmeth = 'B' THEN ceiling(LN.payno / 2) 
		WHEN LN.loanmeth = 'M' THEN LN.payno 
		ELSE 0 
	END as payno, 
	LN.brnum,

	SUM(CASE WHEN TV.Vap_Code IN ('FUN') THEN 
		ROUND(TR.tramount * TV.SP_INEXVAT /
                (TV1.Tv1Sum)  * (100/ (100+coalesce(TV.VAT_PC,0)) ),2) ELSE 0 END) AS Cancel_FunVapExcl, 
	SUM(CASE WHEN TV.Vap_Code IN ('FUN') THEN 
		ROUND( TR.tramount * TV.SP_INEXVAT /
                (TV1.Tv1Sum)  * (coalesce(TV.VAT_PC,0)/(100+coalesce(TV.VAT_PC,0) ) ),2)  ELSE 0 END) AS Cancel_FunVapVat, 
	SUM(CASE WHEN TV.Vap_Code IN ('FUN') THEN 
		ROUND( TR.tramount * TV.SP_INEXVAT/
                (TV1.Tv1Sum),2)  ELSE 0 END) AS Cancel_FunVapIncl,
                
	SUM(CASE WHEN TV.Vap_Code IN ('EDU', 'ACD', 'GRO') THEN 
		ROUND(TR.tramount * TV.SP_INEXVAT /
                (TV1.Tv1Sum)  * (100/ (100+coalesce(TV.VAT_PC,0)) ),2) ELSE 0 END) AS Cancel_AgeVapExcl, 
	SUM(CASE WHEN TV.Vap_Code IN ('EDU', 'ACD', 'GRO') THEN 
		ROUND( TR.tramount * TV.SP_INEXVAT /
                (TV1.Tv1Sum)  * (coalesce(TV.VAT_PC,0)/(100+coalesce(TV.VAT_PC,0) ) ),2)  ELSE 0 END) AS Cancel_AgeVapVat, 
	SUM(CASE WHEN TV.Vap_Code IN ('EDU', 'ACD', 'GRO') THEN 
		ROUND( TR.tramount * TV.SP_INEXVAT/
                (TV1.Tv1Sum),2)  ELSE 0 END) AS Cancel_AgeVapIncl,
                
	SUM(CASE WHEN TV.Vap_Code IN ('VAP') THEN 
		ROUND(TR.tramount * TV.SP_INEXVAT /
                (TV1.Tv1Sum)  * (100/ (100+coalesce(TV.VAT_PC,0)) ),2) ELSE 0 END) AS Cancel_VapExcl, 
	SUM(CASE WHEN TV.Vap_Code IN ('VAP') THEN 
		ROUND( TR.tramount * TV.SP_INEXVAT /
                (TV1.Tv1Sum)  * (coalesce(TV.VAT_PC,0)/(100+coalesce(TV.VAT_PC,0) ) ),2)  ELSE 0 END) AS Cancel_VapVat, 
	SUM(CASE WHEN TV.Vap_Code IN ('VAP') THEN 
		ROUND( TR.tramount * TV.SP_INEXVAT/
                (TV1.Tv1Sum),2)  ELSE 0 END) AS Cancel_VapIncl,
                
	SUM(ROUND(TR.tramount * TV.SP_INEXVAT /
                (TV1.Tv1Sum)  * (100/ (100+coalesce(TV.VAT_PC,0)) ),2)) AS Cancel_TotVapExcl, 
	SUM(ROUND( TR.tramount * TV.SP_INEXVAT /
                (TV1.Tv1Sum)  * (coalesce(TV.VAT_PC,0)/(100+coalesce(TV.VAT_PC,0) ) ),2)) AS Cancel_TotVapVat, 
	SUM(ROUND( TR.tramount * TV.SP_INEXVAT/
                (TV1.Tv1Sum),2)) AS Cancel_TotVapIncl
	FROM company.loans LN 
	INNER JOIN company.trans TR ON TR.loan  = LN.loan 
		AND TR.client = LN.client 
		AND TR.brnum = LN.brnum
		AND ((TR.trdate BETWEEN '{0}' AND '{1}' ))
		and TR.trtype = 'C'
	INNER JOIN company.transvap TV ON TV.loan  = LN.loan 
	      AND TV.client = LN.client 
	      AND TV.brnum = LN.brnum 
        LEFT JOIN (select brnum, client, loan, SUM(SP_INEXVAT) AS Tv1Sum
		from company.transvap 
		group by brnum, client, loan) TV1 ON TV1.brnum = TR.brnum and TV1.client= TR.client and TV1.loan = TR.Loan
	WHERE LN.brnum IN ('{2}')
		and LN.nctrantype = 'VAP'
	GROUP BY 1, 2) VCN ON VSL.Payno = VCN.Payno AND VSL.brnum = VCN.brnum) BB ON BI."PayNo" = BB.payno
	AND BI."LegacyBranchNumber" = BB.brnum