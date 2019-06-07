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
//* クラス名        ：ToStringExtension
//* クラス日本語名  ：ToStringExtensionクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/20  西野 大介         新規作成
//**********************************************************************************

using System.Xml;
using System.IO;
using Touryo.Infrastructure.Public.IO;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>ToStringExtension</summary>
    public static class ToStringExtension
    {
        #region XML

        // EZ-NET: XmlDocument を XML 宣言付きの整形された String に変換する | Visual C# プログラミング
        // http://program.station.ez-net.jp/special/handbook/csharp/xml/to-response.asp

        /// <summary>XmlToString</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="xmlWriterSettings">XmlWriterSettings</param>
        /// <returns>xmlString</returns>
        public static string XmlToString(this XmlDocument xmlDoc, XmlWriterSettings xmlWriterSettings)
        {
            // MemoryStreamに書く
            MemoryStream memoryStream = new MemoryStream();

            XmlWriter xmlWriter =
                XmlWriter.Create(memoryStream, xmlWriterSettings);

            xmlDoc.Save(xmlWriter);

            // MemoryStreamから読む
            memoryStream.Position = 0;
            StreamReader reader = new StreamReader(memoryStream);
            return reader.ReadToEnd();
        }
        #endregion
    }
}
