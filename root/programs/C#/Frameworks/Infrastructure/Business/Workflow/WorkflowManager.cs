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
//* クラス名        ：WorkflowManager
//* クラス日本語名  ：ワークフロー管理クラス
//*
//* 作成者          ：西野  大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/06/20  西野  大介        新規作成
//**********************************************************************************

using System.Globalization;
using System.ComponentModel;

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Touryo.Infrastructure.Business.Util;

namespace Touryo.Infrastructure.Business.Workflow
{
    /// <summary>WorkflowManagerクラス</summary>
    public class WorkflowManager
    {
        /// <summary>ユーザ情報</summary>
        private MyUserInfo _userInfo;

        /// <summary>最新のワークフロー履歴のインスタンス</summary>
        private WorkflowHistory _workflowHistory;

        /// <summary>コンストラクタ</summary>
        public WorkflowManager(MyUserInfo userInfo)
        {
            this._userInfo = userInfo;
        }

        /// <summary>対象ワークフローの遷移先ユーザのリストの取得</summary>
        /// <param name="subsystemId">サブシステムID</param>
        /// <param name="workflowName">ワークフロー名</param>
        /// <param name="actionName">処理名称</param>
        /// <param name="fromCompanyId">遷移元会社ID</param>
        /// <param name="fromSectionId">遷移元組織ID</param>
        /// <param name="fromUserId">遷移元作業者ID</param>
        /// <param name="workflowPathNo">ワークフロー・パス識別番号</param>
        /// <returns>対象ワークフローの遷移先ユーザのリスト</returns>
        public List<string> GetAddressList(
            string subSystemId,
            string workflowName,
            string actionName,
            string fromCompanyId,
            string fromSectionId,
            string fromUserId,
            string workflowPathNo)
        {
            // リターン値
            string workflowAddress = null;
            List<string> resultList = new List<string>();

            // 必須項目のチェック?
            if(
                string.IsNullOrEmpty(subSystemId) | string.IsNullOrEmpty(workflowName) | string.IsNullOrEmpty(actionName)
                | string.IsNullOrEmpty(fromCompanyId) | string.IsNullOrEmpty(fromSectionId) | string.IsNullOrEmpty(fromUserId)
                | string.IsNullOrEmpty(workflowPathNo))
            {
                // 必須項目のチェック
                throw new ArgumentException("必須入力チェックエラー");
            }

            // 条件検索して、WFマスタから遷移先ユーザを取得する。
            DataTable dt =new DataTable();

            // WFマスタとTOのUSERマスタをJOIN
            string sql = "select * from ";
            if(dt.Rows.Count ==0)
            {
                throw new ArgumentException("ワークフローが存在しません。");
            }
            else
            {
                foreach(DataRow dr in dt.Rows)
                {
                    workflowAddress = dr[""].ToString();
                    resultList.Add(workflowAddress );
                }
            }

            return resultList;
        }

        
        /// <summary>
        /// ワークフローの更新を実行します。
        /// メールを自動送信します。
        /// 更新結果判定フラグを返します。
        /// </summary>
        /// <param name="historyNo">履歴番号</param>
        /// <param name="subSystemId">サブシステムID</param>
        /// <param name="workflowName">ワークフロー名</param>
        /// <param name="actionName">処理名称</param>
        /// <param name="workflowPathNo">ワークフロー・パス識別番号</param>
        /// <param name="surrogatekeyBusinessdata">業務データ・サロゲートキー</param>
        /// <param name="toStatus">遷移先でのステータス</param>
        /// <param name="replyDeadline">返信〆切日</param>
        /// <param name="fromCompanyId">遷移元会社ID</param>
        /// <param name="fromSectionId">遷移元組織ID</param>
        /// <param name="fromUserId">遷移元作業者ID</param>
        /// <param name="toCompanyId">遷移先会社ID</param>
        /// <param name="toSectionId">遷移先組織ID</param>
        /// <param name="toUserId">遷移先作業者ID</param>
        /// <param name="optionalText1">業務用予備項目(文字列)１</param>
        /// <param name="optionalText2">業務用予備項目(文字列)２</param>
        /// <param name="optionalText3">業務用予備項目(文字列)３</param>
        /// <param name="optionalDate">業務用予備項目(日付)１</param>
        /// <param name="mailLanguage">自動メール送信時の言語です。</param>
        /// <param name="enforceNotSendFlag">
        /// 自動メール送信強制停止フラグです。
        /// TRUE:強制停止、FALSE:自動送信
        /// </param>
        /// <returns>更新結果判定フラグ</returns>
        public bool Execute(
            // id, no
            int historyNo, 
            string subSystemId,
            string workflowName,
            string actionName,
            string workflowPathNo,
            string surrogatekeyBusinessdata,
            // status
            string toStatus,
            string replyDeadline,
            // to-from
            string fromCompanyId,
            string fromSectionId,
            string fromUserId,
            string toCompanyId,
            string toSectionId,
            string toUserId,
            // optional
            string optionalText1,
            string optionalText2,
            string optionalText3,
            string optionalDate,
            //
            string mailLanguage,
            bool enforceNotSendFlag)
        {
            return true;
        }
    }
}



//    ''' <summary>
//    ''' ワークフローの更新を実行します。メールを自動送信します。更新結果判定フラグを返します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のワークフロー制御対象となる業務キーを表す値です。</para>
//    ''' </param>
//    ''' <param name="historyNo"> <see cref="Integer">Integer</see> 型です。
//    ''' <para>検索条件の履歴番号です。</para>
//    ''' </param>
//    ''' <param name="fromCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="rootNo"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のルート番号です。</para>
//    ''' </param>
//    ''' <param name="actionType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のアクション区分です。</para>
//    ''' </param>
//    ''' <param name="toCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="exclusiveKey"> <see cref="Long">Long</see> 型です。
//    ''' <para>検索条件の排他制御に用いる値です。</para>
//    ''' </param>
//    ''' <param name="mailLanguage"> <see cref="String">String</see> 型です。
//    ''' <para>自動メール送信時の言語です。</para>
//    ''' </param>
//    ''' <param name="replyDeadLine"> <see cref="String">String</see> 型です。
//    ''' <para>作業完了要求日の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText1"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)１の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText2"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)２の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText3"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)３の値です。</para>
//    ''' </param>
//    ''' <param name="optionalDate"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(日付)の値です。</para>
//    ''' </param>
//    ''' <param name="enforceNotSendFlag"> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>自動メール送信強制停止フラグです。</para>
//    ''' <para>TRUE:強制停止、FALSE:自動送信</para>
//    ''' </param>
//    ''' <param name="mailAddressArray"> <see cref="List">List</see> 型です。
//    ''' <para>自動メール送信で宛先を追加した場合のメールアドレスリストです。</para>
//    ''' <para>要素は <see cref="MailAddress">MailAddress</see> 型です。</para>
//    ''' </param>
//    ''' <param name="mailKeyWordsCollection"> <see cref="Dictionary">Dictionary</see> 型です。
//    ''' <para>メールテンプレートの埋め字に対応する文字列を格納していいます。</para>
//    ''' <para>要素は、テンプレートの埋め字箇所を表す <see cref="String">String</see> 型の文字列をKEYに、</para>
//    ''' <para>埋め字を置換する <see cref="String">String</see> 型の文字列をVALUEに取ります。</para>
//    ''' </param>
//    ''' <param name="attachedFileArray"> <see cref="List">List</see> 型です。
//    ''' <para>メール添付ファイルのリストです。</para>
//    ''' </param>
//    ''' <returns> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:正常終了、FALSE:異常終了</para>
//    ''' </returns>
//    ''' <exception cref="DbOptimisticException">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> で発生した排他エラーに対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <exception cref="DbAccessException">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <exception cref="Exception">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> を含め、プライベート関数で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Public Function Execute( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal transactionSurrogateKey As String, _
//     ByVal historyNo As Integer, _
//     ByVal fromCompanyId As String, _
//     ByVal fromSectionId As String, _
//     ByVal fromUserId As String, _
//     ByVal rootNo As String, _
//     ByVal actionType As String, _
//     ByVal toCompanyId As String, _
//     ByVal toSectionId As String, _
//     ByVal toUserId As String, _
//     ByVal exclusiveKey As Long, _
//     ByVal mailLanguage As String, _
//     ByVal replyDeadLine As String, _
//     ByVal optionalText1 As String, _
//     ByVal optionalText2 As String, _
//     ByVal optionalText3 As String, _
//     ByVal optionalDate As String, _
//     ByVal enforceNotSendFlag As Boolean, _
//     ByVal mailAddressArray As List(Of MailAddress), _
//     ByVal mailKeyWordsCollection As Dictionary(Of String, String), _
//     ByVal attachedFileArray As List(Of AttachedFile) _
//    ) As Boolean
//        Dim result As Boolean = False

//        Try
//            result = SubExecute( _
//             subsystemId, _
//             approvalType, _
//             transactionSurrogateKey, _
//             historyNo, _
//             fromCompanyId, _
//             fromSectionId, _
//             fromUserId, _
//             rootNo, _
//             actionType, _
//             toCompanyId, _
//             toSectionId, _
//             toUserId, _
//             exclusiveKey, _
//             mailLanguage, _
//             replyDeadLine, _
//             optionalText1, _
//             optionalText2, _
//             optionalText3, _
//             optionalDate, _
//             enforceNotSendFlag, _
//             mailAddressArray, _
//             mailKeyWordsCollection, _
//             attachedFileArray _
//            )
//        Catch dbaEx As DbAccessException
//            ' 呼出側にスローします。
//            Throw dbaEx
//        Catch ex As Exception
//            ' 呼出側にスローします。
//            Throw ex
//        End Try
//        Return result
//    End Function


//    ''' <summary>
//    ''' Execute実行時のエラーメッセージのリストを返します。
//    ''' </summary>
//    ''' <returns> <see cref="List">List</see> 型です。
//    ''' <para>要素は <see cref="DetectedMessageEventArgs">DetectedMessageEventArgs</see> 型です。</para>
//    ''' </returns>
//    ''' <remarks>メンバのリストが <c>Nothing</c> の場合は空のリストを返します。</remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Public Function GetMessageList() As List(Of DetectedMessageEventArgs)
//        If _
//         Me.messageArray Is Nothing _
//        OrElse _
//         Me.messageArray.Count = 0 _
//        Then
//            ' メッセージがない場合は空のリストを返す。
//            Return New List(Of DetectedMessageEventArgs)
//        Else
//            Return messageArray
//        End If
//    End Function

//    ''' <summary>
//    ''' Execute実行後の最新のワークフロー履歴を返します。
//    ''' </summary>
//    ''' <returns> <see cref="WorkflowHistory">WorkflowHistory</see> 型です。</returns>
//    ''' <remarks>Execute()実行前及びExecute()実行失敗の場合は <c>Nothing</c> を返します。</remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Public Function GetWorkflowHistory() As WorkflowHistory
//        Return workflowHistory
//    End Function

//    ''' <summary>
//    ''' トランザクションワークフローを登録します。
//    ''' </summary>
//    ''' <param name="tCurrentWorkflowEntity"> <see cref="T_CurrentWorkflowEntity">T_CurrentWorkflowEntity</see> 型です。
//    ''' <para>登録内容をプロパティに設定しているものになります。</para>
//    ''' </param>
//    ''' <param name="insHistory"> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:履歴登録有、FALSE:履歴登録無</para>
//    ''' </param>
//    ''' <returns> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:正常終了、FALSE:異常終了</para>
//    ''' </returns>
//    ''' <exception cref="DbAccessException">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <exception cref="Exception">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> を含め、プライベート関数で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Public Function InsertTCurrentWorkflow( _
//     ByVal tCurrentWorkflowEntity As T_CurrentWorkflowEntity, _
//     ByVal insHistory As Boolean _
//    ) As Boolean
//        Dim result As Boolean = False ' リターン値
//        Dim sqlCreator As New SqlCreator ' INSERT文作成クラス
//        Dim sql As String ' SQL文格納変数
//        Dim errorMessage As DetectedMessageEventArgs ' エラーメッセージ格納変数

//        ' メッセージ格納変数を初期化する
//        messageArray = New List(Of DetectedMessageEventArgs)

//        Try
//            ' 引数チェック
//            If tCurrentWorkflowEntity Is Nothing Then
//                Throw New ArgumentException
//            End If

//            ' INSERTモードに強制設定
//            tCurrentWorkflowEntity.SetInsertMode()
//            ' INSERT文作成
//            sql = sqlCreator.MakeSql(tCurrentWorkflowEntity)
//            ' 登録実行
//            dbAccessor.ExecuteNonQuery(sql)

//            If insHistory Then
//                ' 履歴登録要の場合はCurrentの内容をベースにINSERT文を作成
//                Dim tWorkflowHistoryEntity As New T_WorkflowHistoryEntity
//                tWorkflowHistoryEntity.SetInsertMode()
//                tWorkflowHistoryEntity.SubSystemId = tCurrentWorkflowEntity.SubSystemId
//                tWorkflowHistoryEntity.ApprovalType = tCurrentWorkflowEntity.ApprovalType
//                tWorkflowHistoryEntity.TransactionSurrogateKey = tCurrentWorkflowEntity.TransactionSurrogateKey
//                tWorkflowHistoryEntity.HistoryNo = tCurrentWorkflowEntity.HistoryNo
//                tWorkflowHistoryEntity.FromCompanyId = tCurrentWorkflowEntity.FromCompanyId
//                tWorkflowHistoryEntity.FromSectionId = tCurrentWorkflowEntity.FromSectionId
//                tWorkflowHistoryEntity.FromUserId = tCurrentWorkflowEntity.FromUserId
//                tWorkflowHistoryEntity.RootNo = tCurrentWorkflowEntity.RootNo
//                tWorkflowHistoryEntity.ActionType = tCurrentWorkflowEntity.ActionType
//                tWorkflowHistoryEntity.ToCompanyId = tCurrentWorkflowEntity.ToCompanyId
//                tWorkflowHistoryEntity.ToSectionId = tCurrentWorkflowEntity.ToSectionId
//                tWorkflowHistoryEntity.ToUserId = tCurrentWorkflowEntity.ToUserId
//                tWorkflowHistoryEntity.ToStatus = tCurrentWorkflowEntity.ToStatus
//                tWorkflowHistoryEntity.ElapsedTime = Nothing
//                tWorkflowHistoryEntity.NextRootNo = tCurrentWorkflowEntity.NextRootNo
//                tWorkflowHistoryEntity.ReplyDeadline = tCurrentWorkflowEntity.ReplyDeadline
//                tWorkflowHistoryEntity.OptionalText1 = tCurrentWorkflowEntity.OptionalText1
//                tWorkflowHistoryEntity.OptionalText2 = tCurrentWorkflowEntity.OptionalText2
//                tWorkflowHistoryEntity.OptionalText3 = tCurrentWorkflowEntity.OptionalText3
//                tWorkflowHistoryEntity.OptionalDate = tCurrentWorkflowEntity.OptionalDate

//                sql = sqlCreator.MakeSql(tWorkflowHistoryEntity)
//                dbAccessor.ExecuteNonQuery(sql)
//            End If
//            result = True
//        Catch dbaEx As DbAccessException
//            ' DbAccessorからのメッセージをメンバに格納する。
//            errorMessage = New DetectedMessageEventArgs(dbaEx.Message)
//            messageArray.Add(errorMessage)
//            ' 呼出側にスローします。
//            Throw dbaEx
//        Catch ex As Exception
//            ' システムエラーメッセージをメンバに格納する。
//            errorMessage = CommonUtils.CreateErrorArgsFatalError(ex)
//            messageArray.Add(errorMessage)
//            ' 呼出側にスローします。
//            Throw ex
//        End Try
//        Return result
//    End Function

//    ''' <summary>
//    ''' トランザクションワークフローを更新します。排他制御を行います。
//    ''' </summary>
//    ''' <param name="tCurrentWorkflowEntity"> <see cref="T_CurrentWorkflowEntity">T_CurrentWorkflowEntity</see> 型です。
//    ''' <para>更新内容はプロパティに設定している想定です。</para>
//    ''' </param>
//    ''' <param name="exclusiveKey"> <see cref="String">String</see> 型です。
//    ''' <para>排他制御に用いる値です。</para>
//    ''' </param>
//    ''' <param name="elapsedTime"> <see cref="String">String</see> 型です。
//    ''' <para>経過時間です。</para>
//    ''' </param>
//    ''' <param name="updHistory"> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:履歴更新有、FALSE:履歴更新無</para>
//    ''' </param>
//    ''' <returns> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:正常終了、FALSE:異常終了</para>
//    ''' </returns>
//    ''' <exception cref="DbAccessException">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <exception cref="Exception">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> を含め、プライベート関数で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <remarks>履歴の更新に使用する経過時間は前後関係が把握できないので呼出側で算出してください。</remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Public Function UpdateTCurrentWorkflow( _
//     ByVal tCurrentWorkflowEntity As T_CurrentWorkflowEntity, _
//     ByVal exclusiveKey As String, _
//     ByVal elapsedTime As String, _
//     ByVal updHistory As Boolean _
//    ) As Boolean
//        Dim result As Boolean = False ' リターン値
//        Dim sqlCreator As New SqlCreator ' UPDATE文作成クラス
//        Dim sql As String ' UPDATE文格納変数
//        Dim errorMessage As DetectedMessageEventArgs ' エラーメッセージ格納変数

//        ' メッセージ格納変数を初期化する
//        messageArray = New List(Of DetectedMessageEventArgs)

//        Try
//            ' 引数チェック
//            If tCurrentWorkflowEntity Is Nothing Then
//                Throw New ArgumentException
//            End If

//            ' 引数のTCurrentWorkflowを元にエンティティを作り変える
//            ' ExclusiveKeyの更新フラグをfalseのままにしておくための対策
//            Dim lclTCurrentWorkflowEntity As New T_CurrentWorkflowEntity
//            ' UPDATEモードに強制設定
//            lclTCurrentWorkflowEntity.SetUpdateMode()

//            lclTCurrentWorkflowEntity.SubSystemId = tCurrentWorkflowEntity.SubSystemId
//            lclTCurrentWorkflowEntity.ApprovalType = tCurrentWorkflowEntity.ApprovalType
//            lclTCurrentWorkflowEntity.TransactionSurrogateKey = tCurrentWorkflowEntity.TransactionSurrogateKey
//            lclTCurrentWorkflowEntity.HistoryNo = tCurrentWorkflowEntity.HistoryNo
//            lclTCurrentWorkflowEntity.FromCompanyId = tCurrentWorkflowEntity.FromCompanyId
//            lclTCurrentWorkflowEntity.FromSectionId = tCurrentWorkflowEntity.FromSectionId
//            lclTCurrentWorkflowEntity.FromUserId = tCurrentWorkflowEntity.FromUserId
//            lclTCurrentWorkflowEntity.RootNo = tCurrentWorkflowEntity.RootNo
//            lclTCurrentWorkflowEntity.ActionType = tCurrentWorkflowEntity.ActionType
//            lclTCurrentWorkflowEntity.ToCompanyId = tCurrentWorkflowEntity.ToCompanyId
//            lclTCurrentWorkflowEntity.ToSectionId = tCurrentWorkflowEntity.ToSectionId
//            lclTCurrentWorkflowEntity.ToUserId = tCurrentWorkflowEntity.ToUserId
//            lclTCurrentWorkflowEntity.ToStatus = tCurrentWorkflowEntity.ToStatus
//            lclTCurrentWorkflowEntity.NextRootNo = tCurrentWorkflowEntity.NextRootNo
//            lclTCurrentWorkflowEntity.ReplyDeadline = tCurrentWorkflowEntity.ReplyDeadline
//            lclTCurrentWorkflowEntity.OptionalText1 = tCurrentWorkflowEntity.OptionalText1
//            lclTCurrentWorkflowEntity.OptionalText2 = tCurrentWorkflowEntity.OptionalText2
//            lclTCurrentWorkflowEntity.OptionalText3 = tCurrentWorkflowEntity.OptionalText3
//            lclTCurrentWorkflowEntity.OptionalDate = tCurrentWorkflowEntity.OptionalDate

//            ' UPDATE文作成
//            sql = sqlCreator.MakeSql(lclTCurrentWorkflowEntity)
//            ' 排他制御で更新実行
//            dbAccessor.OptimisticUpdate(sql, CDec(exclusiveKey), T_CurrentWorkflowInformation.ExclusiveKey)

//            If updHistory Then
//                ' 履歴更新要の場合はCurrentの内容をベースにINSERT文を作成
//                Dim tWorkflowHistoryEntity As New T_WorkflowHistoryEntity
//                tWorkflowHistoryEntity.SetInsertMode()
//                tWorkflowHistoryEntity.SubSystemId = tCurrentWorkflowEntity.SubSystemId
//                tWorkflowHistoryEntity.ApprovalType = tCurrentWorkflowEntity.ApprovalType
//                tWorkflowHistoryEntity.TransactionSurrogateKey = tCurrentWorkflowEntity.TransactionSurrogateKey
//                tWorkflowHistoryEntity.HistoryNo = tCurrentWorkflowEntity.HistoryNo
//                tWorkflowHistoryEntity.FromCompanyId = tCurrentWorkflowEntity.FromCompanyId
//                tWorkflowHistoryEntity.FromSectionId = tCurrentWorkflowEntity.FromSectionId
//                tWorkflowHistoryEntity.FromUserId = tCurrentWorkflowEntity.FromUserId
//                tWorkflowHistoryEntity.RootNo = tCurrentWorkflowEntity.RootNo
//                tWorkflowHistoryEntity.ActionType = tCurrentWorkflowEntity.ActionType
//                tWorkflowHistoryEntity.ToCompanyId = tCurrentWorkflowEntity.ToCompanyId
//                tWorkflowHistoryEntity.ToSectionId = tCurrentWorkflowEntity.ToSectionId
//                tWorkflowHistoryEntity.ToUserId = tCurrentWorkflowEntity.ToUserId
//                tWorkflowHistoryEntity.ToStatus = tCurrentWorkflowEntity.ToStatus
//                tWorkflowHistoryEntity.ElapsedTime = elapsedTime
//                tWorkflowHistoryEntity.NextRootNo = tCurrentWorkflowEntity.NextRootNo
//                tWorkflowHistoryEntity.ReplyDeadline = tCurrentWorkflowEntity.ReplyDeadline
//                tWorkflowHistoryEntity.OptionalText1 = tCurrentWorkflowEntity.OptionalText1
//                tWorkflowHistoryEntity.OptionalText2 = tCurrentWorkflowEntity.OptionalText2
//                tWorkflowHistoryEntity.OptionalText3 = tCurrentWorkflowEntity.OptionalText3
//                tWorkflowHistoryEntity.OptionalDate = tCurrentWorkflowEntity.OptionalDate

//                sql = sqlCreator.MakeSql(tWorkflowHistoryEntity)
//                dbAccessor.ExecuteNonQuery(sql)
//            End If
//            result = True
//        Catch dbaEx As DbAccessException
//            ' DbAccessorからのメッセージをメンバに格納する。
//            errorMessage = New DetectedMessageEventArgs(dbaEx.Message)
//            messageArray.Add(errorMessage)
//            ' 呼出側にスローします。
//            Throw dbaEx
//        Catch ex As Exception
//            ' システムエラーメッセージをメンバに格納する。
//            errorMessage = CommonUtils.CreateErrorArgsFatalError(ex)
//            messageArray.Add(errorMessage)
//            ' 呼出側にスローします。
//            Throw ex
//        End Try
//        Return result
//    End Function

//    ''' <summary>
//    ''' トランザクションワークフローを削除します。
//    ''' </summary>
//    ''' <param name="tCurrentWorkflowEntity"> <see cref="T_CurrentWorkflowEntity">T_CurrentWorkflowEntity</see> 型です。
//    ''' <para>削除内容はプロパティに設定している想定です。</para>
//    ''' </param>
//    ''' <param name="delHistory"> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:履歴更新有、FALSE:履歴更新無</para></param>
//    ''' <returns> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:正常終了、FALSE:異常終了</para>
//    ''' </returns>
//    ''' <exception cref="DbAccessException">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <exception cref="Exception">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> を含め、プライベート関数で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Public Function DeleteTCurrentWorkflow( _
//     ByVal tCurrentWorkflowEntity As T_CurrentWorkflowEntity, _
//     ByVal delHistory As Boolean _
//    ) As Boolean
//        Dim result As Boolean = False ' リターン値
//        Dim sqlCreator As New SqlCreator ' DELETE文作成クラス
//        Dim sql As String ' SQL文格納変数
//        Dim errorMessage As DetectedMessageEventArgs ' エラーメッセージ格納変数

//        ' メッセージ格納変数を初期化する
//        messageArray = New List(Of DetectedMessageEventArgs)

//        Try
//            ' 引数チェック
//            If tCurrentWorkflowEntity Is Nothing Then
//                Throw New ArgumentException
//            End If

//            ' DELETEモードに強制設定
//            tCurrentWorkflowEntity.SetDeleteMode()
//            ' DELETE文作成
//            sql = sqlCreator.MakeSql(tCurrentWorkflowEntity)
//            ' 削除実行
//            dbAccessor.ExecuteNonQuery(sql)

//            If delHistory Then
//                ' 履歴更新要の場合はCurrentの内容をベースにDELETE文を作成
//                Dim tWorkflowHistoryEntity As New T_WorkflowHistoryEntity
//                tWorkflowHistoryEntity.SetDeleteMode()
//                tWorkflowHistoryEntity.SubSystemId = tCurrentWorkflowEntity.SubSystemId
//                tWorkflowHistoryEntity.ApprovalType = tCurrentWorkflowEntity.ApprovalType
//                tWorkflowHistoryEntity.TransactionSurrogateKey = tCurrentWorkflowEntity.TransactionSurrogateKey
//                tWorkflowHistoryEntity.HistoryNo = tCurrentWorkflowEntity.HistoryNo

//                sql = sqlCreator.MakeSql(tWorkflowHistoryEntity)
//                dbAccessor.ExecuteNonQuery(sql)
//            End If
//            result = True
//        Catch dbaEx As DbAccessException
//            ' DbAccessorからのメッセージをメンバに格納する。
//            errorMessage = New DetectedMessageEventArgs(dbaEx.Message)
//            messageArray.Add(errorMessage)
//            ' 呼出側にスローします。
//            Throw dbaEx
//        Catch ex As Exception
//            ' システムエラーメッセージをメンバに格納する。
//            errorMessage = CommonUtils.CreateErrorArgsFatalError(ex)
//            messageArray.Add(errorMessage)
//            ' 呼出側にスローします。
//            Throw ex
//        End Try
//        Return result
//    End Function

//    ''' <summary>
//    ''' カレントのワークフローを取得します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表す値です。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件となる業務データを特定する値です。</para>
//    ''' </param>
//    ''' <returns> <see cref="T_CurrentWorkflowEntity">T_CurrentWorkflowEntity</see> 型です。</returns>
//    ''' <remarks>外部からの使用を許可します。</remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Public Function GetTCurrentWorkflow( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal transactionSurrogateKey As String _
//    ) As T_CurrentWorkflowEntity
//        Dim tCurrentWorkflowEntity As T_CurrentWorkflowEntity ' リターン値
//        Dim sql As String ' SQL文格納変数

//        ' 必須チェック
//        If _
//         (subsystemId Is Nothing OrElse subsystemId.Trim().Equals(String.Empty)) _
//        OrElse _
//         (approvalType Is Nothing OrElse approvalType.Trim().Equals(String.Empty)) _
//        OrElse _
//         (transactionSurrogateKey Is Nothing OrElse transactionSurrogateKey.Trim().Equals(String.Empty)) _
//        Then
//            Throw New ArgumentException
//        End If

//        ' SELECT文作成
//        sql = _
//         GetSqlTCurrentWorkflow( _
//          subsystemId, _
//          approvalType, _
//          transactionSurrogateKey _
//         )

//        ' 検索実行
//        tCurrentWorkflowEntity = DirectCast(dbAccessor.ExecuteSelect(sql).Get(0), T_CurrentWorkflowEntity)

//        Return tCurrentWorkflowEntity
//    End Function
//#End Region
//#Region "プライベートメソッド"
//    ''' <summary>
//    ''' ワークフローの更新を実行します。メールを自動送信します。更新結果判定フラグを返します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のワークフロー制御対象となる業務キーを表す値です。</para>
//    ''' </param>
//    ''' <param name="historyNo"> <see cref="Integer">Integer</see> 型です。
//    ''' <para>検索条件の履歴番号です。</para>
//    ''' </param>
//    ''' <param name="fromCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="rootNo"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のルート番号です。</para>
//    ''' </param>
//    ''' <param name="actionType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のアクション区分です。</para>
//    ''' </param>
//    ''' <param name="toCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="exclusiveKey"> <see cref="Long">Long</see> 型です。
//    ''' <para>検索条件の排他制御に用いる値です。</para>
//    ''' </param>
//    ''' <param name="mailLanguage"> <see cref="String">String</see> 型です。
//    ''' <para>自動メール送信時の言語です。</para>
//    ''' </param>
//    ''' <param name="replyDeadLine"> <see cref="String">String</see> 型です。
//    ''' <para>作業完了要求日の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText1"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)１の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText2"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)２の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText3"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)３の値です。</para>
//    ''' </param>
//    ''' <param name="optionalDate"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(日付)の値です。</para>
//    ''' </param>
//    ''' <param name="enforceNotSendFlag"> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>自動メール送信強制停止フラグです。</para>
//    ''' <para>TRUE:強制停止、FALSE:自動送信</para>
//    ''' </param>
//    ''' <param name="mailAddressArray"> <see cref="List">List</see> 型です。
//    ''' <para>自動メール送信で宛先を追加した場合のメールアドレスリストです。</para>
//    ''' <para>要素は <see cref="MailAddress">MailAddress</see> 型です。</para>
//    ''' </param>
//    ''' <param name="mailKeyWordsCollection"> <see cref="Dictionary">Dictionary</see> 型です。
//    ''' <para>メールテンプレートの埋め字に対応する文字列を格納していいます。</para>
//    ''' <para>要素は、テンプレートの埋め字箇所を表す <see cref="String">String</see> 型の文字列をKEYに、</para>
//    ''' <para>埋め字を置換する <see cref="String">String</see> 型の文字列をVALUEに取ります。</para>
//    ''' </param>
//    ''' <param name="attachedFileArray"> <see cref="List">List</see> 型です。
//    ''' <para>メール添付ファイルのリストです。</para>
//    ''' </param>
//    ''' <returns> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:正常終了、FALSE:異常終了</para>
//    ''' </returns>
//    ''' <exception cref="DbOptimisticException">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> で発生した排他エラーに対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <exception cref="DbAccessException">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <exception cref="Exception">
//    ''' <para> <see cref="DbAccessor">DbAccessor</see> を含め、プライベート関数で発生した例外に対して</para>
//    ''' <para>メッセージを取得してメンバに格納します。</para>
//    ''' </exception>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function SubExecute( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal transactionSurrogateKey As String, _
//     ByVal historyNo As Integer, _
//     ByVal fromCompanyId As String, _
//     ByVal fromSectionId As String, _
//     ByVal fromUserId As String, _
//     ByVal rootNo As String, _
//     ByVal actionType As String, _
//     ByVal toCompanyId As String, _
//     ByVal toSectionId As String, _
//     ByVal toUserId As String, _
//     ByVal exclusiveKey As Long, _
//     ByVal mailLanguage As String, _
//     ByVal replyDeadLine As String, _
//     ByVal optionalText1 As String, _
//     ByVal optionalText2 As String, _
//     ByVal optionalText3 As String, _
//     ByVal optionalDate As String, _
//     ByVal enforceNotSendFlag As Boolean, _
//     Optional ByVal mailAddressArray As List(Of MailAddress) = Nothing, _
//     Optional ByVal mailKeyWordsCollection As Dictionary(Of String, String) = Nothing, _
//     Optional ByVal attachedFileArray As List(Of AttachedFile) = Nothing _
//    ) As Boolean
//        Dim result As Boolean = False ' リターン値
//        Dim errorMessage As DetectedMessageEventArgs ' エラーメッセージ格納変数

//        ' メッセージ格納変数を初期化する
//        messageArray = New List(Of DetectedMessageEventArgs)

//        Try
//            ' 必須項目チェック
//            If _
//             (subsystemId Is Nothing OrElse subsystemId.Trim().Equals(String.Empty)) _
//             OrElse _
//             (approvalType Is Nothing OrElse approvalType.Trim().Equals(String.Empty)) _
//             OrElse _
//             (transactionSurrogateKey Is Nothing OrElse transactionSurrogateKey.Trim().Equals(String.Empty)) _
//             OrElse _
//             (fromCompanyId Is Nothing OrElse fromCompanyId.Trim().Equals(String.Empty)) _
//             OrElse _
//             (fromSectionId Is Nothing OrElse fromSectionId.Trim().Equals(String.Empty)) _
//             OrElse _
//             (rootNo Is Nothing OrElse rootNo.Trim().Equals(String.Empty)) _
//             OrElse _
//             (actionType Is Nothing OrElse actionType.Trim().Equals(String.Empty)) _
//             OrElse _
//             (toCompanyId Is Nothing OrElse toCompanyId.Trim().Equals(String.Empty)) _
//             OrElse _
//             (toSectionId Is Nothing OrElse toSectionId.Trim().Equals(String.Empty)) _
//            Then
//                Throw New ArgumentException
//            End If

//            Dim mWorkflowRootEntity As M_WorkflowRootEntity

//            ' マスタ存在チェック
//            Dim mWorkflowRootEntityArray As EntityList = _
//             GetMWorkflowRoot4Execute( _
//              subsystemId, _
//              approvalType, _
//              fromCompanyId, _
//              fromSectionId, _
//              fromUserId, _
//              rootNo, _
//              actionType, _
//              toCompanyId, _
//              toSectionId, _
//              toUserId _
//             )

//            If _
//             mWorkflowRootEntityArray Is Nothing _
//            OrElse _
//             mWorkflowRootEntityArray.Count <> 1 _
//            Then
//                ' マスタが取得できない又は特定できない場合はエラー
//                errorMessage = New DetectedMessageEventArgs(NOT_MASTER_ERROR_CODE, New String() {M_WorkflowRootInformation.TableName})
//                messageArray.Add(errorMessage)
//                Return False
//            Else
//                ' マスタが1件取得できた場合はローカル変数に格納
//                mWorkflowRootEntity = DirectCast(mWorkflowRootEntityArray.Get(0), M_WorkflowRootEntity)
//            End If

//            ' ワークフロー更新実行
//            result = _
//             Regist( _
//              mWorkflowRootEntity, _
//              transactionSurrogateKey, _
//              historyNo, _
//              fromUserId, _
//              toUserId, _
//              exclusiveKey, _
//              replyDeadLine, _
//              optionalText1, _
//              optionalText2, _
//              optionalText3, _
//              optionalDate _
//             )

//            ' 自動メール送信
//            If _
//             result _
//            AndAlso _
//             Not enforceNotSendFlag _
//            AndAlso _
//             (Not mWorkflowRootEntity.MailTemplateId Is Nothing OrElse Not mWorkflowRootEntity.MailTemplateId Is Nothing) _
//            Then
//                ' ワークフロー更新が正常終了、強制停止フラグOFF且つメールテンプレートの指定がある場合のみ実行
//                If _
//                 mailLanguage Is Nothing _
//                OrElse _
//                 mailLanguage.Trim().Equals(String.Empty) _
//                Then
//                    ' メール送信言語の指定がない場合は実行ユーザの使用言語を設定する。
//                    mailLanguage = userManager.UseLanguage
//                End If
//                result = _
//                 AutoSendMail( _
//                  mWorkflowRootEntity, _
//                  mailLanguage, _
//                  mailAddressArray, _
//                  mailKeyWordsCollection, _
//                  attachedFileArray _
//                 )
//            End If
//        Catch optEx As DbOptimisticException
//            ' 排他エラーの場合(リトライが可能なので呼出側にスローしない)
//            result = False
//            ' DbAccessorからのメッセージをメンバに格納する。
//            errorMessage = New DetectedMessageEventArgs(optEx.Message)
//            messageArray.Add(errorMessage)
//        Catch dbaEx As DbAccessException
//            result = False
//            ' DbAccessorからのメッセージをメンバに格納する。
//            errorMessage = New DetectedMessageEventArgs(dbaEx.Message)
//            messageArray.Add(errorMessage)
//            ' 呼出側にスローします。
//            Throw dbaEx
//        Catch ex As Exception
//            result = False
//            ' システムエラーメッセージをメンバに格納する。
//            errorMessage = CommonUtils.CreateErrorArgsFatalError(ex)
//            messageArray.Add(errorMessage)
//            ' 呼出側にスローします。
//            Throw ex
//        End Try
//        Return result
//    End Function

//    ''' <summary>
//    ''' 検索条件を元に次のワークフロー候補となるマスタを取得します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表す値です。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="fromCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="rootNo"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のルート番号です。</para>
//    ''' </param>
//    ''' <param name="actionType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のアクション区分です。</para>
//    ''' </param>
//    ''' <returns> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型です。</para>
//    ''' </returns>
//    ''' <remarks>取得結果の階層構造は以下です。
//    ''' <para> <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> -</para>
//    ''' <para>   <see cref="M_WorkflowMailDeliverToEntity">M_WorkflowMailDeliverToEntity</see> -</para>
//    ''' <para>     <see cref="M_UserEntity">M_UserEntity</see> - </para>
//    ''' <para>       <see cref="M_MailTemplateEntity">M_MailTemplateEntity</see> </para>
//    ''' </remarks>
//    Private Function GetMWorkflowRoot4NextCandidates( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal fromCompanyId As String, _
//     ByVal fromSectionId As String, _
//     ByVal fromUserId As String, _
//     ByVal rootNo As String, _
//     ByVal actionType As String _
//    ) As EntityList
//        Dim resultArray As New EntityList ' リターン値

//        ' SELECT文作成
//        Dim sqlStr As String = _
//         GetSqlMWorkFlowRoot4NextCandidates( _
//          subsystemId, _
//          approvalType, _
//          fromCompanyId, _
//          fromSectionId, _
//          fromUserId, _
//          rootNo, _
//          actionType _
//        )

//        ' 検索実行
//        resultArray = dbAccessor.ExecuteSelect(sqlStr)

//        Return resultArray
//    End Function

//    ''' <summary>
//    ''' 検索条件からワークフローの遷移先に指定しているユーザの情報を取得します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntityArray"> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型になります。</para>
//    ''' <para>遷移先の候補になります。</para>
//    ''' </param>
//    ''' <returns> <see cref="List">List</see> 型です。
//    ''' <para>要素は <see cref="WorkflowAddress">WorkflowAddress</see> 型になります。</para>
//    ''' </returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetDefaultToUsersFromMWorkFlowRoot( _
//     ByVal mWorkflowRootEntityArray As EntityList _
//    ) As List(Of WorkflowAddress)
//        Dim resultArray As New List(Of WorkflowAddress)	' リターン値
//        Dim mUserEntityArray As New EntityList ' 検索結果格納変数

//        ' SELECT文作成
//        Dim sqlStr As String = GetSqlDefaultToUsersFromMWorkFlowRoot(mWorkflowRootEntityArray)

//        ' 検索実行
//        mUserEntityArray = dbAccessor.ExecuteSelect(sqlStr)

//        ' 検索結果をListに詰め替える
//        resultArray = RefillToWorkflowAddressList(mUserEntityArray)

//        Return resultArray
//    End Function

//    ' QUO2-HB-B0017 対応 2012/06/18 luo start
//    ''' <summary>
//    ''' 検索条件からワークフローの遷移先に所属しているユーザの情報を取得します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntityArray"> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型になります。</para>
//    ''' <para>遷移先の候補になります。</para>
//    ''' </param>
//    ''' <returns> <see cref="List">List</see> 型です。
//    ''' <para>要素は <see cref="WorkFlowAddress">WorkFlowAddress</see> 型になります。</para>
//    ''' </returns>
//    ''' <remarks></remarks>
//    Private Function GetToUsersFromMUserForReplyToACT( _
//     ByVal mWorkflowRootEntityArray As EntityList _
//    ) As List(Of WorkflowAddress)
//        Dim resultArray As New List(Of WorkflowAddress)	' リターン値
//        Dim mUserEntityArray As New EntityList ' 検索結果格納変数

//        ' SELECT文作成
//        Dim sqlStr As String = GetSqlToUsersFromMUserForReplyToACT(mWorkflowRootEntityArray)

//        ' 検索実行
//        mUserEntityArray = dbAccessor.ExecuteSelect(sqlStr)

//        ' 検索結果をListに詰め替える
//        resultArray = RefillToWorkflowAddressList(mUserEntityArray)

//        Return resultArray
//    End Function
//    ' QUO2-HB-B0017 対応 2012/06/18 luo end

//    ''' <summary>
//    ''' 検索条件からワークフローの遷移先に所属しているユーザの情報を取得します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntityArray"> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型になります。</para>
//    ''' <para>遷移先の候補になります。</para>
//    ''' </param>
//    ''' <returns> <see cref="List">List</see> 型です。
//    ''' <para>要素は <see cref="WorkFlowAddress">WorkFlowAddress</see> 型になります。</para>
//    ''' </returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetToUsersFromMUser( _
//     ByVal mWorkflowRootEntityArray As EntityList _
//    ) As List(Of WorkflowAddress)
//        Dim resultArray As New List(Of WorkflowAddress)	' リターン値
//        Dim mUserEntityArray As New EntityList ' 検索結果格納変数

//        ' SELECT文作成
//        Dim sqlStr As String = GetSqlToUsersFromMUser(mWorkflowRootEntityArray)

//        ' 検索実行
//        mUserEntityArray = dbAccessor.ExecuteSelect(sqlStr)

//        ' 検索結果をListに詰め替える
//        resultArray = RefillToWorkflowAddressList(mUserEntityArray)

//        Return resultArray
//    End Function

//    ''' <summary>
//    ''' ユーザ情報の一覧をマスタ指定有、マスタ指定無の順でソートします。
//    ''' </summary>
//    ''' <param name="defaultWorkflowAddressArray"> <see cref="List">List</see> 型です。
//    ''' <para>マスタで指定されたユーザの情報です。</para>
//    ''' <para>要素は <see cref="WorkflowAddress">WorkflowAddress</see> 型です。</para>
//    ''' </param>
//    ''' <param name="workflowAddressArray"> <see cref="List">List</see> 型です。
//    ''' <para>マスタに指定されていないユーザの情報です。</para>
//    ''' <para>要素は <see cref="WorkflowAddress">WorkflowAddress</see> 型です。</para>
//    ''' </param>
//    ''' <returns> <see cref="List">List</see> 型です。
//    ''' <para>要素は <see cref="WorkflowAddress">WorkflowAddress</see> 型です。</para>
//    ''' </returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function SortAddressList( _
//     ByVal defaultWorkflowAddressArray As List(Of WorkflowAddress), _
//     ByVal workflowAddressArray As List(Of WorkflowAddress) _
//    ) As List(Of WorkflowAddress)
//        Dim resultArray As New List(Of WorkflowAddress)	' リターン値

//        If 0 < defaultWorkflowAddressArray.Count Then
//            ' マスタに指定があるものを先頭に格納します
//            resultArray.AddRange(defaultWorkflowAddressArray)
//        End If

//        ' 次にユーザ情報のリストを格納します
//        resultArray.AddRange(workflowAddressArray)

//        Return resultArray
//    End Function

//    ''' <summary>
//    ''' ユーザ情報に紐付く会社、組織の情報をWorkflowAddressに変換します。
//    ''' </summary>
//    ''' <param name="mUserEntityArray"> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>遷移先のユーザ情報リストです。</para>
//    ''' <para>要素は <see cref="M_UserEntity">M_UserEntity</see> 型です。</para>
//    ''' </param>
//    ''' <returns> <see cref="List">List</see> 型です。
//    ''' <para>要素は <see cref="WorkflowAddress">WorkflowAddress</see> 型です。</para>
//    ''' </returns>
//    ''' <remarks>引数の階層構造は以下です。M_UserEntity-M_CompanyEntity-M_SectionEntity</remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function RefillToWorkflowAddressList( _
//     ByVal mUserEntityArray As EntityList _
//    ) As List(Of WorkflowAddress)
//        Dim resultArray As New List(Of WorkflowAddress)	' リターン値

//        For Each mUserEntity As M_UserEntity In mUserEntityArray
//            ' ベースはユーザ単位
//            For Each mSectionEntity As M_SectionEntity In mUserEntity.ChildEntityList
//                ' 1組織には1企業なのでリストの先頭のみ取得
//                Dim mCompanyEntity As M_CompanyEntity = DirectCast(mSectionEntity.ChildEntityList.Get(0), M_CompanyEntity)
//                ' ワークフローから予備項目取得
//                Dim mWorkflowRootEntity As M_WorkflowRootEntity = DirectCast(mCompanyEntity.ChildEntityList.Get(0), M_WorkflowRootEntity)
//                Dim optionalText1 As String = Nothing
//                Dim optionalText2 As String = Nothing
//                Dim optionalText3 As String = Nothing
//                Dim optionalDate As DateTime = Nothing

//                optionalText1 = mWorkflowRootEntity.OptionalText1
//                optionalText2 = mWorkflowRootEntity.OptionalText2
//                optionalText3 = mWorkflowRootEntity.OptionalText3
//                If _
//                 Not mWorkflowRootEntity.OptionalDate Is Nothing _
//                AndAlso _
//                 Not mWorkflowRootEntity.OptionalDate.Trim().Equals(String.Empty) _
//                Then
//                    ' 予備項目(日付)に値がある場合は型変換を行う
//                    optionalDate = DateTime.Parse(mWorkflowRootEntity.OptionalDate)
//                End If

//                ' 兼務を考慮して組織単位に遷移先リストを作成する。
//                Dim workflowAddress As New WorkflowAddress( _
//                 CStr(mCompanyEntity.Id), _
//                 mCompanyEntity.Name_EN, _
//                 mCompanyEntity.Name_DM, _
//                 mCompanyEntity.AbbreviatedName_EN, _
//                 mCompanyEntity.AbbreviatedName_DM, _
//                 CStr(mSectionEntity.Id), _
//                 mSectionEntity.Name_EN, _
//                 mSectionEntity.Name_DM, _
//                 mSectionEntity.AbbreviatedName_EN, _
//                 mSectionEntity.AbbreviatedName_DM, _
//                 CStr(mUserEntity.Id), _
//                 mUserEntity.Name_EN, _
//                 mUserEntity.Name_DM, _
//                 mUserEntity.UseLanguage, _
//                 mUserEntity.MailAddress, _
//                 optionalText1, _
//                 optionalText2, _
//                 optionalText3, _
//                 optionalDate _
//                )
//                resultArray.Add(workflowAddress)
//            Next

//            'Dim mCompanyEntity As M_CompanyEntity = DirectCast(mUserEntity.ChildEntityList.Get(0), M_CompanyEntity)
//            'Dim mSectionEntity As M_SectionEntity = DirectCast(mCompanyEntity.ChildEntityList.Get(0), M_SectionEntity)
//            'Dim mWorkflowRootEntity As M_WorkflowRootEntity
//            'Dim optionalText1 As String = Nothing
//            'Dim optionalText2 As String = Nothing
//            'Dim optionalText3 As String = Nothing
//            'Dim optionalDate As DateTime = Nothing

//            'mWorkflowRootEntity = DirectCast(mSectionEntity.ChildEntityList.Get(0), M_WorkflowRootEntity)
//            'optionalText1 = mWorkflowRootEntity.OptionalText1
//            'optionalText2 = mWorkflowRootEntity.OptionalText2
//            'optionalText3 = mWorkflowRootEntity.OptionalText3
//            'If _
//            ' Not mWorkflowRootEntity.OptionalDate Is Nothing _
//            'AndAlso _
//            ' Not mWorkflowRootEntity.OptionalDate.Trim().Equals(String.Empty) _
//            'Then
//            '	' 予備項目(日付)に値がある場合は型変換を行う
//            '	optionalDate = DateTime.Parse(mWorkflowRootEntity.OptionalDate)
//            'End If

//            'Dim workflowAddress As New WorkflowAddress( _
//            ' CStr(mCompanyEntity.Id), _
//            ' mCompanyEntity.Name_EN, _
//            ' mCompanyEntity.Name_DM, _
//            ' mCompanyEntity.AbbreviatedName_EN, _
//            ' mCompanyEntity.AbbreviatedName_DM, _
//            ' CStr(mSectionEntity.Id), _
//            ' mSectionEntity.Name_EN, _
//            ' mSectionEntity.Name_DM, _
//            ' mSectionEntity.AbbreviatedName_EN, _
//            ' mSectionEntity.AbbreviatedName_DM, _
//            ' CStr(mUserEntity.Id), _
//            ' mUserEntity.Name_EN, _
//            ' mUserEntity.Name_DM, _
//            ' mUserEntity.UseLanguage, _
//            ' mUserEntity.MailAddress, _
//            ' optionalText1, _
//            ' optionalText2, _
//            ' optionalText3, _
//            ' optionalDate _
//            ')
//            'resultArray.Add(workflowAddress)
//        Next

//        Return resultArray
//    End Function

//    ''' <summary>
//    ''' 検索条件を元にワークフローマスタを取得します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表す値です。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="fromCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="rootNo"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のルート番号です。</para>
//    ''' </param>
//    ''' <param name="actionType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のアクション区分です。</para>
//    ''' </param>
//    ''' <param name="toCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <returns> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型です。</para>
//    ''' </returns>
//    ''' <remarks>取得結果の階層構造は以下です。
//    ''' <para> <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> -</para>
//    ''' <para>   <see cref="M_WorkflowMailDeliverToEntity">M_WorkflowMailDeliverToEntity</see> -</para>
//    ''' <para>     <see cref="M_UserEntity">M_UserEntity</see> - </para>
//    ''' <para>       <see cref="M_MailTemplateEntity">M_MailTemplateEntity</see> </para>
//    ''' </remarks>
//    Private Function GetMWorkflowRoot4Execute( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal fromCompanyId As String, _
//     ByVal fromSectionId As String, _
//     ByVal fromUserId As String, _
//     ByVal rootNo As String, _
//     ByVal actionType As String, _
//     ByVal toCompanyId As String, _
//     ByVal toSectionId As String, _
//     ByVal toUserId As String _
//    ) As EntityList
//        Dim resultArray As New EntityList ' リターン値

//        ' SELECT文作成
//        Dim sqlStr As String = _
//         GetSqlMWorkFlowRoot4Execute( _
//          subsystemId, _
//          approvalType, _
//          fromCompanyId, _
//          fromSectionId, _
//          fromUserId, _
//          rootNo, _
//          actionType, _
//          toCompanyId, _
//          toSectionId, _
//          toUserId _
//         )

//        ' 検索実行
//        resultArray = dbAccessor.ExecuteSelect(sqlStr)

//        Return resultArray
//    End Function

//    ''' <summary>
//    ''' ワークフローの登録を行います。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntity"> <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型です。
//    ''' <para>子階層に <see cref="M_WorkflowMailDeliverToEntity">M_WorkflowMailDeliverToEntity</see> と</para>
//    ''' <para> <see cref="M_UserEntity">M_UserEntity</see> と</para>
//    ''' <para> <see cref="M_MailTemplateEntity">M_MailTemplateEntity</see> </para>
//    ''' <para>を保持している想定です。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のワークフロー制御対象となる業務キーを表す値です。</para>
//    ''' </param>
//    ''' <param name="historyNo"> <see cref="Integer">Integer</see> 型です。
//    ''' <para>検索条件の履歴番号です。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移先ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="exclusiveKey"> <see cref="Long">Long</see> 型です。
//    ''' <para>検索条件の排他制御に用いる値です。</para>
//    ''' </param>
//    ''' <param name="replyDeadLine"> <see cref="String">String</see> 型です。
//    ''' <para>作業完了要求日の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText1"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)１の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText2"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)２の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText3"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)３の値です。</para>
//    ''' </param>
//    ''' <param name="optionalDate"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(日付)の値です。</para>
//    ''' </param>
//    ''' <returns> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:正常終了、FALSE:異常終了</para>
//    ''' </returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function Regist( _
//     ByVal mWorkflowRootEntity As M_WorkflowRootEntity, _
//     ByVal transactionSurrogateKey As String, _
//     ByVal historyNo As Integer, _
//     ByVal fromUserId As String, _
//     ByVal toUserId As String, _
//     ByVal exclusiveKey As Long, _
//     ByVal replyDeadLine As String, _
//     ByVal optionalText1 As String, _
//     ByVal optionalText2 As String, _
//     ByVal optionalText3 As String, _
//     ByVal optionalDate As String _
//    ) As Boolean
//        Dim result = False ' リターン値
//        Dim errorMessage As DetectedMessageEventArgs ' エラーメッセージ格納変数

//        ' 次のワークフローを取得
//        Dim nextMWorkflowRootEntity As M_WorkflowRootEntity
//        If _
//         Not mWorkflowRootEntity.NextRootNo Is Nothing _
//        AndAlso _
//         Not mWorkflowRootEntity.NextRootNo.Trim().Equals(String.Empty) _
//        Then
//            ' 最新のワークフローに次のワークフローの指定がある場合のみ取得する
//            Dim nextMWorkflowRootEntityArray As EntityList = GetNextMWorkflowRoot(mWorkflowRootEntity, toUserId)
//            If nextMWorkflowRootEntityArray.Count = 0 Then
//                ' 次のワークフロー候補が取得できない場合はエラー
//                errorMessage = New DetectedMessageEventArgs(NOT_MASTER_ERROR_CODE, New String() {M_WorkflowRootInformation.TableName})
//                messageArray.Add(errorMessage)
//                Return False
//            Else
//                nextMWorkflowRootEntity = DirectCast(nextMWorkflowRootEntityArray.Get(0), M_WorkflowRootEntity)
//            End If
//        End If

//        ' 最新のカレントの取得を試みる
//        Dim tCurrentWorkflowEntity As T_CurrentWorkflowEntity
//        tCurrentWorkflowEntity = _
//         GetTCurrentWorkflow( _
//          mWorkflowRootEntity.SubSystemId, _
//          mWorkflowRootEntity.ApprovalType, _
//          transactionSurrogateKey _
//         )
//        Dim sql As String
//        If tCurrentWorkflowEntity Is Nothing Then
//            ' カレントワークフローが存在しない場合はINSERT実行
//            sql = _
//             GetSqlInsertTCurrentWorkflow( _
//              mWorkflowRootEntity, _
//              transactionSurrogateKey, _
//              historyNo, _
//              fromUserId, _
//              toUserId, _
//              exclusiveKey, _
//              replyDeadLine, _
//              optionalText1, _
//              optionalText2, _
//              optionalText3, _
//              optionalDate _
//             )
//            ' カレントを登録
//            dbAccessor.ExecuteNonQuery(sql)

//            ' 
//            sql = _
//             GetSqlInsertTWorkflowHistory( _
//              mWorkflowRootEntity, _
//              tCurrentWorkflowEntity, _
//              transactionSurrogateKey, _
//              historyNo, _
//              fromUserId, _
//              toUserId, _
//              exclusiveKey, _
//              replyDeadLine, _
//              optionalText1, _
//              optionalText2, _
//              optionalText3, _
//              optionalDate _
//             )
//            ' 履歴を登録
//            dbAccessor.ExecuteNonQuery(sql)
//        Else
//            ' 更新の場合はカレントのUPDATE文(排他制御込)と履歴のINSERT文を作成
//            sql = _
//             GetSqlUpdateTCurrentWorkflow( _
//              mWorkflowRootEntity, _
//              transactionSurrogateKey, _
//              historyNo, _
//              fromUserId, _
//              toUserId, _
//              exclusiveKey, _
//              replyDeadLine, _
//              optionalText1, _
//              optionalText2, _
//              optionalText3, _
//              optionalDate _
//             )
//            dbAccessor.OptimisticUpdate(sql, CDec(exclusiveKey), T_CurrentWorkflowInformation.ExclusiveKey)

//            ' 
//            sql = _
//             GetSqlInsertTWorkflowHistory( _
//              mWorkflowRootEntity, _
//              tCurrentWorkflowEntity, _
//              transactionSurrogateKey, _
//              historyNo, _
//              fromUserId, _
//              toUserId, _
//              exclusiveKey + 1, _
//              replyDeadLine, _
//              optionalText1, _
//              optionalText2, _
//              optionalText3, _
//              optionalDate _
//             )
//            dbAccessor.ExecuteNonQuery(sql)
//        End If

//        ' 登録更新完了時の最新履歴を取得する
//        sql = _
//         GetSqlLastTWorkflowHistory( _
//           mWorkflowRootEntity.SubSystemId, _
//           mWorkflowRootEntity.ApprovalType, _
//           transactionSurrogateKey _
//        )
//        Dim tWorkflowHistoryEntityArray As EntityList = dbAccessor.ExecuteSelect(sql)
//        workflowHistory = New WorkflowHistory(DirectCast(tWorkflowHistoryEntityArray.Get(0), T_WorkflowHistoryEntity))

//        result = True

//        Return result
//    End Function

//    ''' <summary>
//    ''' 現在のワークフローマスタから次のワークフローの候補を取得します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntity"> <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型です。
//    ''' <para>子階層に <see cref="M_WorkflowMailDeliverToEntity">M_WorkflowMailDeliverToEntity</see> と</para>
//    ''' <para> <see cref="M_UserEntity">M_UserEntity</see> と</para>
//    ''' <para> <see cref="M_MailTemplateEntity">M_MailTemplateEntity</see> </para>
//    ''' <para>を保持している想定です。</para>
//    ''' </param>
//    ''' <param name="toUserId"> <see cref="String">String</see> 型です。
//    ''' <para>業務ロジック側で指定された遷移先ユーザIDになります。</para></param>
//    ''' <returns> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see>型になります。</para>
//    ''' </returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetNextMWorkflowRoot( _
//     ByVal mWorkflowRootEntity As M_WorkflowRootEntity, _
//     ByVal toUserId As String _
//    ) As EntityList
//        Dim resultArray As New EntityList ' リターン値
//        Dim sql As String ' SQL文格納変数

//        ' SELECT文作成
//        sql = _
//         GetSqlMWorkFlowRoot4NextWorkflowRoot( _
//          mWorkflowRootEntity.SubSystemId, _
//          mWorkflowRootEntity.ApprovalType, _
//          mWorkflowRootEntity.ToCompanyId, _
//          mWorkflowRootEntity.ToSectionId, _
//          mWorkflowRootEntity.ToUserId, _
//          mWorkflowRootEntity.NextRootNo _
//         )

//        ' 検索実行
//        resultArray = dbAccessor.ExecuteSelect(sql)

//        Return resultArray
//    End Function

//    ''' <summary>
//    ''' メール送信情報を登録します。MailManagerクラスの機能を使用します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntity"> <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型です。
//    ''' <para>子階層に <see cref="M_WorkflowMailDeliverToEntity">M_WorkflowMailDeliverToEntity</see> と</para>
//    ''' <para> <see cref="M_UserEntity">M_UserEntity</see> と</para>
//    ''' <para> <see cref="M_MailTemplateEntity">M_MailTemplateEntity</see> </para>
//    ''' <para>を保持している想定です。</para>
//    ''' </param>
//    ''' <param name="mailLanguage"> <see cref="String">String</see> 型です。
//    ''' <para>送信メールの記載言語です。</para>
//    ''' </param>
//    ''' <param name="addMailAddressArray"> <see cref="List">List</see> 型です。
//    ''' <para>マスタ定義以外の追加アドレスリストになります。</para>
//    ''' <para>要素は <see cref="MailAddress">MailAddress</see> 型です。</para>
//    ''' </param>
//    ''' <param name="mailKeyWordsCollection"> <see cref="Dictionary">Dictionary</see> 型です。
//    ''' <para>メールテンプレートの埋め字に対応する文字列を格納していいます。</para>
//    ''' <para>要素は、テンプレートの埋め字箇所を表す <see cref="String">String</see> 型の文字列をKEYに、</para>
//    ''' <para>埋め字を置換する <see cref="String">String</see> 型の文字列をVALUEに取ります。</para>
//    ''' </param>
//    ''' <param name="attachedFileArray"> <see cref="List">List</see> 型です。
//    ''' <para>メール添付ファイルのリストです。</para>
//    ''' </param>
//    ''' <returns> <see cref="Boolean">Boolean</see> 型です。
//    ''' <para>TRUE:正常終了、FALSE:異常終了</para>
//    ''' </returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function AutoSendMail( _
//     ByVal mWorkflowRootEntity As M_WorkflowRootEntity, _
//     ByVal mailLanguage As String, _
//     ByVal addMailAddressArray As List(Of MailAddress), _
//     ByVal mailKeyWordsCollection As Dictionary(Of String, String), _
//     ByVal attachedFileArray As List(Of AttachedFile) _
//    ) As Boolean
//        Dim result As Boolean = True ' リターン値

//        Dim mailManager As New MailManager(dbAccessor, userManager)
//        Dim mMailTemplateEntity As M_MailTemplateEntity
//        ' メールテンプレートを取り出す
//        mMailTemplateEntity = _
//         DirectCast( _
//          DirectCast( _
//           DirectCast( _
//         mWorkflowRootEntity.ChildEntityList.Get(0), M_WorkflowMailDeliverToEntity _
//           ).ChildEntityList.Get(0), M_UserEntity _
//          ).ChildEntityList.Get(0), M_MailTemplateEntity _
//         )

//        ' 送信先情報をマージする
//        Dim workflowMailDeliverToEntityArray As EntityList = mWorkflowRootEntity.ChildEntityList
//        Dim mailAddressArray As List(Of MailAddress) = MergeDeliverAddress(workflowMailDeliverToEntityArray, addMailAddressArray)

//        If _
//         Not mailAddressArray Is Nothing _
//        AndAlso _
//         0 < mailAddressArray.Count _
//        Then
//            ' 送信先がある場合のみ送信処理を続行する

//            ' 送信者は御中ID
//            Dim senderAddress As String = Nothing

//            ' メール情報をロードする
//            mailManager.Load(mMailTemplateEntity.SubSystemId, mMailTemplateEntity.Code)

//            ' メール送信情報を登録する
//            result = mailManager.SendMail(senderAddress, mailLanguage, mailAddressArray, mailKeyWordsCollection, attachedFileArray)

//            If Not result Then
//                ' メール機能が異常終了した場合はメッセージを取得する
//                messageArray.AddRange(mailManager.GetMessageList())
//            End If
//        End If
//        Return result
//    End Function

//    ''' <summary>
//    ''' マスタに登録されている送信先アドレスと業務側で追加した送信先アドレスをマージします。
//    ''' </summary>
//    ''' <param name="mWorkflowMailDeliverToEntityArray"> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>ワークフローに紐付く送信先マスタのリストになります。</para>
//    ''' <para>要素は <see cref="M_WorkflowMailDeliverToEntity">M_WorkflowMailDeliverToEntity</see> 型です。</para>
//    ''' </param>
//    ''' <param name="addMailAddressArray"> <see cref="List">List</see> 型です。
//    ''' <para>マスタ定義以外の追加アドレスリストになります。</para>
//    ''' <para>要素は <see cref="MailAddress">MailAddress</see> 型です。</para>
//    ''' </param>
//    ''' <returns> <see cref="List">List</see> 型です。
//    ''' <para>要素は <see cref="MailAddress">MailAddress</see> 型です。</para>
//    ''' </returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function MergeDeliverAddress( _
//     ByVal mWorkflowMailDeliverToEntityArray As EntityList, _
//     ByVal addMailAddressArray As List(Of MailAddress) _
//    ) As List(Of MailAddress)
//        Dim resultArray As New List(Of MailAddress)	'リターン値

//        If _
//         addMailAddressArray Is Nothing _
//        OrElse _
//         addMailAddressArray.Count = 0 _
//        Then
//            ' データがない場合は空のリストに置き換える
//            addMailAddressArray = New List(Of MailAddress)
//        End If

//        If _
//         mWorkflowMailDeliverToEntityArray Is Nothing _
//        OrElse _
//         (mWorkflowMailDeliverToEntityArray.Count = 1 _
//          AndAlso _
//          DirectCast(mWorkflowMailDeliverToEntityArray.Get(0), M_WorkflowMailDeliverToEntity).WorkflowRootId Is Nothing _
//          AndAlso _
//          DirectCast(mWorkflowMailDeliverToEntityArray.Get(0), M_WorkflowMailDeliverToEntity).DeliveryType Is Nothing _
//          AndAlso _
//          DirectCast(mWorkflowMailDeliverToEntityArray.Get(0), M_WorkflowMailDeliverToEntity).SequenceNo Is Nothing _
//         ) _
//        Then
//            ' マスタに定義がない場合は追加アドレスのリストを返す
//            Return addMailAddressArray
//        End If

//        For Each mWorkflowMailDeliverToEntity As M_WorkflowMailDeliverToEntity In mWorkflowMailDeliverToEntityArray
//            ' M_WorkflowMailDeliverToEntityの内容をMailAddressに置き換える
//            Dim mailAddress As New MailAddress( _
//             mWorkflowMailDeliverToEntity.DeliveryType, _
//             DirectCast(mWorkflowMailDeliverToEntity.ChildEntityList.Get(0), M_UserEntity).MailAddress _
//            )
//            resultArray.Add(mailAddress)
//        Next
//        ' 追加分をリストの後に追加
//        ' 追加アドレスがある場合はリストに追加する。
//        resultArray.AddRange(addMailAddressArray)

//        Return resultArray
//    End Function

//#Region "SQL文作成メソッド"
//    ''' <summary>
//    ''' データの有効期限を判定するWHERE句の条件を作成します。
//    ''' </summary>
//    ''' <param name="sql"> <see cref="SqlStringBuilder">SqlStringBuilder</see> 型です。
//    ''' <para>検索条件を追記します。</para>
//    ''' </param>
//    ''' <param name="tableName"> <see cref="String">String</see> 型です。
//    ''' <para>対象となるテーブルの名称です。</para>
//    ''' </param>
//    ''' <param name="startColumnName"> <see cref="String">String</see> 型です。
//    ''' <para>x有効期限開始日のカラム名称です。</para>
//    ''' </param>
//    ''' <param name="endColumnName"> <see cref="String">String</see> 型です。
//    ''' <para>有効期限終了日のカラム名称です。</para>
//    ''' </param>
//    ''' <remarks>引数の <see cref="SqlStringBuilder">SqlStringBuilder</see> に検索条件を追加します。</remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Sub AddWhereExpire( _
//     sql As SqlStringBuilder, _
//     ByVal tableName As String, _
//     ByVal startColumnName As String, _
//     ByVal endColumnName As String _
//    )
//        sql.AppendAnd()
//        sql.Append("CONVERT(CHAR,")
//        sql.AppendColumn(tableName, startColumnName)
//        sql.Append(",112) ")
//        sql.Append("<= CONVERT(CHAR,GETUTCDATE(),112) ")
//        sql.AppendAnd()
//        sql.Append("(")
//        sql.AppendColumn(tableName, endColumnName)
//        sql.Append(" IS NULL ")
//        sql.AppendOr()
//        sql.Append("CONVERT(CHAR,GETUTCDATE(),112) <= ")
//        sql.Append("CONVERT(CHAR,")
//        sql.AppendColumn(tableName, endColumnName)
//        sql.Append(",112) ")
//        sql.Append(") ")
//    End Sub

//    ''' <summary>
//    ''' 検索条件を元に次のワークフロー候補となるマスタを取得するSELECT文を作成します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表す値です。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>x検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="fromCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="rootNo"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のルート番号です。</para>
//    ''' </param>
//    ''' <param name="actionType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のアクション区分です。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。SELECT文です。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlMWorkFlowRoot4Execute( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal fromCompanyId As String, _
//     ByVal fromSectionId As String, _
//     ByVal fromUserId As String, _
//     ByVal rootNo As String, _
//     ByVal actionType As String, _
//     ByVal toCompanyId As String, _
//     ByVal toSectionId As String, _
//     ByVal toUserId As String _
//    ) As String
//        Dim sql As SqlStringBuilder = New SqlStringBuilder ' SQL文作成クラス

//        sql.AppendSelect()
//        ' M_WorkflowRootのカラム
//        For i = 0 To M_WorkflowRootInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If
//            sql.AppendQualifiedColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ColumnNameAll(i))
//        Next
//        ' M_WorkflowMailDeliverToのカラム
//        For i = 0 To M_WorkflowMailDeliverToInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.ColumnNameAll(i))
//        Next
//        ' M_Userのカラム
//        For i = 0 To M_UserInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_UserInformation.TableName, M_UserInformation.ColumnNameAll(i))
//        Next
//        ' M_MailTemplateのカラム
//        For i = 0 To M_MailTemplateInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_MailTemplateInformation.TableName, M_MailTemplateInformation.ColumnNameAll(i))
//        Next
//        sql.AppendFrom()
//        sql.AppendTable(GetType(M_WorkflowRootInformation), M_WorkflowRootInformation.TableName)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_WorkflowMailDeliverToInformation), M_WorkflowMailDeliverToInformation.TableName)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_UserInformation), M_UserInformation.TableName)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.UserId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        ' M_Userの有効期限
//        AddWhereExpire(sql, M_UserInformation.TableName, M_UserInformation.EffectiveDate, M_UserInformation.ExpireDate)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.Id)
//        sql.AppendEqual()
//        sql.AppendColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.WorkflowRootId)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_MailTemplateInformation), M_MailTemplateInformation.TableName)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_MailTemplateInformation.TableName, M_MailTemplateInformation.SubSystemId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.MailTemplateId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_MailTemplateInformation.TableName, M_MailTemplateInformation.Id)
//        sql.AppendWhere()
//        ' 検索条件
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendStrVal(subsystemId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ApprovalType)
//        sql.AppendEqual()
//        sql.AppendStrVal(approvalType)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromCompanyId)
//        sql.AppendEqual()
//        sql.AppendStrVal(fromCompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromSectionId)
//        sql.AppendEqual()
//        sql.AppendStrVal(fromSectionId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.RootNo)
//        sql.AppendEqual()
//        sql.AppendStrVal(rootNo)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ActionType)
//        sql.AppendEqual()
//        sql.AppendStrVal(actionType)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToCompanyId)
//        sql.AppendEqual()
//        sql.AppendStrVal(toCompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToSectionId)
//        sql.AppendEqual()
//        sql.AppendStrVal(toSectionId)

//        If _
//          (Not fromUserId Is Nothing AndAlso Not fromUserId.Trim().Equals(String.Empty)) _
//         AndAlso _
//          (Not toUserId Is Nothing AndAlso Not toUserId.Trim().Equals(String.Empty)) _
//         Then
//            ' From/ToUserId両方に指定がある場合の検索条件
//            ' マスタに指定がなく、UIで追加した場合を考慮して両方の組合せ全パターンを検索する
//            sql.AppendAnd()
//            sql.Append(" ( ")
//            ' From/To両方の指定有
//            sql.Append(" ( ")
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.AppendEqual()
//            sql.AppendStrVal(fromUserId)
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToUserId)
//            sql.AppendEqual()
//            sql.AppendStrVal(toUserId)
//            sql.Append(" ) ")
//            sql.AppendOr()
//            ' Fromのみ指定有
//            sql.Append(" ( ")
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.AppendEqual()
//            sql.AppendStrVal(fromUserId)
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToUserId)
//            sql.Append(" IS NULL ")
//            sql.Append(" ) ")
//            sql.AppendOr()
//            ' Toのみ指定有
//            sql.Append(" ( ")
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.Append(" IS NULL ")
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToUserId)
//            sql.AppendEqual()
//            sql.AppendStrVal(toUserId)
//            sql.Append(" ) ")
//            sql.AppendOr()
//            ' From/Toの両方指定無
//            sql.Append(" ( ")
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.Append(" IS NULL ")
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToUserId)
//            sql.Append(" IS NULL ")
//            sql.Append(" ) ")
//            sql.Append(" ) ")
//        ElseIf _
//         (Not fromUserId Is Nothing AndAlso Not fromUserId.Trim().Equals(String.Empty)) _
//        AndAlso _
//         (toUserId Is Nothing OrElse toUserId.Trim().Equals(String.Empty)) _
//        Then
//            ' FromUserIdのみに指定がある場合の検索条件
//            sql.AppendAnd()
//            sql.Append(" ( ")
//            ' Fromのみ指定有
//            sql.Append(" ( ")
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.AppendEqual()
//            sql.AppendStrVal(fromUserId)
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToUserId)
//            sql.Append(" IS NULL ")
//            sql.Append(" ) ")
//            sql.AppendOr()
//            ' From/Toの両方指定無
//            sql.Append(" ( ")
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.Append(" IS NULL ")
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToUserId)
//            sql.Append(" IS NULL ")
//            sql.Append(" ) ")
//            sql.Append(" ) ")
//        End If

//        sql.Append(" ORDER BY ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.SequenceNo)
//        sql.Append(" ASC ")
//        Return sql.ToString
//    End Function

//    ''' <summary>
//    ''' 検索条件を元に次のワークフロー候補となるマスタを取得するSELECT文を作成します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表す値です。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>x検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="fromCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="rootNo"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のルート番号です。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。SELECT文です。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlMWorkFlowRoot4NextWorkflowRoot( _
//     ByVal subSystemId As String, _
//     ByVal approvalType As String, _
//     ByVal fromCompanyId As String, _
//     ByVal fromSectionId As String, _
//     ByVal fromUserId As String, _
//     ByVal rootNo As String _
//    ) As String
//        Dim sql As SqlStringBuilder = New SqlStringBuilder ' SQL文作成クラス

//        sql.AppendSelect()
//        ' M_WorkflowRootのカラム
//        For i = 0 To M_WorkflowRootInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If
//            sql.AppendQualifiedColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ColumnNameAll(i))
//        Next
//        sql.AppendFrom()
//        sql.AppendTable(GetType(M_WorkflowRootInformation), M_WorkflowRootInformation.TableName)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_WorkflowMailDeliverToInformation), M_WorkflowMailDeliverToInformation.TableName)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_UserInformation), M_UserInformation.TableName)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.UserId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        ' M_Userの有効期限
//        AddWhereExpire(sql, M_UserInformation.TableName, M_UserInformation.EffectiveDate, M_UserInformation.ExpireDate)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.Id)
//        sql.AppendEqual()
//        sql.AppendColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.WorkflowRootId)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_MailTemplateInformation), M_MailTemplateInformation.TableName)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_MailTemplateInformation.TableName, M_MailTemplateInformation.SubSystemId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.MailTemplateId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_MailTemplateInformation.TableName, M_MailTemplateInformation.Id)
//        sql.AppendWhere()
//        ' 検索条件
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendStrVal(subSystemId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ApprovalType)
//        sql.AppendEqual()
//        sql.AppendStrVal(approvalType)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromCompanyId)
//        sql.AppendEqual()
//        sql.AppendStrVal(fromCompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromSectionId)
//        sql.AppendEqual()
//        sql.AppendStrVal(fromSectionId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.RootNo)
//        sql.AppendEqual()
//        sql.AppendStrVal(rootNo)
//        If Not fromUserId Is Nothing AndAlso Not fromUserId.Trim().Equals(String.Empty) Then
//            sql.AppendAnd()
//            sql.Append(" ( ")
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.AppendEqual()
//            sql.AppendStrVal(fromUserId)
//            sql.AppendOr()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.Append(" IS NULL ")
//            sql.Append(" ) ")
//        End If
//        sql.Append(" ORDER BY ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.SequenceNo)
//        sql.Append(" ASC ")
//        Return sql.ToString
//    End Function

//    ''' <summary>
//    ''' 検索条件を元に次のワークフロー候補となるマスタを取得するSELECT文を作成します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表す値です。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>x検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="fromCompanyId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元会社を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromSectionId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元組織を表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="rootNo"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のルート番号です。</para>
//    ''' </param>
//    ''' <param name="actionType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のアクション区分です。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。SELECT文です。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlMWorkFlowRoot4NextCandidates( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal fromCompanyId As String, _
//     ByVal fromSectionId As String, _
//     ByVal fromUserId As String, _
//     ByVal rootNo As String, _
//     ByVal actionType As String _
//    ) As String
//        Dim sql As SqlStringBuilder = New SqlStringBuilder ' SQL文作成クラス

//        sql.AppendSelect()
//        ' M_WorkflowRootのカラム
//        For i = 0 To M_WorkflowRootInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If
//            sql.AppendQualifiedColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ColumnNameAll(i))
//        Next
//        ' M_WorkflowMailDeliverToのカラム
//        For i = 0 To M_WorkflowMailDeliverToInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.ColumnNameAll(i))
//        Next
//        ' M_Userのカラム
//        For i = 0 To M_UserInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_UserInformation.TableName, M_UserInformation.ColumnNameAll(i))
//        Next
//        ' M_MailTemplateのカラム
//        For i = 0 To M_MailTemplateInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_MailTemplateInformation.TableName, M_MailTemplateInformation.ColumnNameAll(i))
//        Next
//        sql.AppendFrom()
//        sql.AppendTable(GetType(M_WorkflowRootInformation), M_WorkflowRootInformation.TableName)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_WorkflowMailDeliverToInformation), M_WorkflowMailDeliverToInformation.TableName)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_UserInformation), M_UserInformation.TableName)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.UserId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        ' M_Userの有効期限
//        AddWhereExpire(sql, M_UserInformation.TableName, M_UserInformation.EffectiveDate, M_UserInformation.ExpireDate)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.Id)
//        sql.AppendEqual()
//        sql.AppendColumn(M_WorkflowMailDeliverToInformation.TableName, M_WorkflowMailDeliverToInformation.WorkflowRootId)
//        sql.Append(" LEFT OUTER JOIN ")
//        sql.AppendTable(GetType(M_MailTemplateInformation), M_MailTemplateInformation.TableName)
//        sql.Append(" ON ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_MailTemplateInformation.TableName, M_MailTemplateInformation.SubSystemId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.MailTemplateId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_MailTemplateInformation.TableName, M_MailTemplateInformation.Id)
//        sql.AppendWhere()
//        ' 検索条件
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendStrVal(subsystemId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ApprovalType)
//        sql.AppendEqual()
//        sql.AppendStrVal(approvalType)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromCompanyId)
//        sql.AppendEqual()
//        sql.AppendStrVal(fromCompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromSectionId)
//        sql.AppendEqual()
//        sql.AppendStrVal(fromSectionId)
//        If _
//         Not fromUserId Is Nothing _
//        AndAlso _
//         Not fromUserId.Trim().Equals(String.Empty) _
//        Then
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//            sql.AppendEqual()
//            sql.AppendStrVal(fromUserId)
//        End If
//        If _
//         Not rootNo Is Nothing _
//        AndAlso _
//         Not rootNo.Trim().Equals(String.Empty) _
//        Then
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.RootNo)
//            sql.AppendEqual()
//            sql.AppendStrVal(rootNo)
//        End If
//        If _
//         Not actionType Is Nothing _
//        AndAlso _
//         Not actionType.Trim().Equals(String.Empty) _
//        Then
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ActionType)
//            sql.AppendEqual()
//            sql.AppendStrVal(actionType)
//        End If
//        sql.Append(" ORDER BY ")
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        sql.Append(" ASC ")
//        Return sql.ToString
//    End Function

//    ''' <summary>
//    ''' ワークフローに定義した遷移先ユーザの情報を取得するSELECT文を作成します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntityArray"> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型になります。</para>
//    ''' <para>遷移先の候補になります。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。SELECT文です。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlDefaultToUsersFromMWorkFlowRoot( _
//     ByVal mWorkflowRootEntityArray As EntityList _
//    ) As String
//        Dim sql As SqlStringBuilder = New SqlStringBuilder ' SQL文作成クラス
//        ' B-TSI1-3041
//        sql.AppendSelect()
//        ' M_User
//        sql.Append(" DISTINCT ")
//        For i = 0 To M_UserInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If

//            sql.AppendQualifiedColumn(M_UserInformation.TableName, M_UserInformation.ColumnNameAll(i))
//        Next
//        ' M_Section
//        For i = 0 To M_SectionInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_SectionInformation.TableName, M_SectionInformation.ColumnNameAll(i))
//        Next
//        ' M_Company
//        For i = 0 To M_CompanyInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_CompanyInformation.TableName, M_CompanyInformation.ColumnNameAll(i))
//        Next
//        ' M_WorkflowRoot
//        For i = 0 To M_WorkflowRootInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ColumnNameAll(i))
//        Next
//        sql.AppendFrom()
//        sql.AppendTable(GetType(M_UserInformation), M_UserInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_CompanyInformation), M_CompanyInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_SectionInformation), M_SectionInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_UserSectionRelationInformation), M_UserSectionRelationInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_WorkflowRootInformation), M_WorkflowRootInformation.TableName)
//        sql.AppendWhere()
//        ' 結合条件
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToCompanyId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.CompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToSectionId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.SectionId)
//        ' この結合条件にて、ワークフローに定義したToUserIdのみを対象とする。
//        sql.AppendAnd()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToUserId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.UserId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.CompanyId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.SectionId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.UserId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.AppendEqual()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.CompanyId)
//        ' M_Userの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_UserInformation.TableName, M_UserInformation.EffectiveDate, M_UserInformation.ExpireDate)
//        ' M_Companyの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_CompanyInformation.TableName, M_CompanyInformation.EffectiveDate, M_CompanyInformation.ExpireDate)
//        ' M_Sectionの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_SectionInformation.TableName, M_SectionInformation.EffectiveDate, M_SectionInformation.ExpireDate)
//        ' M_UserSectionRelationの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.EffectiveDate, M_UserSectionRelationInformation.ExpireDate)
//        ' 検索条件
//        Dim firstFlg As Boolean = True
//        For Each mWorkflowEntity As M_WorkflowRootEntity In mWorkflowRootEntityArray
//            If firstFlg Then
//                sql.AppendAnd()
//                sql.Append(" ( ")
//                firstFlg = False
//            Else
//                sql.AppendOr()
//            End If
//            sql.Append(" (")
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.SubSystemId)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowEntity.SubSystemId)
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ApprovalType)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowEntity.ApprovalType)
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromCompanyId)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowEntity.FromCompanyId)
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromSectionId)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowEntity.FromSectionId)
//            If _
//             Not mWorkflowEntity.FromUserId Is Nothing _
//            AndAlso _
//             Not mWorkflowEntity.FromUserId.Trim().Equals(String.Empty) _
//            Then
//                ' FromUserIdの指定がある場合は条件に追加
//                sql.AppendAnd()
//                sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.FromUserId)
//                sql.AppendEqual()
//                sql.AppendStrVal(mWorkflowEntity.FromUserId)
//            End If
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.RootNo)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowEntity.RootNo)
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ActionType)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowEntity.ActionType)
//            sql.Append(") ")
//        Next
//        If Not firstFlg Then
//            sql.Append(" ) ")
//        End If
//        sql.Append(" ORDER BY ")
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.Id)
//        sql.Append(" ASC ")

//        Return sql.ToString
//    End Function

//    ' QUO2-HB-B0017 対応 2012/06/18 luo start
//    ''' <summary>
//    ''' ワークフローに定義した遷移先に所属するユーザの情報を取得するSELECT文を作成します。マスタに指定されたユーザは除きます。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntityArray"> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型になります。</para>
//    ''' <para>遷移先の候補になります。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。SELECT文です。</returns>
//    ''' <remarks></remarks>
//    Private Function GetSqlToUsersFromMUserForReplyToACT( _
//     ByVal mWorkflowRootEntityArray As EntityList _
//    ) As String
//        Dim sql As SqlStringBuilder = New SqlStringBuilder ' SQL文作成クラス
//        Dim exclusizeUserIdArray As New List(Of String)	' ワークフロー定義済の除外ユーザのリスト
//        ' B-TSI1-3041
//        sql.AppendSelect()
//        ' M_User
//        sql.Append(" DISTINCT ")
//        For i = 0 To M_UserInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If

//            sql.AppendQualifiedColumn(M_UserInformation.TableName, M_UserInformation.ColumnNameAll(i))
//        Next
//        ' M_Section
//        For i = 0 To M_SectionInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_SectionInformation.TableName, M_SectionInformation.ColumnNameAll(i))
//        Next
//        ' M_Company
//        For i = 0 To M_CompanyInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_CompanyInformation.TableName, M_CompanyInformation.ColumnNameAll(i))
//        Next
//        ' M_WorkflowRoot
//        For i = 0 To M_WorkflowRootInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ColumnNameAll(i))
//        Next
//        sql.AppendFrom()
//        sql.AppendTable(GetType(M_UserInformation), M_UserInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_CompanyInformation), M_CompanyInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_SectionInformation), M_SectionInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_UserSectionRelationInformation), M_UserSectionRelationInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_WorkflowRootInformation), M_WorkflowRootInformation.TableName)
//        sql.AppendWhere()
//        ' 結合条件
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.CompanyId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.SectionId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.UserId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.AppendEqual()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.CompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.CompanyId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToCompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.SectionId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToSectionId)
//        ' M_Companyの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_CompanyInformation.TableName, M_CompanyInformation.EffectiveDate, M_CompanyInformation.ExpireDate)
//        ' M_Sectionの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_SectionInformation.TableName, M_SectionInformation.EffectiveDate, M_SectionInformation.ExpireDate)
//        ' 検索条件
//        Dim firstFlg As Boolean = True
//        For Each mWorkflowRootEntity As M_WorkflowRootEntity In mWorkflowRootEntityArray
//            If firstFlg Then
//                sql.AppendAnd()
//                sql.Append(" ( ")
//                firstFlg = False
//            Else
//                sql.AppendOr()
//            End If
//            sql.Append(" (")
//            sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.CompanyId)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowRootEntity.ToCompanyId)
//            sql.AppendAnd()
//            sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.SectionId)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowRootEntity.ToSectionId)
//            If _
//             Not mWorkflowRootEntity.ToUserId Is Nothing _
//            AndAlso _
//             Not mWorkflowRootEntity.ToUserId.Trim().Equals(String.Empty) _
//            Then
//                ' マスタにユーザの指定がある場合は検索から除外する(別メソッドで取得済のため)
//                'sql.AppendAnd()
//                'sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.UserId)
//                'sql.AppendNotEqual()
//                'sql.AppendStrVal(mWorkflowRootEntity.ToUserId)
//                exclusizeUserIdArray.Add(mWorkflowRootEntity.ToUserId)
//            End If
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.Id)
//            sql.AppendEqual()
//            sql.AppendStrVal(CStr(mWorkflowRootEntity.Id))
//            sql.Append(") ")
//        Next
//        If Not firstFlg Then
//            ' ワークフローマスタの条件が挿入された場合は閉じ括弧を追加する
//            sql.Append(" ) ")
//        End If
//        If exclusizeUserIdArray.Count > 0 Then
//            ' 除外ユーザがある場合
//            sql.Append(" AND ")
//            sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//            sql.Append(" NOT IN( ")
//            For i = 0 To exclusizeUserIdArray.Count - 1
//                If 0 < i Then
//                    ' 2件目以降はカンマ挿入
//                    sql.AppendComma()
//                End If
//                sql.AppendStrVal(exclusizeUserIdArray.Item(i))
//            Next
//            sql.Append(" ) ")
//        End If
//        sql.Append(" ORDER BY ")
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.Id)
//        sql.Append(" ASC ")

//        Return sql.ToString
//    End Function
//    ' QUO2-HB-B0017 対応 2012/06/18 luo end

//    ''' <summary>
//    ''' ワークフローに定義した遷移先に所属するユーザの情報を取得するSELECT文を作成します。マスタに指定されたユーザは除きます。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntityArray"> <see cref="EntityList">EntityList</see> 型です。
//    ''' <para>要素は <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型になります。</para>
//    ''' <para>遷移先の候補になります。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。SELECT文です。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlToUsersFromMUser( _
//     ByVal mWorkflowRootEntityArray As EntityList _
//    ) As String
//        Dim sql As SqlStringBuilder = New SqlStringBuilder ' SQL文作成クラス
//        Dim exclusizeUserIdArray As New List(Of String)	' ワークフロー定義済の除外ユーザのリスト
//        ' B-TSI1-3041
//        sql.AppendSelect()
//        ' M_User
//        sql.Append(" DISTINCT ")
//        For i = 0 To M_UserInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If

//            sql.AppendQualifiedColumn(M_UserInformation.TableName, M_UserInformation.ColumnNameAll(i))
//        Next
//        ' M_Section
//        For i = 0 To M_SectionInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_SectionInformation.TableName, M_SectionInformation.ColumnNameAll(i))
//        Next
//        ' M_Company
//        For i = 0 To M_CompanyInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_CompanyInformation.TableName, M_CompanyInformation.ColumnNameAll(i))
//        Next
//        ' M_WorkflowRoot
//        For i = 0 To M_WorkflowRootInformation.ColumnNameAll.Count - 1
//            sql.AppendComma()
//            sql.AppendQualifiedColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ColumnNameAll(i))
//        Next
//        sql.AppendFrom()
//        sql.AppendTable(GetType(M_UserInformation), M_UserInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_CompanyInformation), M_CompanyInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_SectionInformation), M_SectionInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_UserSectionRelationInformation), M_UserSectionRelationInformation.TableName)
//        sql.AppendComma()
//        sql.AppendTable(GetType(M_WorkflowRootInformation), M_WorkflowRootInformation.TableName)
//        sql.AppendWhere()
//        ' 結合条件
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.CompanyId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.SectionId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.UserId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        sql.AppendAnd()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.AppendEqual()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.CompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.CompanyId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToCompanyId)
//        sql.AppendAnd()
//        sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.SectionId)
//        sql.AppendEqual()
//        sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.ToSectionId)
//        ' M_Userの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_UserInformation.TableName, M_UserInformation.EffectiveDate, M_UserInformation.ExpireDate)
//        ' M_Companyの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_CompanyInformation.TableName, M_CompanyInformation.EffectiveDate, M_CompanyInformation.ExpireDate)
//        ' M_Sectionの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_SectionInformation.TableName, M_SectionInformation.EffectiveDate, M_SectionInformation.ExpireDate)
//        ' M_UserSectionRelationの有効期限判定(yyyymmdd形式で大小判定)
//        AddWhereExpire(sql, M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.EffectiveDate, M_UserSectionRelationInformation.ExpireDate)
//        ' 検索条件
//        Dim firstFlg As Boolean = True
//        For Each mWorkflowRootEntity As M_WorkflowRootEntity In mWorkflowRootEntityArray
//            If firstFlg Then
//                sql.AppendAnd()
//                sql.Append(" ( ")
//                firstFlg = False
//            Else
//                sql.AppendOr()
//            End If
//            sql.Append(" (")
//            sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.CompanyId)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowRootEntity.ToCompanyId)
//            sql.AppendAnd()
//            sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.SectionId)
//            sql.AppendEqual()
//            sql.AppendStrVal(mWorkflowRootEntity.ToSectionId)
//            If _
//             Not mWorkflowRootEntity.ToUserId Is Nothing _
//            AndAlso _
//             Not mWorkflowRootEntity.ToUserId.Trim().Equals(String.Empty) _
//            Then
//                ' マスタにユーザの指定がある場合は検索から除外する(別メソッドで取得済のため)
//                'sql.AppendAnd()
//                'sql.AppendColumn(M_UserSectionRelationInformation.TableName, M_UserSectionRelationInformation.UserId)
//                'sql.AppendNotEqual()
//                'sql.AppendStrVal(mWorkflowRootEntity.ToUserId)
//                exclusizeUserIdArray.Add(mWorkflowRootEntity.ToUserId)
//            End If
//            sql.AppendAnd()
//            sql.AppendColumn(M_WorkflowRootInformation.TableName, M_WorkflowRootInformation.Id)
//            sql.AppendEqual()
//            sql.AppendStrVal(CStr(mWorkflowRootEntity.Id))
//            sql.Append(") ")
//        Next
//        If Not firstFlg Then
//            ' ワークフローマスタの条件が挿入された場合は閉じ括弧を追加する
//            sql.Append(" ) ")
//        End If
//        If exclusizeUserIdArray.Count > 0 Then
//            ' 除外ユーザがある場合
//            sql.Append(" AND ")
//            sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//            sql.Append(" NOT IN( ")
//            For i = 0 To exclusizeUserIdArray.Count - 1
//                If 0 < i Then
//                    ' 2件目以降はカンマ挿入
//                    sql.AppendComma()
//                End If
//                sql.AppendStrVal(exclusizeUserIdArray.Item(i))
//            Next
//            sql.Append(" ) ")
//        End If
//        sql.Append(" ORDER BY ")
//        sql.AppendColumn(M_UserInformation.TableName, M_UserInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_CompanyInformation.TableName, M_CompanyInformation.Id)
//        sql.Append(" ASC ")
//        sql.AppendComma()
//        sql.AppendColumn(M_SectionInformation.TableName, M_SectionInformation.Id)
//        sql.Append(" ASC ")

//        Return sql.ToString
//    End Function

//    ''' <summary>
//    ''' 最新のワークフローを取得するSELECT文を作成します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件ののサブシステムを表す値です。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のワークフロー制御対象となる業務キーを表す値です。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。SELECT文です。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlTCurrentWorkflow( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal transactionSurrogateKey As String _
//    ) As String
//        Dim sql As New SqlStringBuilder	' SQL文作成クラス

//        sql.AppendSelect()
//        For i = 0 To T_CurrentWorkflowInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If

//            sql.AppendQualifiedColumn(T_CurrentWorkflowInformation.TableName, T_CurrentWorkflowInformation.ColumnNameAll(i))
//        Next
//        sql.AppendComma()
//        sql.AppendQualifiedColumn(T_CurrentWorkflowInformation.TableName, T_CurrentWorkflowInformation.LastModifiedDate)
//        sql.AppendFrom()
//        sql.AppendTable(GetType(T_CurrentWorkflowInformation), T_CurrentWorkflowInformation.TableName)
//        sql.AppendWhere()
//        sql.AppendColumn(T_CurrentWorkflowInformation.TableName, T_CurrentWorkflowInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendStrVal(subsystemId)
//        sql.AppendAnd()
//        sql.AppendColumn(T_CurrentWorkflowInformation.TableName, T_CurrentWorkflowInformation.ApprovalType)
//        sql.AppendEqual()
//        sql.AppendStrVal(approvalType)
//        sql.AppendAnd()
//        sql.AppendColumn(T_CurrentWorkflowInformation.TableName, T_CurrentWorkflowInformation.TransactionSurrogateKey)
//        sql.AppendEqual()
//        sql.AppendStrVal(transactionSurrogateKey)

//        Return sql.ToString
//    End Function

//    ''' <summary>
//    ''' ワークフローのINSERT文を作成します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntity"> <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型です。
//    ''' <para>次のワークフロー情報です。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>ワークフロー制御対象となる業務キーを表す値です。</para>
//    ''' </param>
//    ''' <param name="historyNo"> <see cref="Integer">Integer</see> 型です。
//    ''' <para>履歴番号です。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toUserId"> <see cref="String">String</see> 型です。
//    ''' <para>遷移先ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="exclusiveKey"> <see cref="Long">Long</see> 型です。
//    ''' <para>排他制御に用いる値です。</para>
//    ''' </param>
//    ''' <param name="replyDeadLine"> <see cref="String">String</see> 型です。
//    ''' <para>作業完了要求日の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText1"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)１の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText2"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)２の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText3"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)３の値です。</para>
//    ''' </param>
//    ''' <param name="optionalDate"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(日付)の値です。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。INSERT文になります。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlInsertTCurrentWorkflow( _
//     ByVal mWorkflowRootEntity As M_WorkflowRootEntity, _
//     ByVal transactionSurrogateKey As String, _
//     ByVal historyNo As Integer, _
//     ByVal fromUserId As String, _
//     ByVal toUserId As String, _
//     ByVal exclusiveKey As Long, _
//     ByVal replyDeadLine As String, _
//     ByVal optionalText1 As String, _
//     ByVal optionalText2 As String, _
//     ByVal optionalText3 As String, _
//     ByVal optionalDate As String _
//    ) As String
//        Dim sql As New SqlStringBuilder	' SQL文作成クラス

//        sql.Append("INSERT INTO ")
//        sql.Append(T_CurrentWorkflowInformation.TableName)
//        sql.Append(" ( ")
//        For i = 0 To T_CurrentWorkflowInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If

//            sql.AppendColumn(T_CurrentWorkflowInformation.TableName, T_CurrentWorkflowInformation.ColumnNameAll(i))
//        Next
//        sql.Append(" ) VALUES ( ")
//        sql.AppendStrVal(mWorkflowRootEntity.SubSystemId) ' サブシステムID
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ApprovalType) ' 承認タイプ
//        sql.AppendComma()
//        sql.AppendStrVal(transactionSurrogateKey) ' 業務サロゲートキー
//        sql.AppendComma()
//        sql.AppendStrVal(CStr(historyNo + 1)) ' 履歴番号
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.FromCompanyId)	' 遷移元会社ID
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.FromSectionId)	' 遷移元組織ID
//        sql.AppendComma()
//        If fromUserId Is Nothing OrElse fromUserId.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            fromUserId = Nothing
//        End If
//        sql.AppendStrVal(fromUserId) ' 遷移元作業者
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.RootNo) ' 承認ルート番号
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ActionType) ' 処理内容
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ToCompanyId) ' 遷移先会社ID
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ToSectionId) ' 遷移先組織ID
//        sql.AppendComma()
//        If toUserId Is Nothing OrElse toUserId.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            toUserId = Nothing
//        End If
//        sql.AppendStrVal(toUserId) ' 遷移先作業者
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ToStatus) ' ステータス
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.NextRootNo) ' 次回承認ルート
//        sql.AppendComma()
//        If exclusiveKey = 0 Then
//            ' 新規なので1を設定
//            exclusiveKey = 1
//        End If
//        sql.AppendStrVal(CStr(exclusiveKey))	' 排他キー
//        sql.AppendComma()
//        If replyDeadLine Is Nothing OrElse replyDeadLine.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            replyDeadLine = Nothing
//        End If
//        sql.AppendStrVal(replyDeadLine)	' 要求日
//        sql.AppendComma()
//        If optionalText1 Is Nothing OrElse optionalText1.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            optionalText1 = Nothing
//        End If
//        sql.AppendStrVal(optionalText1)	' 業務用予備項目(文字列)１
//        sql.AppendComma()
//        If optionalText2 Is Nothing OrElse optionalText2.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            optionalText2 = Nothing
//        End If
//        sql.AppendStrVal(optionalText2)	' 業務用予備項目(文字列)２
//        sql.AppendComma()
//        If optionalText3 Is Nothing OrElse optionalText3.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            optionalText3 = Nothing
//        End If
//        sql.AppendStrVal(optionalText3)	' 業務用予備項目(文字列)３
//        sql.AppendComma()
//        If optionalDate Is Nothing OrElse optionalDate.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            optionalDate = Nothing
//        End If
//        sql.AppendStrVal(optionalDate) ' 業務用予備項目(日付)
//        sql.Append(" ) ")

//        Return sql.ToString
//    End Function

//    ''' <summary>
//    ''' ワークフロー履歴のINSERT文を作成します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntity"> <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型です。
//    ''' <para>次のワークフロー情報です。</para>
//    ''' </param>
//    ''' <param name="tCurrentWorkflowEntity"> <see cref="T_CurrentWorkflowEntity">T_CurrentWorkflowEntity</see> 型です。
//    ''' <para>経過時間の算出等に使用します。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>ワークフロー制御対象となる業務キーを表す値です。</para></param>
//    ''' <param name="historyNo"> <see cref="Integer">Integer</see> 型です。
//    ''' <para>履歴番号です。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toUserId"> <see cref="String">String</see> 型です。
//    ''' <para>遷移先ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="exclusiveKey"> <see cref="Long">Long</see> 型です。
//    ''' <para>排他制御に用いる値です。</para>
//    ''' </param>
//    ''' <param name="replyDeadLine"> <see cref="String">String</see> 型です。
//    ''' <para>作業完了要求日の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText1"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)１の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText2"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)２の値です。</para>
//    ''' </param>
//    ''' <param name="optionalDate"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(日付)の値です。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。INSERT文です。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlInsertTWorkflowHistory( _
//     ByVal mWorkflowRootEntity As M_WorkflowRootEntity, _
//     ByVal tCurrentWorkflowEntity As T_CurrentWorkflowEntity, _
//     ByVal transactionSurrogateKey As String, _
//     ByVal historyNo As Integer, _
//     ByVal fromUserId As String, _
//     ByVal toUserId As String, _
//     ByVal exclusiveKey As Long, _
//     ByVal replyDeadLine As String, _
//     ByVal optionalText1 As String, _
//     ByVal optionalText2 As String, _
//     ByVal optionalText3 As String, _
//     ByVal optionalDate As String _
//    ) As String
//        Dim sql As New SqlStringBuilder	' SQL文作成クラス

//        sql.Append("INSERT INTO ")
//        sql.Append(T_WorkflowHistoryInformation.TableName)
//        sql.Append(" ( ")
//        For i = 0 To T_WorkflowHistoryInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If

//            sql.AppendColumn(T_WorkflowHistoryInformation.TableName, T_WorkflowHistoryInformation.ColumnNameAll(i))
//        Next
//        sql.Append(" ) VALUES ( ")
//        sql.AppendStrVal(mWorkflowRootEntity.SubSystemId) ' サブシステムID
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ApprovalType) ' 承認タイプ
//        sql.AppendComma()
//        sql.AppendStrVal(transactionSurrogateKey) ' 業務サロゲートキー
//        sql.AppendComma()
//        sql.AppendStrVal(CStr(historyNo + 1)) ' 履歴番号
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.FromCompanyId)	' 遷移元会社ID
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.FromSectionId)	' 遷移元組織ID
//        sql.AppendComma()
//        If fromUserId Is Nothing OrElse fromUserId.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            fromUserId = Nothing
//        End If
//        sql.AppendStrVal(fromUserId) ' 遷移元作業者
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.RootNo) ' 承認ルート番号
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ActionType) ' 処理内容
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ToCompanyId) ' 遷移先会社ID
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ToSectionId) ' 遷移先組織ID
//        sql.AppendComma()
//        If toUserId Is Nothing OrElse toUserId.Equals(String.Empty) Then
//            toUserId = Nothing
//        End If
//        sql.AppendStrVal(toUserId) ' 遷移先作業者
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.ToStatus) ' ステータス
//        sql.AppendComma()
//        sql.AppendStrVal(mWorkflowRootEntity.NextRootNo) ' 次回承認ルート
//        If Not tCurrentWorkflowEntity Is Nothing Then
//            ' カレントがある場合は経過時間を算出
//            sql.AppendComma()
//            sql.Append(" ( ")
//            sql.AppendEscapedStr(GetElapsedTimeQuery(tCurrentWorkflowEntity.LastModifiedDate))	' 経過時間
//            sql.Append(" ) ")
//        Else
//            sql.AppendComma()
//            sql.Append(" NULL ")
//        End If
//        sql.AppendComma()
//        If replyDeadLine Is Nothing OrElse replyDeadLine.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            replyDeadLine = Nothing
//        End If
//        sql.AppendStrVal(replyDeadLine)	' 要求日
//        sql.AppendComma()
//        If optionalText1 Is Nothing OrElse optionalText1.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            optionalText1 = Nothing
//        End If
//        sql.AppendStrVal(optionalText1)	' 業務用予備項目(文字列)１
//        sql.AppendComma()
//        If optionalText2 Is Nothing OrElse optionalText2.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            optionalText2 = Nothing
//        End If
//        sql.AppendStrVal(optionalText2)	' 業務用予備項目(文字列)２
//        sql.AppendComma()
//        If optionalText3 Is Nothing OrElse optionalText3.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            optionalText3 = Nothing
//        End If
//        sql.AppendStrVal(optionalText3)	' 業務用予備項目(文字列)３
//        sql.AppendComma()
//        If optionalDate Is Nothing OrElse optionalDate.Equals(String.Empty) Then
//            ' 無効値の場合は変数を初期化
//            optionalDate = Nothing
//        End If
//        sql.AppendStrVal(optionalDate) ' 業務用予備項目(日付)
//        sql.Append(" ) ")

//        Return sql.ToString
//    End Function

//    ''' <summary>
//    ''' カレントワークフローのUPDATE文を作成します。
//    ''' </summary>
//    ''' <param name="mWorkflowRootEntity"> <see cref="M_WorkflowRootEntity">M_WorkflowRootEntity</see> 型です。
//    ''' <para>次のワークフロー情報です。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>ワークフロー制御対象となる業務キーを表す値です。</para>
//    ''' </param>
//    ''' <param name="historyNo"> <see cref="Integer">Integer</see> 型です。
//    ''' <para>履歴番号です。</para>
//    ''' </param>
//    ''' <param name="fromUserId"> <see cref="String">String</see> 型です。
//    ''' <para>遷移元ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="toUserId"> <see cref="String">String</see> 型です。
//    ''' <para>遷移先ユーザを表すシーケンスIDです。</para>
//    ''' </param>
//    ''' <param name="exclusiveKey"> <see cref="Long">Long</see> 型です。
//    ''' <para>排他制御に用いる値です。</para>
//    ''' </param>
//    ''' <param name="replyDeadLine"> <see cref="String">String</see> 型です。
//    ''' <para>作業完了要求日の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText1"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(文字列)１の値です。</para>
//    ''' </param>
//    ''' <param name="optionalText2"> <see cref="String">String</see> 型です。
//    ''' <para>x予備項目(文字列)２の値です。</para>
//    ''' </param>
//    ''' <param name="optionalDate"> <see cref="String">String</see> 型です。
//    ''' <para>予備項目(日付)の値です。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。UPDATE文です。</returns>
//    ''' <remarks>排他キーはDBAccessor内で付与されます。</remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlUpdateTCurrentWorkflow( _
//     ByVal mWorkflowRootEntity As M_WorkflowRootEntity, _
//     ByVal transactionSurrogateKey As String, _
//     ByVal historyNo As Integer, _
//     ByVal fromUserId As String, _
//     ByVal toUserId As String, _
//     ByVal exclusiveKey As Long, _
//     ByVal replyDeadLine As String, _
//     ByVal optionalText1 As String, _
//     ByVal optionalText2 As String, _
//     ByVal optionalText3 As String, _
//     ByVal optionalDate As String _
//    ) As String
//        Dim sql As New SqlStringBuilder	' SQL文作成クラス

//        sql.Append("UPDATE ")
//        sql.AppendTable(GetType(T_CurrentWorkflowInformation), Nothing)
//        sql.Append(" SET ")
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.HistoryNo)
//        sql.AppendEqual()
//        sql.AppendStrVal(CStr(historyNo + 1)) ' 履歴番号
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.FromCompanyId)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.FromCompanyId)	' 遷移元会社ID
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.FromSectionId)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.FromSectionId)	' 遷移元組織ID
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.FromUserId)
//        sql.AppendEqual()
//        sql.AppendStrVal(fromUserId) ' 遷移元作業者
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.RootNo)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.RootNo) ' 承認ルート番号
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.ActionType)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.ActionType) ' 処理内容
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.ToCompanyId)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.ToCompanyId) ' 遷移先会社ID
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.ToSectionId)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.ToSectionId) ' 遷移先組織ID
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.ToUserId)
//        sql.AppendEqual()
//        sql.AppendStrVal(toUserId) ' 遷移先作業者
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.ToStatus)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.ToStatus) ' ステータス
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.NextRootNo)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.NextRootNo) ' 次回承認ルート
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.ReplyDeadline)
//        sql.AppendEqual()
//        sql.AppendStrVal(replyDeadLine)	' 要求日
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.OptionalText1)
//        sql.AppendEqual()
//        sql.AppendStrVal(optionalText1)	' 業務用予備項目(文字列)１
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.OptionalText2)
//        sql.AppendEqual()
//        sql.AppendStrVal(optionalText2)	' 業務用予備項目(文字列)２
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.OptionalText3)
//        sql.AppendEqual()
//        sql.AppendStrVal(optionalText3)	' 業務用予備項目(文字列)３
//        sql.AppendComma()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.OptionalDate)
//        sql.AppendEqual()
//        sql.AppendStrVal(optionalDate) ' 業務用予備項目(日付)
//        sql.AppendWhere()
//        ' 検索条件
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.SubSystemId) ' サブシステムID
//        sql.AppendAnd()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.ApprovalType)
//        sql.AppendEqual()
//        sql.AppendStrVal(mWorkflowRootEntity.ApprovalType) ' 承認タイプ
//        sql.AppendAnd()
//        sql.AppendColumn(Nothing, T_CurrentWorkflowInformation.TransactionSurrogateKey)
//        sql.AppendEqual()
//        sql.AppendStrVal(transactionSurrogateKey) ' 業務サロゲートキー

//        Return sql.ToString
//    End Function

//    ''' <summary>
//    ''' 経過時間を算出するサブクエリを作成します。
//    ''' </summary>
//    ''' <param name="prevLastModifiedDate"> <see cref="String">String</see> 型です。
//    ''' <para>直前のワークフローの最終更新日時になります。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。ElapsedTimeを算出するサブクエリを返します。</returns>
//    ''' <remarks>
//    ''' <para>分単位で取得した <c>DateTime</c> について(秒以下は切り捨て)、</para>
//    ''' <para>分の一桁目を四捨五入する。</para>
//    ''' </remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetElapsedTimeQuery( _
//     ByVal prevLastModifiedDate As String _
//    ) As String
//        Dim partialSql As New StringBuilder	' SQL文作成クラス

//        'select 
//        '	case
//        '	when 
//        '	 DATEDIFF(
//        '		MI,
//        '		CONVERT(datetime,'prevLastModifiedDate'),
//        '		CONVERT(datetime,sysutcdatetime())
//        '	 )%10<5 
//        '	then 
//        '	 (
//        '		DATEDIFF(
//        '			MI,
//        '			CONVERT(datetime,'prevLastModifiedDate'),
//        '			CONVERT(datetime,sysutcdatetime())
//        '		)
//        '	 )/10*10
//        '	when 
//        '	 DATEDIFF(
//        '		MI,
//        '		CONVERT(datetime,'prevLastModifiedDate'),
//        '		CONVERT(datetime,sysutcdatetime())
//        '	 )%10>=5 
//        '	then 
//        '	 (
//        '	 	(
//        '		 DATEDIFF(
//        '			MI,
//        '			CONVERT(datetime,'prevLastModifiedDate'),
//        '			CONVERT(datetime,sysutcdatetime())
//        '		 )/10
//        '		)+1
//        '	 )*10
//        partialSql.Append("SELECT ")
//        partialSql.Append("CASE ")
//        partialSql.Append("WHEN ")
//        partialSql.Append("DATEDIFF(MI,CONVERT(DATETIME,'") _
//         .Append(prevLastModifiedDate) _
//         .Append("'),CONVERT(DATETIME,") _
//         .Append(SqlStringBuilder.GET_UTC_DATE).Append("))%10 < 5 ")
//        partialSql.Append("THEN ")
//        partialSql.Append("(DATEDIFF(MI,CONVERT(DATETIME,'") _
//         .Append(prevLastModifiedDate) _
//         .Append("'),CONVERT(DATETIME,") _
//         .Append(SqlStringBuilder.GET_UTC_DATE).Append(")))/10*10 ")
//        partialSql.Append("WHEN ")
//        partialSql.Append("DATEDIFF(MI,CONVERT(DATETIME,'") _
//         .Append(prevLastModifiedDate) _
//         .Append("'),CONVERT(DATETIME,") _
//         .Append(SqlStringBuilder.GET_UTC_DATE) _
//         .Append("))%10 >= 5 ")
//        partialSql.Append("THEN ")
//        partialSql.Append("((DATEDIFF(MI,CONVERT(DATETIME,'") _
//         .Append(prevLastModifiedDate) _
//         .Append("'),CONVERT(DATETIME,") _
//         .Append(SqlStringBuilder.GET_UTC_DATE) _
//         .Append("))/10)+1)*10")
//        partialSql.Append(" END ")

//        Return partialSql.ToString()
//    End Function

//    ''' <summary>
//    ''' 最新のワークフロー履歴を取得するSELECT文を作成します。
//    ''' </summary>
//    ''' <param name="subsystemId"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のサブシステムを表す値です。</para>
//    ''' </param>
//    ''' <param name="approvalType"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件の承認区分です。</para>
//    ''' </param>
//    ''' <param name="transactionSurrogateKey"> <see cref="String">String</see> 型です。
//    ''' <para>検索条件のワークフロー制御対象となる業務キーを表す値です。</para>
//    ''' </param>
//    ''' <returns> <see cref="String">String</see> 型です。SELECT文です。</returns>
//    ''' <remarks></remarks>
//    ''' 作成日付：2011/11/28
//    ''' 作成者：HISOL K.Sakai
//    ''' 更新者：
//    Private Function GetSqlLastTWorkflowHistory( _
//     ByVal subsystemId As String, _
//     ByVal approvalType As String, _
//     ByVal transactionSurrogateKey As String _
//    ) As String
//        Dim sql As New SqlStringBuilder	' SQL文作成クラス

//        sql.AppendSelect()
//        For i = 0 To T_WorkflowHistoryInformation.ColumnNameAll.Count - 1
//            If 0 < i Then
//                ' 2件目以降はカンマ挿入
//                sql.AppendComma()
//            End If

//            sql.AppendQualifiedColumn(T_WorkflowHistoryInformation.TableName, T_WorkflowHistoryInformation.ColumnNameAll(i))
//        Next
//        sql.AppendFrom()
//        sql.AppendTable(GetType(T_WorkflowHistoryInformation), T_WorkflowHistoryInformation.TableName)
//        sql.AppendWhere()
//        sql.AppendColumn(T_WorkflowHistoryInformation.TableName, T_WorkflowHistoryInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendStrVal(subsystemId)
//        sql.AppendAnd()
//        sql.AppendColumn(T_WorkflowHistoryInformation.TableName, T_WorkflowHistoryInformation.ApprovalType)
//        sql.AppendEqual()
//        sql.AppendStrVal(approvalType)
//        sql.AppendAnd()
//        sql.AppendColumn(T_WorkflowHistoryInformation.TableName, T_WorkflowHistoryInformation.TransactionSurrogateKey)
//        sql.AppendEqual()
//        sql.AppendStrVal(transactionSurrogateKey)
//        sql.AppendAnd()
//        sql.AppendColumn(T_WorkflowHistoryInformation.TableName, T_WorkflowHistoryInformation.HistoryNo)
//        sql.AppendEqual()
//        sql.Append(" ( ")
//        sql.AppendSelect()
//        sql.Append(" MAX( ")
//        sql.Append(T_WorkflowHistoryInformation.HistoryNo)
//        sql.Append(") ")
//        sql.AppendFrom()
//        sql.Append(T_WorkflowHistoryInformation.TableName)
//        sql.AppendWhere()
//        sql.Append(T_WorkflowHistoryInformation.SubSystemId)
//        sql.AppendEqual()
//        sql.AppendStrVal(subsystemId)
//        sql.AppendAnd()
//        sql.Append(T_WorkflowHistoryInformation.ApprovalType)
//        sql.AppendEqual()
//        sql.AppendStrVal(approvalType)
//        sql.AppendAnd()
//        sql.Append(T_WorkflowHistoryInformation.TransactionSurrogateKey)
//        sql.AppendEqual()
//        sql.AppendStrVal(transactionSurrogateKey)
//        sql.Append(" ) ")

//        Return sql.ToString
//    End Function

//#End Region
//#End Region
//End Class
