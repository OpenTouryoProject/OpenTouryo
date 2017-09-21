'**********************************************************************************
'* フレームワーク・テストクラス（Ｂ層）
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：TestMTC
'* クラス日本語名  ：Ｂ層のテスト（手動トランザクション制御）
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

Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Util

''' <summary>
''' TestMTC の概要の説明です
''' </summary>
Public Class TestMTC
    Inherits MyFcBaseLogic
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

        ' カバレージ上げ用
        Dim idcnn As IDbConnection = Nothing
        Dim idtx As IDbTransaction = Nothing
        Dim idcmd As IDbCommand = Nothing
        Dim idapt As IDataAdapter = Nothing
        Dim ds As DataSet = Nothing

        ' SQLの戻り値を受ける
        Dim obj As Object

        '#Region "SQL Server"

        damWork = New DamSqlSvr()

        '#Region "接続しない"

        BaseLogic.InitDam("XXXX", damWork)
        Me.SetDam(damWork)

        ' なにもしない。

        ' プロパティにアクセス（デバッガで確認）
        idcnn = DirectCast(Me.GetDam(), DamSqlSvr).DamSqlConnection
        idtx = DirectCast(Me.GetDam(), DamSqlSvr).DamSqlTransaction

        ' nullの時に呼んだ場合。
        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "SQL_NT"

        BaseLogic.InitDam("SQL_NT", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam().CommitTransaction();
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "SQL_UC"

        BaseLogic.InitDam("SQL_UC", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "SQL_RC"

        BaseLogic.InitDam("SQL_RC", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        ' プロパティにアクセス（デバッガで確認）
        idcnn = DirectCast(Me.GetDam(), DamSqlSvr).DamSqlConnection
        idtx = DirectCast(Me.GetDam(), DamSqlSvr).DamSqlTransaction
        idcmd = DirectCast(Me.GetDam(), DamSqlSvr).DamSqlCommand
        idapt = DirectCast(Me.GetDam(), DamSqlSvr).DamSqlDataAdapter
        ds = New DataSet()
        idapt.Fill(ds)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        ' ２連続で呼んだ場合。
        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "SQL_RR"

        BaseLogic.InitDam("SQL_RR", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "SQL_SZ"

        BaseLogic.InitDam("SQL_SZ", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "SQL_SS"

        BaseLogic.InitDam("SQL_SS", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "SQL_DF"

        BaseLogic.InitDam("SQL_DF", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#End Region

        '#Region "Oracle"

        damWork = New DamManagedOdp()

        '#Region "接続しない"

        BaseLogic.InitDam("XXXX", damWork)
        Me.SetDam(damWork)

        ' なにもしない。

        ' プロパティにアクセス（デバッガで確認）
        idcnn = DirectCast(Me.GetDam(), DamManagedOdp).DamOracleConnection
        idtx = DirectCast(Me.GetDam(), DamManagedOdp).DamOracleTransaction

        ' nullの時に呼んだ場合。
        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "ODP_NT"

        BaseLogic.InitDam("ODP_NT", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam().CommitTransaction();
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "ODP_UC"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "ODP_RC"

        BaseLogic.InitDam("ODP_RC", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        ' プロパティにアクセス（デバッガで確認）
        idcnn = DirectCast(Me.GetDam(), DamManagedOdp).DamOracleConnection
        idtx = DirectCast(Me.GetDam(), DamManagedOdp).DamOracleTransaction
        idcmd = DirectCast(Me.GetDam(), DamManagedOdp).DamOracleCommand
        idapt = DirectCast(Me.GetDam(), DamManagedOdp).DamOracleDataAdapter
        ds = New DataSet()
        idapt.Fill(ds)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        ' ２連続で呼んだ場合。
        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "ODP_RR"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "ODP_SZ"

        BaseLogic.InitDam("ODP_SZ", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "ODP_SS"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "ODP_DF"

        BaseLogic.InitDam("ODP_DF", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#End Region

        '#Region "MySQL"

        damWork = New DamMySQL()

        '#Region "接続しない"

        BaseLogic.InitDam("XXXX", damWork)
        Me.SetDam(damWork)

        ' なにもしない。

        ' プロパティにアクセス（デバッガで確認）
        idcnn = DirectCast(Me.GetDam(), DamMySQL).DamMySqlConnection
        idtx = DirectCast(Me.GetDam(), DamMySQL).DamMySqlTransaction

        ' nullの時に呼んだ場合。
        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "MCN_NT"

        BaseLogic.InitDam("MCN_NT", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        'this.GetDam().CommitTransaction();
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "MCN_UC"

        BaseLogic.InitDam("MCN_UC", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "MCN_RC"

        BaseLogic.InitDam("MCN_RC", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        ' プロパティにアクセス（デバッガで確認）
        idcnn = DirectCast(Me.GetDam(), DamMySQL).DamMySqlConnection
        idtx = DirectCast(Me.GetDam(), DamMySQL).DamMySqlTransaction
        idcmd = DirectCast(Me.GetDam(), DamMySQL).DamMySqlCommand
        idapt = DirectCast(Me.GetDam(), DamMySQL).DamMySqlDataAdapter
        ds = New DataSet()
        idapt.Fill(ds)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        ' ２連続で呼んだ場合。
        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "MCN_RR"

        BaseLogic.InitDam("MCN_RR", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "MCN_SZ"

        BaseLogic.InitDam("MCN_SZ", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#Region "MCN_SS"

        ' ★ サポートされない分離レベル

        '#End Region

        '#Region "MCN_DF"

        BaseLogic.InitDam("MCN_DF", damWork)
        Me.SetDam(damWork)

        ' 行数
        ' Damを直接使用することもできるが、
        ' 通常は、データアクセスにはDaoを使用する。        
        cmnDao = New CmnDao(Me.GetDam())
        cmnDao.SQLText = "SELECT COUNT(*) FROM SHIPPERS"
        obj = DirectCast(cmnDao.ExecSelectScalar(), Object)

        Me.GetDam().CommitTransaction()
        Me.GetDam().ConnectionClose()

        '#End Region

        '#End Region

        '#Region "エラー処理（ロールバックのテスト）"

        If (parameterValue.ActionType.Split("%"c))(1) <> "-" Then
            '#Region "エラー時のDamの状態選択"

            If (parameterValue.ActionType.Split("%"c))(2) = "UT" Then
                ' トランザクションあり
                damWork = New DamSqlSvr()
                damWork.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_SQLSvr"))
                damWork.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted)
                Me.SetDam(damWork)
            ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NT" Then
                ' トランザクションなし
                damWork = New DamSqlSvr()
                damWork.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_SQLSvr"))
                Me.SetDam(damWork)
            ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NC" Then
                ' コネクションなし
                damWork = New DamSqlSvr()
                Me.SetDam(damWork)
            ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NULL" Then
                ' データアクセス制御クラス = Null
                Me.SetDam(Nothing)
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

                '#End Region
            End If
        End If

        '#End Region

    End Sub
End Class
