SELECT
  ShipperID, CompanyName, Phone
FROM
  Shippers
WHERE
  ShipperID = @p1
  AND CompanyName = @p2

/*PARAM* p2, String, Speedy Express *PARAM*/
/*PARAM* p1, String, 1 *PARAM*/