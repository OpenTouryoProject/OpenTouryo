-- DaoProducts_S1_Insert
-- 2014/2/9 日立 太郎
INSERT INTO 
  [Products]
    (
      [ProductName],
      [SupplierID],
      [CategoryID],
      [QuantityPerUnit],
      [UnitPrice],
      [UnitsInStock],
      [UnitsOnOrder],
      [ReorderLevel],
      [Discontinued]
    )
VALUES
    (
      @ProductName,
      @SupplierID,
      @CategoryID,
      @QuantityPerUnit,
      @UnitPrice,
      @UnitsInStock,
      @UnitsOnOrder,
      @ReorderLevel,
      @Discontinued
    )
