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
//* クラス名        ：ZipBaseV2
//* クラス日本語名  ：SharpZipLib部品ベース クラス
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/08  玄人 幸道         新規作成（#524）
//*  2026/08/08  玄人 幸道         ZippedFiles・ExtractedFilesの追加に伴う共通部の調整（#528）
//**********************************************************************************

using System;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>SharpZipLib部品ベース クラス</summary>
    /// <remarks>
    /// 旧 ZipBase（DotNetZip）の代替（#524）。
    /// DotNetZip は非推奨かつ既知脆弱性（GHSA-xhg6-9j5j-w4vf）があるため、
    /// SharpZipLib で作り直したもの。**旧クラスは残してあるが、ビルド対象外。**
    ///
    /// ＜旧から落とした機能＞
    ///   ・自己解凍書庫（SaveSelfExtractor）… SharpZipLib に相当が無い。
    ///     呼び出し元（DeployZipPackWithHTTP）にも消費経路が無いことを確認済み。
    ///   ・選択条件の文字列（"name = *.txt" 等）… DotNetZip 独自の DSL。
    ///     SelectionDelegate に一本化した。
    ///   ・StatusMSG … DotNetZip が吐くログ文言。**同じ文字列は作れない。**
    ///     解凍したパスは UnZipperV2.ExtractedFiles で受け取ること。
    ///   ・AddProgress / ReadProgress / ZipError … 呼び出し元で未使用。
    /// </remarks>
    public abstract class ZipBaseV2
    {
        /// <summary>コンストラクタ</summary>
        public ZipBaseV2()
        {
            // 選択処理を実装するデリゲート
            this._selectionDlgt = new SelectionDelegate(this.DefaultSelectionDlgt);
        }

        #region 選択処理

        /// <summary>選択処理を実装するデリゲート</summary>
        /// <param name="o">
        /// 圧縮時：ファイル（FileInfo）
        /// 解凍時：エントリ名（string）
        /// </param>
        /// <param name="info">選択規準情報</param>
        /// <returns>
        /// true：ファイルを圧縮 or 解凍する。
        /// false：ファイルを圧縮 or 解凍しない。
        /// </returns>
        public delegate bool SelectionDelegate(object o, object info);

        /// <summary>選択処理を実装するデリゲート</summary>
        protected SelectionDelegate _selectionDlgt = null;

        /// <summary>選択処理を実装するデリゲート</summary>
        public SelectionDelegate SelectionDlgt
        {
            private set { this._selectionDlgt = value; }
            get { return this._selectionDlgt; }
        }

        /// <summary>選択処理を実装するデリゲート</summary>
        /// <param name="o">
        /// 圧縮時：ファイル（FileInfo）
        /// 解凍時：エントリ名（string）
        /// </param>
        /// <param name="info">選択規準情報</param>
        /// <returns>
        /// true：ファイルを圧縮 or 解凍する。
        /// false：ファイルを圧縮 or 解凍しない。
        /// </returns>
        protected bool DefaultSelectionDlgt(object o, object info)
        {
            return true;
        }

        /// <summary>選択基準情報</summary>
        protected object _selectionCriteriaInfo = null;

        /// <summary>選択基準情報</summary>
        public object SelectionCriteriaInfo
        {
            private set { this._selectionCriteriaInfo = value; }
            get { return this._selectionCriteriaInfo; }
        }

        /// <summary>選択基準を設定する</summary>
        /// <param name="selectionDlgt">ファイル選択デリゲード</param>
        /// <param name="selectionCriteriaInfo">ファイル選択基準情報</param>
        /// <remarks>
        /// **デリゲートが null のときは既定（すべて選択）のまま**にする。
        /// 旧実装と同じ振る舞い。
        /// </remarks>
        protected void SetSelectionCriteria(
            SelectionDelegate selectionDlgt, object selectionCriteriaInfo)
        {
            if (selectionDlgt != null)
            {
                this._selectionDlgt = selectionDlgt;
                this._selectionCriteriaInfo = selectionCriteriaInfo;
            }
        }

        #endregion

        #region 進捗イベント

        /// <summary>SaveProgressイベント ハンドラ</summary>
        protected EventHandler<ZipProgressEventArgsV2> _saveProgress = null;

        /// <summary>SaveProgressイベント ハンドラ</summary>
        public EventHandler<ZipProgressEventArgsV2> SaveProgress
        {
            set { this._saveProgress = value; }
            get { return this._saveProgress; }
        }

        /// <summary>ExtractProgressイベント ハンドラ</summary>
        protected EventHandler<ZipProgressEventArgsV2> _extractProgress = null;

        /// <summary>ExtractProgressイベント ハンドラ</summary>
        public EventHandler<ZipProgressEventArgsV2> ExtractProgress
        {
            set { this._extractProgress = value; }
            get { return this._extractProgress; }
        }

        /// <summary>SaveProgressイベントを発生させる</summary>
        /// <param name="e">ZipProgressEventArgsV2</param>
        protected void OnSaveProgress(ZipProgressEventArgsV2 e)
        {
            EventHandler<ZipProgressEventArgsV2> h = this._saveProgress;
            if (h != null) { h(this, e); }
        }

        /// <summary>ExtractProgressイベントを発生させる</summary>
        /// <param name="e">ZipProgressEventArgsV2</param>
        protected void OnExtractProgress(ZipProgressEventArgsV2 e)
        {
            EventHandler<ZipProgressEventArgsV2> h = this._extractProgress;
            if (h != null) { h(this, e); }
        }

        #endregion

        #region 書庫の設定

        /// <summary>書庫に付けるコメント</summary>
        public const string ZipComment = "ZipperV2 @ Powered by SharpZipLib";

        /// <summary>文字コードの設定を作る</summary>
        /// <param name="enc">エンコーディング（nullなら既定）</param>
        /// <returns>StringCodec</returns>
        /// <remarks>
        /// **ZipStrings（プロセス全体の静的設定）は触らない。**
        /// 触ると同一プロセスの他の処理に波及するため、
        /// インスタンス単位で持てる StringCodec を使う。
        ///
        /// **プロパティに代入してはいけない。** セッターが init のため、
        /// C# 7.3 でビルドする net48 側でコンパイル エラーになる（CS0200）。
        /// 用意されている FromEncoding / WithForcedLegacyEncoding を使う。
        ///
        /// WithForcedLegacyEncoding を呼ぶのは、指定した文字コードを
        /// 実際にエントリ名へ使わせるため（既定では UTF-8 が優先される）。
        /// </remarks>
        protected StringCodec GetStringCodec(Encoding enc)
        {
            if (enc == null) { return ZipStrings.GetStringCodec(); }

            return StringCodec.FromEncoding(enc).WithForcedLegacyEncoding();
        }

        #endregion
    }

    /// <summary>進捗イベントの引数</summary>
    /// <remarks>
    /// 旧 Ionic.Zip の SaveProgressEventArgs / ExtractProgressEventArgs の代替。
    /// **プロパティ名は旧に合わせてある**（EventType / EntriesTotal / CurrentEntry 等）。
    /// </remarks>
    public class ZipProgressEventArgsV2 : EventArgs
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="eventType">進捗の種別</param>
        /// <param name="archiveName">書庫のファイル名</param>
        public ZipProgressEventArgsV2(ZipProgressEventTypeV2 eventType, string archiveName)
        {
            this.EventType = eventType;
            this.ArchiveName = archiveName;
        }

        /// <summary>進捗の種別</summary>
        public ZipProgressEventTypeV2 EventType { get; private set; }

        /// <summary>書庫のファイル名</summary>
        public string ArchiveName { get; private set; }

        /// <summary>エントリの総数</summary>
        /// <remarks>種別が Started / Completed 以外では 0 のことがある。</remarks>
        public int EntriesTotal { get; set; }

        /// <summary>処理済みのエントリ数</summary>
        public int EntriesProcessed { get; set; }

        /// <summary>処理中のエントリ名</summary>
        public string CurrentEntryName { get; set; }

        /// <summary>処理中のエントリの総バイト数</summary>
        public long TotalBytesToTransfer { get; set; }

        /// <summary>処理中のエントリの転送済みバイト数</summary>
        public long BytesTransferred { get; set; }

        /// <summary>解凍先のフォルダ</summary>
        /// <remarks>解凍時のみ設定される。</remarks>
        public string ExtractLocation { get; set; }

        /// <summary>ハンドラへの問い合わせか</summary>
        /// <remarks>
        /// Extracting_ExtractEntryWouldOverwrite は 2 つの意味で起きる。
        ///   true  … **問い合わせ**。ExtractExistingFile に返答を設定すること
        ///   false … **通知**。既に「上書きしない」と決まっており、設定しても効かない
        /// **既定値に頼って見分けてはいけない**ため、明示的に持たせている。
        /// </remarks>
        public bool IsQuery { get; set; }

        /// <summary>このエントリをどう扱うか</summary>
        /// <remarks>
        /// **ハンドラが書き換えるためのプロパティ。**
        /// EventType が Extracting_ExtractEntryWouldOverwrite のときだけ意味を持つ
        /// （ExtractExistingFileActionV2.InvokeExtractProgressEvent を指定した場合）。
        ///
        /// 旧 Ionic では ZipEntry 側のプロパティ（e.CurrentEntry.ExtractExistingFile）
        /// だったが、こちらはイベント引数に持たせている。
        /// </remarks>
        public ExtractExistingFileActionV2 ExtractExistingFile { get; set; }

        /// <summary>以降の処理を打ち切るか</summary>
        /// <remarks>ハンドラが true を設定すると、残りのエントリを処理しない。</remarks>
        public bool Cancel { get; set; }
    }
}
