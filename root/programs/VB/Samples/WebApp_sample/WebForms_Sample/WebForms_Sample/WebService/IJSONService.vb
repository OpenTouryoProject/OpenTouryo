
'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：IJSONService
'* クラス日本語名  ：IJSONService
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/09/11  Supragyan         Created IJSONService interface to invoking GetProductData
'*  2015/03/20  Supragyan         Modified method 'GetProductData()' return type to Message 
'**********************************************************************************
' System
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.ServiceModel.Channels

''' <summary>
''' IJSONService interface to invoking GetProductData method
''' </summary>
<ServiceContract(SessionMode:=SessionMode.Allowed)>
Public Interface IJSONService
    <OperationContract()>
    <WebGet(ResponseFormat:=WebMessageFormat.Json, UriTemplate:="GetProductData")>
    Function GetProductData() As Message
End Interface


