//**********************************************************************************
//* フレームワーク・テスト API
//**********************************************************************************

//  API画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：JSONService
//* クラス日本語名  ：JSONService
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/03/05  Supragyan         Created IJSONService interface for invoking Product table data.
//*  2015/03/20  Supragyan         Modified method 'GetProductData()' return type to Message 
//**********************************************************************************

using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;

/// <summary>ProjectX_sample</summary>
namespace ProjectX_sample
{
    /// <summary>IJSONService</summary>
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IJSONService" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IJSONService
    {
        /// <summary>GetProductData()</summary>
        /// <returns>Message</returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetProductData")]
        Message GetProductData();
    }
}