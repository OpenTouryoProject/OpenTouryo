-- DaoShippers_S1_Insert
-- 2013/1/10 日立 太郎
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
