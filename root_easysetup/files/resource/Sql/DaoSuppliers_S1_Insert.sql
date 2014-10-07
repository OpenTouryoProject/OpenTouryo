-- DaoSuppliers_S1_Insert
-- 2013/1/10 日立 太郎
INSERT INTO 
  [Suppliers]
    (
      [CompanyName],
      [ContactName],
      [ContactTitle],
      [Address],
      [City],
      [Region],
      [PostalCode],
      [Country],
      [Phone],
      [Fax],
      [HomePage]
    )
VALUES
    (
      @CompanyName,
      @ContactName,
      @ContactTitle,
      @Address,
      @City,
      @Region,
      @PostalCode,
      @Country,
      @Phone,
      @Fax,
      @HomePage
    )
