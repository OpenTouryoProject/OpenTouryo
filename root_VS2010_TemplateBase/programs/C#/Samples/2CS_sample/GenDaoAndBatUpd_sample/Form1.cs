//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Form1
//* クラス日本語名  ：自動生成したDaoの利用サンプル
//*                   ＋ データテーブルを使用したバッチ更新サンプル
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

// Windowアプリケーション
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

// 型情報
using GenDaoAndBatUpd_sample.Common;
using GenDaoAndBatUpd_sample.Business;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

using Touryo.Infrastructure.Business.RichClient.Business;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

using Touryo.Infrastructure.Framework.RichClient.Business;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace GenDaoAndBatUpd_sample
{
    /// <summary>自動生成したDaoの利用サンプル＋データテーブルを使用したバッチ更新サンプル</summary>
    public partial class Form1 : Form
    {
        /// <summary>ユーザ情報</summary>
        MyUserInfo myUserInfo;

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();

            // 埋め込まれたリソースモード
            MyBaseDao.UseEmbeddedResource = true;
        }

        /// <summary>ロード イベント</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            #region フローレイアウト風にする。

            // タブ
            this.tabControl1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            // グリッド
            this.dataGridView1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.dataGridView2.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.dataGridView3.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            // ピクチャ
            this.pictureBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            // ボタンＡ
            this.btnInsert1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnInsert2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            this.btnSelect1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnSelect2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            this.btnUpdate1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnUpdate2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            this.btnDelete1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnDelete2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            // ボタンＢ
            this.btnSelectAll1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnSelectAll2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnSelectAll3.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            this.btnClear1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnClear2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnClear3.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            this.btnBatUpd.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            
            #endregion

            // ユーザ情報
            this.myUserInfo = new MyUserInfo("userName", Environment.MachineName);
        }

        #endregion

        #region データのロード

        /// <summary>Suppliersテーブルの取得</summary>
        private DataTable GetSuppliers(string controlId)
        {
            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, controlId, "SelectAll", "SQL", this.myUserInfo);

            // Ｂ層呼び出し
            LayerB_Static lb = new LayerB_Static();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // 戻り値
            return (DataTable)testReturnValue.dt;
        }

        /// <summary>Categoryテーブルの取得</summary>
        private DataTable GetCategory(string controlId)
        {
            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, controlId, "SelectAll", "SQL", this.myUserInfo);

            // Ｂ層呼び出し
            LayerB_Dynamic lb = new LayerB_Dynamic();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // 戻り値
            return (DataTable)testReturnValue.dt;
        }

        #endregion

        #region タブ１

        #region 色をクリア

        /// <summary>色をクリア</summary>
        private void ClearColor1()
        {
            txtSupplierID.BackColor = Color.White;
            txtCompanyName.BackColor = Color.White;
            txtContactName.BackColor = Color.White;
            txtContactTitle.BackColor = Color.White;
            txtAddress.BackColor = Color.White;
            txtCity.BackColor = Color.White;
            txtRegion.BackColor = Color.White;
            txtPostalCode.BackColor = Color.White;
            txtCountry.BackColor = Color.White;
            txtPhone.BackColor = Color.White;
            txtFax.BackColor = Color.White;
            txtHomePage.BackColor = Color.White;
        }

        #endregion

        #region データグリッド１

        /// <summary>グリッド１にデータをロード</summary>
        private void btnSelectAll1_Click(object sender, EventArgs e)
        {
            // データをロード
            this.dataGridView1.DataSource = this.GetSuppliers(((Button)sender).Name);
        }

        /// <summary>グリッド１をクリア</summary>
        private void btnClear1_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor1();

            // クリア
            this.dataGridView1.DataSource = null;
        }

        #endregion

        # region 静的SQLのCRUD

        /// <summary>インサート</summary>
        private void btnInsert1_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor1();

            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Insert", "SQL", this.myUserInfo);

            testParameterValue.field1 = "";                     // SupplierID

            testParameterValue.field2 = txtCompanyName.Text;    // CompanyName
            txtCompanyName.BackColor = Color.LightYellow;

            testParameterValue.field3 = txtContactName.Text;    // ContactName
            txtContactName.BackColor = Color.LightYellow;

            testParameterValue.field4 = txtContactTitle.Text;   // ContactTitle
            txtContactTitle.BackColor = Color.LightYellow;

            testParameterValue.field5 = txtAddress.Text;        // Address
            txtAddress.BackColor = Color.LightYellow;

            testParameterValue.field6 = txtCity.Text;           // City
            txtCity.BackColor = Color.LightYellow;

            testParameterValue.field7 = txtRegion.Text;         // Region
            txtRegion.BackColor = Color.LightYellow;

            testParameterValue.field8 = txtPostalCode.Text;     // PostalCode
            txtPostalCode.BackColor = Color.LightYellow;

            testParameterValue.field9 = txtCountry.Text;        // Country
            txtCountry.BackColor = Color.LightYellow;

            testParameterValue.field10 = txtPhone.Text;         // Phone
            txtPhone.BackColor = Color.LightYellow;

            testParameterValue.field11 = txtFax.Text;           // Fax
            txtFax.BackColor = Color.LightYellow;

            testParameterValue.field12 = txtHomePage.Text;      // HomePage
            txtHomePage.BackColor = Color.LightYellow;

            // Ｂ層呼び出し
            LayerB_Static lb = new LayerB_Static();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // データグリッドを更新
            this.btnSelectAll1_Click(sender,e);
        }

        /// <summary>セレクト</summary>
        private void btnSelect1_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor1();

            // 主キーが無ければ、何もしない。
            if (txtSupplierID.Text == "")
            {
                MessageBox.Show("主キー（SupplierID）を入力してください。");
                return;
            }

            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Select", "SQL", this.myUserInfo);

            testParameterValue.field1 = txtSupplierID.Text;     // SupplierID
            txtSupplierID.BackColor = Color.LightYellow;

            // Ｂ層呼び出し
            LayerB_Static lb = new LayerB_Static();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // 戻り値
            txtCompanyName.Text = testReturnValue.field2.ToString();    // CompanyName
            txtContactName.Text = testReturnValue.field3.ToString();    // ContactName
            txtContactTitle.Text = testReturnValue.field4.ToString();   // ContactTitle
            txtAddress.Text = testReturnValue.field5.ToString();        // Address
            txtCity.Text = testReturnValue.field6.ToString();           // City
            txtRegion.Text = testReturnValue.field7.ToString();         // Region
            txtPostalCode.Text = testReturnValue.field8.ToString();     // PostalCode
            txtCountry.Text = testReturnValue.field9.ToString();        // Country
            txtPhone.Text = testReturnValue.field10.ToString();         // Phone
            txtFax.Text = testReturnValue.field11.ToString();           // Fax
            txtHomePage.Text = testReturnValue.field12.ToString();      // HomePage
        }

        /// <summary>アップデート</summary>
        private void btnUpdate1_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor1();

            // 主キーが無ければ、何もしない。
            if (txtSupplierID.Text == "")
            {
                MessageBox.Show("主キー（SupplierID）を入力してください。");
                return;
            }

            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Update", "SQL", this.myUserInfo);

            testParameterValue.field1 = txtSupplierID.Text;            // SupplierID
            txtSupplierID.BackColor = Color.LightYellow;

            testParameterValue.field2_ForUpd = txtCompanyName.Text;    // CompanyName
            txtCompanyName.BackColor = Color.LightYellow;

            testParameterValue.field3_ForUpd = txtContactName.Text;    // ContactName
            txtContactName.BackColor = Color.LightYellow;

            testParameterValue.field4_ForUpd = txtContactTitle.Text;   // ContactTitle
            txtContactTitle.BackColor = Color.LightYellow;

            testParameterValue.field5_ForUpd = txtAddress.Text;        // Address
            txtAddress.BackColor = Color.LightYellow;

            testParameterValue.field6_ForUpd = txtCity.Text;           // City
            txtCity.BackColor = Color.LightYellow;

            testParameterValue.field7_ForUpd = txtRegion.Text;         // Region
            txtRegion.BackColor = Color.LightYellow;

            testParameterValue.field8_ForUpd = txtPostalCode.Text;     // PostalCode
            txtPostalCode.BackColor = Color.LightYellow;

            testParameterValue.field9_ForUpd = txtCountry.Text;        // Country
            txtCountry.BackColor = Color.LightYellow;

            testParameterValue.field10_ForUpd = txtPhone.Text;         // Phone
            txtPhone.BackColor = Color.LightYellow;

            testParameterValue.field11_ForUpd = txtFax.Text;           // Fax
            txtFax.BackColor = Color.LightYellow;

            testParameterValue.field12_ForUpd = txtHomePage.Text;      // HomePage
            txtHomePage.BackColor = Color.LightYellow;

            // Ｂ層呼び出し
            LayerB_Static lb = new LayerB_Static();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // データグリッドを更新
            this.btnSelectAll1_Click(sender, e);
        }

        /// <summary>デリート</summary>
        private void btnDelete1_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor1();

            // 主キーが無ければ、何もしない。
            if (txtSupplierID.Text == "")
            {
                MessageBox.Show("主キー（SupplierID）を入力してください。");
                return;
            }

            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Delete", "SQL", this.myUserInfo);

            testParameterValue.field1 = txtSupplierID.Text;     // SupplierID
            txtSupplierID.BackColor = Color.LightYellow;

            // Ｂ層呼び出し
            LayerB_Static lb = new LayerB_Static();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // データグリッドを更新
            this.btnSelectAll1_Click(sender, e);
        }

        #endregion

        #endregion

        #region タブ２

        #region 色をクリア

        /// <summary>色をクリア</summary>
        private void ClearColor2()
        {
            txtCategoryID.BackColor = Color.White;
            txtCategoryName.BackColor = Color.White;
            txtDescription.BackColor = Color.White;
            //txtPicture.BackColor = Color.White;

            txtCategoryID_where.BackColor = Color.White;
            txtCategoryName_where.BackColor = Color.White;
            //txtDescription_where.BackColor = Color.White;
            //txtPicture_where.BackColor = Color.White;
        }

        #endregion

        #region データグリッド２

        /// <summary>グリッド２にデータをロード</summary>
        private void btnSelectAll2_Click(object sender, EventArgs e)
        {
            // データをロード
            this.dataGridView2.DataSource = this.GetCategory(((Button)sender).Name);    
        }

        /// <summary>グリッド２をクリア</summary>
        private void btnClear2_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor2();

            // クリア
            this.dataGridView2.DataSource = null;
        }

        #endregion

        #region 動的SQLのCRUD

        /// <summary>インサート</summary>
        private void btnInsert2_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor2();

            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Insert", "SQL", this.myUserInfo);

            // データを入力できないのでパス
            //testParameterValue.field1 = this.txtCategoryID;          // CategoryID
            //this.txtCategoryID.BackColor = Color.LightYellow;

            testParameterValue.field2 = this.txtCategoryName.Text;   // CategoryName
            this.txtCategoryName.BackColor = Color.LightYellow;

            testParameterValue.field3 = this.txtDescription.Text;    // Description
            this.txtDescription.BackColor = Color.LightYellow;

            // データを入力できないのでパス
            //testParameterValue.field4 = this.txtPicture.Text;      // Picture
            //this.txtPicture.BackColor = Color.LightYellow;

            // Ｂ層呼び出し
            LayerB_Dynamic lb = new LayerB_Dynamic();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // データグリッドを更新
            this.btnSelectAll2_Click(sender, e);
        }

        /// <summary>セレクト</summary>
        private void btnSelect2_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor2();

            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Select", "SQL", this.myUserInfo);

            testParameterValue.field1_ForSearch = txtCategoryID_where.Text;     // CategoryID_where
            txtCategoryID_where.BackColor = Color.LightYellow;

            testParameterValue.field2_ForSearch = txtCategoryName_where.Text;   // CategoryName_where
            txtCategoryName_where.BackColor = Color.LightYellow;

            // 検索条件に使えない↓

            //testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
            //txtDescription_where.BackColor = Color.LightYellow;

            //testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
            //txtPicture_where.BackColor = Color.LightYellow;

            // Ｂ層呼び出し
            LayerB_Dynamic lb = new LayerB_Dynamic();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // 戻り値を設定
            this.dataGridView2.DataSource = testReturnValue.dt;
        }

        /// <summary>アップデート</summary>
        private void btnUpdate2_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor2();

            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Update", "SQL", this.myUserInfo);

            // 更新値
            //testParameterValue.field1_ForUpd = txtCategoryID.Text;     // CategoryID
            //txtCategoryID.BackColor = Color.LightYellow;

            testParameterValue.field2_ForUpd = txtCategoryName.Text;   // CategoryName
            txtCategoryName.BackColor = Color.LightYellow;

            testParameterValue.field3_ForUpd = txtDescription.Text;    // Description
            txtDescription.BackColor = Color.LightYellow;

            // 検索条件
            testParameterValue.field1_ForSearch = txtCategoryID_where.Text;     // CategoryID_where
            txtCategoryID_where.BackColor = Color.LightYellow;

            testParameterValue.field2_ForSearch = txtCategoryName_where.Text;   // CategoryName_where
            txtCategoryName_where.BackColor = Color.LightYellow;

            // 検索条件に使えない↓

            //testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
            //txtDescription_where.BackColor = Color.LightYellow;

            //testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
            //txtPicture_where.BackColor = Color.LightYellow;

            // Ｂ層呼び出し
            LayerB_Dynamic lb = new LayerB_Dynamic();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // データグリッドを更新
            this.btnSelectAll2_Click(sender, e);
        }

        /// <summary>デリート</summary>
        private void btnDelete2_Click(object sender, EventArgs e)
        {
            // 色のクリア
            this.ClearColor2();

            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Delete", "SQL", this.myUserInfo);

            // 検索条件
            testParameterValue.field1_ForSearch = txtCategoryID_where.Text;     // CategoryID_where
            txtCategoryID_where.BackColor = Color.LightYellow;

            testParameterValue.field2_ForSearch = txtCategoryName_where.Text;   // CategoryName_where
            txtCategoryName_where.BackColor = Color.LightYellow;

            // 検索条件に使えない↓

            //testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
            //txtDescription_where.BackColor = Color.LightYellow;

            //testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
            //txtPicture_where.BackColor = Color.LightYellow;

            // Ｂ層呼び出し
            LayerB_Dynamic lb = new LayerB_Dynamic();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // データグリッドを更新
            this.btnSelectAll2_Click(sender, e);
        }

        #endregion

        #endregion

        #region タブ３

        /// <summary>グリッド３にデータをロード</summary>
        private void btnSelectAll3_Click(object sender, EventArgs e)
        {
            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "SelectAll", "SQL", this.myUserInfo);

            // Ｂ層呼び出し
            LayerB_BatUpd lb = new LayerB_BatUpd();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // 戻り値を設定（列が自動的に作成されないようにする）
            this.dataGridView3.Columns.Clear();
            this.dataGridView3.AutoGenerateColumns = false;
            this.dataGridView3.DataSource = testReturnValue.dt;

            #region マスタのコンボ生成

            #region SupplierID - ComboBox

            DataTable dtSuppliers = this.GetSuppliers(((Button)sender).Name);

            // DataGridViewComboBoxColumnを作成
            DataGridViewComboBoxColumn cmbColSuppliers = new DataGridViewComboBoxColumn();
            this.InitDataGridViewComboBoxColumn(cmbColSuppliers);

            // "SupplierID"列にバインドされているデータと関連付け、
            cmbColSuppliers.DataPropertyName = "SupplierID";
            // ヘッダーのテキストを変更
            cmbColSuppliers.HeaderText = "Supplier";

            //DataGridViewComboBoxColumnのDataSourceを設定
            cmbColSuppliers.DataSource = dtSuppliers;

            // 実際の値が"SupplierID"列
            // 表示するテキストが"CompanyName"列
            cmbColSuppliers.ValueMember = "SupplierID";
            cmbColSuppliers.DisplayMember = "CompanyName";

            #endregion

            #region CategoryID - ComboBox

            DataTable dtCategory = this.GetCategory("btnSelectAll3");

            // DataGridViewComboBoxColumnを作成
            DataGridViewComboBoxColumn cmbColCategory = new DataGridViewComboBoxColumn();
            this.InitDataGridViewComboBoxColumn(cmbColCategory);

            // "SupplierID"列にバインドされているデータと関連付け、
            cmbColCategory.DataPropertyName = "CategoryID";
            // ヘッダーのテキストを変更
            cmbColCategory.HeaderText = "Category";

            // DataGridViewComboBoxColumnのDataSourceを設定
            cmbColCategory.DataSource = dtCategory;

            // 実際の値が"CategoryID"列
            // 表示するテキストが"CategoryName"列
            cmbColCategory.ValueMember = "CategoryID";
            cmbColCategory.DisplayMember = "CategoryName";

            #endregion

            #endregion

            #region 手動でデータバインド

            // はじめにクリア

            // DataGridViewTextBoxColumn
            DataGridViewTextBoxColumn textColumn;

            DataGridViewCheckBoxColumn checkColumn;

            //データソースの"ProductID"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "ProductID";
            textColumn.Name = "ProductID";
            textColumn.HeaderText = "ProductID";

            // 主キーは読み取り専用
            textColumn.ReadOnly = true;

            this.dataGridView3.Columns.Add(textColumn);

            //データソースの"ProductName"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "ProductName";
            textColumn.Name = "ProductName";
            textColumn.HeaderText = "ProductName";
            this.dataGridView3.Columns.Add(textColumn);

            //データソースの"SupplierID"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "SupplierID";
            textColumn.Name = "SupplierID";
            textColumn.HeaderText = "SupplierID";
            this.dataGridView3.Columns.Add(textColumn);

            // 見えなくしてマスタをコンボを追加
            this.dataGridView3.Columns["SupplierID"].Visible = false;
            this.dataGridView3.Columns.Add(cmbColSuppliers);

            //データソースの"CategoryID"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "CategoryID";
            textColumn.Name = "CategoryID";
            textColumn.HeaderText = "CategoryID";
            this.dataGridView3.Columns.Add(textColumn);

            // 見えなくしてマスタをコンボを追加
            this.dataGridView3.Columns["CategoryID"].Visible = false;
            this.dataGridView3.Columns.Add(cmbColCategory);

            //データソースの"QuantityPerUnit"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "QuantityPerUnit";
            textColumn.Name = "QuantityPerUnit";
            textColumn.HeaderText = "QuantityPerUnit";
            this.dataGridView3.Columns.Add(textColumn);

            //データソースの"UnitPrice"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "UnitPrice";
            textColumn.Name = "UnitPrice";
            textColumn.HeaderText = "UnitPrice";
            this.dataGridView3.Columns.Add(textColumn);

            //データソースの"UnitsInStock"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "UnitsInStock";
            textColumn.Name = "UnitsInStock";
            textColumn.HeaderText = "UnitsInStock";
            this.dataGridView3.Columns.Add(textColumn);

            //データソースの"UnitsOnOrder"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "UnitsOnOrder";
            textColumn.Name = "UnitsOnOrder";
            textColumn.HeaderText = "UnitsOnOrder";
            this.dataGridView3.Columns.Add(textColumn);

            //データソースの"ReorderLevel"列をバインドする
            textColumn = new DataGridViewTextBoxColumn();
            textColumn.DataPropertyName = "ReorderLevel";
            textColumn.Name = "ReorderLevel";
            textColumn.HeaderText = "ReorderLevel";
            this.dataGridView3.Columns.Add(textColumn);

            //データソースの"Discontinued"列をバインドする
            checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.DataPropertyName = "Discontinued";
            checkColumn.Name = "Discontinued";
            checkColumn.HeaderText = "Discontinued";
            this.dataGridView3.Columns.Add(checkColumn);

            #endregion
        }

        /// <summary>DataGridViewComboBoxColumnのスタイルを初期化する。</summary>
        private void InitDataGridViewComboBoxColumn(DataGridViewComboBoxColumn cmbCol)
        {
            // 現在のセルしかコンボボックスが表示されないようにする。
            cmbCol.DisplayStyleForCurrentCellOnly = true;
            // 編集モードの時だけコンボボックスを表示する。
            cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            // マウスポインタ下のセルが強調表示されるようにする。
            cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            // マウスポインタ下のセルにポップアップが表示されるようにする。
            cmbCol.FlatStyle = FlatStyle.Popup;
        }

        /// <summary>バッチ更新</summary>
        private void btnBatUpd_Click(object sender, EventArgs e)
        {
            // 引数
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "BatUpd", "SQL", this.myUserInfo);

            // 編集済みのDataTableを設定
            testParameterValue.dt = (DataTable)this.dataGridView3.DataSource;

            // Ｂ層呼び出し
            LayerB_BatUpd lb = new LayerB_BatUpd();

            TestReturnValue testReturnValue =
                (TestReturnValue)lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // コミット
            BaseLogic2CS.CommitAndClose();

            // データグリッドを更新
            this.btnSelectAll3_Click(sender, e);
        }

        /// <summary>データエラー時のイベントハンドラ</summary>
        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        /// <summary>クリア</summary>
        private void btnClear3_Click(object sender, EventArgs e)
        {
            // クリア
            this.dataGridView3.DataSource = null;
        }

        #endregion        
    }
}
