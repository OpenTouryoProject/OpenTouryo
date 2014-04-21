SELECT 
  ShipperID, CompanyName, Phone
FROM
  Shippers
WHERE
  ShipperID = @P1

/*PARAM* P1, int32, -2147483645 *PARAM*/