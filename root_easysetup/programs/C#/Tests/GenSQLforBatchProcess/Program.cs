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
//* クラス名        ：Test code for Internationalization
//* クラス日本語名  ：国際化対応のテスト・コード
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/01/20  西野 大介         新規作成
//*  2014/01/20  Sai Krishna       added code for batch processing supporting PostGreSQL  
//*  2014/01/20  Santoshkumar      added code for batch processing supporting Oracle  
//*  2014/01/24  Sai Krishna       added code for batch processing supporting PostGreSQL
//*  2014/01/24  Santoshkumar      added code for batch processing supporting Oracle
//*  2014/01/30  Sai Krishna       added code for batch processing supporting MySQL
//*  2014/01/30  Santoshkumar      added code for batch processing supporting DB2
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using System.Data;
using System.Data.Common;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Util;

//using System.Data.SqlClient;
//using Oracle.DataAccess.Client;
//using IBM.Data.DB2;
//using MySql.Data.MySqlClient;
//using Npgsql;

namespace ConsoleApplication1
{
    /// <summary>
    /// Program
    /// </summary>
    class Program
    {
        /// <summary>DBMSの種類</summary>
        private static DbEnum.DBMSType _dbms;
        
        /// <summary>Main</summary>
        static void Main(string[] args)
        {
            //// Database connection using IDbCommand
            //IDbConnection dbConnection = null;
            //IDbCommand dbCommand = null;

            Dictionary<string, string> dic = null;
            List<string> list = null;

            PubCmnFunction.GetCommandArgs('/', out dic, out list);

            // Get the type of DB
            if (dic.ContainsKey("/DBMS"))
            {
                switch (dic["/DBMS"])
                {
                    case "ODP":
                        Program._dbms = DbEnum.DBMSType.Oracle;
                        //dbConnection = new OracleConnection(GetConfigParameter.GetConfigValue("Oracle"));
                        break;
                    case "DB2":
                        Program._dbms = DbEnum.DBMSType.DB2;
                        //dbConnection = new DB2Connection(GetConfigParameter.GetConfigValue("DB2Conn"));
                        break;
                    case "MCN":
                        Program._dbms = DbEnum.DBMSType.MySQL;
                        //dbConnection = new MySqlConnection(GetConfigParameter.GetConfigValue("MySQLConn"));
                        break;
                    case "NPS":
                        Program._dbms = DbEnum.DBMSType.PstGrS;
                        //dbConnection = new NpgsqlConnection(GetConfigParameter.GetConfigValue("PostgreSQLConn"));
                        break;
                    default:
                        Program._dbms = DbEnum.DBMSType.SQLServer;
                        //dbConnection = new SqlConnection(GetConfigParameter.GetConfigValue("SQLServerConn"));
                        break;
                }
            }

            DataTable dt = null;
            DataRow dr = null;

            try
            {
                #region Datatable creation

                dt = new DataTable("table1");

                dt.Columns.Add(new DataColumn("column1", typeof(string)));
                dt.Columns.Add(new DataColumn("column2", typeof(Int32)));
                dt.Columns.Add(new DataColumn("column3", typeof(byte[])));
                dt.Columns.Add(new DataColumn("column4", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("column5", typeof(char)));
                dt.Columns.Add(new DataColumn("column6", typeof(string)));

                dr = dt.NewRow();
                dr["column1"] = "test1";
                dr["column2"] = 1111;
                dr["column3"] = new byte[] { 1, 2, 3, 4, 5, 6 };
                dr["column4"] = DateTime.Now;
                dr["column5"] = DBNull.Value;
                dr["column6"] = string.Empty;
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr["column1"] = "test2";
                dr["column2"] = 22222;
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr["column1"] = "test3";
                dr["column2"] = 3;
                dr["column3"] = DBNull.Value;
                dr["column4"] = DBNull.Value;                
                dr["column5"] = 'D';
                dr["column6"] = "Test's";
                dt.Rows.Add(dr);

                #endregion

                #region Generate SQL for batch process

                string[] strs = null;
                StringBuilder sb = null;
                string collist = "";
                string temp = "";
                string query = "";

                // Second and third argument optional.
                SQLUtility su = new SQLUtility(Program._dbms);

                #region Assembly of parts of SQL insert.

                // Forming insert query.
                strs = su.GetInsertSQLParts(dt);

                Console.WriteLine("<InsertSQLParts>");
                foreach (string str in strs)
                {
                    Console.WriteLine(str);
                }
                Console.WriteLine("\r\n");

                sb = new StringBuilder();

                // INSERT
                switch (Program._dbms)
                {
                    case DbEnum.DBMSType.Oracle:
                        foreach (string str in strs)
                        {
                            if (string.IsNullOrEmpty(collist))
                            {
                                collist = str;
                                // for oracle: remove double quotes from the "column" name
                                collist = collist.Replace("\"", string.Empty);
                            }
                            else
                            {
                                sb.Append(str + ";");
                            }
                        }

                        // 最後のカンマを削る。
                        temp = sb.ToString();
                        temp = temp.Substring(0, temp.Length - 1);

                        string[] temp2 = temp.Split(';');
                        StringBuilder sbOracle = new StringBuilder();
                        // Prepare Oracle Insert Statement to execute
                        sbOracle.Append("INSERT ALL ");
                        foreach (string str in temp2)
                        {
                            sbOracle.Append(string.Format(
                                " INTO "
                                + su.OpeningBracket + dt.TableName + su.ClosingBracket + " {0} "
                                + " VALUES{1} \r\n", collist, str));
                        }

                        sbOracle.Append(" SELECT * FROM DUAL");
                        query = sbOracle.ToString();
                        break;

                    case DbEnum.DBMSType.DB2:
                        //・・・
                        foreach (string str in strs)
                        {
                            if (string.IsNullOrEmpty(collist))
                            {
                                collist = str;
                                collist = collist.Replace("\"", string.Empty);
                            }
                            else
                            {
                                sb.Append(str + ",\r\n");
                            }
                        }

                        // Sharpen the last comma.
                        temp = sb.ToString();
                        temp = temp.Substring(0, temp.Length - 3);
                        query =
                            @"INSERT INTO "
                            + su.OpeningBracket + dt.TableName + su.ClosingBracket + collist
                            + "VALUES \r\n" + temp;
                        break;

                    default:
                        // PostGreSQL
                        // MySQL
                        // SQLServer
                        // Preparing Insert query insert values.
                        foreach (string str in strs)
                        {
                            if (string.IsNullOrEmpty(collist))
                            {
                                collist = str;
                            }
                            else
                            {
                                sb.Append(str + ",\r\n");
                            }
                        }
                        // Sharpen the last comma.
                        temp = sb.ToString();
                        temp = temp.Substring(0, temp.Length - 3);

                        query =
                            @"INSERT INTO "
                            + su.OpeningBracket + dt.TableName + su.ClosingBracket + collist
                            + "VALUES \r\n" + temp;

                        break;
                }

                Console.WriteLine("<INSERT>");
                Console.WriteLine(query);
                Console.WriteLine("\r\n");

                #endregion

                #region Assembly of parts of SQL update.

                // Forming update query
                strs = su.GetUpdateSQLParts(dt, new string[] { "column1", "column2" });

                Console.WriteLine("<UpdateSQLParts>");
                foreach (string str in strs)
                {
                    Console.WriteLine(str);
                }
                Console.WriteLine("\r\n");

                sb = new StringBuilder();

                // UPDATE
                switch (Program._dbms)
                {
                    case DbEnum.DBMSType.Oracle:
                        // Oracle
                        // To Run multiple sql statements in oracle from application we need to prepend "BEGIN" to the  Query 
                        sb.Append("BEGIN ");
                        foreach (string s in strs)
                        {
                            string ss = s.Replace("\"", string.Empty);
                            //Add ";" at the end of each update statement
                            sb.Append(@"UPDATE " + su.OpeningBracket + dt.TableName + su.ClosingBracket + " " + ss + ";\r\n");
                        }
                        // To Run multiple sql statements in oracle from application we need to append "END;" to the Query
                        sb.Append("END;");
                        query = sb.ToString();
                        break;

                    case DbEnum.DBMSType.DB2:
                        // To Run multiple sql statements in DB2 from an application we need to prepend "BEGIN" to the  Query
                        sb.Append("BEGIN ");
                        foreach (string s in strs)
                        {
                            //Add ";" at the end of each update statement to execute multiple update statements in DB2 datbase from .net
                            sb.Append(@"UPDATE " + su.OpeningBracket + dt.TableName + su.ClosingBracket + " " + s + ";\r\n");
                        }
                        // To Run multiple sql statements in DB2 from .net we need to append "END;" to the Query
                        sb.Append("END;");
                        query = sb.ToString();
                        query = query.Replace("\"", string.Empty);
                        break;

                    default:
                        // PostGreSQL
                        // MySQL
                        // SQLServer
                        foreach (string s in strs)
                        {
                            sb.Append(@"UPDATE " + su.OpeningBracket + dt.TableName + su.ClosingBracket + " " + s + "\r\n");
                        }

                        query = sb.ToString();
                        break;
                }

                Console.WriteLine("<UPDATE>");
                Console.WriteLine(query);
                Console.WriteLine("\r\n");

                #endregion

                #endregion

                ////Executing queries using ADO.NET Icommand connection and command.
                //dbConnection.Open();
                //dbCommand = dbConnection.CreateCommand();
                //dbCommand.CommandText = query;
                //dbCommand.ExecuteNonQuery();

                Console.WriteLine("Query executed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
