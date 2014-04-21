using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections;

using Touryo.Infrastructure.CustomControl.RichClient;

namespace CustCtrl_sample
{
    /// <summary>フォーム</summary>
    public partial class Form1 : Form
    {
        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();

            // 異常な設定状態をテスト

            //// 「HowToCut = null」では、数値と認識されないこと（例外も起きないこと）。
            //this.winCustomTextBox1.EditDigitsAfterDP = new EditDigitsAfterDP();
            //this.winCustomTextBox1.EditDigitsAfterDP.HowToCut = null;
            //this.winCustomTextBox1.EditDigitsAfterDP.DigitsAfterDP = 100;
        }

        DataTable Dt = null;
        BindingSource BindingSource1 = null;

        /// <summary>ロード</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // ハンドラ
            this.winCustomTextBox8.ValueChanged += new EventHandler(winCustomTextBox8_ValueChanged);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // コンボ初期化
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            // Webサービスでマスタをロード
            ArrayList al = null;
            CmnMasterDatasForList.ClearMasterData();

            al = new ArrayList();
            al.Add(new ListItem("1", "aaa"));
            al.Add(new ListItem("2", "bbb"));
            al.Add(new ListItem("3", "ccc"));
            CmnMasterDatasForList.SetMasterData("Test1", al);

            al = new ArrayList();
            al.Add(new ListItem("1", "AAA"));
            al.Add(new ListItem("2", "BBB"));
            al.Add(new ListItem("3", "CCC"));
            CmnMasterDatasForList.SetMasterData("Test2", al);

            al = new ArrayList();
            al.Add(new ListItem("1", "あ"));
            al.Add(new ListItem("2", "い"));
            al.Add(new ListItem("3", "う"));
            CmnMasterDatasForList.SetMasterData("Test3", al);

            //// InitItemsで初期化
            //this.winCustomDropDownList1.InitItems();
            //this.winCustomDropDownList2.InitItems();
            //this.winCustomDropDownList3.InitItems();

            // InitDataSourceで初期化
            this.winCustomDropDownList1.InitDataSource();
            this.winCustomDropDownList2.InitDataSource();
            this.winCustomDropDownList3.InitDataSource();

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // データバインディングをテストする。
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            // DataBindingsのFormatString（桁区切り）はdecimalで無いと効かない。
            // また、DataBindingsでは初期設定時のTextのReEditも効かない。
            // 従って、DataBindings時の方式としてはdecimal＆FormatStringに寄せる必要がある。

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // 単項目の入力コントロールとのデータバインディングをテストする。
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            DataView dv = null;
            this.Dt = this.CreateDataTable();

            // FormatStringとは相性が悪いので併用NGとした。

            // 設定なし
            dv = new DataView(this.Dt, "id = 1", "", DataViewRowState.Unchanged);
            this.winCustomTextBox1.DataBindings.Add("Text", dv, "aaa", true, DataSourceUpdateMode.OnPropertyChanged, "hogehoge");//, "#,##0.########");

            // 桁区切り3
            dv = new DataView(this.Dt, "id = 2", "", DataViewRowState.Unchanged);
            this.winCustomTextBox2.DataBindings.Add("Text", dv, "aaa", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");

            // 桁区切り4
            dv = new DataView(this.Dt, "id = 3", "", DataViewRowState.Unchanged);
            this.winCustomTextBox3.DataBindings.Add("Text", dv, "aaa", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");

            // 小数点以下2, 6
            dv = new DataView(this.Dt, "id = 4", "", DataViewRowState.Unchanged);
            this.winCustomTextBox4.DataBindings.Add("Text", dv, "aaa", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");

            // 小数点以下4, 8
            dv = new DataView(this.Dt, "id = 5", "", DataViewRowState.Unchanged);
            this.winCustomTextBox5.DataBindings.Add("Text", dv, "aaa", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");

            // パッド
            dv = new DataView(this.Dt, "id = 6", "", DataViewRowState.Unchanged);
            this.winCustomTextBox6.DataBindings.Add("Text", dv, "aaa", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");

            // パッド
            dv = new DataView(this.Dt, "id = 7", "", DataViewRowState.Unchanged);
            this.winCustomTextBox7.DataBindings.Add("Text", dv, "aaa", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");

            //---

            this.BindingSource1 = new BindingSource();
            this.BindingSource1.DataSource = new Bean(88888888, DateTime.Now, "88888888");

            // 複合（桁区切り3＋小数点以下2、6＋単位変換100万→10^6乗）
            //dv = new DataView(this.Dt, "id = 8", "", DataViewRowState.Unchanged);
            //this.winCustomTextBox8.DataBindings.Add("Value", dv, "aaa", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");
            this.winCustomTextBox8.DataBindings.Add("Value", this.BindingSource1, "AAA", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");

            // Textはdatetime、Text2はstringとのバインディングもテスト（日付）
            //dv = new DataView(this.Dt, "id = 1", "", DataViewRowState.Unchanged);
            //this.winCustomTextBox8.DataBindings.Add("Text2", dv, "bbb", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "#,##0.########");
            this.winCustomMaskedTextBox9.DataBindings.Add("Text2", this.BindingSource1, "BBB", true, DataSourceUpdateMode.OnPropertyChanged, null);//, "yyyy/MM/dd");
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // データグリッドとのデータバインディングをテストする。
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////
            // WinCustomTextBoxの場合
            //////////////////////////////////////////////////
            // WinCustomTextBoxDgvColを作成
            WinCustomTextBoxDgvCol nomalColumn = new WinCustomTextBoxDgvCol();

            nomalColumn.MaxLength = 15;
            nomalColumn.IsNumeric = true;

            // EditInitialValue
            //nomalColumn.EditInitialValue = EditInitialValue.Zero;

            // 編集中、小数点以下（Editingのみ実装）
            nomalColumn.EditDigitsAfterDP_Editing= new EditDigitsAfterDP(CutMethod.Ceiling, 6);

            // パッド
            //nomalColumn.EditPadding = new EditPadding(PadDirection.Right, '0');
            //nomalColumn.EditPadding = new EditPadding(PadDirection.Left, '0');

            // 桁区切り（FormatStringで対応）
            // DisplayUnits（処理で対応）

            nomalColumn.DataPropertyName = "aaa";
            nomalColumn.HeaderText = "aaa";

            // FormatString（編集後、カンマ区切りで小数点2桁）
            nomalColumn.DefaultCellStyle.Format = "#,##0.##";
            
            this.dataGridView1.Columns.Add(nomalColumn);

            //////////////////////////////////////////////////
            // WinCustomMaskedTextBoxの場合
            //////////////////////////////////////////////////

            // WinCustomMaskedTextBoxDgvColを作成
            WinCustomMaskedTextBoxDgvCol maskedColumn =
                new WinCustomMaskedTextBoxDgvCol();
            maskedColumn.DataPropertyName = "bbb";
            maskedColumn.HeaderText = "bbb";

            // Maskと、Mask_Editingを逆にすると上手くいかない。
            // 初期表示時と、編集後で、セル（バインド先）のFormatが変わってしまうため。
            
            maskedColumn.EditInitialValue = EditInitialValue.Blank;

            maskedColumn.Mask = "9999/99/99";
            maskedColumn.Mask_Editing = "9999年99月99日";

            maskedColumn.EditToHankaku = true;
            maskedColumn.EditToYYYYMMDD = true;

            // FormatString（編集後、カンマ区切りで小数点2桁）
            maskedColumn.DefaultCellStyle.Format = "yyyy/MM/dd";
            
            this.dataGridView1.Columns.Add(maskedColumn);

            //////////////////////////////////////////////////
            // WinCustomDropDownListの場合
            //////////////////////////////////////////////////

            // WinCustomDropDownListDgvColを作成
            DataGridViewComboBoxColumn comboColumn =
                new DataGridViewComboBoxColumn();
            comboColumn.DataPropertyName = "ccc";
            comboColumn.HeaderText = "ccc";
            comboColumn.DataSource = CmnMasterDatasForList.GetMasterData("Test1"); // ↓どちらでも良い
            //MasterDatasForList.GetMasterData("Test1", comboColumn.Items); // ↑どちらでも良い
            comboColumn.ValueMember = "ID"; // 必須
            comboColumn.DisplayMember = "Name"; // 必須
            this.dataGridView1.Columns.Add(comboColumn);

            // ---

            // また、DataBindingsでは初期設定時のTextのReEditも効かない。
            // 従って、DataBindings時の方式としてはdecimal＆FormatStringに寄せる必要がある。
            this.dataGridView1.DataSource = this.CreateDataTable();
            this.dataGridView1.Columns["id"].Visible = false;
            this.dataGridView1.Columns["ddd"].Visible = false; // 変更通知を発生させる用途の列。
        }

        /// <summary>DataTable生成</summary>
        /// <returns>DataTable</returns>
        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("aaa", typeof(decimal));
            dt.Columns.Add("bbb", typeof(DateTime));
            dt.Columns.Add("ccc");
            dt.Columns.Add("ddd");

            DataRow dr = dt.NewRow();

            dr["id"] = "1"; 
            dr["aaa"] = "11111111";
            dr["bbb"] = "2001/01/01";
            dr["ccc"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["id"] = "2"; 
            dr["aaa"] = "22222222";
            dr["bbb"] = "2002/02/02";
            dr["ccc"] = "2";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["id"] = "3"; 
            dr["aaa"] = "33333333";
            dr["bbb"] = "2003/03/03";
            dr["ccc"] = "3";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["id"] = "4"; 
            dr["aaa"] = "44444444";
            dr["bbb"] = "2004/04/04";
            dr["ccc"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["id"] = "5"; 
            dr["aaa"] = "55555555";
            dr["bbb"] = "2005/05/05";
            dr["ccc"] = "2";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["id"] = "6";
            dr["aaa"] = "66666666";
            dr["bbb"] = "2006/06/06";
            dr["ccc"] = "3";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["id"] = "7";
            dr["aaa"] = "77777777";
            dr["bbb"] = "2007/07/07";
            dr["ccc"] = "1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["id"] = "8";
            dr["aaa"] = "88888888";
            dr["bbb"] = "2008/08/08";
            dr["ccc"] = "1";
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }

        /// <summary>一括チェックのテスト</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            string ret = "";

            List<CheckResult> lcr = new List<CheckResult>();
            if (CmnCheckFunction.HasErrors(this, lcr))
            {
                foreach (CheckResult cr in lcr)
                {
                    ret += cr.CtrlName + "\r\n";
                    foreach (string checkErrorInfo in cr.CheckErrorInfo)
                    {
                        ret += "・" + checkErrorInfo + "\r\n";
                    }
                    ret += "\r\n";
                }
            }

            MessageBox.Show(ret);
        }

        /// <summary>値取得プロパティ プロシージャのテスト（WinCustomTextBox）</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            //winCustomMaskedTextBox1.GetDateTime();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetValue(this.winCustomTextBox1));
            sb.AppendLine(this.GetValue(this.winCustomTextBox2));
            sb.AppendLine(this.GetValue(this.winCustomTextBox3));
            sb.AppendLine(this.GetValue(this.winCustomTextBox4));
            sb.AppendLine(this.GetValue(this.winCustomTextBox5));
            sb.AppendLine(this.GetValue(this.winCustomTextBox6));
            sb.AppendLine(this.GetValue(this.winCustomTextBox7));
            sb.AppendLine(this.GetValue(this.winCustomTextBox8));
            MessageBox.Show(sb.ToString());
        }

        /// <summary>値取得プロパティ プロシージャのテスト（WinCustomMaskedTextBox）</summary>
        private void button3_Click(object sender, EventArgs e)
        {
            //winCustomMaskedTextBox1.GetDateTime();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox1));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox2));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox3));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox4));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox5));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox6));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox7));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox8));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox9));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox10));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox11));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox12));
            sb.AppendLine(this.GetValue(this.winCustomMaskedTextBox13));
            MessageBox.Show(sb.ToString());
        }

        /// <summary>データソースからの変更通知を発生</summary>
        private void button4_Click(object sender, EventArgs e)
        {
            DataTable  dt =this.Dt;
            foreach (DataRow row in dt.Rows)
            {
                row["ddd"] = this.textBox1.Text;
            }
            dt.AcceptChanges();

            dt = (DataTable)this.dataGridView1.DataSource;
            foreach (DataRow row in dt.Rows)
            {
                row["ddd"] = this.textBox1.Text;
            }
            dt.AcceptChanges();

            ((Bean)this.BindingSource1.DataSource).AAA = 99999999;
            ((Bean)this.BindingSource1.DataSource).BBB = DateTime.Now;
            ((Bean)this.BindingSource1.DataSource).CCC = "99999999";
            this.BindingSource1.ResetBindings(false);

        }

        /// <summary>値取得プロパティ プロシージャのテスト</summary>
        private string GetValue(IGetValue igv)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(((Control)igv).Name);
            try { sb.AppendLine("GetInt16:" + igv.GetInt16().ToString()); } catch { }
            try { sb.AppendLine("GetInt32:" + igv.GetInt32().ToString()); } catch { }
            try { sb.AppendLine("GetInt64:" + igv.GetInt64().ToString()); } catch { }
            try { sb.AppendLine("GetFloat:" + igv.GetFloat().ToString()); } catch { }
            try { sb.AppendLine("GetDouble:" + igv.GetDouble().ToString()); } catch { }
            try { sb.AppendLine("GetDecimal:" + igv.GetDecimal().ToString()); } catch { }
            try { sb.AppendLine("GetDateTime:" + igv.GetDateTime().ToString()); } catch { }

            if (igv is WinCustomTextBox)
            {
                WinCustomTextBox wctbx = (WinCustomTextBox)igv;
                // 通常のTextプロパティ（可変）
                sb.AppendLine("Text:" + wctbx.Text);
                // ユーザの入力値だけ取得する
                sb.AppendLine("Text2:" + wctbx.Text2);
                // 編集処理を適用した値を取得する
                sb.AppendLine("Text3:" + wctbx.Text3);
                // データバインディング用プロパティ値を取得する
                sb.AppendLine("Value:" + wctbx.Value);
            }
            else if (igv is WinCustomMaskedTextBox)
            {
                WinCustomMaskedTextBox wcmtbx = (WinCustomMaskedTextBox)igv;
                // 通常のTextプロパティ（可変）
                sb.AppendLine("Text:" + wcmtbx.Text);
                // ユーザの入力値だけ取得する
                sb.AppendLine("Text2:" + wcmtbx.Text2);
                // 入力時マスクを適用した値を取得する
                sb.AppendLine("Text3:" + wcmtbx.Text3);
            }

            return sb.ToString();
        }

        /// <summary>winCustomTextBox_TextChanged</summary>
        private void winCustomTextBox_TextChanged(object sender, EventArgs e)
        {
            string s = ((TextBox)sender).Name;

            switch (s.Substring(s.Length - 1, 1))
            {
                case "2":
                    this.winCustomTextBox2_2.Text = this.winCustomTextBox2.Text2;
                    break;
                case "3":
                    this.winCustomTextBox3_2.Text = this.winCustomTextBox3.Text2;
                    break;
                case "4":
                    this.winCustomTextBox4_2.Text = this.winCustomTextBox4.Text2;
                    break;
                case "5":
                    this.winCustomTextBox5_2.Text = this.winCustomTextBox5.Text2;
                    break;
                case "6":
                    this.winCustomTextBox6_2.Text = this.winCustomTextBox6.Text2;
                    break;
                case "7":
                    this.winCustomTextBox7_2.Text = this.winCustomTextBox7.Text2;
                    break;
                default:
                    break;
            }
        }

        /// <summary>winCustomMaskedTextBox_TextChanged</summary>
        private void winCustomMaskedTextBox_TextChanged(object sender, EventArgs e)
        {
            string s = ((MaskedTextBox)sender).Name;

            switch (s.Substring(s.Length - 1, 1))
            {
                case "2":
                    this.winCustomMaskedTextBox2_2.Text = this.winCustomMaskedTextBox2.Text2;
                    break;
                case "3":
                    this.winCustomMaskedTextBox3_2.Text = this.winCustomMaskedTextBox3.Text2;
                    break;
                case "4":
                    this.winCustomMaskedTextBox4_2.Text = this.winCustomMaskedTextBox4.Text2;
                    break;
                case "5":
                    this.winCustomMaskedTextBox5_2.Text = this.winCustomMaskedTextBox5.Text2;
                    break;
                case "6":
                    this.winCustomMaskedTextBox6_2.Text = this.winCustomMaskedTextBox6.Text2;
                    break;
                case "7":
                    this.winCustomMaskedTextBox7_2.Text = this.winCustomMaskedTextBox7.Text2;
                    break;
                case "8":
                    this.winCustomMaskedTextBox8_2.Text = this.winCustomMaskedTextBox8.Text2;
                    break;
                case "9":
                    this.winCustomMaskedTextBox9_2.Text = this.winCustomMaskedTextBox9.Text2;
                    break;
                case "10":
                    this.winCustomMaskedTextBox10_2.Text = this.winCustomMaskedTextBox10.Text2;
                    break;
                case "11":
                    this.winCustomMaskedTextBox11_2.Text = this.winCustomMaskedTextBox11.Text2;
                    break;
                default:
                    break;
            }
        }

        /// <summary>デザイナで設定できなくした</summary>
        private void winCustomTextBox8_ValueChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ValueChanged:" + ((WinCustomTextBox)sender).Name);
        }
    }
}
