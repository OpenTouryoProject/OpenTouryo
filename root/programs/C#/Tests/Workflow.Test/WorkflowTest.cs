//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License

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
//* クラス名        ：WorkflowTest.cs
//* クラス日本語名  ：
//*
//* 作成者          ：Sai
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  10/22/2014   Sai               Testcode development for Workflow.
//**********************************************************************************
// 型情報

// System
using System;
using System.IO;
using System.Data;
using System.Collections.Generic;

// フレームワーク
// 業務フレームワーク
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Business.Workflow;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Util;

//// 型情報
// testing framework
using NUnit.Framework;
using System.Reflection;


namespace WorkflowTest
{
    /// <summary>
    /// Test code class for Workflow
    /// </summary>
    [TestFixture]
    public class WorkflowTest
    {
        private BaseDam _dam = null;

        /// <summary>
        /// Initial setup method
        /// </summary>
        [SetUp]
        public void Init()
        {
            // Damの初期化
            //this.InitDam();
        }

        /// <summary>
        /// Setup method for creating resource files required for testing Workflow methods
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // SQLは埋め込まれたリソースを使用する。
            MyBaseDao.UseEmbeddedResource = true;

            List<string> resourcePath = new List<string> { };
            Assembly excuteAssembly = Assembly.GetExecutingAssembly();

            foreach (string resource in excuteAssembly.GetManifestResourceNames())
            {
                string fileName = Path.GetFileName(resource);
                Stream stream = excuteAssembly.GetManifestResourceStream(resource);
                StreamReader streamReader = new StreamReader(stream);
                StreamWriter streamWriter = File.CreateText(fileName);
                resourcePath.Add(fileName);
                streamWriter.Write(streamReader.ReadToEnd());
                streamWriter.Flush();
                streamWriter.Close();
                streamReader.Close();
            }
        }

        /// <summary>
        /// TearDown Method for deleteing reource files used for testing
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            List<string> resourcesPath = new List<string> { };

            // Gets the resource files in Executing assembly
            Assembly executeAssembly = Assembly.GetExecutingAssembly();
            foreach (string resource in executeAssembly.GetManifestResourceNames())
            {
                string fileName = Path.GetFileName(resource);
                resourcesPath.Add(fileName);
            }

            // Deletes the files listed in Executing assembly
            foreach (string resourcePath in resourcesPath)
            {
                FileInfo file = new FileInfo(resourcePath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
        }

        /// <summary>Damの初期化</summary>
        private void InitDam()
        {
            // Initializes dam
            this._dam = new DamSqlSvr();
            this._dam.Obj = new MyUserInfo("WorkflowTest", "127.0.0.1");

            // Gets the connection string from config and open the db connection
            this._dam.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_SQL"));
            this._dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
        }

        /// <summary>ユーザ情報を取得する</summary>
        /// <param name="uid">ユーザID</param>
        /// <returns>ユーザ情報</returns>
        private string MyGetUserInfo(decimal uid)
        {
            string temp = "";

            DataTable dt = new DataTable();
            DaoM_User dao = new DaoM_User(this._dam);

            dao.PK_Id = uid;
            dao.D2_Select(dt);

            // ユーザ情報の生成
            foreach (object obj in dt.Rows[0].ItemArray)
            {
                if (temp == "")
                {
                    temp += obj.ToString();
                }
                else
                {
                    temp += "_" + obj.ToString();
                }
            }
            return temp;
        }

        /// <summary>
        /// Property for test cases. 
        /// </summary>
        public IEnumerable<TestCaseData> TestCaseTestWorkflow
        {
            get
            {
                yield return new TestCaseData("1", "2", "2", "1", new string[] { "Start", "Request", "Request" }, new string[] { "1", "2" },
                                              new string[] { "2", "3", "4" }, false, "Reserve area", DateTime.Now, true, false, false, false).SetName("TestID-001N");
                yield return new TestCaseData("1", "2", "3", "1", new string[] { "Request", "Request", "Request" }, new string[] { "2", "3" },
                                              new string[] { "3", "5" }, false, "Reserve area", DateTime.Now, true, false, false, false).SetName("TestID-002N");
                yield return new TestCaseData("1", "2", "5", "1", new string[] { "Request", "TurnBack", "Request" }, new string[] { "3", "5" },
                                              new string[] { "5", "3", "5", "6" }, true, "Reserve area", DateTime.Now, false, false, false, false).SetName("TestID-003N");
                yield return new TestCaseData("1", "2", "6", "1", new string[] { "Request", "Reply" }, new string[] { "5", "6" }, new string[] { "6", "5", "6", "5" },
                                              true, "Reserve area", DateTime.Now, true, false, false, false).SetName("TestID-004N");

                yield return new TestCaseData("1", "2", "2", "1", new string[] { "Start", "Request", "Request" }, new string[] { "1", "2" },
                                              new string[] { "2", "3", "4" }, false, "Reserve area", DateTime.Now, true, false, false, false).SetName("TestID-005N");
                yield return new TestCaseData("1", "2", "3", "1", new string[] { "Request", "Request", "Request" }, new string[] { "2", "3" },
                                              new string[] { "3", "5" }, false, "Reserve area", DateTime.Now, true, false, false, false).SetName("TestID-006N");
                yield return new TestCaseData("1", "2", "5", "1", new string[] { "Request", "TurnBack", "Request" }, new string[] { "3", "5" },
                                              new string[] { "5", "3", "5", "6" }, true, "Reserve area", DateTime.Now, true, false, false, false).SetName("TestID-007N");

                yield return new TestCaseData("1", "2", "2", "1", new string[] { "Start", "Request", "Request" }, new string[] { "1", "2" },
                                              new string[] { "2", "3", "4" }, false, "Reserve area", DateTime.Now, true, true, true, true).SetName("TestID-008N");
                yield return new TestCaseData("1", "2", "3", "1", new string[] { "Request", "Request", "Request" }, new string[] { "2", "3" },
                                              new string[] { "3", "5" }, false, "Reserve area", DateTime.Now, true, true, true, true).SetName("TestID-009N");
                yield return new TestCaseData("1", "2", "5", "1", new string[] { "Request", "TurnBack", "Request" }, new string[] { "3", "5" },
                                              new string[] { "5", "3", "5", "6" }, true, "Reserve area", DateTime.Now, true, true, true, true).SetName("TestID-010N");

                yield return new TestCaseData("1", string.Empty, "2", "1", new string[] { "Start", "Request", "Request" }, new string[] { "1", "2" },
                                              new string[] { "2", "3", "4" }, false, "Reserve area", DateTime.Now, true, false, false, false).SetName("TestID-011A");
                yield return new TestCaseData("1", "2", string.Empty, "1", new string[] { "Start", "Request", "Request" }, new string[] { "1", "2" },
                                              new string[] { "2", "3", "4" }, false, "Reserve area", DateTime.Now, true, false, false, false).SetName("TestID-012A");

            }
        }

        /// <summary>
        /// Test method for testing TestWorkflow
        /// </summary>
        /// <param name="dearSirUID"></param>
        /// <param name="fromUserID"></param>
        /// <param name="userID"></param>
        /// <param name="dearSirPTitleId"></param>
        /// <param name="actionType"></param>
        /// <param name="workflowNo"></param>
        /// <param name="expectedToUserID"></param>
        /// <param name="isReturn"></param>
        /// <param name="currentWorkflowReserveArea"></param>
        /// <param name="replyDeadlineDate"></param>
        /// <param name="isTurnBack"></param>
        /// <param name="isForcedTermination"></param>
        /// <param name="isTurnbackSlipIssuance"></param>
        /// <param name="isSwitchPersonInCharge"></param>
        [TestCaseSource("TestCaseTestWorkflow")]
        public void TestWorkflow(string dearSirUID, string fromUserID, string userID, string dearSirPTitleId, string[] actionType, string[] workflowNo,
                                 string[] expectedToUserID, Boolean isReturn, string currentWorkflowReserveArea, DateTime replyDeadlineDate,
                                 Boolean isTurnBack, Boolean isForcedTermination, Boolean isTurnbackSlipIssuance, Boolean isSwitchPersonInCharge)
        {
            DataTable dt = null;

            try
            {
                string subSystemId;
                string workflowName = string.Empty;
                DataRow startWorkflow = null;
                DataRow selectedRow = null;

                workflowName = GetConfigParameter.GetConfigValue("WorkflowName");
                subSystemId = GetConfigParameter.GetConfigValue("SubSystemId");

                // GetUserInfoDelegateの設定
                if (Workflow.GetUserInfo == null)
                {
                    Workflow.GetUserInfo = new GetUserInfoDelegate(MyGetUserInfo);
                }

                dt = new DataTable();
                DataTable dt1 = null;
                DataTable dt2 = null;

                InitDam();

                // Creating instance of Workflow class
                Workflow wf = new Workflow(this._dam);

                int mailTemplateId = 0;
                if (workflowNo[0] == "1")
                {

                    HelperClass.WorkflowControlNumber = Guid.NewGuid().ToString();

                    dt1 = wf.PreStartWorkflow(
                     subSystemId, workflowName, 1);

                    dt2 = wf.PreStartWorkflow(
                        subSystemId, workflowName, 2);

                    //  開始可能なワークフローを表示
                    if (dt1 != null)
                    {
                        dt.Merge(dt1);
                    }
                    if (dt2 != null)
                    {
                        dt.Merge(dt2);
                    }

                    selectedRow = dt.Rows[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (selectedRow != null)
                            if ((decimal)dr[0] == (decimal)selectedRow[0])
                            {
                                startWorkflow = dr;
                            }
                    }

                    mailTemplateId = wf.StartWorkflow(startWorkflow, HelperClass.WorkflowControlNumber, decimal.Parse(fromUserID), decimal.Parse(userID),
                                                      "ReserveArea", "CurrentWorkflowReserveArea", DateTime.Now);
                }

                dt = new DataTable();

                // 御中ID
                if (!string.IsNullOrEmpty(dearSirUID)
                    && !string.IsNullOrEmpty(dearSirPTitleId))
                {
                    // Calling GetWfRequest
                    dt1 = wf.GetWfRequest(subSystemId, workflowName, HelperClass.WorkflowControlNumber, decimal.Parse(dearSirUID), int.Parse(dearSirPTitleId));
                }
                // 個人ID
                if (!string.IsNullOrEmpty(userID))
                {
                    // Calling GetWfRequest
                    dt2 = wf.GetWfRequest(subSystemId, workflowName, HelperClass.WorkflowControlNumber, decimal.Parse(userID), null);
                }

                //  開始可能なワークフローを表示
                if (dt1 != null)
                {
                    dt.Merge(dt1);
                }
                if (dt2 != null)
                {
                    dt.Merge(dt2);
                }

                // Commit transactions
                this._dam.CommitTransaction();

                // Assert tests
                Assert.AreEqual(expectedToUserID[0], dt.Rows[0]["ToUserId"].ToString());
                Assert.AreEqual(fromUserID, dt.Rows[0]["FromUserId"].ToString());
                Assert.AreEqual(actionType[0], dt.Rows[0]["ActionType"].ToString());
                Assert.AreEqual(workflowNo[0], dt.Rows[0]["WorkflowNo"].ToString());
                Assert.AreEqual(subSystemId, dt.Rows[0]["SubSystemId"].ToString());
                Assert.AreEqual(workflowName, dt.Rows[0]["WorkflowName"].ToString());

                selectedRow = dt.Rows[0];
                DataRow workflowRequest = null;
                foreach (DataRow dr in dt.Rows)
                {
                    if ((string)dr[0] == (string)selectedRow[0])
                    {
                        workflowRequest = dr;
                    }
                }

                // Calling AcceptWfRequest                
                wf.AcceptWfRequest(workflowRequest, decimal.Parse(userID));

                // Commit transactions
                this._dam.CommitTransaction();

                // Assert tests
                Assert.AreEqual(expectedToUserID[0], dt.Rows[0]["ToUserId"].ToString());
                Assert.AreEqual(fromUserID, dt.Rows[0]["FromUserId"].ToString());
                Assert.AreEqual(actionType[0], dt.Rows[0]["ActionType"].ToString());
                Assert.AreEqual(workflowNo[0], dt.Rows[0]["WorkflowNo"].ToString());
                Assert.AreEqual(subSystemId, dt.Rows[0]["SubSystemId"].ToString());
                Assert.AreEqual(workflowName, dt.Rows[0]["WorkflowName"].ToString());

                // Calling GetProcessingWfRequest
                dt = wf.GetProcessingWfRequest(subSystemId, workflowName, HelperClass.WorkflowControlNumber, decimal.Parse(userID));

                // Commit transactions
                this._dam.CommitTransaction();

                // Assert tests
                Assert.AreEqual(expectedToUserID[0], dt.Rows[0]["ToUserId"].ToString());
                Assert.AreEqual(fromUserID, dt.Rows[0]["FromUserId"].ToString());
                Assert.AreEqual(actionType[0], dt.Rows[0]["ActionType"].ToString());
                Assert.AreEqual(workflowNo[0], dt.Rows[0]["WorkflowNo"].ToString());
                Assert.AreEqual(subSystemId, dt.Rows[0]["SubSystemId"].ToString());
                Assert.AreEqual(workflowName, dt.Rows[0]["WorkflowName"].ToString());

                selectedRow = dt.Rows[0];

                // 処理中ワークフロー依頼を取得
                DataRow processingWfReq = null;

                foreach (DataRow dr in dt.Rows)
                {
                    if ((string)dr[0] == (string)selectedRow[0])
                    {
                        processingWfReq = dr;
                    }
                }

                if (!string.IsNullOrEmpty(dearSirUID))
                {
                    dt1 = wf.GetNextWfRequest(processingWfReq, decimal.Parse(dearSirUID));
                }
                if (!string.IsNullOrEmpty(userID.ToString()))
                {
                    dt2 = wf.GetNextWfRequest(processingWfReq, decimal.Parse(userID));
                }

                //  次のワークフロー依頼を表示
                dt = new DataTable();

                if (dt1 != null)
                {
                    dt.Merge(dt1);
                }
                if (dt2 != null)
                {
                    dt.Merge(dt2);
                }

                // Commit transactions
                this._dam.CommitTransaction();

                // Assert tests
                Assert.AreEqual(expectedToUserID[1], dt.Rows[0]["ToUserId"].ToString());
                Assert.AreEqual(userID, dt.Rows[0]["FromUserId"].ToString());
                Assert.AreEqual(actionType[1], dt.Rows[0]["ActionType"].ToString());
                Assert.AreEqual(workflowNo[1], dt.Rows[0]["WorkflowNo"].ToString());
                Assert.AreEqual(subSystemId, dt.Rows[0]["SubSystemId"].ToString());
                Assert.AreEqual(workflowName, dt.Rows[0]["WorkflowName"].ToString());

                if (dt.Rows.Count > 3)
                {
                    // Assert tests
                    Assert.AreEqual(expectedToUserID[3], dt.Rows[1]["ToUserId"].ToString());
                    Assert.AreEqual(fromUserID, dt.Rows[1]["FromUserId"].ToString());
                    Assert.AreEqual(actionType[2], dt.Rows[1]["ActionType"].ToString());
                    Assert.AreEqual(workflowNo[1], dt.Rows[1]["WorkflowNo"].ToString());
                    Assert.AreEqual(subSystemId, dt.Rows[1]["SubSystemId"].ToString());
                    Assert.AreEqual(workflowName, dt.Rows[1]["WorkflowName"].ToString());
                }

                if (isTurnBack)
                {
                    selectedRow = dt.Rows[0];
                }
                else
                {
                    selectedRow = dt.Rows[1];
                }

                DataRow nextWorkflow = null;
                foreach (DataRow dr in dt.Rows)
                {
                    if ((decimal)dr[0] == (decimal)selectedRow[0])
                    {
                        nextWorkflow = dr;
                    }
                }

                // GetTurnBackToUser, StartWorkflow
                wf = new Workflow(this._dam);

                // fromUserIDを取得

                decimal? toUserId = null;

                if (isReturn
                    && (string)nextWorkflow["ActionType"] == "TurnBack")
                {
                    // TurnBack(送信元に差戻)
                    toUserId = wf.GetTurnBackToUser(nextWorkflow, HelperClass.WorkflowControlNumber);
                }
                else if (isReturn
                    && (string)nextWorkflow["ActionType"] == "Reply")
                {
                    // Reply(送信元に返信)
                    toUserId = wf.GetReplyToUser(nextWorkflow, HelperClass.WorkflowControlNumber);
                }
                else if ((string)nextWorkflow["ActionType"] == "End")
                {
                    // End
                    toUserId = null;
                }
                else
                {
                    // TurnBack, Reply, End以外
                    // nextWorkflow["ToUserId"]を使用する。
                }

                // Calling RequestWfApproval
                mailTemplateId = wf.RequestWfApproval(nextWorkflow, HelperClass.WorkflowControlNumber, decimal.Parse(fromUserID), toUserId, currentWorkflowReserveArea, replyDeadlineDate);

                // Calling TurnbackSlipIssuanceUserID based on flag
                if (isTurnbackSlipIssuance)
                {
                    wf.TurnbackSlipIssuanceUserID(subSystemId, HelperClass.WorkflowControlNumber, decimal.Parse(fromUserID), 1m, currentWorkflowReserveArea);
                }
                // Calling SwitchPersonInCharge based on flag
                if (isSwitchPersonInCharge)
                {
                    wf.SwitchPersonInCharge(nextWorkflow, HelperClass.WorkflowControlNumber);
                }
                //Calling ForcedTermination based on flag
                if (isForcedTermination)
                {
                    wf.ForcedTermination(nextWorkflow, HelperClass.WorkflowControlNumber, decimal.Parse(fromUserID), currentWorkflowReserveArea);
                }

                // Commit transactions
                this._dam.CommitTransaction();

                // Assert tests
                Assert.Greater(mailTemplateId, 0);
                Assert.AreEqual(workflowNo[1], dt.Rows[0]["WorkflowNo"].ToString());
                Assert.AreEqual(subSystemId, dt.Rows[0]["SubSystemId"].ToString());
                Assert.AreEqual(workflowName, dt.Rows[0]["WorkflowName"].ToString());
            }
            catch (Exception ex)
            {
                if (dt != null)
                {
                    // Rollback transactions
                    this._dam.RollbackTransaction();
                }
                Console.WriteLine(ex.ToString());
            }
        }
    }
}