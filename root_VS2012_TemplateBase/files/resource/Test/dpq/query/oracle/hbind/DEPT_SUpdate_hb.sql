UPDATE
  "DEPT"
SET
  "DNAME" = :DNAME,
  "LOC" = :LOC
WHERE
  "DEPTNO" = :DEPTNO
  
/*PARAM* DEPTNO, int32[], 1, 2, 3, 4, 5, 6, 7, 8, 9 *PARAM*/
/*PARAM* DNAME, string[], aaa, bbb, ccc, ddd, eee, fff, ggg, hhh, iii *PARAM*/
/*PARAM* LOC, string[], aaa, bbb, ccc, ddd, eee, fff, ggg, hhh, iii *PARAM*/
