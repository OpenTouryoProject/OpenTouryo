-- Daots_test_table3_S1_Insert
-- 2014/2/9 日立 太郎
INSERT INTO 
  [ts_test_table3]
    (
      [id],
      [ts],
      [val]
    )
VALUES
    (
      @id,
      RAND(),
      @val
    )
