-- DaoShippers_S1_Insert
-- 2014/2/9 日立 太郎
INSERT INTO 
  [Shippers]
    (
      [ShipperID],
      [CompanyName],
      [Phone]
    )
VALUES
    (
      @ShipperID,
      @CompanyName,
      @Phone
    )
