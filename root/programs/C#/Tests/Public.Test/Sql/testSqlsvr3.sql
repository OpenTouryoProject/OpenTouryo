SELECT * FROM Employees
WHERE
EmployeeID IN (SELECT EmployeeID FROM Employees)
AND ReportsTo IN ( @P1 , @P2 )
ORDER BY %COLUMN% %SEQUENCE%

/*PARAM* P1, String, 2 *PARAM*/
/*PARAM* P2, String, 5 *PARAM*/
/*PARAM* COLUMN, EmployeeID *PARAM*/
/*PARAM* SEQUENCE, DESC *PARAM*/

-- パラメタ設定済み