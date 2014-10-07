-- P1はテスト用（←＠P1とコメントに書くとエラー）
SELECT 
  ShipperID, CompanyName, Phone
FROM
  Shippers
WHERE
  CompanyName != @P1
ORDER BY %COLUMN% %SEQUENCE%
  