SELECT * FROM Employees
WHERE
EmployeeID IN (SELECT EmployeeID FROM Employees)
AND Country IN (@P1,@P2)
ORDER BY %COLUMN% %SEQUENCE%

/*PARAM* P1, String, USA *PARAM*/
/*PARAM* P2, String, UK *PARAM*/
/*PARAM* COLUMN, EmployeeID *PARAM*/
/*PARAM* SEQUENCE, DESC *PARAM*/

-- パラメタ設定済み