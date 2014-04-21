-- ＠P1はテスト用
SELECT 
  ShipperID, CompanyName, Phone
FROM
  Shippers
WHERE
  CompanyName != @P1
ORDER BY %COLUMN% %SEQUENCE%

/*PARAM* P1, string, Speedy Express *PARAM*/
/*PARAM* COLUMN, CompanyName *PARAM*/
/*PARAM* SEQUENCE, DESC *PARAM*/