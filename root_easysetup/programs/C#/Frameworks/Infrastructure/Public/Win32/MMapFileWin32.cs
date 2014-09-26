//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

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
//* クラス名        ：MMapFileWin32
//* クラス日本語名  ：メモリ マップト ファイル 関連Win32 API宣言クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/10/26  西野  大介        新規作成
//*  2013/02/18  西野  大介        SetLastError対応
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Win32
{
    /// <summary>
    /// メモリ マップト ファイル 関連Win32 API宣言クラス
    /// 
    /// メモリ マップト ファイルは、
    /// ・ファイル マッピング オブジェクト（FileMap）
    /// ・マップト ビュー（MapView）
    /// から構成される。
    /// 
    /// 詳細は、Advanced Windowsや、以下のURLを参照
    /// 
    /// （新）APIから知るWindowsの仕組み
    /// 第5回 ファイル・マッピング・オブジェクト
    /// によるプロセス間メモリー共有の仕組みを学ぶ 
    /// http://itpro.nikkeibp.co.jp/article/COLUMN/20071107/286607/
    /// </summary>
    public class MMapFileWin32
    {
        #region 列挙型

        #region flProtectに指定する保護属性

        /// <summary>
        /// 以下の値を、ビット単位のOR演算子により組み合わせて指定する。
        /// 
        /// PageXXX（保護属性）:
        /// ファイルをマップする際に、FileMapに割り当てられる保護属性を指定する。
        /// 
        /// SectionXXX（セクション属性値）:
        /// セクションの割り当て方法を指定する。
        /// </summary>
        /// <remarks>
        /// 詳細は http://msdn.microsoft.com/ja-jp/library/cc430039.aspx の「flProtect」を参照
        /// </remarks>
        [Flags]
        public enum FileMapProtection : uint
        {
            // 以下 保護属性

            /// <summary>
            /// [保護属性]
            /// 読み取り専用アクセスを割り当てる。
            /// 
            /// 書き込みまたは実行を試みると、アクセス違反が発生する。
            /// hFile パラメータで指定するファイルは、
            /// GENERIC_READ アクセスを指定して作成したものであること。
            /// </summary>
            PageReadOnly = 0x02,

            /// <summary>
            /// [保護属性]
            /// 読み書きアクセスを割り当てる。
            /// 
            /// hFile パラメータで指定するファイルは、GENERIC_READ
            /// および GENERIC_WRITE アクセスを指定して作成したものであること。
            /// </summary>
            PageReadWrite = 0x04,

            /// <summary>
            /// [保護属性]
            /// （コピーオンライト保護の）読み書きアクセスを割り当てる。
            /// ※ この場合、元のファイルは変更されず、PFが使われる。
            /// 
            /// hFile パラメータで指定するファイルは、GENERIC_READ
            /// および GENERIC_WRITE アクセスを指定して作成したものであること。
            /// </summary>
            PageWriteCopy = 0x08,

            /// <summary>
            /// [保護属性]...
            /// </summary>
            PageExecuteRead = 0x20,

            /// <summary>
            /// [保護属性]...
            /// </summary>
            PageExecuteReadWrite = 0x40,

            // 以下 セクション属性値

            /// <summary>
            /// [セクション属性値]
            /// コミット（物理記憶またはディスク上のファイルを割り当てる）
            /// ※ 既定値、SectionReserveとは排他的。
            /// </summary>
            SectionCommit = 0x8000000,

            /// <summary>
            /// [セクション属性値]
            /// 指定したファイルが、PE形式のファイルであることを示す。
            /// 
            /// この値を指定した場合は、PE形式のファイルからマップ情報とファイル保護
            /// を取得するので、SEC_IMAGE と共に他の属性を指定しても有効にならない。
            /// </summary>
            SectionImage = 0x1000000,

            /// <summary>
            /// [セクション属性値]
            /// キャッシュ不可能にする（更新時のファイル アクセスが頻繁になる）。
            /// 
            /// この属性は、プロセッサにまだフェッチされていないメモリに対して
            /// さまざまなロックメカニズムを必要とするアーキテクチャを想定している。
            /// ※ Advanced Windowsには「このフラグは、デバイスドライバの
            /// 　 開発者用であり、通常のアプリケーションでは使用しない。」とある。
            /// 
            /// x86 と MIPS の各コンピュータでは、このようなメカニズムに対して
            /// キャッシュを適用しても、ハードウェアがキャッシュの一貫性
            /// を維持する必要が生じ、かえってパフォーマンスが低下する。
            /// </summary>
            SectionNoCache = 0x10000000,

            /// <summary>
            /// [セクション属性値]
            /// 予約（物理記憶またはディスク上のファイルを割り当てない）。
            /// ※ SectionCommitとは排他的。
            /// 
            /// 予約済みのページは、解放しない限り、1 つの割り当て操作で使うことはできない。
            /// その後、VirtualAlloc 関数を呼び出すと、予約済みのページをコミットできる。
            /// この属性は、hFile パラメータに INVALID_HANDLE_VALUE を指定したときにだけ有効。
            /// </summary>
            SectionReserve = 0x4000000,

        }

        #endregion

        #region dwDesiredAccessに指定するアクセスタイプ

        /// <summary>
        /// FileMapへのアクセスタイプを指定する。
        /// </summary>
        /// <remarks>
        /// OpenFileMapping, MapViewOfFile, MapViewOfFileEx関数で使用
        /// </remarks>
        [Flags]
        public enum FileMapAccess : uint
        {
            /// <summary>
            /// コピーへの読み書きアクセス。
            /// 
            /// アクセス対象のFileMapは、
            /// FileMapProtection.PageWriteCopyの
            /// 保護を指定して作成したものであること。
            /// 
            /// このフラグが設定された場合、コピー用にPFから物理ストレージをコミット、
            /// コピーオンライトが行われた際に、PFにデータをコピー＋マッピングを変更する。
            /// 
            /// ファイルに対して、コピーへの読み書きが可能なビューをマップすることを認める。
            /// </summary>
            FileMapCopy = 0x0001,

            /// <summary>
            /// 読み書きアクセス。
            /// 
            /// アクセス対象のFileMapは、
            /// FileMapProtection.PageReadWriteの
            /// 保護を指定して作成したものであること。
            /// 
            /// ファイルに対して、読み書き可能なビューをマップすることを認める。
            /// </summary>
            FileMapWrite = 0x0002,

            /// <summary>
            /// 読み取り専用アクセス。
            /// 
            /// アクセス対象のFileMapは、
            /// ・FileMapProtection.PageReadWriteか
            /// ・FileMapProtection.PageReadOnlyの
            /// 保護を指定して作成したものであること。
            /// 
            /// ファイルに対して、読み取り専用のビューをマップすることを認める。
            /// </summary>
            FileMapRead = 0x0004,

            /// <summary>
            /// あらゆるアクセス（FileMapWrite と同じ）。
            /// 
            /// アクセス対象のFileMapは、
            /// FileMapProtection.PageReadWriteの
            /// 保護を指定して作成したものであること。
            /// 
            /// ファイルに対して、読み書き可能なビューをマップすることを認める。
            /// </summary>
            FileMapAllAccess = 0x000f001f, // pinvoke.net では 0x001f ……どちらが正しい?

            /// <summary>
            /// ...
            /// </summary>
            FileMapExecute = 0x0020,
        }

        #endregion

        #endregion

        #region API

        #region CreateFileMapping関数

        /// <summary>
        /// 指定されたファイルに対する、
        /// ・名前付き
        /// ・名前なし
        /// のFileMapを生成。
        /// 
        /// この段階では、ファイルはアドレス空間にマップされていない。
        /// MapViewOfFileにより、アドレス空間にマップされるようになる。
        /// アドレス空間にマップされたのち、ファイルがページインする。
        /// </summary>
        /// <param name="hFile">
        /// FileMap作成元ファイルのハンドルを指定。
        /// 
        /// INVALID_HANDLE_VALUE（new IntPtr(-1)）を指定した場合、
        /// ページング ファイル（以下、PFと略す）を使用。
        /// </param>
        /// <param name="lpFileMappingAttributes">
        /// 取得したハンドルを子プロセスへ継承することを許可
        /// するかどうかを決定するSECURITY_ATTRIBUTES 構造体へのポインタを指定。
        /// 
        /// NULL（IntPtr.Zero）を指定すると、FileMapのハンドルを継承できない。
        /// ここでの継承は、子プロセスへのハンドル継承を意味する。
        /// </param>
        /// <param name="flProtect">
        /// FileMapに割り当てられる保護属性を指定。
        /// 
        /// ビット単位の OR 演算子を使って、ページ保護に関係する前述の値に、
        /// セクション属性値の 1 つまたは複数を組み合わせることができる。
        /// </param>
        /// <param name="dwMaximumSizeHigh">
        /// FileMapの最大サイズ(の上位32bit)を指定する。
        /// </param>
        /// <param name="dwMaximumSizeLow">
        /// FileMapの最大サイズ(の下位32bit)を指定する。
        /// 
        /// dwMaximumSizeLow と dwMaximumSizeHigh の両方のパラメータ
        /// で 0 を指定すると、hFile パラメータで指定したファイルの
        /// 現在のサイズが、FileMapの最大サイズになる。
        /// （サイズ 0 のファイルに対して 0 を指定すると失敗する）
        /// 
        /// なお、PageReadWrite保護属性を指定した状態でFileMapのサイズに
        /// ファイル サイズより大きな値を指定した場合、ファイルが拡張される。
        /// それ以外の場合は、ファイル サイズより小さな値を指定しないと失敗する。
        /// </param>
        /// <param name="lpName">
        /// FileMapの名前を保持している文字列を指定する。
        /// </param>
        /// <returns>
        /// ● FileMapの新規作成に成功した場合は、新しいFileMapのハンドルが返る。
        /// 
        /// ● 指定したFileMapが既に存在していた場合は、既存のFileMapのハンドルが返る。
        /// 
        ///    この場合、指定したサイズではなく、既存のFileMapのサイズになる。
        ///    また、GetLastError 関数の戻り値は ERROR_ALREADY_EXISTS になる。
        ///    
        /// ● 関数が失敗すると、IntPtr.Zero が返る。
        /// 
        /// ● 拡張エラー情報を取得するには、GetLastError 関数を使う。
        /// </returns>
        /// <remarks>
        /// 詳細は http://msdn.microsoft.com/ja-jp/library/cc430039.aspx を参照
        /// </remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile,
            IntPtr lpFileMappingAttributes, FileMapProtection flProtect,
            uint dwMaximumSizeHigh, uint dwMaximumSizeLow,
            [MarshalAs(UnmanagedType.LPTStr)] string lpName);

        #endregion

        #region OpenFileMapping関数

        /// <summary>
        /// 指定されたファイルに対する、
        /// ・名前付き
        /// ・名前なし
        /// のFileMapを開く。
        /// 
        /// この段階では、ファイルはアドレス空間にマップされていない。
        /// MapViewOfFileにより、アドレス空間にマップされるようになる。
        /// アドレス空間にマップされたのち、ファイルがページインする。
        /// </summary>
        /// <param name="dwDesiredAccess">
        /// FileMapへのアクセスのタイプを指定する。
        /// </param>
        /// <param name="bInheritHandle">
        /// 返されたFileMapのハンドルの継承可否を指定する。
        /// ※ ここでの継承は、子プロセスへのハンドル継承を意味する。
        /// </param>
        /// <param name="lpName">
        /// 開くべきFileMapの名前を指定する。
        /// 
        /// 同じ名前のFileMapに対して既にハンドルが開いていて、
        /// そのFileMapに関するセキュリティ記述子が、dwDesiredAccess パラメータ
        /// の指定と競合しない場合は、そのFileMapを開くことに成功する。
        /// 
        /// [Terminal Services] グローバル名前空間またはセッション名前空間でオブジェクトを
        /// 明示的に開くために、「Global\」または「Local\」の接頭辞を付けることができる。
        /// 名前の残りの部分は、円記号（\）を除き、任意の文字を記述できる。
        /// 詳細は http://support.microsoft.com/kb/308403/ja を参照
        /// </param>
        /// <returns>
        /// ● 関数が成功すると、FileMapに関連する、既に開いているハンドルが返る。
        /// ● 関数が失敗すると、IntPtr.Zero が返る。
        /// ● 拡張エラー情報を取得するには、GetLastError 関数を使う。
        /// </returns>
        /// <remarks>詳細は http://msdn.microsoft.com/ja-jp/library/cc430187.aspx を参照</remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenFileMapping(
            FileMapAccess dwDesiredAccess, bool bInheritHandle, string lpName);

        #endregion

        #region MapViewOfFile（Ex）関数

        /// <summary>
        /// 呼び出し側プロセスのアドレス空間に、FileMapをマップしMapViewを返す。
        /// 
        /// アドレス空間にマップされた後、当該アドレス
        /// にアクセスするとファイルがページインする。
        /// 
        /// 同じFileMapを使用している場合、
        /// ページインした物理メモリを共有する。
        /// →　これを共有メモリの仕組みに利用できる。
        /// 
        /// 推奨されるベースアドレスを指定するには、
        /// <see cref="MMapFileWin32.MapViewOfFileEx"/>
        /// 関数を使う。
        /// </summary>
        /// <param name="hFileMappingObject">
        /// 開いているFileMapのハンドルを指定する。
        /// <see cref="MMapFileWin32.CreateFileMapping"/> と
        /// <see cref="MMapFileWin32.OpenFileMapping"/> の
        /// 各関数がFileMapのハンドルを返す。
        /// </param>
        /// <param name="dwDesiredAccess">
        /// MapView（ページ）のアクセスタイプ（保護タイプ）を指定する。</param>
        /// <param name="dwFileOffsetHigh">
        /// マップを開始するFileMapのオフセット(の上位32bit)を指定する。
        /// </param>
        /// <param name="dwFileOffsetLow">
        /// マップを開始するFileMapのオフセット(の下位32bit)を指定する。
        /// 
        /// 上位と下位の組み合わせによって形成される組み合わせで、システムの
        /// メモリ割り当ての粒度（最小単位）に一致するオフセットを指定する必要がある。
        /// 
        /// それ以外の場合、関数は失敗する。
        /// ※ つまり、このオフセットは、
        /// 　 メモリ割り当ての粒度の倍数である必要がある。
        /// 
        /// 関数を呼び出し、構造体の dwAllocationGranularity メンバに
        /// 書き込まれた値を使って、システムのメモリ割り当ての粒度を取得する。
        /// </param>
        /// <param name="dwNumberOfBytesToMap">
        /// マップするFileMapのバイト数を指定する。
        /// 0 を指定すると、全体がマップされる。
        /// </param>
        /// <returns>
        /// ● 関数が成功すると、MapViewの開始アドレスが返る。
        /// ● 関数が失敗すると、IntPtr.Zero が返る。
        /// ● 拡張エラー情報を取得するには、GetLastError 関数を使う。
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject, FileMapAccess dwDesiredAccess,
            uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        /// <summary>
        /// 基本的にMapViewOfFileと同じ。
        /// 
        /// ただし、本拡張関数では、マップ先の
        /// ベースアドレスを呼び出し側プロセスで指定できる。
        /// </summary>
        /// <param name="hFileMappingObject">
        /// 開いているFileMapのハンドルを指定する。
        /// <see cref="MMapFileWin32.CreateFileMapping"/> と
        /// <see cref="MMapFileWin32.OpenFileMapping"/> の
        /// 各関数がFileMapのハンドルを返す。
        /// </param>
        /// <param name="dwDesiredAccess">
        /// MapView（ページ）のアクセスタイプ（保護タイプ）を指定する。
        /// </param>
        /// <param name="dwFileOffsetHigh">
        /// マップを開始するFileMapのオフセット(の上位32bit)を指定する。
        /// </param>
        /// <param name="dwFileOffsetLow">
        /// マップを開始するFileMapのオフセット(の下位32bit)を指定する。
        /// 
        /// 上位と下位の組み合わせによって形成される組み合わせで、システムの
        /// メモリ割り当ての粒度（最小単位）に一致するオフセットを指定する必要がある。
        /// 
        /// それ以外の場合、関数は失敗する。
        /// ※ つまり、このオフセットは、
        /// 　 メモリ割り当ての粒度の倍数である必要がある。
        /// 
        /// 関数を呼び出し、構造体の dwAllocationGranularity メンバに
        /// 書き込まれた値を使って、システムのメモリ割り当ての粒度を取得する。
        /// </param>
        /// <param name="dwNumberOfBytesToMap">
        /// マップするFileMapのバイト数を指定する。
        /// 0 を指定すると、全体がマップされる。
        /// </param>
        /// <param name="lpBaseAddress">
        /// マップ先のベースアドレス（メモリ ポインタ）を指定する。
        /// 
        /// この値は、システムのメモリ割り当ての単位の倍数でなければならない。
        /// 関数を呼び出し、構造体の dwAllocationGranularity メンバに
        /// 書き込まれた値を使って、システムのメモリ割り当ての粒度を取得する。
        /// 
        /// ● それ以外の値を指定すると、関数は失敗する。
        /// ● 指定されたアドレスに十分なアドレス空間がないと、関数は失敗する。
        /// 
        /// IntPtr.Zero を指定すると、OSがマップアドレスを選択する。
        /// この場合、本関数は MapViewOfFile関数と同じ機能を果たす。
        /// </param>
        /// <returns>
        /// ● 関数が成功すると、MapViewの開始アドレスが返る。
        /// ● 関数が失敗すると、IntPtr.Zero が返る。
        /// ● 拡張エラー情報を取得するには、GetLastError 関数を使う。
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFileEx(IntPtr hFileMappingObject,
           FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow,
           UIntPtr dwNumberOfBytesToMap, IntPtr lpBaseAddress);

        #endregion

        #region FlushViewOfFile関数

        /// <summary>
        /// MapView内の指定された範囲のデータをディスクに書き込む。
        /// 
        /// ※ 特にファイルをネットワーク越しに共有している場合、
        /// 　 本関数によりキャッシュを確実にネットワークに送るが、
        /// 　 サーバがキャッシュする可能性があるので、この場合、
        /// 　 CreateFileにFILE_FLAG_WRITE_THROUGHフラグを指定
        /// 　 することで、書込みを保証する同期呼出しとなる。
        /// 　 （PFファイルを使用する際は関係ない）
        /// </summary>
        /// <param name="lpBaseAddress">
        /// 開始アドレス（ページ境界に調整される）
        /// </param>
        /// <param name="dwNumberOfBytesToFlush">
        /// 範囲内のバイト数
        /// 
        /// ※ 0 を指定すると、開始アドレスから最後
        /// 　 までのデータがディスクに書き込まれる。 
        /// </param>
        /// <returns>
        /// 成功：true
        /// 失敗：false
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FlushViewOfFile(
            IntPtr lpBaseAddress, UIntPtr dwNumberOfBytesToFlush);

        #endregion

        #region UnmapViewOfFile関数

        /// <summary>
        /// 呼び出し側プロセスのアドレス空間から、MapViewをアンマップする。
        /// </summary>
        /// <param name="lpBaseAddress">
        /// MapViewのベースアドレスを指定する。
        /// 
        /// この値は、以前に呼び出した
        /// ・MapViewOfFile
        /// ・MapViewOfFileEx 
        /// 関数が返した値を使用する。
        /// </param>
        /// <returns>
        /// ● 関数が成功すると、true が返り、
        ///    指定された内容が変更されたページが、ディスクに遅延書き込みされる。
        ///    
        /// ● 関数が失敗すると、false が返る。
        /// ● 拡張エラー情報を取得するには、GetLastError 関数を使う。
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        #endregion

        #endregion
    }
}
