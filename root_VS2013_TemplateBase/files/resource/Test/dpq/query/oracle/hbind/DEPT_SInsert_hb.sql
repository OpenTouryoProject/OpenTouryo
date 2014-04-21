INSERT INTO 
  "DEPT"
    (
      "DEPTNO",
      "DNAME",
      "LOC"
    )
  VALUES
    (
      :DEPTNO,
      :DNAME,
      :LOC
    )

/*PARAM* DEPTNO, int32[], 1, 2, 3, 4, 5, 6, 7, 8, 9 *PARAM*/
/*PARAM* DNAME, string[], AAA, BBB, CCC, DDD, EEE, FFF, GGG, HHH, III *PARAM*/
/*PARAM* LOC, string[], AAA, BBB, CCC, DDD, EEE, FFF, GGG, HHH, III *PARAM*/
