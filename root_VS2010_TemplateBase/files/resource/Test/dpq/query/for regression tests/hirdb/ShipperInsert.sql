INSERT INTO 
  Shippers (ShipperID, CompanyName, Phone)
VALUES
  (NEXT VALUE FOR TS_ShipperID, @P2, @P3)

/*PARAM* P2, string, くろだ *PARAM*/
/*PARAM* P3, string, ひろし *PARAM*/
