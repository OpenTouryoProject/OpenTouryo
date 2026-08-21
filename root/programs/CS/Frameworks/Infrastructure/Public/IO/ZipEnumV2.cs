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
//* クラス名        ：ZipEnumV2
//* クラス日本語名  ：ZIP部品（V2）の列挙型
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/08  玄人 幸道         新規作成（#524）
//*  2026/08/08  玄人 幸道         ZippedFiles・ExtractedFilesの追加に伴う定義の調整（#528）
//**********************************************************************************

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>暗号化方式</summary>
    /// <remarks>
    /// 旧 Zipper が使っていた Ionic.Zip.EncryptionAlgorithm の代替。
    /// **メンバ名は旧と同じ**にしてある（移植時に読み替えが要らないように）。
    ///
    /// 旧にあった Unsupported は持たない。
    /// Ionic では「設定不可能」を表す番人だったが、SharpZipLib に相当が無く、
    /// None との違いを呼び出し側が意識する必要も無いため。
    /// </remarks>
    public enum ZipEncryptionAlgorithmV2
    {
        /// <summary>暗号化しない</summary>
        None = 0,

        /// <summary>ZipCrypto（PKZIP 伝統方式）</summary>
        /// <remarks>
        /// **強度は低い。** 互換性のためだけに残している。
        /// 新規に選ぶなら WinZipAes256 を使うこと。
        /// </remarks>
        PkzipWeak = 1,

        /// <summary>WinZip AES 128bit</summary>
        WinZipAes128 = 2,

        /// <summary>WinZip AES 256bit</summary>
        WinZipAes256 = 3,
    }

    /// <summary>圧縮レベル</summary>
    /// <remarks>
    /// 旧 Zipper が使っていた Ionic.Zlib.CompressionLevel の代替。
    /// Ionic は 0〜9 の全段階を列挙していたが、実用上の 4 段階だけを持つ。
    /// 値は SharpZipLib の ZipOutputStream.SetLevel に渡す 0〜9 に対応する。
    /// </remarks>
    public enum ZipCompressionLevelV2
    {
        /// <summary>無圧縮（格納のみ）</summary>
        None = 0,

        /// <summary>速度優先</summary>
        BestSpeed = 1,

        /// <summary>既定</summary>
        Default = 6,

        /// <summary>圧縮率優先</summary>
        BestCompression = 9,
    }

    /// <summary>解凍先に同名ファイルがあったときの動作</summary>
    /// <remarks>
    /// 旧 UnZipper が使っていた Ionic.Zip.ExtractExistingFileAction の代替。
    /// **メンバ名は旧と同じ**。
    /// </remarks>
    public enum ExtractExistingFileActionV2
    {
        /// <summary>例外にする</summary>
        Throw = 0,

        /// <summary>黙って上書きする</summary>
        OverwriteSilently = 1,

        /// <summary>上書きせず、そのファイルを飛ばす</summary>
        DoNotOverwrite = 2,

        /// <summary>ExtractProgressイベントで、1件ずつ問い合わせる</summary>
        /// <remarks>
        /// Extracting_ExtractEntryWouldOverwrite でイベントが起き、
        /// ハンドラが ZipProgressEventArgsV2.ExtractExistingFile に
        /// Throw / OverwriteSilently / DoNotOverwrite のいずれかを設定して返す。
        /// Cancel に true を設定すると、以降の解凍を打ち切る。
        ///
        /// **ここに InvokeExtractProgressEvent を設定して返してはいけない**（無限に問い合わせるため）。
        /// その場合は DoNotOverwrite として扱う。
        /// </remarks>
        InvokeExtractProgressEvent = 3,
    }

    /// <summary>進捗の種別</summary>
    /// <remarks>
    /// 旧 Zipper / UnZipper が使っていた Ionic.Zip.ZipProgressEventType のうち、
    /// **呼び出し元が実際に分岐していたものだけ**を持つ。
    /// </remarks>
    public enum ZipProgressEventTypeV2
    {
        /// <summary>圧縮を開始した</summary>
        Saving_Started = 0,

        /// <summary>1 エントリの書き込み前</summary>
        Saving_BeforeWriteEntry = 1,

        /// <summary>1 エントリの書き込み中（バイト単位）</summary>
        Saving_EntryBytesRead = 2,

        /// <summary>1 エントリの書き込み後</summary>
        Saving_AfterWriteEntry = 3,

        /// <summary>圧縮が完了した</summary>
        Saving_Completed = 4,

        /// <summary>解凍を開始した</summary>
        Extracting_Started = 5,

        /// <summary>1 エントリの解凍前</summary>
        Extracting_BeforeExtractEntry = 6,

        /// <summary>1 エントリの解凍中（バイト単位）</summary>
        Extracting_EntryBytesWritten = 7,

        /// <summary>1 エントリの解凍後</summary>
        Extracting_AfterExtractEntry = 8,

        /// <summary>上書きになるため飛ばした</summary>
        Extracting_ExtractEntryWouldOverwrite = 9,

        /// <summary>解凍が完了した</summary>
        Extracting_Completed = 10,
    }
}
