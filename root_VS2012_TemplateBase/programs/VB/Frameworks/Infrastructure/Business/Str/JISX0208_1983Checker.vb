'**********************************************************************************
'* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
'
'  
' 
'  
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License. 
' You may obtain a copy of the License at
'
' http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

'**********************************************************************************
'* クラス名        ：JISX0208_1983Checker
'* クラス日本語名  ：JIS X 0208-1983文字コード範囲チェック・クラス
'* 　　　　　　　　　・01～08区：記号、英数字、かな
'* 　　　　　　　　　・16～47区：JIS第1水準漢字
'* 　　　　　　　　　・48～84区：JIS第2水準漢字
'*   　　　　　　　　※JIS X 0208-1990で追加された「凜[7425]」「熙[7426]」は含まれない
'* 　　　　　　　　　※NEC機種依存文字、NECのIBM拡張文字、IBM拡張文字は含まれない
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2012/06/20  西野  大介        新規作成
'**********************************************************************************

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' System.Web
Imports System.Web
Imports System.Web.Security

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.Str
	''' <summary>
	''' JIS X 0208-1983文字コード範囲チェック・クラス
	''' ・01～08区：記号、英数字、かな
	''' ・16～47区：JIS第1水準漢字
	''' ・48～84区：JIS第2水準漢字
	''' ※JIS X 0208-1990で追加された「凜[7425]」「熙[7426]」は含まれない
	''' ※NEC機種依存文字、NECのIBM拡張文字、IBM拡張文字は含まれない
	''' </summary>
	''' <remarks>同じアルゴリズムで他の文字コード範囲チェック・クラスも開発できる</remarks>
	Public Class JISX0208_1983Checker
		''' <summary>CheckCharCodeのリスト（許可されるコード範囲のリスト）</summary>
		Private Shared CCCList As List(Of CheckCharCode)

		''' <summary>シングルトン</summary>
		Private Shared _JISX0208_1983Checker As New JISX0208_1983Checker()

		''' <summary>コンストラクタ</summary>
		Public Sub New()
			' エンコーディングと、コード範囲のリストを定義する。
			JISX0208_1983Checker.CCCList = New List(Of CheckCharCode)()
			Dim sjisEncoding As Encoding = Encoding.GetEncoding(CustomEncode.shift_jis)

			' 文字コード表 シフトJIS(Shift_JIS)
			' http://charset.7jp.net/sjis.html

			' NUL文字
			' DEL文字
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode(ControlChars.NullChar.ToString(), ChrW(127).ToString(), sjisEncoding))

			' 半角カナ
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("｡", "ﾟ", sjisEncoding))

			' 以下、全角の範囲
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("　", "〓", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("∈", "∩", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("∧", "∃", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("∠", "∬", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("Å", "¶", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("◯", "◯", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("０", "９", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("Ａ", "Ｚ", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("ａ", "ｚ", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("ぁ", "ん", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("ァ", "ヶ", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("Α", "Ω", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("α", "ω", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("А", "Я", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("а", "я", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("─", "╂", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("亜", "腕", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("弌", "滌", sjisEncoding))
			JISX0208_1983Checker.CCCList.Add(New CheckCharCode("漾", "瑤", sjisEncoding))
		End Sub

		''' <summary>範囲チェックする</summary>
		''' <param name="input">チェック対象文字列</param>
		''' <param name="index">
		''' エラー文字のインデックス
		''' 戻り値 = falseでindex = -1の場合は、SJISでない場合を表す。
		''' </param>
		''' <param name="ch">
		''' エラーの文字
		''' </param>
		''' <returns>
		''' true：範囲内
		''' false：範囲外
		''' </returns>
		''' <remarks>
		''' 空文字列が指定された場合は、trueが返ります。
		''' </remarks>
		Public Shared Function IsJISX0208_1983(input As String, ByRef index As Integer, ByRef ch As String) As Boolean
			' trueの場合
			index = -1
			ch = ""

            '/ 改行コードを除去
			'input = input.Replace("\r", "");
			'input = input.Replace("\n", "");

			If input.Length = 0 Then
				' 空文字列が指定された場合は、true
				Return True
			Else
				' SJISチェック
				If StringChecker.IsShift_Jis(input) Then
					' SJISである。
					For i As Integer = 0 To input.Length - 1
						' 当該文字は、

						' チェック対象文字・チェック結果
						Dim result As Boolean = False
						Dim tempChar As String = input(i).ToString()

						For Each ccc As CheckCharCode In JISX0208_1983Checker.CCCList
							' 当該範囲の、
                            If ccc.IsInRange(tempChar) Then
                                ' 範囲内
                                result = True
                                ' 範囲外
                            Else
                            End If
						Next

						' 当該文字は、全範囲外
						If Not result Then
							index = i
							ch = tempChar
							Return False
						End If
					Next

					' 全文字、範囲内
					Return True
				Else
					' SJISでない。
					Return False
				End If
			End If
		End Function
	End Class
End Namespace
