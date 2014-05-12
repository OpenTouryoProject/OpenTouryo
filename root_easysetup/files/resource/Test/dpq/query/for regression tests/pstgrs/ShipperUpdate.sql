UPDATE
  Shippers
SET
  CompanyName = :P2, Phone = @P3
WHERE
  ShipperID = :P1

/*PARAM* P1, string, 4 *PARAM*/
/*PARAM* P2, string, くろだ *PARAM*/
/*PARAM* P3, string, ぴろし *PARAM*/

/* 静的クエリはパラメタ記号、混在OKだが、基本的に避けてね。 */