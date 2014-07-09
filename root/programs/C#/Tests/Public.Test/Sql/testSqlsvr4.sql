SELECT * FROM Employees
WHERE
FirstName=@FN
AND LastName=@LN1
AND EmployeeID IN (SELECT EmployeeID FROM Employees WHERE LastName=@LN2)
AND ReportsTo IN ( @P1 , @P2 )
ORDER BY %COLUMN% %SEQUENCE%

/*PARAM* FN, String, Nancy *PARAM*/
/*PARAM* LN1, String, Davolio *PARAM*/
/*PARAM* LN2, String, Davolio *PARAM*/
/*PARAM* P1, String, 2 *PARAM*/
/*PARAM* P2, String, 5 *PARAM*/
/*PARAM* COLUMN, EmployeeID *PARAM*/
/*PARAM* SEQUENCE, DESC *PARAM*/

-- パラメタ設定済み