//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：TestMTC_mcn
//* クラス日本語名  ：Ｂ層のテスト（手動トランザクション制御－複数コネクション版）
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Db;

namespace WebForms_Sample
{
    /// <summary>
    /// TestMTC_mcn の概要の説明です
    /// </summary>
    public class TestMTC_mcn : MyFcBaseLogic
    {
        /// <summary>業務処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        protected override void UOC_DoAction(BaseParameterValue parameterValue, ref BaseReturnValue returnValue)
        {
            // 戻り値を生成しておく。
            returnValue = new MyReturnValue();

            // 自動トランザクションで開始したトランザクションを閉じる。
            this.GetDam().CommitTransaction();

            // コネクションを閉じる。
            this.GetDam().ConnectionClose();

            // データアクセス制御クラスをクリア。
            this.SetDam(null);

            // Dam用ワーク
            BaseDam damWork;

            // 共通Dao
            CmnDao cmnDao;

            // SQLの戻り値を受ける
            object obj;

            #region SQL Server

            #region SQL_NT

            // Damを生成
            damWork = new DamSqlSvr();
            // Damを初期化
            BaseLogic.InitDam("SQL_NT", damWork);
            // Damを設定
            this.SetDam("SQL_NT", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("SQL_NT"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_NT', 'SQL_NT')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("SQL_NT").CommitTransaction();
            //this.GetDam("SQL_NT").ConnectionClose();

            #endregion

            #region SQL_UC

            // Damを生成
            damWork = new DamSqlSvr();
            // Damを初期化
            BaseLogic.InitDam("SQL_UC", damWork);
            // Damを設定
            this.SetDam("SQL_UC", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("SQL_UC"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_UC', 'SQL_UC')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("SQL_UC").CommitTransaction();
            //this.GetDam("SQL_UC").ConnectionClose();

            #endregion

            #region SQL_RC

            // Damを生成
            damWork = new DamSqlSvr();
            // Damを初期化
            BaseLogic.InitDam("SQL_RC", damWork);
            // Damを設定
            this.SetDam("SQL_RC", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("SQL_RC"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_RC', 'SQL_RC')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("SQL_RC").CommitTransaction();
            //this.GetDam("SQL_RC").ConnectionClose();

            #endregion

            #region SQL_RR

            // Damを生成
            damWork = new DamSqlSvr();
            // Damを初期化
            BaseLogic.InitDam("SQL_RR", damWork);
            // Damを設定
            this.SetDam("SQL_RR", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("SQL_RR"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_RR', 'SQL_RR')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("SQL_RR").CommitTransaction();
            //this.GetDam("SQL_RR").ConnectionClose();

            #endregion

            #region SQL_SZ

            // Damを生成
            damWork = new DamSqlSvr();
            // Damを初期化
            BaseLogic.InitDam("SQL_SZ", damWork);
            // Damを設定
            this.SetDam("SQL_SZ", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("SQL_SZ"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_SZ', 'SQL_SZ')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("SQL_SZ").CommitTransaction();
            //this.GetDam("SQL_SZ").ConnectionClose();

            #endregion

            #region SQL_SS

            // Damを生成
            damWork = new DamSqlSvr();
            // Damを初期化
            BaseLogic.InitDam("SQL_SS", damWork);
            // Damを設定
            this.SetDam("SQL_SS", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("SQL_SS"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_SS', 'SQL_SS')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("SQL_SS").CommitTransaction();
            //this.GetDam("SQL_SS").ConnectionClose();

            #endregion

            #region SQL_DF

            // Damを生成
            damWork = new DamSqlSvr();
            // Damを初期化
            BaseLogic.InitDam("SQL_DF", damWork);
            // Damを設定
            this.SetDam("SQL_DF", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("SQL_DF"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_DF', 'SQL_DF')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("SQL_DF").CommitTransaction();
            //this.GetDam("SQL_DF").ConnectionClose();

            #endregion

            #endregion

            #region Oracle

            #region ODP_NT

            // Damを生成
            damWork = new DamManagedOdp();
            // Damを初期化
            BaseLogic.InitDam("ODP_NT", damWork);
            // Damを設定
            this.SetDam("ODP_NT", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("ODP_NT"));
            cmnDao.SQLText = "INSERT INTO Shippers(ShipperID, CompanyName, Phone) VALUES(TS_ShipperID.NEXTVAL, 'ODP_NT', 'ODP_NT')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("ODP_NT").CommitTransaction();
            //this.GetDam("ODP_NT").ConnectionClose();

            #endregion

            #region ODP_UC

            // ★ サポートされない分離レベル

            #endregion

            #region ODP_RC

            // Damを生成
            damWork = new DamManagedOdp();
            // Damを初期化
            BaseLogic.InitDam("ODP_RC", damWork);
            // Damを設定
            this.SetDam("ODP_RC", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("ODP_RC"));
            cmnDao.SQLText = "INSERT INTO Shippers(ShipperID, CompanyName, Phone) VALUES(TS_ShipperID.NEXTVAL, 'ODP_RC', 'ODP_RC')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("ODP_RC").CommitTransaction();
            //this.GetDam("ODP_RC").ConnectionClose();

            #endregion

            #region ODP_RR

            // ★ サポートされない分離レベル

            #endregion

            #region ODP_SZ

            // Damを生成
            damWork = new DamManagedOdp();
            // Damを初期化
            BaseLogic.InitDam("ODP_SZ", damWork);
            // Damを設定
            this.SetDam("ODP_SZ", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("ODP_SZ"));
            cmnDao.SQLText = "INSERT INTO Shippers(ShipperID, CompanyName, Phone) VALUES(TS_ShipperID.NEXTVAL, 'ODP_SZ', 'ODP_SZ')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("ODP_SZ").CommitTransaction();
            //this.GetDam("ODP_SZ").ConnectionClose();

            #endregion

            #region ODP_SS

            // ★ サポートされない分離レベル

            #endregion

            #region ODP_DF

            // Damを生成
            damWork = new DamManagedOdp();
            // Damを初期化
            BaseLogic.InitDam("ODP_DF", damWork);
            // Damを設定
            this.SetDam("ODP_DF", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("ODP_DF"));
            cmnDao.SQLText = "INSERT INTO Shippers(ShipperID, CompanyName, Phone) VALUES(TS_ShipperID.NEXTVAL, 'ODP_DF', 'ODP_DF')";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("ODP_DF").CommitTransaction();
            //this.GetDam("ODP_DF").ConnectionClose();

            #endregion

            #endregion
            
            #region MySQL

            #region MCN_NT

            // Damを生成
            damWork = new DamMySQL();
            // Damを初期化
            BaseLogic.InitDam("MCN_NT", damWork);
            // Damを設定
            this.SetDam("MCN_NT", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("MCN_NT"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_NT', 'MCN_NT');";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("MCN_NT").CommitTransaction();
            //this.GetDam("MCN_NT").ConnectionClose();

            #endregion

            #region MCN_UC

            // Damを生成
            damWork = new DamMySQL();
            // Damを初期化
            BaseLogic.InitDam("MCN_UC", damWork);
            // Damを設定
            this.SetDam("MCN_UC", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("MCN_UC"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_UC', 'MCN_UC');";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("MCN_UC").CommitTransaction();
            //this.GetDam("MCN_UC").ConnectionClose();

            #endregion

            #region MCN_RC

            // Damを生成
            damWork = new DamMySQL();
            // Damを初期化
            BaseLogic.InitDam("MCN_RC", damWork);
            // Damを設定
            this.SetDam("MCN_RC", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("MCN_RC"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_RC', 'MCN_RC');";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("MCN_RC").CommitTransaction();
            //this.GetDam("MCN_RC").ConnectionClose();

            #endregion

            #region MCN_RR

            // Damを生成
            damWork = new DamMySQL();
            // Damを初期化
            BaseLogic.InitDam("MCN_RR", damWork);
            // Damを設定
            this.SetDam("MCN_RR", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("MCN_RR"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_RR', 'MCN_RR');";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("MCN_RR").CommitTransaction();
            //this.GetDam("MCN_RR").ConnectionClose();

            #endregion

            #region MCN_SZ

            // Damを生成
            damWork = new DamMySQL();
            // Damを初期化
            BaseLogic.InitDam("MCN_SZ", damWork);
            // Damを設定
            this.SetDam("MCN_SZ", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("MCN_SZ"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_SZ', 'MCN_SZ');";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("MCN_SZ").CommitTransaction();
            //this.GetDam("MCN_SZ").ConnectionClose();

            #endregion

            #region MCN_SS

            // ★ サポートされない分離レベル

            #endregion

            #region MCN_DF

            // Damを生成
            damWork = new DamMySQL();
            // Damを初期化
            BaseLogic.InitDam("MCN_DF", damWork);
            // Damを設定
            this.SetDam("MCN_DF", damWork);

            // インサート
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam("MCN_DF"));
            cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_DF', 'MCN_DF');";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam("MCN_DF").CommitTransaction();
            //this.GetDam("MCN_DF").ConnectionClose();

            #endregion

            #endregion

            #region 終了時の状態選択

            #region Damの状態選択

            if ((parameterValue.ActionType.Split('%'))[2] == "UT")
            {
                // トランザクションあり
            }
            else if ((parameterValue.ActionType.Split('%'))[2] == "NT")
            {
                // トランザクションなし
                // → まえもってロールバックしておく

                #region ロールバック

                this.GetDam("SQL_NT").RollbackTransaction();
                this.GetDam("SQL_UC").RollbackTransaction();
                this.GetDam("SQL_RC").RollbackTransaction();
                this.GetDam("SQL_RR").RollbackTransaction();
                this.GetDam("SQL_SZ").RollbackTransaction();
                this.GetDam("SQL_SS").RollbackTransaction();
                this.GetDam("SQL_DF").RollbackTransaction();

                this.GetDam("ODP_NT").RollbackTransaction();
                //this.GetDam("ODP_UC").RollbackTransaction();
                this.GetDam("ODP_RC").RollbackTransaction();
                //this.GetDam("ODP_RR").RollbackTransaction();
                this.GetDam("ODP_SZ").RollbackTransaction();
                //this.GetDam("ODP_SS").RollbackTransaction();
                this.GetDam("ODP_DF").RollbackTransaction();

                this.GetDam("MCN_NT").RollbackTransaction();
                this.GetDam("MCN_UC").RollbackTransaction();
                this.GetDam("MCN_RC").RollbackTransaction();
                this.GetDam("MCN_RR").RollbackTransaction();
                this.GetDam("MCN_SZ").RollbackTransaction();
                //this.GetDam("MCN_SS").RollbackTransaction();
                this.GetDam("MCN_DF").RollbackTransaction();

                #endregion
            }
            else if ((parameterValue.ActionType.Split('%'))[2] == "NC")
            {
                // コネクションなし
                // → まえもってロールバック、コネクションクローズしておく
                //
                // ※ トランザクションを開始して
                //    コミットしないで閉じると、ロールバック扱い。

                #region ロールバック

                this.GetDam("SQL_NT").RollbackTransaction();
                this.GetDam("SQL_UC").RollbackTransaction();
                this.GetDam("SQL_RC").RollbackTransaction();
                this.GetDam("SQL_RR").RollbackTransaction();
                this.GetDam("SQL_SZ").RollbackTransaction();
                this.GetDam("SQL_SS").RollbackTransaction();
                this.GetDam("SQL_DF").RollbackTransaction();

                this.GetDam("ODP_NT").RollbackTransaction();
                //this.GetDam("ODP_UC").RollbackTransaction();
                this.GetDam("ODP_RC").RollbackTransaction();
                //this.GetDam("ODP_RR").RollbackTransaction();
                this.GetDam("ODP_SZ").RollbackTransaction();
                //this.GetDam("ODP_SS").RollbackTransaction();
                this.GetDam("ODP_DF").RollbackTransaction();

                this.GetDam("MCN_NT").RollbackTransaction();
                this.GetDam("MCN_UC").RollbackTransaction();
                this.GetDam("MCN_RC").RollbackTransaction();
                this.GetDam("MCN_RR").RollbackTransaction();
                this.GetDam("MCN_SZ").RollbackTransaction();
                //this.GetDam("MCN_SS").RollbackTransaction();
                this.GetDam("MCN_DF").RollbackTransaction();

                #endregion

                #region コネクションクローズ

                this.GetDam("SQL_NT").ConnectionClose();
                this.GetDam("SQL_UC").ConnectionClose();
                this.GetDam("SQL_RC").ConnectionClose();
                this.GetDam("SQL_RR").ConnectionClose();
                this.GetDam("SQL_SZ").ConnectionClose();
                this.GetDam("SQL_SS").ConnectionClose();
                this.GetDam("SQL_DF").ConnectionClose();

                this.GetDam("ODP_NT").ConnectionClose();
                //this.GetDam("ODP_UC").ConnectionClose();
                this.GetDam("ODP_RC").ConnectionClose();
                //this.GetDam("ODP_RR").ConnectionClose();
                this.GetDam("ODP_SZ").ConnectionClose();
                //this.GetDam("ODP_SS").ConnectionClose();
                this.GetDam("ODP_DF").ConnectionClose();

                this.GetDam("MCN_NT").ConnectionClose();
                this.GetDam("MCN_UC").ConnectionClose();
                this.GetDam("MCN_RC").ConnectionClose();
                this.GetDam("MCN_RR").ConnectionClose();
                this.GetDam("MCN_SZ").ConnectionClose();
                //this.GetDam("MCN_SS").ConnectionClose();
                this.GetDam("MCN_DF").ConnectionClose();

                #endregion
            }
            else if ((parameterValue.ActionType.Split('%'))[2] == "NULL")
            {
                // データアクセス制御クラス = Null
                // → まえもってロールバック、コネクションクローズ、Nullクリアしておく
                //
                // ※ トランザクションを開始して
                //    コミットしないで閉じると、ロールバック扱い。

                #region ロールバック

                this.GetDam("SQL_NT").RollbackTransaction();
                this.GetDam("SQL_UC").RollbackTransaction();
                this.GetDam("SQL_RC").RollbackTransaction();
                this.GetDam("SQL_RR").RollbackTransaction();
                this.GetDam("SQL_SZ").RollbackTransaction();
                this.GetDam("SQL_SS").RollbackTransaction();
                this.GetDam("SQL_DF").RollbackTransaction();

                this.GetDam("ODP_NT").RollbackTransaction();
                //this.GetDam("ODP_UC").RollbackTransaction();
                this.GetDam("ODP_RC").RollbackTransaction();
                //this.GetDam("ODP_RR").RollbackTransaction();
                this.GetDam("ODP_SZ").RollbackTransaction();
                //this.GetDam("ODP_SS").RollbackTransaction();
                this.GetDam("ODP_DF").RollbackTransaction();

                this.GetDam("MCN_NT").RollbackTransaction();
                this.GetDam("MCN_UC").RollbackTransaction();
                this.GetDam("MCN_RC").RollbackTransaction();
                this.GetDam("MCN_RR").RollbackTransaction();
                this.GetDam("MCN_SZ").RollbackTransaction();
                //this.GetDam("MCN_SS").RollbackTransaction();
                this.GetDam("MCN_DF").RollbackTransaction();

                #endregion

                #region コネクションクローズ

                this.GetDam("SQL_NT").ConnectionClose();
                this.GetDam("SQL_UC").ConnectionClose();
                this.GetDam("SQL_RC").ConnectionClose();
                this.GetDam("SQL_RR").ConnectionClose();
                this.GetDam("SQL_SZ").ConnectionClose();
                this.GetDam("SQL_SS").ConnectionClose();
                this.GetDam("SQL_DF").ConnectionClose();

                this.GetDam("ODP_NT").ConnectionClose();
                //this.GetDam("ODP_UC").ConnectionClose();
                this.GetDam("ODP_RC").ConnectionClose();
                //this.GetDam("ODP_RR").ConnectionClose();
                this.GetDam("ODP_SZ").ConnectionClose();
                //this.GetDam("ODP_SS").ConnectionClose();
                this.GetDam("ODP_DF").ConnectionClose();

                this.GetDam("MCN_NT").ConnectionClose();
                this.GetDam("MCN_UC").ConnectionClose();
                this.GetDam("MCN_RC").ConnectionClose();
                this.GetDam("MCN_RR").ConnectionClose();
                this.GetDam("MCN_SZ").ConnectionClose();
                //this.GetDam("MCN_SS").ConnectionClose();
                this.GetDam("MCN_DF").ConnectionClose();

                #endregion

                #region Nullクリア

                this.SetDam("SQL_NT", null);
                this.SetDam("SQL_UC", null);
                this.SetDam("SQL_RC", null);
                this.SetDam("SQL_RR", null);
                this.SetDam("SQL_SZ", null);
                this.SetDam("SQL_SS", null);
                this.SetDam("SQL_DF", null);

                this.SetDam("ODP_NT", null);
                //this.SetDam("ODP_UC",null);
                this.SetDam("ODP_RC", null);
                //this.SetDam("ODP_RR",null);
                this.SetDam("ODP_SZ", null);
                //this.SetDam("ODP_SS",null);
                this.SetDam("ODP_DF", null);

                this.SetDam("MCN_NT", null);
                this.SetDam("MCN_UC", null);
                this.SetDam("MCN_RC", null);
                this.SetDam("MCN_RR", null);
                this.SetDam("MCN_SZ", null);
                //this.SetDam("MCN_SS",null);
                this.SetDam("MCN_DF", null);

                #endregion
            }

            #endregion

            #region エラーのスロー

            if ((parameterValue.ActionType.Split('%'))[1] == "Business")
            {
                // 業務例外のスロー
                throw new BusinessApplicationException(
                    "ロールバックのテスト",
                    "ロールバックのテスト",
                    "エラー情報");
            }
            else if ((parameterValue.ActionType.Split('%'))[1] == "System")
            {
                // システム例外のスロー
                throw new BusinessSystemException(
                    "ロールバックのテスト",
                    "ロールバックのテスト");
            }
            else if ((parameterValue.ActionType.Split('%'))[1] == "Other")
            {
                // その他、一般的な例外のスロー
                throw new Exception("ロールバックのテスト");
            }
            else if ((parameterValue.ActionType.Split('%'))[1] == "Other-Business")
            {
                // その他、一般的な例外（業務例外へ振り替え）のスロー
                throw new Exception("Other-Business");
            }
            else if ((parameterValue.ActionType.Split('%'))[1] == "Other-System")
            {
                // その他、一般的な例外（システム例外へ振り替え）のスロー
                throw new Exception("Other-System");
            }

            #endregion

            #endregion

        }
    }
    
}