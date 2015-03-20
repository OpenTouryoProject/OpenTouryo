//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

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
//*  2015/03/20  Sai               Modified method 'GetProductData()' return type to Json string 
//**********************************************************************************
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IJSONService" in both code and config file together.
[ServiceContract(SessionMode = SessionMode.Allowed)]
public interface IJSONService
{
    [OperationContract]
    [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetProductData")]
    string GetProductData(int startIndex, int lastindex);
}
