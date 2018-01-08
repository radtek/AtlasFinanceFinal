SELECT 
	CASE 
		WHEN COUNT(*) > 0 THEN true 
		ELSE false 
	END AS noofenquiries
FROM company.cs_ctrl
WHERE nlrenqref = '{0}'