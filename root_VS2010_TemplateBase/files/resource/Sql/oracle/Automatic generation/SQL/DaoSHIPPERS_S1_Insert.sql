-- DaoSHIPPERS_S1_Insert
-- 2014/2/9 日立 太郎
INSERT INTO 
  "SHIPPERS"
    (
      "SHIPPERID",
      "COMPANYNAME",
      "PHONE"
    )
VALUES
    (
      TS_ShipperID.NEXTVAL,
      :COMPANYNAME,
      :PHONE
    )
