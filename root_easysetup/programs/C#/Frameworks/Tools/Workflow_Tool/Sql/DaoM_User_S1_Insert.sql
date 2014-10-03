-- DaoM_User_S1_Insert
-- 2014/7/18 日立 太郎
INSERT INTO 
  [M_User]
    (
      [Id],
      [Section],
      [Name],
      [PositionTitlesId]
    )
VALUES
    (
      @Id,
      @Section,
      @Name,
      @PositionTitlesId
    )
