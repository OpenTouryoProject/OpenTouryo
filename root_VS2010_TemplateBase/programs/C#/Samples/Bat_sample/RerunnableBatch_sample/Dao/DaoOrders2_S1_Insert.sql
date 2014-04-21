-- DaoOrders2_S1_Insert
-- 2014/2/9 日立 太郎
INSERT INTO 
  [Orders2]
    (
      [OrderID],
      [CustomerID],
      [EmployeeID],
      [OrderDate],
      [RequiredDate],
      [ShippedDate],
      [ShipVia],
      [Freight],
      [ShipName],
      [ShipAddress],
      [ShipCity],
      [ShipRegion],
      [ShipPostalCode],
      [ShipCountry]
    )
VALUES
    (
      @OrderID,
      @CustomerID,
      @EmployeeID,
      @OrderDate,
      @RequiredDate,
      @ShippedDate,
      @ShipVia,
      @Freight,
      @ShipName,
      @ShipAddress,
      @ShipCity,
      @ShipRegion,
      @ShipPostalCode,
      @ShipCountry
    )
