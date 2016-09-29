﻿'**********************************************************************************
'* フレームワーク・テストクラス（Ｂ層）
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：TestMTC_mcn
'* クラス日本語名  ：Ｂ層のテスト（手動トランザクション制御－複数コネクション版）
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

' System
Imports System

' データセット利用
Imports System.Data

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Common

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Exceptions

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Util

''' <summary>
''' TestMTC_mcn の概要の説明です
''' </summary>
Public Class TestMTC_mcn
    Inherits MyBaseLogic
    ''' <summary>
    ''' 業務処理を実装
    ''' </summary>
    ''' <param name="parameterValue">引数クラス</param>
    ''' <param name="returnValue">戻り値クラス</param>
    Protected Overloads Overrides Sub UOC_DoAction(ByVal parameterValue As BaseParameterValue, ByRef returnValue As BaseReturnValue)
        ' 戻り値を生成しておく。
        returnValue = New MyReturnValue()

        ' 自動トランザクションで開始したトランザクションを閉じる。
        Me.GetDam().CommitTransaction()

        ' コネクションを閉じる。
        Me.GetDam().ConnectionClose()

        ' データアクセス制御クラスをクリア。
        Me.SetDam(Nothing)

        ' Dam用ワーク
        Dim damWork As BaseDam

        ' 共通Dao
        Dim cmnDao As CmnDao

        ' SQLの戻り値を受ける
        Dim obj As Object

        '#Region "SQL Server"

        '#Region "SQL_NT"

        ' Damを生成
        damWork = New DamSqlSvr()
        ' Damを初期化
        BaseLogic.InitDam("SQL_NT", damWork)
        ' Damを設定
        Me.SetDam("SQL_NT", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("SQL_NT"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_NT', 'SQL_NT')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("SQL_NT").CommitTransaction();
        'this.GetDam("SQL_NT").ConnectionClose();

        '#End Region

        '#Region "SQL_UC"

        ' Damを生成
        damWork = New DamSqlSvr()
        ' Damを初期化
        BaseLogic.InitDam("SQL_UC", damWork)
        ' Damを設定
        Me.SetDam("SQL_UC", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("SQL_UC"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_UC', 'SQL_UC')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("SQL_UC").CommitTransaction();
        'this.GetDam("SQL_UC").ConnectionClose();

        '#End Region

        '#Region "SQL_RC"

        ' Damを生成
        damWork = New DamSqlSvr()
        ' Damを初期化
        BaseLogic.InitDam("SQL_RC", damWork)
        ' Damを設定
        Me.SetDam("SQL_RC", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("SQL_RC"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_RC', 'SQL_RC')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("SQL_RC").CommitTransaction();
        'this.GetDam("SQL_RC").ConnectionClose();

        '#End Region

        '#Region "SQL_RR"

        ' Damを生成
        damWork = New DamSqlSvr()
        ' Damを初期化
        BaseLogic.InitDam("SQL_RR", damWork)
        ' Damを設定
        Me.SetDam("SQL_RR", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("SQL_RR"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_RR', 'SQL_RR')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("SQL_RR").CommitTransaction();
        'this.GetDam("SQL_RR").ConnectionClose();

        '#End Region

        '#Region "SQL_SZ"

        ' Damを生成
        damWork = New DamSqlSvr()
        ' Damを初期化
        BaseLogic.InitDam("SQL_SZ", damWork)
        ' Damを設定
        Me.SetDam("SQL_SZ", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("SQL_SZ"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_SZ', 'SQL_SZ')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("SQL_SZ").CommitTransaction();
        'this.GetDam("SQL_SZ").ConnectionClose();

        '#End Region

        '#Region "SQL_SS"

        ' Damを生成
        damWork = New DamSqlSvr()
        ' Damを初期化
        BaseLogic.InitDam("SQL_SS", damWork)
        ' Damを設定
        Me.SetDam("SQL_SS", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("SQL_SS"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_SS', 'SQL_SS')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("SQL_SS").CommitTransaction();
        'this.GetDam("SQL_SS").ConnectionClose();

        '#End Region

        '#Region "SQL_DF"

        ' Damを生成
        damWork = New DamSqlSvr()
        ' Damを初期化
        BaseLogic.InitDam("SQL_DF", damWork)
        ' Damを設定
        Me.SetDam("SQL_DF", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("SQL_DF"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('SQL_DF', 'SQL_DF')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("SQL_DF").CommitTransaction();
        'this.GetDam("SQL_DF").ConnectionClose();

        '#End Region

        '#End Region

        '#Region "Oracle"

        '#Region "ODP2_NT"

        ' Damを生成
        damWork = New DamOraOdp()
        ' Damを初期化
        BaseLogic.InitDam("ODP2_NT", damWork)
        ' Damを設定
        Me.SetDam("ODP2_NT", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("ODP2_NT"))
        cmnDao.SQLText = "INSERT INTO Shippers(ShipperID, CompanyName, Phone) VALUES(TS_ShipperID.NEXTVAL, 'ODP2_NT', 'ODP2_NT')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("ODP2_NT").CommitTransaction();
        'this.GetDam("ODP2_NT").ConnectionClose();

        '#End Region

        '#Region "ODP2_UC"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "ODP2_RC"

        ' Damを生成
        damWork = New DamOraOdp()
        ' Damを初期化
        BaseLogic.InitDam("ODP2_RC", damWork)
        ' Damを設定
        Me.SetDam("ODP2_RC", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("ODP2_RC"))
        cmnDao.SQLText = "INSERT INTO Shippers(ShipperID, CompanyName, Phone) VALUES(TS_ShipperID.NEXTVAL, 'ODP2_RC', 'ODP2_RC')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("ODP2_RC").CommitTransaction();
        'this.GetDam("ODP2_RC").ConnectionClose();

        '#End Region

        '#Region "ODP2_RR"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "ODP2_SZ"

        ' Damを生成
        damWork = New DamOraOdp()
        ' Damを初期化
        BaseLogic.InitDam("ODP2_SZ", damWork)
        ' Damを設定
        Me.SetDam("ODP2_SZ", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("ODP2_SZ"))
        cmnDao.SQLText = "INSERT INTO Shippers(ShipperID, CompanyName, Phone) VALUES(TS_ShipperID.NEXTVAL, 'ODP2_SZ', 'ODP2_SZ')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("ODP2_SZ").CommitTransaction();
        'this.GetDam("ODP2_SZ").ConnectionClose();

        '#End Region

        '#Region "ODP2_SS"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "ODP2_DF"

        ' Damを生成
        damWork = New DamOraOdp()
        ' Damを初期化
        BaseLogic.InitDam("ODP2_DF", damWork)
        ' Damを設定
        Me.SetDam("ODP2_DF", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("ODP2_DF"))
        cmnDao.SQLText = "INSERT INTO Shippers(ShipperID, CompanyName, Phone) VALUES(TS_ShipperID.NEXTVAL, 'ODP2_DF', 'ODP2_DF')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("ODP2_DF").CommitTransaction();
        'this.GetDam("ODP2_DF").ConnectionClose();

        '#End Region

        '#End Region

        '#Region "DB2"

        '#Region "DB2_NT"

        ' Damを生成
        damWork = New DamDB2()
        ' Damを初期化
        BaseLogic.InitDam("DB2_NT", damWork)
        ' Damを設定
        Me.SetDam("DB2_NT", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("DB2_NT"))
        cmnDao.SQLText = "INSERT INTO Shippers (ShipperID, CompanyName, Phone) VALUES(NEXTVAL for TS_ShipperID, 'DB2_NT', 'DB2_NT')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("DB2_NT").CommitTransaction();
        'this.GetDam("DB2_NT").ConnectionClose();

        '#End Region

        '#Region "DB2_UC"

        ' Damを生成
        damWork = New DamDB2()
        ' Damを初期化
        BaseLogic.InitDam("DB2_UC", damWork)
        ' Damを設定
        Me.SetDam("DB2_UC", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("DB2_UC"))
        cmnDao.SQLText = "INSERT INTO Shippers (ShipperID, CompanyName, Phone) VALUES(NEXTVAL for TS_ShipperID, 'DB2_UC', 'DB2_UC')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("DB2_UC").CommitTransaction();
        'this.GetDam("DB2_UC").ConnectionClose();

        '#End Region

        '#Region "DB2_RC"

        ' Damを生成
        damWork = New DamDB2()
        ' Damを初期化
        BaseLogic.InitDam("DB2_RC", damWork)
        ' Damを設定
        Me.SetDam("DB2_RC", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("DB2_UC"))
        cmnDao.SQLText = "INSERT INTO Shippers (ShipperID, CompanyName, Phone) VALUES(NEXTVAL for TS_ShipperID, 'DB2_RC', 'DB2_RC')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("DB2_RC").CommitTransaction();
        'this.GetDam("DB2_RC").ConnectionClose();

        '#End Region

        '#Region "DB2_RR"

        ' Damを生成
        damWork = New DamDB2()
        ' Damを初期化
        BaseLogic.InitDam("DB2_RR", damWork)
        ' Damを設定
        Me.SetDam("DB2_RR", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("DB2_RR"))
        cmnDao.SQLText = "INSERT INTO Shippers (ShipperID, CompanyName, Phone) VALUES(NEXTVAL for TS_ShipperID, 'DB2_RR', 'DB2_RR')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("DB2_RR").CommitTransaction();
        'this.GetDam("DB2_RR").ConnectionClose();

        '#End Region

        '#Region "DB2_SZ"

        ' Damを生成
        damWork = New DamDB2()
        ' Damを初期化
        BaseLogic.InitDam("DB2_SZ", damWork)
        ' Damを設定
        Me.SetDam("DB2_SZ", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("DB2_SZ"))
        cmnDao.SQLText = "INSERT INTO Shippers (ShipperID, CompanyName, Phone) VALUES(NEXTVAL for TS_ShipperID, 'DB2_SZ', 'DB2_SZ')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("DB2_SZ").CommitTransaction();
        'this.GetDam("DB2_SZ").ConnectionClose();

        '#End Region

        '#Region "DB2_SS"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "DB2_DF"

        ' Damを生成
        damWork = New DamDB2()
        ' Damを初期化
        BaseLogic.InitDam("DB2_DF", damWork)
        ' Damを設定
        Me.SetDam("DB2_DF", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("DB2_DF"))
        cmnDao.SQLText = "INSERT INTO Shippers (ShipperID, CompanyName, Phone) VALUES(NEXTVAL for TS_ShipperID, 'DB2_DF', 'DB2_DF')"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("DB2_DF").CommitTransaction();
        'this.GetDam("DB2_DF").ConnectionClose();

        '#End Region

        '#End Region

        '#Region "MySQL"

        '#Region "MCN_NT"

        ' Damを生成
        damWork = New DamMySQL()
        ' Damを初期化
        BaseLogic.InitDam("MCN_NT", damWork)
        ' Damを設定
        Me.SetDam("MCN_NT", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("MCN_NT"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_NT', 'MCN_NT');"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("MCN_NT").CommitTransaction();
        'this.GetDam("MCN_NT").ConnectionClose();

        '#End Region

        '#Region "MCN_UC"

        ' Damを生成
        damWork = New DamMySQL()
        ' Damを初期化
        BaseLogic.InitDam("MCN_UC", damWork)
        ' Damを設定
        Me.SetDam("MCN_UC", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("MCN_UC"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_UC', 'MCN_UC');"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("MCN_UC").CommitTransaction();
        'this.GetDam("MCN_UC").ConnectionClose();

        '#End Region

        '#Region "MCN_RC"

        ' Damを生成
        damWork = New DamMySQL()
        ' Damを初期化
        BaseLogic.InitDam("MCN_RC", damWork)
        ' Damを設定
        Me.SetDam("MCN_RC", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("MCN_RC"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_RC', 'MCN_RC');"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("MCN_RC").CommitTransaction();
        'this.GetDam("MCN_RC").ConnectionClose();

        '#End Region

        '#Region "MCN_RR"

        ' Damを生成
        damWork = New DamMySQL()
        ' Damを初期化
        BaseLogic.InitDam("MCN_RR", damWork)
        ' Damを設定
        Me.SetDam("MCN_RR", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("MCN_RR"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_RR', 'MCN_RR');"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("MCN_RR").CommitTransaction();
        'this.GetDam("MCN_RR").ConnectionClose();

        '#End Region

        '#Region "MCN_SZ"

        ' Damを生成
        damWork = New DamMySQL()
        ' Damを初期化
        BaseLogic.InitDam("MCN_SZ", damWork)
        ' Damを設定
        Me.SetDam("MCN_SZ", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("MCN_SZ"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_SZ', 'MCN_SZ');"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("MCN_SZ").CommitTransaction();
        'this.GetDam("MCN_SZ").ConnectionClose();

        '#End Region

        '#Region "MCN_SS"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "MCN_DF"

        ' Damを生成
        damWork = New DamMySQL()
        ' Damを初期化
        BaseLogic.InitDam("MCN_DF", damWork)
        ' Damを設定
        Me.SetDam("MCN_DF", damWork)

        ' インサート
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam("MCN_DF"))
        cmnDao.SQLText = "INSERT INTO Shippers(CompanyName, Phone) VALUES('MCN_DF', 'MCN_DF');"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam("MCN_DF").CommitTransaction();
        'this.GetDam("MCN_DF").ConnectionClose();

        '#End Region

        '#End Region

        '#Region "終了時の状態選択"

        '#Region "Damの状態選択"

        ' トランザクションあり
        If (parameterValue.ActionType.Split("%"c))(2) = "UT" Then
        ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NT" Then
            ' トランザクションなし
            ' → まえもってロールバックしておく

            '#Region "ロールバック"

            Me.GetDam("SQL_NT").RollbackTransaction()
            Me.GetDam("SQL_UC").RollbackTransaction()
            Me.GetDam("SQL_RC").RollbackTransaction()
            Me.GetDam("SQL_RR").RollbackTransaction()
            Me.GetDam("SQL_SZ").RollbackTransaction()
            Me.GetDam("SQL_SS").RollbackTransaction()
            Me.GetDam("SQL_DF").RollbackTransaction()

            Me.GetDam("ODP2_NT").RollbackTransaction()
            'this.GetDam("ODP2_UC").RollbackTransaction();
            Me.GetDam("ODP2_RC").RollbackTransaction()
            'this.GetDam("ODP2_RR").RollbackTransaction();
            Me.GetDam("ODP2_SZ").RollbackTransaction()
            'this.GetDam("ODP2_SS").RollbackTransaction();
            Me.GetDam("ODP2_DF").RollbackTransaction()

            Me.GetDam("DB2_NT").RollbackTransaction()
            Me.GetDam("DB2_UC").RollbackTransaction()
            Me.GetDam("DB2_RC").RollbackTransaction()
            Me.GetDam("DB2_RR").RollbackTransaction()
            Me.GetDam("DB2_SZ").RollbackTransaction()
            'this.GetDam("DB2_SS").RollbackTransaction();
            Me.GetDam("DB2_DF").RollbackTransaction()

            Me.GetDam("MCN_NT").RollbackTransaction()
            Me.GetDam("MCN_UC").RollbackTransaction()
            Me.GetDam("MCN_RC").RollbackTransaction()
            Me.GetDam("MCN_RR").RollbackTransaction()
            Me.GetDam("MCN_SZ").RollbackTransaction()
            'this.GetDam("MCN_SS").RollbackTransaction();

            '#End Region
            Me.GetDam("MCN_DF").RollbackTransaction()
        ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NC" Then
            ' コネクションなし
            ' → まえもってロールバック、コネクションクローズしておく
            '
            ' ※ トランザクションを開始して
            '    コミットしないで閉じると、ロールバック扱い。

            '#Region "ロールバック"

            Me.GetDam("SQL_NT").RollbackTransaction()
            Me.GetDam("SQL_UC").RollbackTransaction()
            Me.GetDam("SQL_RC").RollbackTransaction()
            Me.GetDam("SQL_RR").RollbackTransaction()
            Me.GetDam("SQL_SZ").RollbackTransaction()
            Me.GetDam("SQL_SS").RollbackTransaction()
            Me.GetDam("SQL_DF").RollbackTransaction()

            Me.GetDam("ODP2_NT").RollbackTransaction()
            'this.GetDam("ODP2_UC").RollbackTransaction();
            Me.GetDam("ODP2_RC").RollbackTransaction()
            'this.GetDam("ODP2_RR").RollbackTransaction();
            Me.GetDam("ODP2_SZ").RollbackTransaction()
            'this.GetDam("ODP2_SS").RollbackTransaction();
            Me.GetDam("ODP2_DF").RollbackTransaction()

            Me.GetDam("DB2_NT").RollbackTransaction()
            Me.GetDam("DB2_UC").RollbackTransaction()
            Me.GetDam("DB2_RC").RollbackTransaction()
            Me.GetDam("DB2_RR").RollbackTransaction()
            Me.GetDam("DB2_SZ").RollbackTransaction()
            'this.GetDam("DB2_SS").RollbackTransaction();
            Me.GetDam("DB2_DF").RollbackTransaction()

            Me.GetDam("MCN_NT").RollbackTransaction()
            Me.GetDam("MCN_UC").RollbackTransaction()
            Me.GetDam("MCN_RC").RollbackTransaction()
            Me.GetDam("MCN_RR").RollbackTransaction()
            Me.GetDam("MCN_SZ").RollbackTransaction()
            'this.GetDam("MCN_SS").RollbackTransaction();
            Me.GetDam("MCN_DF").RollbackTransaction()

            '#End Region

            '#Region "コネクションクローズ"

            Me.GetDam("SQL_NT").ConnectionClose()
            Me.GetDam("SQL_UC").ConnectionClose()
            Me.GetDam("SQL_RC").ConnectionClose()
            Me.GetDam("SQL_RR").ConnectionClose()
            Me.GetDam("SQL_SZ").ConnectionClose()
            Me.GetDam("SQL_SS").ConnectionClose()
            Me.GetDam("SQL_DF").ConnectionClose()

            Me.GetDam("ODP2_NT").ConnectionClose()
            'this.GetDam("ODP2_UC").ConnectionClose();
            Me.GetDam("ODP2_RC").ConnectionClose()
            'this.GetDam("ODP2_RR").ConnectionClose();
            Me.GetDam("ODP2_SZ").ConnectionClose()
            'this.GetDam("ODP2_SS").ConnectionClose();
            Me.GetDam("ODP2_DF").ConnectionClose()

            Me.GetDam("DB2_NT").ConnectionClose()
            Me.GetDam("DB2_UC").ConnectionClose()
            Me.GetDam("DB2_RC").ConnectionClose()
            Me.GetDam("DB2_RR").ConnectionClose()
            Me.GetDam("DB2_SZ").ConnectionClose()
            'this.GetDam("DB2_SS").ConnectionClose();
            Me.GetDam("DB2_DF").ConnectionClose()

            Me.GetDam("MCN_NT").ConnectionClose()
            Me.GetDam("MCN_UC").ConnectionClose()
            Me.GetDam("MCN_RC").ConnectionClose()
            Me.GetDam("MCN_RR").ConnectionClose()
            Me.GetDam("MCN_SZ").ConnectionClose()
            'this.GetDam("MCN_SS").ConnectionClose();

            '#End Region
            Me.GetDam("MCN_DF").ConnectionClose()
        ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NULL" Then
            ' データアクセス制御クラス = Null
            ' → まえもってロールバック、コネクションクローズ、Nullクリアしておく
            '
            ' ※ トランザクションを開始して
            '    コミットしないで閉じると、ロールバック扱い。

            '#Region "ロールバック"

            Me.GetDam("SQL_NT").RollbackTransaction()
            Me.GetDam("SQL_UC").RollbackTransaction()
            Me.GetDam("SQL_RC").RollbackTransaction()
            Me.GetDam("SQL_RR").RollbackTransaction()
            Me.GetDam("SQL_SZ").RollbackTransaction()
            Me.GetDam("SQL_SS").RollbackTransaction()
            Me.GetDam("SQL_DF").RollbackTransaction()

            Me.GetDam("ODP2_NT").RollbackTransaction()
            'this.GetDam("ODP2_UC").RollbackTransaction();
            Me.GetDam("ODP2_RC").RollbackTransaction()
            'this.GetDam("ODP2_RR").RollbackTransaction();
            Me.GetDam("ODP2_SZ").RollbackTransaction()
            'this.GetDam("ODP2_SS").RollbackTransaction();
            Me.GetDam("ODP2_DF").RollbackTransaction()

            Me.GetDam("DB2_NT").RollbackTransaction()
            Me.GetDam("DB2_UC").RollbackTransaction()
            Me.GetDam("DB2_RC").RollbackTransaction()
            Me.GetDam("DB2_RR").RollbackTransaction()
            Me.GetDam("DB2_SZ").RollbackTransaction()
            'this.GetDam("DB2_SS").RollbackTransaction();
            Me.GetDam("DB2_DF").RollbackTransaction()

            Me.GetDam("MCN_NT").RollbackTransaction()
            Me.GetDam("MCN_UC").RollbackTransaction()
            Me.GetDam("MCN_RC").RollbackTransaction()
            Me.GetDam("MCN_RR").RollbackTransaction()
            Me.GetDam("MCN_SZ").RollbackTransaction()
            'this.GetDam("MCN_SS").RollbackTransaction();
            Me.GetDam("MCN_DF").RollbackTransaction()

            '#End Region

            '#Region "コネクションクローズ"

            Me.GetDam("SQL_NT").ConnectionClose()
            Me.GetDam("SQL_UC").ConnectionClose()
            Me.GetDam("SQL_RC").ConnectionClose()
            Me.GetDam("SQL_RR").ConnectionClose()
            Me.GetDam("SQL_SZ").ConnectionClose()
            Me.GetDam("SQL_SS").ConnectionClose()
            Me.GetDam("SQL_DF").ConnectionClose()

            Me.GetDam("ODP2_NT").ConnectionClose()
            'this.GetDam("ODP2_UC").ConnectionClose();
            Me.GetDam("ODP2_RC").ConnectionClose()
            'this.GetDam("ODP2_RR").ConnectionClose();
            Me.GetDam("ODP2_SZ").ConnectionClose()
            'this.GetDam("ODP2_SS").ConnectionClose();
            Me.GetDam("ODP2_DF").ConnectionClose()

            Me.GetDam("DB2_NT").ConnectionClose()
            Me.GetDam("DB2_UC").ConnectionClose()
            Me.GetDam("DB2_RC").ConnectionClose()
            Me.GetDam("DB2_RR").ConnectionClose()
            Me.GetDam("DB2_SZ").ConnectionClose()
            'this.GetDam("DB2_SS").ConnectionClose();
            Me.GetDam("DB2_DF").ConnectionClose()

            Me.GetDam("MCN_NT").ConnectionClose()
            Me.GetDam("MCN_UC").ConnectionClose()
            Me.GetDam("MCN_RC").ConnectionClose()
            Me.GetDam("MCN_RR").ConnectionClose()
            Me.GetDam("MCN_SZ").ConnectionClose()
            'this.GetDam("MCN_SS").ConnectionClose();
            Me.GetDam("MCN_DF").ConnectionClose()

            '#End Region

            '#Region "Nullクリア"

            Me.SetDam("SQL_NT", Nothing)
            Me.SetDam("SQL_UC", Nothing)
            Me.SetDam("SQL_RC", Nothing)
            Me.SetDam("SQL_RR", Nothing)
            Me.SetDam("SQL_SZ", Nothing)
            Me.SetDam("SQL_SS", Nothing)
            Me.SetDam("SQL_DF", Nothing)

            Me.SetDam("ODP2_NT", Nothing)
            'this.SetDam("ODP2_UC",null);
            Me.SetDam("ODP2_RC", Nothing)
            'this.SetDam("ODP2_RR",null);
            Me.SetDam("ODP2_SZ", Nothing)
            'this.SetDam("ODP2_SS",null);
            Me.SetDam("ODP2_DF", Nothing)

            Me.SetDam("DB2_NT", Nothing)
            Me.SetDam("DB2_UC", Nothing)
            Me.SetDam("DB2_RC", Nothing)
            Me.SetDam("DB2_RR", Nothing)
            Me.SetDam("DB2_SZ", Nothing)
            'this.SetDam("DB2_SS",null);
            Me.SetDam("DB2_DF", Nothing)

            Me.SetDam("MCN_NT", Nothing)
            Me.SetDam("MCN_UC", Nothing)
            Me.SetDam("MCN_RC", Nothing)
            Me.SetDam("MCN_RR", Nothing)
            Me.SetDam("MCN_SZ", Nothing)
            'this.SetDam("MCN_SS",null);

            '#End Region
            Me.SetDam("MCN_DF", Nothing)
        End If

        '#End Region

        '#Region "エラーのスロー"

        If (parameterValue.ActionType.Split("%"c))(1) = "Business" Then
            ' 業務例外のスロー
            Throw New BusinessApplicationException("ロールバックのテスト", "ロールバックのテスト", "エラー情報")
        ElseIf (parameterValue.ActionType.Split("%"c))(1) = "System" Then
            ' システム例外のスロー
            Throw New BusinessSystemException("ロールバックのテスト", "ロールバックのテスト")
        ElseIf (parameterValue.ActionType.Split("%"c))(1) = "Other" Then
            ' その他、一般的な例外のスロー
            Throw New Exception("ロールバックのテスト")
        ElseIf (parameterValue.ActionType.Split("%"c))(1) = "Other-Business" Then
            ' その他、一般的な例外（業務例外へ振り替え）のスロー
            Throw New Exception("Other-Business")
        ElseIf (parameterValue.ActionType.Split("%"c))(1) = "Other-System" Then
            ' その他、一般的な例外（システム例外へ振り替え）のスロー
            Throw New Exception("Other-System")
        End If

        '#End Region

        '#End Region

    End Sub
End Class
