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
//* クラス名        ：Deflate
//* クラス日本語名  ：Deflate圧縮クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/20  西野 大介         新規作成
//**********************************************************************************

using System;
using System.IO;
using System.IO.Compression;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>
    /// Deflate圧縮クラス
    /// https://docs.microsoft.com/ja-jp/dotnet/api/system.io.compression.deflatestream
    /// </summary>
    public static class Deflate
    {
        /// <summary>Deflate圧縮</summary>
        /// <param name="input">入力 byte[]</param>
        /// <returns>圧縮 byte[]</returns>
        public static byte[] Compress(byte[] input)
        {
            return Deflate.Common(input, CompressionMode.Compress);
        }

        /// <summary>Deflate解凍</summary>
        /// <param name="input">入力 byte[]</param>
        /// <returns>解凍 byte[]</returns>
        public static byte[] Decompress(byte[] input)
        {
            return Deflate.Common(input, CompressionMode.Decompress);
        }

        /// <summary>Deflate解凍</summary>
        /// <param name="input">入力 byte[]</param>
        /// <param name="compressionMode">CompressionMode</param>
        /// <returns>解凍 byte[]</returns>
        private static byte[] Common(byte[] input, CompressionMode compressionMode)
        {
            MemoryStream memInput = new MemoryStream(input);
            DeflateStream compressionStream = new DeflateStream(memInput, compressionMode);
            MemoryStream memOutput = new MemoryStream();
            compressionStream.CopyTo(memOutput);
            return memOutput.ToArray();
        }
    }
}
