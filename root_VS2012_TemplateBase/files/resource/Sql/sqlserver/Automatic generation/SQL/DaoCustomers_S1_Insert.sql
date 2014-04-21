-- DaoCustomers_S1_Insert
-- 2014/2/9 日立 太郎
INSERT INTO 
  [Customers]
    (
      [CustomerID],
      [CompanyName],
      [ContactName],
      [ContactTitle],
      [Address],
      [City],
      [Region],
      [PostalCode],
      [Country],
      [Phone],
      [Fax]
    )
VALUES
    (
      @CustomerID,
      @CompanyName,
      @ContactName,
      @ContactTitle,
      @Address,
      @City,
      @Region,
      @PostalCode,
      @Country,
      @Phone,
      @Fax
    )
