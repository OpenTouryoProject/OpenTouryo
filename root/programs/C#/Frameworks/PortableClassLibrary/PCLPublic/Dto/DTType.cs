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
//* クラス名        ：DTType
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（型情報-列挙型）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/03/xx  西野  大介        新規作成
//**********************************************************************************

using System;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>型情報-列挙型</summary>
    /// <remarks>
    /// 以下のデータ型をサポートする。
    /// 
    /// DataColumn      .NET型          Java型          WSDL型
    /// Boolean      ⇔ Boolean      ⇔ boolean      ⇔ boolean
    /// Byte         ⇔ byte配列     ⇔ byte配列     ⇔ base64Binary
    /// Char         ⇔ Char         ⇔ char         ⇔ char
    /// DateTime     ⇔ DateTime     ⇔ Date         ⇔ dateTime
    /// Decimal      ⇔ Decimal      ⇔ BigDecimal   ⇔ decimal
    /// Double       ⇔ Double       ⇔ double       ⇔ double
    /// Int16        ⇔ Int16        ⇔ short        ⇔ short
    /// Int32        ⇔ Int32        ⇔ int          ⇔ int
    /// Int64        ⇔ Int64        ⇔ long         ⇔ long
    /// SByte        ⇔ （サポートしない）
    /// Single       ⇔ Single       ⇔ float        ⇔ float
    /// String       ⇔ String       ⇔ String       ⇔ string
    /// TimeSpan     ⇔ （サポートしない）
    /// UInt16       ⇔ UInt16       ⇔ （サポートしない）
    /// UInt32       ⇔ UInt32       ⇔ （サポートしない）
    /// UInt64       ⇔ UInt64       ⇔ （サポートしない）
    /// 
    /// ※ 通信時は全てText（String）だが、byte配列は
    ///    Base64エンコーディングを使用するので注意。
    /// </remarks>
    public enum DTType
    {
        /// <summary>Boolean型</summary>
        Boolean = 1,
        /// <summary>バイト配列</summary>
        ByteArray,
        /// <summary>Char型</summary>
        Char,
        /// <summary>DateTime型</summary>
        DateTime,
        /// <summary>Decimal型</summary>
        Decimal,
        /// <summary>Double型</summary>
        Double,
        /// <summary>Int16（Short）型</summary>
        Int16,
        /// <summary>Int32（Integer）型</summary>
        Int32,
        /// <summary>Int64（Long）型</summary>
        Int64,
        /// <summary>Single（Float）型</summary>
        Single,
        /// <summary>String型</summary>
        String
    }
}
