﻿Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes

Imports System.IO
Imports System.Threading
Imports System.Diagnostics

Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Framework.RichClient.Asynchronous

''' <summary>
''' Window1.xaml の相互作用ロジック
''' </summary>
Partial Public Class Window1
#Region "メンバ変数"

    ''' <summary>名前付きパイプ・サーバ名</summary>
    Private NPS As String = Nothing
    ''' <summary>名前付きパイプ・クライアント</summary>
    Private NPCS As String() = Nothing

    ''' <summary>登録エントリ（Thread）</summary>
    Private AeeTh As AsyncEventEntry = Nothing
    ''' <summary>登録エントリ（ThreadPool）</summary>
    Private AeePl As AsyncEventEntry = Nothing
    ''' <summary>登録エントリ（WinForm）</summary>
    Private AeeWin As AsyncEventEntry = Nothing
    ''' <summary>登録エントリ（WPF）</summary>
    Private AeeWPF As AsyncEventEntry = Nothing

#End Region

#Region "開始・終了処理"

#Region "開始処理"

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()

        ' サーバを起動
        Dim args As String() = Environment.CommandLine.Split("/"c)
        args = args(1).Trim().Split(","c)

        ' this.NPS
        Me.NPS = args(0).Trim()
        Me.Title = Me.NPS

        ' this.NPCS
        Me.NPCS = New String(args.Length - 2) {}
        For i As Integer = 1 To args.Length - 1
            Me.NPCS(i - 1) = args(i).Trim()
        Next

        ' 初期化

        ' 初めが自分の名称、

        '/ ２つ目からが相手の名称
        'MessageBox.Show(
        '    "this.NPS:" + this.NPS
        '    + "\r\nthis.NPCS:" + string.Join(",", this.NPCS),
        '    "コマンドラインのチェック");

        AsyncEventFx.Init(Me.NPS, Me.NPCS, 3000)
    End Sub

    ''' <summary>ロード</summary>
    Private Sub Window_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 登録エントリ

        ' スレッド
        Me.AeeTh = New AsyncEventEntry(AsyncEventEnum.EventClass.Thread, "Thread", Nothing, New ParameterizedThreadStart(AddressOf Me.ParameterizedThreadStartDgt))

        AsyncEventFx.RegisterAsyncEvent(Me.AeeTh)

        ' ---

        ' スレッド プール
        Me.AeePl = New AsyncEventEntry(AsyncEventEnum.EventClass.ThreadPool, "ThreadPool", Nothing, New WaitCallback(AddressOf Me.WaitCallbackDgt))

        AsyncEventFx.RegisterAsyncEvent(Me.AeePl)

        ' ---

        ' WinForm
        Me.AeeWin = New AsyncEventEntry(AsyncEventEnum.EventClass.WinForm, "WinForm", Me, New AsyncEventFx.SetResultDelegate(AddressOf Me.SetResultDgt))

        AsyncEventFx.RegisterAsyncEvent(Me.AeeWin)

        ' ---

        ' WPF
        Me.AeeWPF = New AsyncEventEntry(AsyncEventEnum.EventClass.WPF, "WPF", Me, New AsyncEventFx.SetResultDelegate(AddressOf Me.SetResultDgt))

        AsyncEventFx.RegisterAsyncEvent(Me.AeeWPF)
    End Sub

#End Region

#Region "終了処理"

    ''' <summary>アンロード</summary>
    Private Sub Window_Unloaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 終了
        AsyncEventFx.Final()
    End Sub

    ' ↑↓どっち？？

    ''' <summary>クローズ</summary>
    Private Sub Window_Closed(ByVal sender As Object, ByVal e As EventArgs)
        ' 終了
        AsyncEventFx.Final()
    End Sub

#End Region

#End Region

#Region "各種デリゲード"

    ''' <summary>デリゲード</summary>
    Private Sub ParameterizedThreadStartDgt(ByVal obj As Object)
        Dim param As Object() = DirectCast(obj, Object())
        Dim aeh As AsyncEventHeader = DirectCast(param(0), AsyncEventHeader)
        Dim msg As String = DirectCast(BinarySerialize.BytesToObject(DirectCast(param(1), Byte())), String)

        ' ファイルにテキストを書き出し。
        Using sw As New StreamWriter(Me.NPS & "_test_pts.txt", True)
            sw.WriteLine(vbCr & vbLf & Me.NPS & " - " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "WpfApplication.Window1.ParameterizedThreadStartDgtメソッドが呼ばれた。" & vbCr & vbLf & "DstEventClass:" & aeh.DstEventClass.ToString() & vbCr & vbLf & "DstFuncID:" & (New String(aeh.DstFuncID)).Trim() & vbCr & vbLf & "SrcEventClass:" & aeh.SrcEventClass.ToString() & vbCr & vbLf & "SrcFuncID:" & (New String(aeh.SrcFuncID)).Trim() & vbCr & vbLf & "SrcPipeName:" & (New String(aeh.SrcPipeName)).Trim() & vbCr & vbLf & "メッセージ:" & msg, Me.NPS)
        End Using
    End Sub

    ''' <summary>デリゲード</summary>
    Private Sub WaitCallbackDgt(ByVal state As Object)
        Dim param As Object() = DirectCast(state, Object())
        Dim aeh As AsyncEventHeader = DirectCast(param(0), AsyncEventHeader)
        Dim msg As String = DirectCast(BinarySerialize.BytesToObject(DirectCast(param(1), Byte())), String)

        ' ファイルにテキストを書き出し。
        Using sw As New StreamWriter(Me.NPS & "_test_tpl.txt", True)
            sw.WriteLine(vbCr & vbLf & Me.NPS & " - " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "WpfApplication.Window1.WaitCallbackDgtメソッドが呼ばれた。" & vbCr & vbLf & "DstEventClass:" & aeh.DstEventClass.ToString() & vbCr & vbLf & "DstFuncID:" & (New String(aeh.DstFuncID)).Trim() & vbCr & vbLf & "SrcEventClass:" & aeh.SrcEventClass.ToString() & vbCr & vbLf & "SrcFuncID:" & (New String(aeh.SrcFuncID)).Trim() & vbCr & vbLf & "SrcPipeName:" & (New String(aeh.SrcPipeName)).Trim() & vbCr & vbLf & "メッセージ:" & msg, Me.NPS)
        End Using
    End Sub

    ''' <summary>デリゲード</summary>
    Private Sub SetResultDgt(ByVal result As Object)
        Dim param As Object() = DirectCast(result, Object())
        Dim aeh As AsyncEventHeader = DirectCast(param(0), AsyncEventHeader)
        Dim msg As String = DirectCast(BinarySerialize.BytesToObject(DirectCast(param(1), Byte())), String)

        MessageBox.Show(Me, Me.NPS & " - " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "WpfApplication.Window1.SetResultDgtメソッドが呼ばれた。" & vbCr & vbLf & "DstEventClass:" & aeh.DstEventClass.ToString() & vbCr & vbLf & "DstFuncID:" & (New String(aeh.DstFuncID)).Trim() & vbCr & vbLf & "SrcEventClass:" & aeh.SrcEventClass.ToString() & vbCr & vbLf & "SrcFuncID:" & (New String(aeh.SrcFuncID)).Trim() & vbCr & vbLf & "SrcPipeName:" & (New String(aeh.SrcPipeName)).Trim() & vbCr & vbLf & "メッセージ:" & msg, Me.NPS)
    End Sub

#End Region

#Region "各種ボタン"

    ''' <summary>WPFのThread</summary>
    Private Sub button1_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim bytes As Byte() = BinarySerialize.ObjectToBytes(Me.txtMSG.Text)

        AsyncEventFx.SendAsyncEvent(AsyncEventEnum.EventClass.Thread, "Thread", AsyncEventEnum.EventClass.Thread, "Thread", Me.NPCS(0), Me.NPS, _
         CUInt(bytes.Length), bytes)
    End Sub

    ''' <summary>WPFのThreadPool</summary>
    Private Sub button2_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim bytes As Byte() = BinarySerialize.ObjectToBytes(Me.txtMSG.Text)

        AsyncEventFx.SendAsyncEvent(AsyncEventEnum.EventClass.ThreadPool, "ThreadPool", AsyncEventEnum.EventClass.ThreadPool, "ThreadPool", Me.NPCS(0), Me.NPS, _
         CUInt(bytes.Length), bytes)
    End Sub

    ''' <summary>WPFのUIInvoke</summary>
    Private Sub button3_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim bytes As Byte() = BinarySerialize.ObjectToBytes(Me.txtMSG.Text)

        AsyncEventFx.SendAsyncEvent(AsyncEventEnum.EventClass.WPF, "WPF", AsyncEventEnum.EventClass.WPF, "WPF", Me.NPCS(0), Me.NPS, _
         CUInt(bytes.Length), bytes)
    End Sub

    ''' <summary>WinFormのUIInvoke</summary>
    Private Sub button4_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim bytes As Byte() = BinarySerialize.ObjectToBytes(Me.txtMSG.Text)

        AsyncEventFx.SendAsyncEvent(AsyncEventEnum.EventClass.WinForm, "WinForm", AsyncEventEnum.EventClass.WinForm, "WinForm", Me.NPCS(1), Me.NPS, _
         CUInt(bytes.Length), bytes)
    End Sub

    ''' <summary>へんなところ</summary>
    Private Sub button5_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim bytes As Byte() = BinarySerialize.ObjectToBytes(Me.txtMSG.Text)

        AsyncEventFx.SendAsyncEvent(AsyncEventEnum.EventClass.WinForm, "いいい", AsyncEventEnum.EventClass.WinForm, "いいい", Me.NPCS(1), "あああ", _
         CUInt(bytes.Length), bytes)
        'this.NPCS[1], this.NPS, (uint)bytes.Length, bytes);
    End Sub

#End Region

#Region "各種ボタン（エントリ）"

    ''' <summary>エントリを登録</summary>
    Private Sub button6_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        AsyncEventFx.RegisterAsyncEvent(Me.AeeTh)
        AsyncEventFx.RegisterAsyncEvent(Me.AeePl)
        AsyncEventFx.RegisterAsyncEvent(Me.AeeWin)
        AsyncEventFx.RegisterAsyncEvent(Me.AeeWPF)
    End Sub

    ''' <summary>エントリを削除</summary>
    Private Sub button7_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        AsyncEventFx.UnRegisterAsyncEvent(Me.AeeTh)
        AsyncEventFx.UnRegisterAsyncEvent(Me.AeePl)
        AsyncEventFx.UnRegisterAsyncEvent(Me.AeeWin)
        AsyncEventFx.UnRegisterAsyncEvent(Me.AeeWPF)
    End Sub

#End Region
End Class