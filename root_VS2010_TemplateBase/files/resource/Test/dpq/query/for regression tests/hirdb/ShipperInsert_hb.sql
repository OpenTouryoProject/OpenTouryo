INSERT INTO 
  Shippers (ShipperID, CompanyName, Phone)
VALUES
  (NEXT VALUE FOR TS_ShipperID, @P2, @P3)

/*PARAM* P2, string[], くろだ, クロス, 黒で, 白だ, 嘘だ *PARAM*/
/*PARAM* P3, string[], ひろし, ぴろし, ぴろす, ぴろぴろし, ぴろしき *PARAM*/
