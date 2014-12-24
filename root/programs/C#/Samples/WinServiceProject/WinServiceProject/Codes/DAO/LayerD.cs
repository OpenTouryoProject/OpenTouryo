
// 業務フレームワーク
using Touryo.Infrastructure.Business.Dao;

// フレームワーク

// 部品
using Touryo.Infrastructure.Public.Db;

using WinServiceProject.Codes.Common;


namespace WinServiceProject.Codes.DAO
{
    class LayerD : MyBaseDao
    {
        public LayerD(BaseDam dam) : base(dam) { }

        
        public void テンプレ(TestParameterValue testParameter, TestReturnValue testReturn)
        {

            // ↓DBアクセス-----------------------------------------------------

            // ● 下記のいづれかの方法でSQLを設定する。

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2("ファイル名");

            //   -- 直接指定する場合。
            this.SetSqlByCommand("SQL文");

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P1", testParameter.Id);

            object obj;

            //   -- 追加、更新、削除の場合（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // 戻り値を設定
            testReturn.Obj = obj;
        }


        public void Insert(TestParameterValue testParameter, TestReturnValue testReturn)
        {
            // ↓DBアクセス-----------------------------------------------------

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2("SampleServiceInsert.sql");

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
           // this.SetParameter("P1", testParameter.Id);
            this.SetParameter("P2", testParameter.UserId);
            this.SetParameter("P3", testParameter.ProcessName);
            this.SetParameter("P4", testParameter.Data);
            this.SetParameter("P5", testParameter.RegistrationDateTime);
            this.SetParameter("P6", testParameter.ExecutionStartDateTime);
            this.SetParameter("P7", testParameter.NumberOfRetries);
            this.SetParameter("P8", testParameter.CompletionDateTime);
            this.SetParameter("P9", testParameter.ProgressRate);
            this.SetParameter("P10", testParameter.Status);
            this.SetParameter("P11", testParameter.Command);
            this.SetParameter("P12", testParameter.ReservedArea);
            
            object obj;

            //   -- 追加（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            testReturn.Obj = obj;
        }


        #region Update

        /// <summary>業務処理を実装</summary>
        /// <param name="testParameter">引数クラス</param>
        public void Update(TestParameterValue testParameter, TestReturnValue testReturn)
        {
            string filename = "";

            if ((testParameter.ActionType.Split('%'))[2] == "Static")
            {
                // 静的SQL
                filename = "SampleServiceUpdate.sql";
            }
            else if ((testParameter.ActionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                filename = "SampleServiceUpdate.xml";
            }

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
           // this.SetParameter("@Id", testParameter.Id);
            this.SetParameter("P2", testParameter.UserId);
            this.SetParameter("P3", testParameter.RegistrationDateTime);
            this.SetParameter("P4", testParameter.ExecutionStartDateTime);
            this.SetParameter("P5", testParameter.NumberOfRetries);
            this.SetParameter("P6", testParameter.CompletionDateTime);
            this.SetParameter("P7", testParameter.ProgressRate);
            this.SetParameter("P8", testParameter.Status);
            this.SetParameter("P9", testParameter.Command);


            object obj;

            //   -- 更新（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            testReturn.Obj = obj;
        }
        #endregion
    }


}


