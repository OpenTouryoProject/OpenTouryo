-- :P1はテスト用
SELECT 
  ShipperID, CompanyName, Phone
FROM
  Shippers
WHERE
  CompanyName <> :P1
ORDER BY %COLUMN% %SEQUENCE%
  