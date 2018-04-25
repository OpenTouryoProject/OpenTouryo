//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
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
//* クラス名        ：Entry
//* クラス日本語名  ：エントリ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野 大介         新規作成
//*  2011/04/25  西野 大介         IEquatableの実装（List.Remove対策）
//*                                ・List.Remove メソッド
//*                                  http://msdn.microsoft.com/ja-jp/library/cd666k3e.aspx
//*                                ・EqualityComparer.Default プロパティ
//*                                  http://msdn.microsoft.com/ja-jp/library/ms224763.aspx
//*                                ・IEquatable ジェネリック インターフェイス
//*                                  http://msdn.microsoft.com/ja-jp/library/ms131187.aspx
//**********************************************************************************

using System;
using System.Collections.Generic;

namespace DeployZipPackWithHTTP
{
    /// <summary>エントリ</summary>
    [Serializable()]
    public class Entry : IEquatable<Entry>
    {
        #region メンバ

        /// <summary>WWWサーバのURL</summary>
        public string WWWURL { get; set; }
        /// <summary>WWWサーバのユーザID</summary>
        public string WWWUID { get; set; }
        /// <summary>WWWサーバのパスワード</summary>
        public string WWWPWD { get; set; }
        /// <summary>WWWサーバのドメイン</summary>
        public string WWWDomain { get; set; }

        /// <summary>ProxyサーバのURL</summary>
        public string ProxyURL { get; set; }
        /// <summary>ProxyサーバのユーザID</summary>
        public string ProxyUID { get; set; }
        /// <summary>Proxyサーバのパスワード</summary>
        public string ProxyPWD { get; set; }
        /// <summary>Proxyサーバのドメイン</summary>
        public string ProxyDomain { get; set; }

        /// <summary>マニュフェストの最終更新日付</summary>
        public string HttpLastModified { get; set; }

        /// <summary>インストール ディレクトリ</summary>
        public string InstallDir { get; set; }
        /// <summary>起動exe</summary>
        public string StartExe { get; set; }

        /// <summary>ZipFileコンテンツ</summary>
        public List<string> ZipFiles { get; set; }
        /// <summary>ZipFileコンテンツの最終更新日付</summary>
        public Dictionary<string, string> HttpZipLastModified { get; set; }
        /// <summary>ZipFileコンテンツの内容物（解凍先ファイル）</summary>
        public Dictionary<string, string[]> HttpZipContents { get; set; }
        
        #endregion

        /// <summary>コンストラクタ</summary>
        public Entry()
        {
            this.ZipFiles
                = new List<string>();
            this.HttpZipLastModified
                = new Dictionary<string, string>();
            this.HttpZipContents
                = new Dictionary<string, string[]>();
        }

        #region IEquatableの実装

        /// <summary>ハッシュを返す</summary>
        /// <returns>ハッシュコード</returns>
        /// <remarks>全メンバのハッシュコードのXOR</remarks>
        public override int GetHashCode()
        {
            int ret = 0;
            //---
            if (this.WWWURL != null)
            {
                ret ^= this.WWWURL.GetHashCode();
            }
            if (this.WWWUID != null)
            {
                ret ^= this.WWWUID.GetHashCode();
            }
            if (this.WWWPWD != null)
            {
                ret ^= this.WWWPWD.GetHashCode();
            }
            if (this.WWWDomain != null)
            {
                ret ^= this.WWWDomain.GetHashCode();
            }
            //---
            if (this.ProxyURL != null)
            {
                ret ^= this.ProxyURL.GetHashCode();
            }
            if (this.ProxyUID != null)
            {
                ret ^= this.ProxyUID.GetHashCode();
            }
            if (this.ProxyPWD != null)
            {
                ret ^= this.ProxyPWD.GetHashCode();
            }
            if (this.ProxyDomain != null)
            {
                ret ^= this.ProxyDomain.GetHashCode();
            }
            //---
            if (this.HttpLastModified != null)
            {
                ret ^= this.HttpLastModified.GetHashCode();
            }
            //---
            if (this.InstallDir != null)
            {
                ret ^= this.InstallDir.GetHashCode();
            }
            if (this.StartExe != null)
            {
                ret ^= this.StartExe.GetHashCode();
            }
            //---
            if (this.ZipFiles != null)
            {
                ret ^= this.ZipFiles.GetHashCode();
            }
            if (this.HttpZipLastModified != null)
            {
                ret ^= this.HttpZipLastModified.GetHashCode();
            }
            if (this.HttpZipContents != null)
            {
                ret ^= this.HttpZipContents.GetHashCode();
            }
            
            return ret;
        }

        /// <summary>Equals</summary>
        /// <param name="entry">Entry</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        /// <remarks>全メンバの==のAND</remarks>
        public bool Equals(Entry entry)
        {
            // null対応
            if (entry == null) { return false; }

            // 一意性の保障はWWWURL（ID）だけで判断可能。
            return
                (this.WWWURL == entry.WWWURL);
        }

        /// <summary>Equals</summary>
        /// <param name="obj">Entry</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                // nullの場合（ベースへ）
                return base.Equals(obj);
            }
            else
            {
                // nullでない場合
                if (!(obj is Entry))
                {
                    // 型が違う
                    return false;
                }
                else
                {
                    // 型が一致（オーバロードヘ）
                    return Equals(obj as Entry);
                }
            }
        }

        /// <summary>比較演算子（==）</summary>
        /// <param name="l">右辺</param>
        /// <param name="r">左辺</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public static bool operator ==(Entry l, Entry r)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(l, null))
            {
                // Check for null on right side.
                if (Object.ReferenceEquals(r, null))
                {
                    // null == null = true.
                    return true;
                }
                else
                {
                    // Only the left side is null.
                    return false;
                }
            }
            else
            {
                // Equals handles case of null on right side.
                return l.Equals(r);
            }
        }

        /// <summary>比較演算子（!=）</summary>
        /// <param name="l">右辺</param>
        /// <param name="r">左辺</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public static bool operator !=(Entry l, Entry r)
        {
            // ==演算子の逆
            return !(l == r);
        }

        #endregion
    }
}
