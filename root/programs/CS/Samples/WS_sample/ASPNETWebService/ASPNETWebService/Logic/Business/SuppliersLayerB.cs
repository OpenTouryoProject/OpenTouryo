#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：SuppliersLayerB
//* クラス日本語名  ：Suppliers の業務処理
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/20  玄人 幸道         新規作成（#570）
//**********************************************************************************

using System;
using System.Data;

using ASPNETWebService.Logic.Common;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Framework.Exceptions;

namespace ASPNETWebService.Logic.Business
{
    /// <summary>Suppliers の業務処理</summary>
    /// <remarks>
    /// UOC_〈methodName〉 はレイトバインドで呼ばれる（引数 1 つ・戻り値 void）。
    /// 戻り値は this.ReturnValue で返す（メソッド冒頭で設定する＝例外時にも戻るようにするため）。
    /// トランザクションのコミット／ロールバックはフレームワークが行うので、ここには書かない。
    /// </remarks>
    public class SuppliersLayerB : MyFcBaseLogic
    {
        /// <summary>楽観排他の対象にする列</summary>
        /// <remarks>
        /// **HomePage を含めない。**
        /// HomePage は ntext で、SQL Server では "=" で比較できない
        /// （Msg 402: データ型 ntext と nvarchar は equal to 演算子では互換性がありません）。
        ///
        /// D3_Update の WHERE は列ごとの &lt;IF&gt; で組まれており、
        /// **パラメタを設定しなければ、その &lt;IF&gt; ごと消える**（BaseDam の仕様）。
        /// つまり、ここに挙げた列だけが WHERE に載る。
        ///
        /// **HomePage だけを他者が更新した場合は検知できない。** ntext の制約による割り切り。
        /// </remarks>
        private static readonly string[] OptimisticLockColumns = new string[]
        {
            "SupplierID", "CompanyName", "ContactName", "ContactTitle", "Address",
            "City", "Region", "PostalCode", "Country", "Phone", "Fax"
        };

        /// <summary>更新する列（SET 句）</summary>
        /// <remarks>HomePage は "=" で比較できないだけで、**更新はできる。**</remarks>
        private static readonly string[] UpdateColumns = new string[]
        {
            "CompanyName", "ContactName", "ContactTitle", "Address",
            "City", "Region", "PostalCode", "Country", "Phone", "Fax", "HomePage"
        };

        #region 件数確認

        /// <summary>Suppliers のデータ件数を取得する</summary>
        /// <param name="parameterValue">引数クラス</param>
        private void UOC_SelectCount(SuppliersParameterValue parameterValue)
        {
            // 戻り値クラスは業務処理の前に設定する（例外時にも戻り値を返すため）
            SuppliersReturnValue returnValue = new SuppliersReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            DaoSuppliers dao = new DaoSuppliers(this.GetDam());

            // 条件を設定しなければ、WHERE 句ごと消える（全件が対象になる）。
            returnValue.Count = Convert.ToInt32(dao.D5_SelCnt());

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #region 一覧取得

        /// <summary>Suppliers の一覧を取得する</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <remarks>
        /// **全列を取得する。** 楽観排他で Original と突き合わせるため、
        /// 画面に出さない列も持ち帰る必要がある。
        /// </remarks>
        private void UOC_SelectAll(SuppliersParameterValue parameterValue)
        {
            SuppliersReturnValue returnValue = new SuppliersReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            DaoSuppliers dao = new DaoSuppliers(this.GetDam());

            DataTable dt = new DataTable("Suppliers");
            dao.D2_Select(dt);

            // **追加行（Added）を作れるようにする。**
            //   Fill はスキーマ（NOT NULL）も取り込むため、そのままだと dt.NewRow() の追加が
            //   NoNullAllowedException（列 'SupplierID' に nulls を使用することはできません）になる。
            //   SupplierID は IDENTITY ＝ 実際の採番は DB 側なので、DataTable 上は
            //   実データと衝突しない負値で仮採番しておく（INSERT には渡さない）。
            DataColumn pk = dt.Columns["SupplierID"];
            pk.AutoIncrement = true;
            pk.AutoIncrementSeed = -1;
            pk.AutoIncrementStep = -1;

            // 主キーを持たせておく（行の特定・バッチ更新の前提）
            dt.PrimaryKey = new DataColumn[] { pk };

            returnValue.Suppliers = dt;

            // ↑業務処理-----------------------------------------------------
        }

        #endregion

        #region バッチ更新

        /// <summary>Suppliers の明細をバッチ更新する（DataRowState で CUD を振り分ける）</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <remarks>
        /// **RowState と Original が復元されていることが前提。**
        /// WebAPI 越しに来た場合は、DTTable.FromDataTable(dt, keepOriginal: true) で
        /// 作られた DTTables を経由している必要がある（#567）。
        /// </remarks>
        private void UOC_BatchUpdate(SuppliersParameterValue parameterValue)
        {
            SuppliersReturnValue returnValue = new SuppliersReturnValue();
            this.ReturnValue = returnValue;

            // ↓業務処理-----------------------------------------------------

            DataTable dt = parameterValue.Suppliers;
            if (dt == null)
            {
                throw new BusinessApplicationException(
                    "W0001", "更新対象がありません。先に一覧を取得して下さい。", "-");
            }

            DaoSuppliers dao = new DaoSuppliers(this.GetDam());

            // **Deleted → Added の順に流す。**
            //   Added を先に流すと、まだ消えていない旧行と主キーが衝突しうる。
            this.DeleteRows(dt, dao, returnValue);
            this.InsertAndUpdateRows(dt, dao, returnValue);

            // ↑業務処理-----------------------------------------------------
        }

        /// <summary>削除行を流す</summary>
        /// <param name="dt">対象</param>
        /// <param name="dao">Dao</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <remarks>
        /// **削除は主キーのみで特定する（S4_Delete）。**
        /// 削除は「消えていればよい」ので、Original の突き合わせは要らない。
        /// </remarks>
        private void DeleteRows(DataTable dt, DaoSuppliers dao, SuppliersReturnValue returnValue)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState != DataRowState.Deleted) { continue; }

                dao.ClearParametersFromHt();

                // **削除行は現在値を持たない。** Original から読む。
                dao.PK_SupplierID = dr["SupplierID", DataRowVersion.Original];

                int deleted = dao.S4_Delete();
                if (deleted == 0)
                {
                    // 対象行が既に無い（他者が先に削除した）＝ 再取得すれば続行できるので業務例外
                    throw new BusinessApplicationException(
                        "W0002", "他のユーザによって削除されています。再取得してやり直して下さい。",
                        "SupplierID=" + dr["SupplierID", DataRowVersion.Original]);
                }

                returnValue.DeleteCount += deleted;
            }
        }

        /// <summary>追加行・更新行を流す</summary>
        /// <param name="dt">対象</param>
        /// <param name="dao">Dao</param>
        /// <param name="returnValue">戻り値クラス</param>
        private void InsertAndUpdateRows(DataTable dt, DaoSuppliers dao, SuppliersReturnValue returnValue)
        {
            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:

                        dao.ClearParametersFromHt();

                        // **SupplierID は設定しない。** IDENTITY 列なので DB 側が採番する。
                        //   D1_Insert は「設定した列だけ」を INSERT する（動的 SQL）。
                        foreach (string col in SuppliersLayerB.UpdateColumns)
                        {
                            SuppliersLayerB.SetInsertValue(dao, col, dr[col]);
                        }

                        returnValue.InsertCount += dao.D1_Insert();

                        break;

                    case DataRowState.Modified:

                        dao.ClearParametersFromHt();

                        // **WHERE ＝ 取得時の値（Original）。** これが楽観排他になる。
                        //   HomePage は設定しない（ntext。設定しなければ <IF> ごと消える）。
                        foreach (string col in SuppliersLayerB.OptimisticLockColumns)
                        {
                            SuppliersLayerB.SetWhereValue(dao, col, dr[col, DataRowVersion.Original]);
                        }

                        // SET ＝ 変更後の値（Current）
                        foreach (string col in SuppliersLayerB.UpdateColumns)
                        {
                            SuppliersLayerB.SetUpdateValue(dao, col, dr[col]);
                        }

                        int updated = dao.D3_Update();
                        if (updated == 0)
                        {
                            // **更新件数 0 ＝ 取得時から変わっている**（他者が先に更新／削除した）
                            throw new BusinessApplicationException(
                                "W0002", "他のユーザによって更新されています。再取得してやり直して下さい。",
                                "SupplierID=" + dr["SupplierID", DataRowVersion.Original]);
                        }

                        returnValue.UpdateCount += updated;

                        break;
                }
            }
        }

        #endregion

        #region ユーティリティ

        /// <summary>WHERE 句のパラメタを設定する</summary>
        /// <param name="dao">Dao</param>
        /// <param name="column">列名</param>
        /// <param name="value">値</param>
        /// <remarks>
        /// **null（DBNull）でも設定する。**
        /// 設定しなければ &lt;IF&gt; ごと消えて比較されなくなるが、
        /// 設定して null なら &lt;ELSE&gt; の IS NULL に落ちる。
        /// Region / PostalCode / Fax は NULL を含むため、ここが効く。
        /// </remarks>
        private static void SetWhereValue(DaoSuppliers dao, string column, object value)
        {
            // **NULL は null で渡す。DBNull ではない。**
            //   BaseDam の判定は obj == null で、DBNull.Value は null ではないため、
            //   DBNull を渡すと <IF> 側（= @X）が採られ、SQL 上 `= NULL` になって
            //   **決して一致しない**（Region / Fax が NULL の行が必ず弾かれる）。
            object v = SuppliersLayerB.ToWhereValue(value);

            switch (column)
            {
                case "SupplierID":   dao.PK_SupplierID = v; break;
                case "CompanyName":  dao.CompanyName   = v; break;
                case "ContactName":  dao.ContactName   = v; break;
                case "ContactTitle": dao.ContactTitle  = v; break;
                case "Address":      dao.Address       = v; break;
                case "City":         dao.City          = v; break;
                case "Region":       dao.Region        = v; break;
                case "PostalCode":   dao.PostalCode    = v; break;
                case "Country":      dao.Country       = v; break;
                case "Phone":        dao.Phone         = v; break;
                case "Fax":          dao.Fax           = v; break;
            }
        }

        /// <summary>SET 句のパラメタを設定する</summary>
        /// <param name="dao">Dao</param>
        /// <param name="column">列名</param>
        /// <param name="value">値</param>
        private static void SetUpdateValue(DaoSuppliers dao, string column, object value)
        {
            object v = SuppliersLayerB.ToDbValue(value);

            switch (column)
            {
                case "CompanyName":  dao.Set_CompanyName_forUPD  = v; break;
                case "ContactName":  dao.Set_ContactName_forUPD  = v; break;
                case "ContactTitle": dao.Set_ContactTitle_forUPD = v; break;
                case "Address":      dao.Set_Address_forUPD      = v; break;
                case "City":         dao.Set_City_forUPD         = v; break;
                case "Region":       dao.Set_Region_forUPD       = v; break;
                case "PostalCode":   dao.Set_PostalCode_forUPD   = v; break;
                case "Country":      dao.Set_Country_forUPD      = v; break;
                case "Phone":        dao.Set_Phone_forUPD        = v; break;
                case "Fax":          dao.Set_Fax_forUPD          = v; break;
                case "HomePage":     dao.Set_HomePage_forUPD     = v; break;
            }
        }

        /// <summary>INSERT のパラメタを設定する</summary>
        /// <param name="dao">Dao</param>
        /// <param name="column">列名</param>
        /// <param name="value">値</param>
        private static void SetInsertValue(DaoSuppliers dao, string column, object value)
        {
            object v = SuppliersLayerB.ToDbValue(value);

            switch (column)
            {
                case "CompanyName":  dao.CompanyName  = v; break;
                case "ContactName":  dao.ContactName  = v; break;
                case "ContactTitle": dao.ContactTitle = v; break;
                case "Address":      dao.Address      = v; break;
                case "City":         dao.City         = v; break;
                case "Region":       dao.Region       = v; break;
                case "PostalCode":   dao.PostalCode   = v; break;
                case "Country":      dao.Country      = v; break;
                case "Phone":        dao.Phone        = v; break;
                case "Fax":          dao.Fax          = v; break;
                case "HomePage":     dao.HomePage     = v; break;
            }
        }

        /// <summary>WHERE 用に、NULL 相当を null にする</summary>
        /// <param name="value">列の値</param>
        /// <returns>WHERE へ渡す値</returns>
        /// <remarks>
        /// **null を渡すと &lt;ELSE&gt;（IS NULL）に落ちる。**
        /// DBNull を渡すと &lt;IF&gt;（= @X）が採られ、SQL 上 `= NULL` になって一致しない。
        /// </remarks>
        private static object ToWhereValue(object value)
        {
            if (value == null || value == DBNull.Value) { return null; }
            if (value is string && ((string)value).Length == 0) { return null; }
            return value;
        }

        /// <summary>空文字は NULL 相当（DBNull）として扱う</summary>
        /// <param name="value">列の値</param>
        /// <returns>DB へ渡す値</returns>
        private static object ToDbValue(object value)
        {
            if (value == null || value == DBNull.Value) { return DBNull.Value; }
            if (value is string && ((string)value).Length == 0) { return DBNull.Value; }
            return value;
        }

        #endregion
    }
}
