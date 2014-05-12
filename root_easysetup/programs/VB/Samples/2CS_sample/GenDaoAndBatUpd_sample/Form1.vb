'**********************************************************************************
'* サンプル アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Form1
'* クラス日本語名  ：自動生成したDaoの利用サンプル
'*                   ＋ データテーブルを使用したバッチ更新サンプル
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

' Windowアプリケーション
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

' 型情報
Imports GenDaoAndBatUpd_sample.Common
Imports GenDaoAndBatUpd_sample.Business

' System
Imports System
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Data
Imports System.Collections

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

Imports Touryo.Infrastructure.Business.RichClient.Business

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

Imports Touryo.Infrastructure.Framework.RichClient.Business

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>自動生成したDaoの利用サンプル＋データテーブルを使用したバッチ更新サンプル</summary>
Partial Public Class Form1
    Inherits Form
    ''' <summary>ユーザ情報</summary>
    Private myUserInfo As MyUserInfo

#Region "初期処理"

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()

        ' 埋め込まれたリソースモード
        MyBaseDao.UseEmbeddedResource = True
    End Sub

    ''' <summary>ロード イベント</summary>
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        '#Region "フローレイアウト風にする。"

        ' タブ
        Me.tabControl1.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        ' グリッド
        Me.dataGridView1.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.dataGridView2.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.dataGridView3.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        ' ピクチャ
        Me.pictureBox1.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        ' ボタンＡ
        Me.btnInsert1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)
        Me.btnInsert2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)

        Me.btnSelect1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)
        Me.btnSelect2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)

        Me.btnUpdate1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)
        Me.btnUpdate2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)

        Me.btnDelete1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)
        Me.btnDelete2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)

        ' ボタンＢ
        Me.btnSelectAll1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.btnSelectAll2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.btnSelectAll3.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        Me.btnClear1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.btnClear2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.btnClear3.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        Me.btnBatUpd.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        '#End Region

        ' ユーザ情報
        Me.myUserInfo = New MyUserInfo("userName", Environment.MachineName)
    End Sub

#End Region

#Region "データのロード"

    ''' <summary>Suppliersテーブルの取得</summary>
    Private Function GetSuppliers(ByVal controlId As String) As DataTable
        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, controlId, "SelectAll", "SQL", Me.myUserInfo)

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' 戻り値
        Return DirectCast(testReturnValue.dt, DataTable)
    End Function

    ''' <summary>Categoryテーブルの取得</summary>
    Private Function GetCategory(ByVal controlId As String) As DataTable
        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, controlId, "SelectAll", "SQL", Me.myUserInfo)

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' 戻り値
        Return DirectCast(testReturnValue.dt, DataTable)
    End Function

#End Region

#Region "タブ１"

#Region "色をクリア"

    ''' <summary>色をクリア</summary>
    Private Sub ClearColor1()
        txtSupplierID.BackColor = Color.White
        txtCompanyName.BackColor = Color.White
        txtContactName.BackColor = Color.White
        txtContactTitle.BackColor = Color.White
        txtAddress.BackColor = Color.White
        txtCity.BackColor = Color.White
        txtRegion.BackColor = Color.White
        txtPostalCode.BackColor = Color.White
        txtCountry.BackColor = Color.White
        txtPhone.BackColor = Color.White
        txtFax.BackColor = Color.White
        txtHomePage.BackColor = Color.White
    End Sub

#End Region

#Region "データグリッド１"

    ''' <summary>グリッド１にデータをロード</summary>
    Private Sub btnSelectAll1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll1.Click
        ' データをロード
        Me.dataGridView1.DataSource = Me.GetSuppliers(DirectCast(sender, Button).Name)
    End Sub

    ''' <summary>グリッド１をクリア</summary>
    Private Sub btnClear1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear1.Click
        ' 色のクリア
        Me.ClearColor1()

        ' クリア
        Me.dataGridView1.DataSource = Nothing
    End Sub

#End Region

#Region "静的SQLのCRUD"

    ''' <summary>インサート</summary>
    Private Sub btnInsert1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInsert1.Click
        ' 色のクリア
        Me.ClearColor1()

        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Insert", "SQL", Me.myUserInfo)

        testParameterValue.field1 = ""
        ' SupplierID
        testParameterValue.field2 = txtCompanyName.Text
        ' CompanyName
        txtCompanyName.BackColor = Color.LightYellow

        testParameterValue.field3 = txtContactName.Text
        ' ContactName
        txtContactName.BackColor = Color.LightYellow

        testParameterValue.field4 = txtContactTitle.Text
        ' ContactTitle
        txtContactTitle.BackColor = Color.LightYellow

        testParameterValue.field5 = txtAddress.Text
        ' Address
        txtAddress.BackColor = Color.LightYellow

        testParameterValue.field6 = txtCity.Text
        ' City
        txtCity.BackColor = Color.LightYellow

        testParameterValue.field7 = txtRegion.Text
        ' Region
        txtRegion.BackColor = Color.LightYellow

        testParameterValue.field8 = txtPostalCode.Text
        ' PostalCode
        txtPostalCode.BackColor = Color.LightYellow

        testParameterValue.field9 = txtCountry.Text
        ' Country
        txtCountry.BackColor = Color.LightYellow

        testParameterValue.field10 = txtPhone.Text
        ' Phone
        txtPhone.BackColor = Color.LightYellow

        testParameterValue.field11 = txtFax.Text
        ' Fax
        txtFax.BackColor = Color.LightYellow

        testParameterValue.field12 = txtHomePage.Text
        ' HomePage
        txtHomePage.BackColor = Color.LightYellow

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' データグリッドを更新
        Me.btnSelectAll1_Click(sender, e)
    End Sub

    ''' <summary>セレクト</summary>
    Private Sub btnSelect1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelect1.Click
        ' 色のクリア
        Me.ClearColor1()

        ' 主キーが無ければ、何もしない。
        If txtSupplierID.Text = "" Then
            MessageBox.Show("主キー（SupplierID）を入力してください。")
            Return
        End If

        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Select", "SQL", Me.myUserInfo)

        testParameterValue.field1 = txtSupplierID.Text
        ' SupplierID
        txtSupplierID.BackColor = Color.LightYellow

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' 戻り値
        txtCompanyName.Text = testReturnValue.field2.ToString()
        ' CompanyName
        txtContactName.Text = testReturnValue.field3.ToString()
        ' ContactName
        txtContactTitle.Text = testReturnValue.field4.ToString()
        ' ContactTitle
        txtAddress.Text = testReturnValue.field5.ToString()
        ' Address
        txtCity.Text = testReturnValue.field6.ToString()
        ' City
        txtRegion.Text = testReturnValue.field7.ToString()
        ' Region
        txtPostalCode.Text = testReturnValue.field8.ToString()
        ' PostalCode
        txtCountry.Text = testReturnValue.field9.ToString()
        ' Country
        txtPhone.Text = testReturnValue.field10.ToString()
        ' Phone
        txtFax.Text = testReturnValue.field11.ToString()
        ' Fax
        txtHomePage.Text = testReturnValue.field12.ToString()
        ' HomePage
    End Sub

    ''' <summary>アップデート</summary>
    Private Sub btnUpdate1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate1.Click
        ' 色のクリア
        Me.ClearColor1()

        ' 主キーが無ければ、何もしない。
        If txtSupplierID.Text = "" Then
            MessageBox.Show("主キー（SupplierID）を入力してください。")
            Return
        End If

        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Update", "SQL", Me.myUserInfo)

        testParameterValue.field1 = txtSupplierID.Text
        ' SupplierID
        txtSupplierID.BackColor = Color.LightYellow

        testParameterValue.field2_ForUpd = txtCompanyName.Text
        ' CompanyName
        txtCompanyName.BackColor = Color.LightYellow

        testParameterValue.field3_ForUpd = txtContactName.Text
        ' ContactName
        txtContactName.BackColor = Color.LightYellow

        testParameterValue.field4_ForUpd = txtContactTitle.Text
        ' ContactTitle
        txtContactTitle.BackColor = Color.LightYellow

        testParameterValue.field5_ForUpd = txtAddress.Text
        ' Address
        txtAddress.BackColor = Color.LightYellow

        testParameterValue.field6_ForUpd = txtCity.Text
        ' City
        txtCity.BackColor = Color.LightYellow

        testParameterValue.field7_ForUpd = txtRegion.Text
        ' Region
        txtRegion.BackColor = Color.LightYellow

        testParameterValue.field8_ForUpd = txtPostalCode.Text
        ' PostalCode
        txtPostalCode.BackColor = Color.LightYellow

        testParameterValue.field9_ForUpd = txtCountry.Text
        ' Country
        txtCountry.BackColor = Color.LightYellow

        testParameterValue.field10_ForUpd = txtPhone.Text
        ' Phone
        txtPhone.BackColor = Color.LightYellow

        testParameterValue.field11_ForUpd = txtFax.Text
        ' Fax
        txtFax.BackColor = Color.LightYellow

        testParameterValue.field12_ForUpd = txtHomePage.Text
        ' HomePage
        txtHomePage.BackColor = Color.LightYellow

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' データグリッドを更新
        Me.btnSelectAll1_Click(sender, e)
    End Sub

    ''' <summary>デリート</summary>
    Private Sub btnDelete1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete1.Click
        ' 色のクリア
        Me.ClearColor1()

        ' 主キーが無ければ、何もしない。
        If txtSupplierID.Text = "" Then
            MessageBox.Show("主キー（SupplierID）を入力してください。")
            Return
        End If

        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Delete", "SQL", Me.myUserInfo)

        testParameterValue.field1 = txtSupplierID.Text
        ' SupplierID
        txtSupplierID.BackColor = Color.LightYellow

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' データグリッドを更新
        Me.btnSelectAll1_Click(sender, e)
    End Sub

#End Region

#End Region

#Region "タブ２"

#Region "色をクリア"

    ''' <summary>色をクリア</summary>
    Private Sub ClearColor2()
        txtCategoryID.BackColor = Color.White
        txtCategoryName.BackColor = Color.White
        txtDescription.BackColor = Color.White
        'txtPicture.BackColor = Color.White;

        txtCategoryID_where.BackColor = Color.White
        txtCategoryName_where.BackColor = Color.White
        'txtDescription_where.BackColor = Color.White;
        'txtPicture_where.BackColor = Color.White;
    End Sub

#End Region

#Region "データグリッド２"

    ''' <summary>グリッド２にデータをロード</summary>
    Private Sub btnSelectAll2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll2.Click
        ' データをロード
        Me.dataGridView2.DataSource = Me.GetCategory(DirectCast(sender, Button).Name)
    End Sub

    ''' <summary>グリッド２をクリア</summary>
    Private Sub btnClear2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear2.Click
        ' 色のクリア
        Me.ClearColor2()

        ' クリア
        Me.dataGridView2.DataSource = Nothing
    End Sub

#End Region

#Region "動的SQLのCRUD"

    ''' <summary>インサート</summary>
    Private Sub btnInsert2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInsert2.Click
        ' 色のクリア
        Me.ClearColor2()

        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Insert", "SQL", Me.myUserInfo)

        ' データを入力できないのでパス
        'testParameterValue.field1 = this.txtCategoryID;          // CategoryID
        'this.txtCategoryID.BackColor = Color.LightYellow;

        testParameterValue.field2 = Me.txtCategoryName.Text
        ' CategoryName
        Me.txtCategoryName.BackColor = Color.LightYellow

        testParameterValue.field3 = Me.txtDescription.Text
        ' Description
        Me.txtDescription.BackColor = Color.LightYellow

        ' データを入力できないのでパス
        'testParameterValue.field4 = this.txtPicture.Text;      // Picture
        'this.txtPicture.BackColor = Color.LightYellow;

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' データグリッドを更新
        Me.btnSelectAll2_Click(sender, e)
    End Sub

    ''' <summary>セレクト</summary>
    Private Sub btnSelect2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelect2.Click
        ' 色のクリア
        Me.ClearColor2()

        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Select", "SQL", Me.myUserInfo)

        testParameterValue.field1_ForSearch = txtCategoryID_where.Text
        ' CategoryID_where
        txtCategoryID_where.BackColor = Color.LightYellow

        testParameterValue.field2_ForSearch = txtCategoryName_where.Text
        ' CategoryName_where
        txtCategoryName_where.BackColor = Color.LightYellow

        ' 検索条件に使えない↓

        'testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
        'txtDescription_where.BackColor = Color.LightYellow;

        'testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
        'txtPicture_where.BackColor = Color.LightYellow;

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' 戻り値を設定
        Me.dataGridView2.DataSource = testReturnValue.dt
    End Sub

    ''' <summary>アップデート</summary>
    Private Sub btnUpdate2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate2.Click
        ' 色のクリア
        Me.ClearColor2()

        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Update", "SQL", Me.myUserInfo)

        ' 更新値
        'testParameterValue.field1_ForUpd = txtCategoryID.Text;     // CategoryID
        'txtCategoryID.BackColor = Color.LightYellow;

        testParameterValue.field2_ForUpd = txtCategoryName.Text
        ' CategoryName
        txtCategoryName.BackColor = Color.LightYellow

        testParameterValue.field3_ForUpd = txtDescription.Text
        ' Description
        txtDescription.BackColor = Color.LightYellow

        ' 検索条件
        testParameterValue.field1_ForSearch = txtCategoryID_where.Text
        ' CategoryID_where
        txtCategoryID_where.BackColor = Color.LightYellow

        testParameterValue.field2_ForSearch = txtCategoryName_where.Text
        ' CategoryName_where
        txtCategoryName_where.BackColor = Color.LightYellow

        ' 検索条件に使えない↓

        'testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
        'txtDescription_where.BackColor = Color.LightYellow;

        'testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
        'txtPicture_where.BackColor = Color.LightYellow;

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' データグリッドを更新
        Me.btnSelectAll2_Click(sender, e)
    End Sub

    ''' <summary>デリート</summary>
    Private Sub btnDelete2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete2.Click
        ' 色のクリア
        Me.ClearColor2()

        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Delete", "SQL", Me.myUserInfo)

        ' 検索条件
        testParameterValue.field1_ForSearch = txtCategoryID_where.Text
        ' CategoryID_where
        txtCategoryID_where.BackColor = Color.LightYellow

        testParameterValue.field2_ForSearch = txtCategoryName_where.Text
        ' CategoryName_where
        txtCategoryName_where.BackColor = Color.LightYellow

        ' 検索条件に使えない↓

        'testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
        'txtDescription_where.BackColor = Color.LightYellow;

        'testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
        'txtPicture_where.BackColor = Color.LightYellow;

        ' Ｂ層呼び出し
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' データグリッドを更新
        Me.btnSelectAll2_Click(sender, e)
    End Sub

#End Region

#End Region

#Region "タブ３"

    ''' <summary>グリッド３にデータをロード</summary>
    Private Sub btnSelectAll3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll3.Click
        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "SelectAll", "SQL", Me.myUserInfo)

        ' Ｂ層呼び出し
        Dim lb As New LayerB_BatUpd()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' 戻り値を設定（列が自動的に作成されないようにする）
        Me.dataGridView3.Columns.Clear()
        Me.dataGridView3.AutoGenerateColumns = False
        Me.dataGridView3.DataSource = testReturnValue.dt

        '#Region "マスタのコンボ生成"

        '#Region "SupplierID - ComboBox"

        Dim dtSuppliers As DataTable = Me.GetSuppliers(DirectCast(sender, Button).Name)

        ' DataGridViewComboBoxColumnを作成
        Dim cmbColSuppliers As New DataGridViewComboBoxColumn()
        Me.InitDataGridViewComboBoxColumn(cmbColSuppliers)

        ' "SupplierID"列にバインドされているデータと関連付け、
        cmbColSuppliers.DataPropertyName = "SupplierID"
        ' ヘッダーのテキストを変更
        cmbColSuppliers.HeaderText = "Supplier"

        'DataGridViewComboBoxColumnのDataSourceを設定
        cmbColSuppliers.DataSource = dtSuppliers

        ' 実際の値が"SupplierID"列
        ' 表示するテキストが"CompanyName"列
        cmbColSuppliers.ValueMember = "SupplierID"
        cmbColSuppliers.DisplayMember = "CompanyName"

        '#End Region

        '#Region "CategoryID - ComboBox"

        Dim dtCategory As DataTable = Me.GetCategory("btnSelectAll3")

        ' DataGridViewComboBoxColumnを作成
        Dim cmbColCategory As New DataGridViewComboBoxColumn()
        Me.InitDataGridViewComboBoxColumn(cmbColCategory)

        ' "SupplierID"列にバインドされているデータと関連付け、
        cmbColCategory.DataPropertyName = "CategoryID"
        ' ヘッダーのテキストを変更
        cmbColCategory.HeaderText = "Category"

        ' DataGridViewComboBoxColumnのDataSourceを設定
        cmbColCategory.DataSource = dtCategory

        ' 実際の値が"CategoryID"列
        ' 表示するテキストが"CategoryName"列
        cmbColCategory.ValueMember = "CategoryID"
        cmbColCategory.DisplayMember = "CategoryName"

        '#End Region

        '#End Region

        '#Region "手動でデータバインド"

        ' はじめにクリア

        ' DataGridViewTextBoxColumn
        Dim textColumn As DataGridViewTextBoxColumn

        Dim checkColumn As DataGridViewCheckBoxColumn

        'データソースの"ProductID"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "ProductID"
        textColumn.Name = "ProductID"
        textColumn.HeaderText = "ProductID"

        ' 主キーは読み取り専用
        textColumn.[ReadOnly] = True

        Me.dataGridView3.Columns.Add(textColumn)

        'データソースの"ProductName"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "ProductName"
        textColumn.Name = "ProductName"
        textColumn.HeaderText = "ProductName"
        Me.dataGridView3.Columns.Add(textColumn)

        'データソースの"SupplierID"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "SupplierID"
        textColumn.Name = "SupplierID"
        textColumn.HeaderText = "SupplierID"
        Me.dataGridView3.Columns.Add(textColumn)

        ' 見えなくしてマスタをコンボを追加
        Me.dataGridView3.Columns("SupplierID").Visible = False
        Me.dataGridView3.Columns.Add(cmbColSuppliers)

        'データソースの"CategoryID"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "CategoryID"
        textColumn.Name = "CategoryID"
        textColumn.HeaderText = "CategoryID"
        Me.dataGridView3.Columns.Add(textColumn)

        ' 見えなくしてマスタをコンボを追加
        Me.dataGridView3.Columns("CategoryID").Visible = False
        Me.dataGridView3.Columns.Add(cmbColCategory)

        'データソースの"QuantityPerUnit"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "QuantityPerUnit"
        textColumn.Name = "QuantityPerUnit"
        textColumn.HeaderText = "QuantityPerUnit"
        Me.dataGridView3.Columns.Add(textColumn)

        'データソースの"UnitPrice"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "UnitPrice"
        textColumn.Name = "UnitPrice"
        textColumn.HeaderText = "UnitPrice"
        Me.dataGridView3.Columns.Add(textColumn)

        'データソースの"UnitsInStock"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "UnitsInStock"
        textColumn.Name = "UnitsInStock"
        textColumn.HeaderText = "UnitsInStock"
        Me.dataGridView3.Columns.Add(textColumn)

        'データソースの"UnitsOnOrder"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "UnitsOnOrder"
        textColumn.Name = "UnitsOnOrder"
        textColumn.HeaderText = "UnitsOnOrder"
        Me.dataGridView3.Columns.Add(textColumn)

        'データソースの"ReorderLevel"列をバインドする
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "ReorderLevel"
        textColumn.Name = "ReorderLevel"
        textColumn.HeaderText = "ReorderLevel"
        Me.dataGridView3.Columns.Add(textColumn)

        'データソースの"Discontinued"列をバインドする
        checkColumn = New DataGridViewCheckBoxColumn()
        checkColumn.DataPropertyName = "Discontinued"
        checkColumn.Name = "Discontinued"
        checkColumn.HeaderText = "Discontinued"
        Me.dataGridView3.Columns.Add(checkColumn)

        '#End Region
    End Sub

    ''' <summary>DataGridViewComboBoxColumnのスタイルを初期化する。</summary>
    Private Sub InitDataGridViewComboBoxColumn(ByVal cmbCol As DataGridViewComboBoxColumn)
        ' 現在のセルしかコンボボックスが表示されないようにする。
        cmbCol.DisplayStyleForCurrentCellOnly = True
        ' 編集モードの時だけコンボボックスを表示する。
        cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.[Nothing]

        ' マウスポインタ下のセルが強調表示されるようにする。
        cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
        ' マウスポインタ下のセルにポップアップが表示されるようにする。
        cmbCol.FlatStyle = FlatStyle.Popup
    End Sub

    ''' <summary>バッチ更新</summary>
    Private Sub btnBatUpd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnBatUpd.Click
        ' 引数
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "BatUpd", "SQL", Me.myUserInfo)

        ' 編集済みのDataTableを設定
        testParameterValue.dt = DirectCast(Me.dataGridView3.DataSource, DataTable)

        ' Ｂ層呼び出し
        Dim lb As New LayerB_BatUpd()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' コミット
        BaseLogic2CS.CommitAndClose()

        ' データグリッドを更新
        Me.btnSelectAll3_Click(sender, e)
    End Sub

    ''' <summary>データエラー時のイベントハンドラ</summary>
    Private Sub dataGridView3_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs)
        MessageBox.Show(e.Exception.Message)
    End Sub

    ''' <summary>クリア</summary>
    Private Sub btnClear3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear3.Click
        ' クリア
        Me.dataGridView3.DataSource = Nothing
    End Sub

#End Region
End Class
