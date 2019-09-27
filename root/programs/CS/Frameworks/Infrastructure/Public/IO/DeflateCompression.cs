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
//* クラス名        ：DeflateCompression
//* クラス日本語名  ：DeflateCompressionクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/20  西野 大介         新規作成
//**********************************************************************************

using System.IO;
using System.IO.Compression;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>
    /// DeflateCompressionクラス
    ///   GZipStreamの後継のDeflateStreamを使用する。
    ///   DeflateStreamは、zlib ライブラリを使用して圧縮
    ///   https://docs.microsoft.com/ja-jp/dotnet/api/system.io.compression.deflatestream
    ///   https://www.atmarkit.co.jp/fdotnet/dotnettips/485gzipstream/gzipstream.html
    /// </summary>
    public static class DeflateCompression
    {
        #region FileStream
        /// <summary>Deflate圧縮</summary>
        /// <param name="inFilePath">入力ファイルパス</param>
        /// <param name="ext">圧縮ファイル拡張子</param>
        /// <param name="outFilePath">圧縮ファイルパス</param>
        /// <returns>圧縮ファイルパス</returns>
        public static string Compress(string inFilePath, string ext = "cmp", string outFilePath = "")
        {
            // 圧縮ファイルパス
            if (string.IsNullOrEmpty(outFilePath))
            {
                // inFilePathと同じ位置・ファイル名に、指定の拡張子で
                outFilePath = 
                    Path.GetDirectoryName(inFilePath) + "\\" +
                    Path.GetFileNameWithoutExtension(inFilePath) + "." + ext;
            }
            else
            {
                // outFilePathを使用（拡張子が無い場合は足す。
                if (string.IsNullOrEmpty(Path.GetExtension(outFilePath)))
                {
                    outFilePath += "." + ext;
                }
            }

            byte[] buffer = new byte[1024]; // 1K bytesずつ処理

            using (FileStream fsSrc = new FileStream(inFilePath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsDst = new FileStream(outFilePath, FileMode.Create))
                {
                    using (DeflateStream deflateStream = new DeflateStream(fsDst, CompressionMode.Compress))
                    {
                        //ds.Write(input, 0, input.Length); // 書き方を合わせた。

                        int bytesRead;

                        // msSrc.Read -> deflateStream.Writeで圧縮なのだと。
                        while ((bytesRead = fsSrc.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            deflateStream.Write(buffer, 0, bytesRead);
                        }

                    } // ただし、StreamがCloseされた際に、実際の圧縮を行うらしい。
                }
            }

            return outFilePath;
        }

        /// <summary>Deflate解凍</summary>
        /// <param name="inFilePath">入力ファイルパス</param>
        /// <param name="ext">解凍ファイル拡張子</param>
        /// <param name="outFilePath">解凍ファイルパス</param>
        /// <returns>解凍ファイルパス</returns>
        public static string Decompress(string inFilePath, string ext = "dcmp", string outFilePath = "")
        {
            // 解凍ファイルパス
            if (string.IsNullOrEmpty(outFilePath))
            {
                // inFilePathと同じ位置・ファイル名に、指定の拡張子で
                outFilePath =
                    Path.GetDirectoryName(inFilePath) + "\\" +
                    Path.GetFileNameWithoutExtension(inFilePath) + "." + ext;
            }
            else
            {
                // outFilePathを使用（拡張子が無い場合は足す。
                if (string.IsNullOrEmpty(Path.GetExtension(outFilePath)))
                {
                    outFilePath += "." + ext;
                }
            }

            byte[] buffer = new byte[1024]; // 1K bytesずつ処理

            // Compressと実装が対にならないのが気持ち悪いが、
            using (FileStream fsSrc = new FileStream(inFilePath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsDst = new FileStream(outFilePath, FileMode.Create))
                {
                    using (DeflateStream deflateStream = new DeflateStream(fsSrc, CompressionMode.Decompress))
                    {
                        int bytesRead;

                        // deflateStream.Read -> msDst.Writeで解凍なのだと。
                        while ((bytesRead = deflateStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsDst.Write(buffer, 0, bytesRead);
                        }
                    } // 圧縮と違い、Closeで解凍と言う訳では無さそう（恐らく、msDst.Writeで解凍）。
                }
            }

            return outFilePath;
        }
        #endregion

        #region MemoryStream
        /// <summary>Deflate圧縮</summary>
        /// <param name="input">入力 byte[]</param>
        /// <returns>圧縮 byte[]</returns>
        public static byte[] Compress(byte[] input)
        {
            byte[] buffer = new byte[1024]; // 1K bytesずつ処理

            using (MemoryStream msSrc = new MemoryStream(input))
            {
                using (MemoryStream msDst = new MemoryStream())
                {
                    // Compress時は、第3引数をtrueを指定してmsDstがCloseされないようにする。
                    using (DeflateStream deflateStream = new DeflateStream(msDst, CompressionMode.Compress, true))
                    {
                        //ds.Write(input, 0, input.Length); // 書き方を合わせた。

                        int bytesRead;

                        // msSrc.Read -> deflateStream.Writeで圧縮なのだと。
                        while ((bytesRead = msSrc.Read(buffer, 0, buffer.Length)) > 0)
                        {   
                            deflateStream.Write(buffer, 0, bytesRead);
                        }

                    } // ただし、StreamがCloseされた際に、実際の圧縮を行うらしい。

                    msDst.Position = 0;
                    return msDst.ToArray();
                }
            }
        }

        /// <summary>Deflate解凍</summary>
        /// <param name="input">入力 byte[]</param>
        /// <returns>解凍 byte[]</returns>
        public static byte[] Decompress(byte[] input)
        {
            byte[] buffer = new byte[1024]; // 1K bytesずつ処理

            // Compressと実装が対にならないのが気持ち悪いが、
            using (MemoryStream msSrc = new MemoryStream(input))
            {
                using (MemoryStream msDst = new MemoryStream())
                {
                    // Decompress時では、msDstはCloseされないので第3引数はfalseで良い。
                    using (DeflateStream deflateStream = new DeflateStream(msSrc, CompressionMode.Decompress)) //, true))
                    {
                        int bytesRead;

                        // deflateStream.Read -> msDst.Writeで解凍なのだと。
                        while ((bytesRead = deflateStream.Read(buffer, 0, buffer.Length)) > 0)
                        {   
                            msDst.Write(buffer, 0, bytesRead);
                        }
                    } // 圧縮と違い、Closeで解凍と言う訳では無さそう（恐らく、msDst.Writeで解凍）。

                    msDst.Position = 0;
                    return msDst.ToArray();
                }
            }
        }
        #endregion
    }
}
