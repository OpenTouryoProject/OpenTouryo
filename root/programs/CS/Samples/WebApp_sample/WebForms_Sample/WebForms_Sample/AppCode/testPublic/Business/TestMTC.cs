//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：TestMTC
//* クラス日本語名  ：Ｂ層のテスト（手動トランザクション制御）
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
using System.Data;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Util;

namespace WebForms_Sample
{
    /// <summary>
    /// TestMTC の概要の説明です
    /// </summary>
    public class TestMTC : MyFcBaseLogic
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

            // カバレージ上げ用
            IDbConnection idcnn = null;
            IDbTransaction idtx = null;
            IDbCommand idcmd = null;
            IDataAdapter idapt = null;
            DataSet ds = null;

            // SQLの戻り値を受ける
            object obj;

            #region SQL Server

            damWork = new DamSqlSvr();

            #region 接続しない

            BaseLogic.InitDam("XXXX", damWork);
            this.SetDam(damWork);

            // なにもしない。

            // プロパティにアクセス（デバッガで確認）
            idcnn = ((DamSqlSvr)this.GetDam()).DamSqlConnection;
            idtx = ((DamSqlSvr)this.GetDam()).DamSqlTransaction;

            // nullの時に呼んだ場合。
            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region SQL_NT

            BaseLogic.InitDam("SQL_NT", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region SQL_UC

            BaseLogic.InitDam("SQL_UC", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region SQL_RC

            BaseLogic.InitDam("SQL_RC", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            // プロパティにアクセス（デバッガで確認）
            idcnn = ((DamSqlSvr)this.GetDam()).DamSqlConnection;
            idtx = ((DamSqlSvr)this.GetDam()).DamSqlTransaction;
            idcmd = ((DamSqlSvr)this.GetDam()).DamSqlCommand;
            idapt = ((DamSqlSvr)this.GetDam()).DamSqlDataAdapter;
            ds = new DataSet();
            idapt.Fill(ds);

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            // ２連続で呼んだ場合。
            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region SQL_RR

            BaseLogic.InitDam("SQL_RR", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region SQL_SZ

            BaseLogic.InitDam("SQL_SZ", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region SQL_SS

            BaseLogic.InitDam("SQL_SS", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region SQL_DF

            BaseLogic.InitDam("SQL_DF", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #endregion

            #region Oracle

            damWork = new DamManagedOdp();

            #region 接続しない

            BaseLogic.InitDam("XXXX", damWork);
            this.SetDam(damWork);

            // なにもしない。

            // プロパティにアクセス（デバッガで確認）
            idcnn = ((DamManagedOdp)this.GetDam()).DamOracleConnection;
            idtx = ((DamManagedOdp)this.GetDam()).DamOracleTransaction;

            // nullの時に呼んだ場合。
            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region ODP_NT

            BaseLogic.InitDam("ODP_NT", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region ODP_UC

            // ★ サポートされない分離レベル

            #endregion

            #region ODP_RC

            BaseLogic.InitDam("ODP_RC", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            // プロパティにアクセス（デバッガで確認）
            idcnn = ((DamManagedOdp)this.GetDam()).DamOracleConnection;
            idtx = ((DamManagedOdp)this.GetDam()).DamOracleTransaction;
            idcmd = ((DamManagedOdp)this.GetDam()).DamOracleCommand;
            idapt = ((DamManagedOdp)this.GetDam()).DamOracleDataAdapter;
            ds = new DataSet();
            idapt.Fill(ds);

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            // ２連続で呼んだ場合。
            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region ODP_RR

            // ★ サポートされない分離レベル

            #endregion

            #region ODP_SZ

            BaseLogic.InitDam("ODP_SZ", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region ODP_SS

            // ★ サポートされない分離レベル

            #endregion

            #region ODP_DF

            BaseLogic.InitDam("ODP_DF", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #endregion
            
            #region MySQL

            damWork = new DamMySQL();

            #region 接続しない

            BaseLogic.InitDam("XXXX", damWork);
            this.SetDam(damWork);

            // なにもしない。

            // プロパティにアクセス（デバッガで確認）
            idcnn = ((DamMySQL)this.GetDam()).DamMySqlConnection;
            idtx = ((DamMySQL)this.GetDam()).DamMySqlTransaction;

            // nullの時に呼んだ場合。
            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region MCN_NT

            BaseLogic.InitDam("MCN_NT", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            //this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region MCN_UC

            BaseLogic.InitDam("MCN_UC", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region MCN_RC

            BaseLogic.InitDam("MCN_RC", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            // プロパティにアクセス（デバッガで確認）
            idcnn = ((DamMySQL)this.GetDam()).DamMySqlConnection;
            idtx = ((DamMySQL)this.GetDam()).DamMySqlTransaction;
            idcmd = ((DamMySQL)this.GetDam()).DamMySqlCommand;
            idapt = ((DamMySQL)this.GetDam()).DamMySqlDataAdapter;
            ds = new DataSet();
            idapt.Fill(ds);

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            // ２連続で呼んだ場合。
            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region MCN_RR

            BaseLogic.InitDam("MCN_RR", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region MCN_SZ

            BaseLogic.InitDam("MCN_SZ", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #region MCN_SS

            // ★ サポートされない分離レベル

            #endregion

            #region MCN_DF

            BaseLogic.InitDam("MCN_DF", damWork);
            this.SetDam(damWork);

            // 行数
            // Damを直接使用することもできるが、
            // 通常は、データアクセスにはDaoを使用する。        
            cmnDao = new CmnDao(this.GetDam());
            cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS";
            obj = (object)cmnDao.ExecSelectScalar();

            this.GetDam().CommitTransaction();
            this.GetDam().ConnectionClose();

            #endregion

            #endregion

            #region エラー処理（ロールバックのテスト）

            if ((parameterValue.ActionType.Split('%'))[1] != "-")
            {
                #region エラー時のDamの状態選択

                if ((parameterValue.ActionType.Split('%'))[2] == "UT")
                {
                    // トランザクションあり
                    damWork = new DamSqlSvr();
                    damWork.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_SQL"));
                    damWork.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
                    this.SetDam(damWork);
                }
                else if ((parameterValue.ActionType.Split('%'))[2] == "NT")
                {
                    // トランザクションなし
                    damWork = new DamSqlSvr();
                    damWork.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_SQL"));
                    this.SetDam(damWork);
                }
                else if ((parameterValue.ActionType.Split('%'))[2] == "NC")
                {
                    // コネクションなし
                    damWork = new DamSqlSvr();
                    this.SetDam(damWork);
                }
                else if ((parameterValue.ActionType.Split('%'))[2] == "NULL")
                {
                    // データアクセス制御クラス = Null
                    this.SetDam(null);
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
            }

            #endregion

        }
    }
    
}