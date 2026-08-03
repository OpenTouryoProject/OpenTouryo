-- Daots_test_table1_S1_Insert
-- 2014/2/9 日立 太郎
INSERT INTO 
  [ts_test_table1]
    (
      [val],
      [ts]
    )
VALUES
    (
      @val,
      RAND()
    )
