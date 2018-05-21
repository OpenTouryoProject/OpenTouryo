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
//* クラス名        ：DummyDaps
//* クラス日本語名  ：ダミーのデータアクセスプロバイダ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野 大介         新規作成
//*  2014/02/05  西野 大介         System.Data.OracleClientデータプロバイダ対応
//**********************************************************************************

using System;
using System.Data;

//////////////////////////////////////////////////
// Touryo.Infrastructure.Public.Db
//////////////////////////////////////////////////
namespace Touryo.Infrastructure.Public.Db
{
    /// <summary>
    /// DamOraClientのダミー
    /// </summary>
    public class DamOraClient { }

    ///// <summary>
    ///// DamManagedOdpのダミー
    ///// </summary>
    //public class DamManagedOdp
    //{
    //    public int ArrayBindCount;
    //}

    /// <summary>
    /// DamDB2のダミー
    /// </summary>
    public class DamDB2 { }

    /// <summary>
    /// DamHiRDBのダミー
    /// </summary>
    public class DamHiRDB { }

    ///// <summary>
    ///// DamMySQLのダミー
    ///// </summary>
    //public class DamMySQL { }

    ///// <summary>
    ///// DamPstGrSのダミー
    ///// </summary>
    //public class DamPstGrS { }
}

//////////////////////////////////////////////////
// System.Data.OracleClient
//////////////////////////////////////////////////

namespace System.Data.OracleClient
{
    /// <summary>
    /// OracleConnectionのダミー
    /// </summary>
    public class OracleConnection
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="s">接続文字列</param>
        public OracleConnection(string s)
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>Openメソッド</summary>
        public void Open()
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>Closeメソッド</summary>
        public void Close()
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>GetSchemaメソッド</summary>
        public DataTable GetSchema(object o)
        {
            throw new NotImplementedException("this is dummy.");
            //return null;
        }
    }

    /// <summary>
    /// OracleConnectionStringBuilderのダミー
    /// </summary>
    public class OracleConnectionStringBuilder
    {
        public string DataSource;
        public string UserID;
        public string Password;
        public string ConnectionString;
    }

    /// <summary>
    /// OracleTypeのダミー
    /// </summary>
    public enum OracleType
    {
        BFile,
        Blob,
        Byte,
        Char,
        Clob,
        Cursor,
        DateTime,
        Double,
        Float,
        Int16,
        Int32,
        IntervalDayToSecond,
        IntervalYearToMonth,
        LongRaw,
        LongVarChar,
        NChar,
        NClob,
        Number,
        NVarChar,
        Raw,
        RowId,
        SByte,
        Timestamp,
        TimestampLocal,
        TimestampWithTZ,
        UInt16,
        UInt32,
        VarChar
    }
}

//////////////////////////////////////////////////
// Oracle.DataAccess.Client
//////////////////////////////////////////////////

//namespace Oracle.DataAccess.Client
//{
//    /// <summary>
//    /// OracleConnectionのダミー
//    /// </summary>
//    public class OracleConnection
//    {
//        /// <summary>コンストラクタ</summary>
//        /// <param name="s">接続文字列</param>
//        public OracleConnection(string s)
//        {
//            throw new NotImplementedException("this is dummy.");
//        }

//        /// <summary>Openメソッド</summary>
//        public void Open()
//        {
//            throw new NotImplementedException("this is dummy.");
//        }

//        /// <summary>Closeメソッド</summary>
//        public void Close()
//        {
//            throw new NotImplementedException("this is dummy.");
//        }

//        /// <summary>GetSchemaメソッド</summary>
//        public DataTable GetSchema(object o)
//        {
//            throw new NotImplementedException("this is dummy.");
//            //return null;
//        }
//    }

//    /// <summary>
//    /// OracleConnectionStringBuilderのダミー
//    /// </summary>
//    public class OracleConnectionStringBuilder
//    {
//        public string DataSource;
//        public string UserID;
//        public string Password;
//        public string ConnectionString;
//    }

//    /// <summary>
//    /// OracleDbTypeのダミー
//    /// </summary>
//    public enum OracleDbType
//    {
//        BFile,
//        BinaryFloat,
//        BinaryDouble,
//        Blob,
//        Byte,
//        Char,
//        Clob,
//        Date,
//        Decimal,
//        Double,
//        Int16,
//        Int32,
//        Int64,
//        IntervalDS,
//        IntervalYM,
//        Long,
//        LongRaw,
//        NChar,
//        NClob,
//        NVarchar2,
//        Raw,
//        RefCursor,
//        Single,
//        TimeStamp,
//        TimeStampLTZ,
//        TimeStampTZ,
//        Varchar2,
//        XmlType
//    }
//}

//////////////////////////////////////////////////
// IBM.Data.DB2
//////////////////////////////////////////////////

namespace IBM.Data.DB2
{
    /// <summary>
    /// DB2Connectionのダミー
    /// </summary>
    public class DB2Connection
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="s">接続文字列</param>
        public DB2Connection(string s)
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>Openメソッド</summary>
        public void Open()
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>Closeメソッド</summary>
        public void Close()
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>GetSchemaメソッド</summary>
        public DataTable GetSchema(object o)
        {
            throw new NotImplementedException("this is dummy.");
            //return null;
        }
    }

    /// <summary>
    /// DB2ConnectionStringBuilderのダミー
    /// </summary>
    public class DB2ConnectionStringBuilder
    {
        public string Database;
        public string UserID;
        public string Password;
        public string ConnectionString;
    }

    /// <summary>
    /// DB2Typeのダミー
    /// </summary>
    public enum DB2Type
    {
        BigInt,
        BigSerial,
        Binary,
        Blob,
        Byte,
        Char,
        Clob,
        Date,
        DateTime,
        DbClob,
        Decimal,
        DecimalFloat,
        Double,
        Float,
        Graphic,
        Integer,
        Int8,
        LongVarBinary,
        LongVarGraphic,
        Money,
        Numeric,
        Real,
        Real370,
        RowId,
        Serial,
        Serial8,
        SmallInt,
        Text,
        Time,
        Timestamp,
        VarBinary,
        VarChar,
        VarGraphic,
        Xml
    }
}

//////////////////////////////////////////////////
// Hitachi.HiRDB
//////////////////////////////////////////////////

namespace Hitachi.HiRDB
{
    /// <summary>
    /// HiRDBConnectionのダミー
    /// </summary>
    public class HiRDBConnection
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="s">接続文字列</param>
        public HiRDBConnection(string s)
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>Openメソッド</summary>
        public void Open()
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>Closeメソッド</summary>
        public void Close()
        {
            throw new NotImplementedException("this is dummy.");
        }

        /// <summary>GetSchemaメソッド</summary>
        public DataTable GetSchema(object o)
        {
            throw new NotImplementedException("this is dummy.");
            //return null;
        }
    }
}

//////////////////////////////////////////////////
// MySql.Data.MySqlClient
//////////////////////////////////////////////////

//namespace MySql.Data.MySqlClient
//{
//    /// <summary>
//    /// MySqlConnectionのダミー
//    /// </summary>
//    public class MySqlConnection
//    {
//        /// <summary>コンストラクタ</summary>
//        /// <param name="s">接続文字列</param>
//        public MySqlConnection(string s)
//        {
//            throw new NotImplementedException("this is dummy.");
//        }

//        /// <summary>Openメソッド</summary>
//        public void Open()
//        {
//            throw new NotImplementedException("this is dummy.");
//        }

//        /// <summary>Closeメソッド</summary>
//        public void Close()
//        {
//            throw new NotImplementedException("this is dummy.");
//        }

//        /// <summary>GetSchemaメソッド</summary>
//        public DataTable GetSchema(object o)
//        {
//            throw new NotImplementedException("this is dummy."); 
//            //return null;
//        }
//    }

//    /// <summary>
//    /// MySqlConnectionStringBuilderのダミー
//    /// </summary>
//    public class MySqlConnectionStringBuilder
//    {
//        public string Server;
//        public string Database;
//        public string UserID;
//        public string Password;
//        public string ConnectionString;
//    }
//}

//////////////////////////////////////////////////
// Npgsql
//////////////////////////////////////////////////

//namespace Npgsql
//{
//    /// <summary>
//    /// NpgsqlConnectionのダミー
//    /// </summary>
//    public class NpgsqlConnection
//    {
//        /// <summary>コンストラクタ</summary>
//        /// <param name="s">接続文字列</param>
//        public NpgsqlConnection(string s)
//        {
//            throw new NotImplementedException("this is dummy."); 
//        }

//        /// <summary>Openメソッド</summary>
//        public void Open()
//        {
//            throw new NotImplementedException("this is dummy."); 
//        }

//        /// <summary>Closeメソッド</summary>
//        public void Close()
//        {
//            throw new NotImplementedException("this is dummy."); 
//        }

//        /// <summary>GetSchemaメソッド</summary>
//        public DataTable GetSchema(object o)
//        {
//            throw new NotImplementedException("this is dummy."); 
//            //return null;
//        }
//    }
//}

//namespace NpgsqlTypes
//{
//    /// <summary>
//    /// xxxのダミー
//    /// </summary>
//    public class xxx
//    {
//    }
//}
