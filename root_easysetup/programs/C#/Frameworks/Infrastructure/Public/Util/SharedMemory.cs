//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：SharedMemory
//* クラス日本語名  ：共有メモリクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/10/26  西野  大介        新規作成
//*  2010/11/04  西野  大介        アンロックの実装変更、マップの実装変更、
//*                                ・・・ミューテックス名の指定方法の変更
//*  2011/10/09  西野  大介        国際化対応
//**********************************************************************************

// 同期とマーシャリング
using System.Threading;
using System.Runtime;
using System.Runtime.InteropServices;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Win32;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>
    /// 共有メモリクラス（メモリ マップト ファイル）
    /// 
    /// ・ 更新ロック用のスレッド排他を実装する（Mutex）。
    /// 
    /// ・ この共有メモリは、1 つのFileMapを使用している限り、一貫性が有る。
    /// 　 プロセス間で物理メモリが共有されるため書込も直ちに反映される
    /// 　 （ただし、ディスクへの反映は遅延することもある）。
    /// 　 ※ これに対しMapViewは複数使用しても問題ない。
    /// </summary>
    /// <remarks>
    /// 各種リソース解放のためIDisposableを適切に実装している。
    /// 
    /// MapViewのポインタ（IntPtr）は .ToPointer() でunsafeポインタ（void*）に変換可能。
    /// ポインタ（void*）を使用する場合、C#のプロジェクト プロパティ[ビルド]タブで、
    /// [アンセーフ コードの許可(U)] を ONにする必要がある([Debug], [Release]の両方とも)。
    /// </remarks>
    public class SharedMemory : IDisposable
    {
        #region メンバ＆プロパティ

        /// <summary>FileMapの名前</summary>
        private string _fileMapName = "";

        /// <summary>FileMapの名前</summary>
        public string FileMapName
        {
            get
            {
                return this._fileMapName;
            }
        }

        /// <summary>FileMapのオブジェクト ハンドル</summary>
        private IntPtr _fileMapHandle = IntPtr.Zero;

        /// <summary>FileMapのオブジェクト ハンドル</summary>
        public IntPtr FileMapHandle
        {
            get
            {
                return this._fileMapHandle;
            }
        }

        /// <summary>FileMapの最大バイト数</summary>
        private uint _maxFileMapByteSize = 0;

        /// <summary>FileMapの最大バイト数</summary>
        public uint MaxFileMapByteSize
        {
            get
            {
                return this._maxFileMapByteSize;
            }
        }

        /// <summary>MapViewのポインタ</summary>
        private IntPtr _mappedViewPointer = IntPtr.Zero;

        /// <summary>MapViewのポインタ</summary>
        public IntPtr MappedViewPointer
        {
            get
            {
                return this._mappedViewPointer;
            }
        }

        /// <summary>現在のMapViewのバイト数</summary>
        private uint _currentMapViewByteSize = 0;

        /// <summary>現在のMapViewのバイト数</summary>
        public uint CurrentMapViewByteSize
        {
            get
            {
                return this._currentMapViewByteSize;
            }
        }

        /// <summary>更新ロックを行うMutexの名前</summary>
        private string _updateLockName = null;

        /// <summary>更新ロックを行うMutexの名前</summary>
        public string UpdateLockName
        {
            get
            {
                return this._updateLockName;
            }
        }

        /// <summary>更新ロックを行うMutex</summary>
        private Mutex _updateLock = null;

        /// <summary>更新ロックを行うMutex</summary>
        public Mutex UpdateLock
        {
            get
            {
                return this._updateLock;
            }
        }
        
        #endregion

        /// <summary>
        /// コンストラクタ１
        /// </summary>
        /// <param name="fileMapName">
        /// FileMapのオブジェクト名
        /// </param>
        /// <param name="maxByteSize">
        /// FileMapのバイト数(0を指定すると、全体を対象)
        /// ※ uintなので最大、4.294967295GBまで指定可能。
        /// </param>
        /// <param name="updateLockName">
        /// 更新ロックを行うMutexの名前
        /// </param>
        public SharedMemory(string fileMapName, uint maxByteSize, string updateLockName)
        {
            // メンバに設定

            // 各種設定を保持
            this._fileMapName = fileMapName;
            this._maxFileMapByteSize = maxByteSize;
            this._updateLockName = updateLockName;

            this._mappedViewPointer = IntPtr.Zero; // 初期化

            // FileMapを開く。
            this._fileMapHandle = MMapFileWin32.OpenFileMapping(
                 MMapFileWin32.FileMapAccess.FileMapAllAccess, false, this.FileMapName);

            // 戻り値のチェック
            if (this.FileMapHandle == IntPtr.Zero)
            {
                // 開けなかった場合、
                
                // エラーコードをチェック
                if (CmnWin32.ErrorCodes.ERROR_FILE_NOT_FOUND == CmnWin32.GetLastError())
                {
                    // ファイルが存在しない場合、生成する。
                    this._fileMapHandle = MMapFileWin32.CreateFileMapping(
                        //IntPtr.Zero, IntPtr.Zero,
                        new IntPtr(-1), IntPtr.Zero,
                        MMapFileWin32.FileMapProtection.PageReadWrite
                        | MMapFileWin32.FileMapProtection.SectionCommit,
                        0, this.MaxFileMapByteSize, this.FileMapName);
                }
                else
                {
                    // 戻り値のチェック
                    if (this.FileMapHandle == IntPtr.Zero)
                    {
                        // 生成できなかった場合

                        this.Dispose(); // GC前にクリーンナップ

                        throw new WindowsAPIErrorException(
                            CmnWin32.GetLastError(), string.Format(
                                WindowsAPIErrorException.MessageTemplate, "CreateFileMapping"));
                    }
                    else
                    {
                        // 生成できた場合
                    }
                }
            }
            else
            {
                // 開けた場合
            }
        }

        /// <summary>
        /// FileMapをメモリ空間にマップし、MapViewを取得する。
        /// </summary>
        /// <param name="offset">
        /// FileMapの下位オフセット（32bitに制限）
        /// ※ uintなので最大、4.294967295GBまで指定可能。
        /// </param>
        /// <param name="mapViewByteSize">
        /// FileMapのバイト数(0を指定すると、全体を対象)
        /// ※ uintなので最大、4.294967295GBまで指定可能。
        /// </param>
        public void Map(uint offset, uint mapViewByteSize)
        {
            // チェック
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException("SharedMemory");//, "Dispose済み。");
            }

            // offsetHighは設定しない（32bitに制限するため）。
            uint offsetHigh = 0;
            uint offsetLow = offset;

            // マイナス値や、FileMapのサイズを超える場合は、FileMapのサイズに合わせる
            if (mapViewByteSize < 0 || (this.MaxFileMapByteSize < mapViewByteSize))
            {
                this._currentMapViewByteSize = this.MaxFileMapByteSize;
            }

            // 既にマップされている場合は、
            if (this._mappedViewPointer != IntPtr.Zero)
            {
                // 一度アンマップしてから、
                this.Unmap();
            }
            // マップしなおす（↓）。

            // FileMapをメモリ空間にマップし、
            // MapViewを取得する（MapViewのアドレスを返す）。
            this._mappedViewPointer
                = MMapFileWin32.MapViewOfFile(this.FileMapHandle,
                    MMapFileWin32.FileMapAccess.FileMapAllAccess,
                    offsetHigh, offsetLow, this.CurrentMapViewByteSize);

            // 0を指定した際の仕様に合わせて
            if (this._currentMapViewByteSize == 0)
            {
                this._currentMapViewByteSize = this.MaxFileMapByteSize;
            }

            // MapViewの取得エラー 
            if (this.MappedViewPointer == IntPtr.Zero)
            {
                this.Dispose(); // GC前にクリーンナップ

                throw new WindowsAPIErrorException(
                    CmnWin32.GetLastError(), string.Format(
                        WindowsAPIErrorException.MessageTemplate, "MapViewOfFile"));
            }
        }

        /// <summary>MapViewをメモリ空間からアンマップする。</summary>
        public void Unmap()
        {
            // チェック
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException("SharedMemory");//, "Dispose済み。");
            }

            // MapViewが存在しない場合は処理しない
            if (this.MappedViewPointer != IntPtr.Zero)
            {
                // UnmapViewOfFileでアンマップ
                MMapFileWin32.UnmapViewOfFile(this.MappedViewPointer);

                // IntPtr.Zeroでクリア
                this._mappedViewPointer = IntPtr.Zero;
            }
        }

        /// <summary>更新ロックの開始</summary>
        /// <remarks>複数のスレッドから書きこむ場合、（読み込む前から）ロックを開始する。</remarks>
        public void Lock()
        {
            // Mutexが無い場合は
            if (this._updateLock == null)
            {
                // 作成する（名前付き）
                this._updateLock = new Mutex(false, this._updateLockName);
            }

            // 更新ロックの開始
            this.UpdateLock.WaitOne();// タイムアウトを指定しない場合は、trueしか返らない。
        }

        /// <summary>更新ロックの終了</summary>
        /// <remarks>複数のスレッドから書きこむ場合、（読み込む前から）ロックを開始する。</remarks>
        public void Unlock()
        {
            // 更新ロックの終了
            if (this.UpdateLock != null)
            {
                // ★ ロックしたスレッドが解放しないで終了すると例外発生。
                //    メッセージ：放棄されたミューテックスのため、待機は完了しました。
                //    →　本クラスのデストラクタでは、マネージリソース（Mutex）の解放は行わない。
                //        Mutex自身のデストラクタでミューテックスが解放されることで以降、利用可能になる。

                // ★ 上記の例外を発生させないように解放処理を実装すると２重解放になることがあるが、この場合も例外発生。
                //    メッセージ：オブジェクト同期メソッドは、コードの非同期ブロックから呼び出されました。
                //    →　この例外を潰すことで対応する（利便性は向上するが、問題あるか？）。

                try
                {
                    this.UpdateLock.ReleaseMutex();
                }
                catch//(Exception ex)
                {
                    //// 潰してOK？
                    //if (ex.Message.IndexOf("非同期ブロック") == -1)
                    //{
                    //    throw; // リスロー
                    //}

                    // ↑英語環境などに対応できないので、NG（一律潰す）
                }
            }
        }

        /// <summary>MapViewのメモリ内容をbyte配列（.NET）にコピーする。</summary>
        /// <param name="buf">MapViewのメモリ内容（.NETのbyte配列）</param>
        /// <param name="size">
        /// コピーするサイズ（バイト）
        /// 0以下の値を指定すると、MapViewのサイズを使用する。
        /// </param>
        /// <returns>
        /// 成功：true
        /// 失敗：false
        /// </returns>
        public bool GetMemory(out byte[] buf, int size)
        {
            // チェック
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException("SharedMemory");//, "Dispose済み。");
            }

            // nullポインタの場合
            if (this.MappedViewPointer == IntPtr.Zero)
            {
                buf = null;
                return false;
            }
            else
            {
                // MapViewのサイズもチェック
                int temp = (int)this.CurrentMapViewByteSize;

                // オーバーフロー時、intの最大値を使用
                if (temp < 0) { temp = int.MaxValue; }

                if (size <= 0)
                {
                    // サイズの自動指定
                    size = temp;
                }
                else
                {
                    // MapViewより大きなサイズを指定しない。
                    if (temp < size) { size = temp; }
                }

                // バッファの用意
                buf = new byte[size];

                // Marshal.Copyを使用して.NETに持ってくる。

                // Marshal.Copy：データをコピー
                // ・マネージ メモリ ポインタ → マネージ バイト配列
                Marshal.Copy(this.MappedViewPointer, buf, 0, size);

                return true;
            }
        }

        /// <summary>byte配列（.NET）の内容をMapViewのメモリにコピーする。</summary>
        /// <param name="buf">コピーする内容（.NETのbyte配列）</param>
        /// <param name="size">
        /// コピーするサイズ（バイト）
        /// 0以下の値を指定すると、bufのサイズを使用する。
        /// </param>
        /// <returns>
        /// 成功：true
        /// 失敗：false
        /// </returns>
        public bool SetMemory(byte[] buf, int size)
        {
            // チェック
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException("SharedMemory");//, "Dispose済み。");
            }

            // nullポインタの場合
            if (this.MappedViewPointer == IntPtr.Zero)
            {
                return false;
            }
            else
            {
                // MapViewのサイズもチェック
                int temp = (int)this.CurrentMapViewByteSize;

                // オーバーフロー時、intの最大値を使用
                if (temp < 0) { temp = int.MaxValue; }

                if (size <= 0)
                {
                    // サイズの自動指定
                    size = buf.Length;
                }
                else
                {
                    // MapViewより大きなサイズを指定しない。
                    if (temp < size) { size = temp; }
                }

                // Marshal.Copyを使用してMapViewに持っていく。

                // Marshal.Copy：データをコピー
                // ・マネージ バイト配列 → マネージ メモリ ポインタ
                Marshal.Copy(buf, 0, this.MappedViewPointer, size);
                
                return true;
            }
        }

        #region IDisposable メンバ

        /// <summary>Disposeが呼ばれたかどうかを追跡する</summary>
        /// <remarks>
        /// MSDN > .NET 開発
        /// 
        /// > アンマネージ リソースのクリーンアップ > Dispose メソッドの実装
        /// http://msdn.microsoft.com/ja-jp/library/fs2xkftw.aspx
        /// http://msdn.microsoft.com/ja-jp/library/fs2xkftw(VS.80).aspx
        /// 
        /// > クラス ライブラリ開発のデザイン ガイドライン > デザイン パターン
        ///  > アンマネージ リソースをクリーンアップするための Finalize および Dispose の実装
        /// http://msdn.microsoft.com/ja-jp/library/b1yfkh5e.aspx
        /// http://msdn.microsoft.com/ja-jp/library/b1yfkh5e(VS.80).aspx
        /// </remarks>
        private bool IsDisposed = false;

        /// <summary>Close（→ Dispose）</summary>
        /// <remarks>
        /// これはvirtualメソッドにしない。
        /// 派生しているクラスはこのメソッドを
        /// オーバーライドできないようにすべき。
        /// </remarks>
        public void Close()
        {
            this.Dispose();
        }

        /// <summary>IDisposable.Dispose（１） </summary>
        /// <remarks>
        /// これはvirtualメソッドにしない。
        /// 派生しているクラスはこのメソッドを
        /// オーバーライドできないようにすべき。
        /// </remarks>
        public void Dispose()
        {
            // trueはユーザからの直接・間接的実行を意味する。
            this.Dispose(true);

            // このクラスのデストラクタ（Finalizeメソッド）を呼び出さないようGCに命令。
            GC.SuppressFinalize(this);
        }

        /// <summary>IDisposable.Dispose（２）</summary>
        /// <param name="disposing">
        /// true：ユーザからの直接・間接的実行を意味する。
        /// false：デストラクタ（Finalizeメソッド）からの実行を意味する。
        /// </param>
        /// <remarks>
        /// このメソッドは異なった２つのシナリオを作成する。
        /// 
        /// ・ disposing = true
        /// 　 → マネージ・アンマネージ リソースをクリーンナップする。
        /// 　 
        /// ・disposing = false
        /// 　 → アンマネージ リソースのみクリーンナップする。
        /// 　 　 マネージ リソースはGCによりクリーンナップされるため。
        /// 　 　 
        /// ※ 本メソッドのオーバライド時は、base.Dispose(disposing)を呼ぶこと。
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            // ここのコードは必要であれば、スレッド セーフに実装する。

            // Disposeが既に呼ばれたかチェック
            if (!this.IsDisposed)
            {
                // まだ呼ばれていない場合、
                // 全てのリソースをクリーンナップ

                if (disposing)
                {
                    // ユーザからの直接・間接的実行
                    this.DisposeManagedResources();
                    this.DisposeUnManagedResources();
                }
                else
                {
                    // デストラクタ（Finalizeメソッド）からの実行
                    this.DisposeUnManagedResources();
                }

                // Disposeが既に呼ばれたとフラグを立てる。
                this.IsDisposed = true;
            }
            else
            {
                // 既に呼ばれている場合、
                // なにもしない。
            }
        }

        /// <summary>マネージ リソースをクリーンナップ</summary>
        /// <remarks>
        /// こちらは、ユーザの実行する
        /// Close、Disposeメソッドからのみ実行される。
        /// </remarks>
        private void DisposeManagedResources()
        {
            // ロックしていたら解放する。
            this.Unlock();

            // リソースの開放
            if (this.UpdateLock != null)
            {
                this.UpdateLock.Close();
            }
        }

        /// <summary>アンマネージ リソースをクリーンナップ</summary>
        /// <remarks>
        /// こちらは、ユーザ、デストラクタ（Finalizeメソッド）
        /// の双方が実行するClose、Disposeメソッドから実行される。
        /// </remarks>
        private void DisposeUnManagedResources()
        {
            // MapViewのアンマップ
            this.Unmap();

            // FileMapのハンドラをクローズする。
            if (this._fileMapHandle != IntPtr.Zero)
            {
                CmnWin32.CloseHandle(this.FileMapHandle);
                this._fileMapHandle = IntPtr.Zero;
            }
        }

        /// <summary>デストラクタ（Finalizeメソッド）を実装</summary>
        /// <remarks>
        /// デストラクタ（Finalizeメソッド）は、GCにより呼び出される。
        /// C#ではFinalizeメソッドはオーバライドできない。デストラクタを使用する。
        /// 
        /// MSDN > .NET Framework の拡張開発
        /// > ガベージ コレクション
        /// http://msdn.microsoft.com/ja-jp/library/0xy59wtx.aspx
        /// http://msdn.microsoft.com/ja-jp/library/0xy59wtx(VS.80).aspx
        /// > Finalize メソッドおよびデストラクタ
        /// http://msdn.microsoft.com/ja-jp/library/0s71x931.aspx
        /// http://msdn.microsoft.com/ja-jp/library/0s71x931(VS80).aspx
        /// </remarks>
        ~SharedMemory() // アクセス修飾子はない（ユーザから呼べない）
        {
            // falseはデストラクタ（Finalizeメソッド）からの実行を意味する。
            this.Dispose(false);
        }

        #endregion
    }
}
