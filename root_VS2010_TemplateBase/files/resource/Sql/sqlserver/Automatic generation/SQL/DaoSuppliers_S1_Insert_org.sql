-- DaoSuppliers_S1_Insert
-- 2014/2/9 日立 太郎
INSERT INTO 
  [Suppliers]
    (
      [SupplierID],
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
      @SupplierID,
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
