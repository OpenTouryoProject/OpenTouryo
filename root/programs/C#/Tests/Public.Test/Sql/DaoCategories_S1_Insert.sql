-- DaoCategories_S1_Insert
-- 2013/1/10 日立 太郎
INSERT INTO 
  [Categories]
    (
      [CategoryID],
      [CategoryName],
      [Description],
      [Picture]
    )
VALUES
    (
      @CategoryID,
      @CategoryName,
      @Description,
      @Picture
    )
