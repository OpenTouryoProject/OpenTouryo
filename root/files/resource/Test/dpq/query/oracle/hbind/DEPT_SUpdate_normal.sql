UPDATE
  "DEPT"
SET
  "DNAME" = :DNAME,
  "LOC" = :LOC
WHERE
  "DEPTNO" = :DEPTNO
  
/*PARAM* DEPTNO, int32, 11 *PARAM*/
/*PARAM* DNAME, string, xxx *PARAM*/
/*PARAM* LOC, string, xxx *PARAM*/
