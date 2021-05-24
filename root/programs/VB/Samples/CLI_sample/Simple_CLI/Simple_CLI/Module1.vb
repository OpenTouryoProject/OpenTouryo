
' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Program
'* クラス日本語名  ：単純CLIサンプル アプリ
'*
'* 作成日時        ：－
'* 作成者          ：開発基盤部会
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Imports System.IO
Imports System.Threading
Imports System.Diagnostics

Imports System.CommandLine
Imports System.CommandLine.Invocation

Imports Sharprompt

Imports Newtonsoft
Imports Newtonsoft.Json
Module Module1

    ''' <summary>
    ''' VBで非同期化できる？
    ''' Function Main(args As String()) As Task(Of Integer)
    ''' </summary>
    ''' <param name="args">string[]</param>
    Sub Main(args As String())

#Region "rootCommand"
        ' Create a root command with some options
        ' alias、default value、description
        Dim rootCommand As Command = New RootCommand() From {
                New [Option](Of Integer)([alias]:="--int-option", getDefaultValue:=Function() 42, description:="An option whose argument is parsed as an int"),
                New [Option](Of Boolean)([alias]:="--bool-option", description:="An option whose argument is parsed as a bool"),
                New [Option](Of FileInfo)([alias]:="--file-option", description:="An option whose argument is parsed as a FileInfo"),
                New [Option](Of FileAccess)([alias]:="--file-access-option", getDefaultValue:=Function() FileAccess.Read, description:="An option whose argument is parsed as a FileAccess")
            }

        rootCommand.Description = "My sample app"

        ' Note that the parameters of the handler method are matched according to the names of the options
        rootCommand.Handler = CommandHandler.Create(Of Integer, Boolean, FileInfo, FileAccess)(AddressOf Module1.RootCommand)
#End Region

#Region "subCommand"

#Region "subCommand1"
        Dim subCommand1 As New Command(name:="cmd1", description:="Sub command cmd1")
        subCommand1.AddOption(New [Option](Of Integer)([alias]:="--an-int"))
        subCommand1.Handler = CommandHandler.Create(Of Integer)(
            Sub(anInt As Integer)
                Console.WriteLine("Sub command cmd1: {anInt}")
            End Sub)
        rootCommand.AddCommand(subCommand1)
#End Region

#Region "subCommand2"
        Dim subCommand2 As New Command(name:="cmd2", description:="Sub command cmd2")
        subCommand2.AddOption(New [Option](Of String)([alias]:="--a-string"))
        subCommand2.Handler = CommandHandler.Create(Of String)(
            Sub(aString As String)
                Console.WriteLine("Sub command cmd2: {aString}")
            End Sub)
        rootCommand.AddCommand(subCommand2)
#End Region

#Region "subCommandComplex"
        Dim subCommandComplex As New Command(name:="complex", description:="Sub command complex") From
            {
                New [Option](Of Integer)("--an-int"),
                New [Option](Of String)("--a-string")
            }

        subCommandComplex.Handler = CommandHandler.Create(
            Sub(complexType As ComplexType)
                Console.WriteLine("Sub command complex: {JsonConvert.SerializeObject(complexType)}")
            End Sub)
        rootCommand.AddCommand(subCommandComplex)
#End Region

#Region "subCommandInteractive"
        Dim subCommandInteractive As New Command(name:="interactive", description:="Sub command interactive")
        subCommandInteractive.AddOption(New [Option](Of String)([alias]:="--a-string"))
        subCommandInteractive.Handler = CommandHandler.Create(Of String)(
            Sub(aString As String)
                Console.WriteLine("Sub command interactive (Ctrl-C terminate): {aString}")
                Prompt.ColorSchema.Answer = ConsoleColor.DarkRed
                Prompt.ColorSchema.[Select] = ConsoleColor.DarkCyan
                Console.OutputEncoding = Encoding.UTF8

                Dim name As String = Prompt.Input(Of String)("名前")
                Console.WriteLine(String.Format("こんにちは, {0}", name))

                Dim age As Integer = Prompt.Input(Of Integer)("年齢")
                Console.WriteLine(String.Format("年齢: {0}", age))

                Dim password As String = Prompt.Password("Type new password")
                Console.WriteLine("Password OK")

                Dim answer As Boolean = Prompt.Confirm("Are you ready?")
                Console.WriteLine(String.Format("Your answer is {0}", answer))

                Dim value As EnumMonth = Prompt.[Select](Of EnumMonth)("Select enum value")
                Console.WriteLine(String.Format("You selected {0}", value))

                Dim prefectureList As String() = Nothing
                Module1.GetPrefectureList(prefectureList)
                Dim prefecture As String = Prompt.[Select]("都道府県", prefectureList, pageSize:=5)
                Console.WriteLine(String.Format("都道府県: {0}", prefecture))

            End Sub)
        rootCommand.AddCommand(subCommandInteractive)
#End Region

#Region "subCommandDelay"
        ' 非同期前提なので移植できず。
#End Region

#End Region

        ' テストの実行
        Module1.Test(rootCommand)

        ' Parse the incoming args and invoke the handler
        rootCommand.Invoke(args)

    End Sub

#Region "Command"
#Region "RootCommand"
    ''' <summary>RootCommand</summary>
    ''' <param name="intOption">int</param>
    ''' <param name="boolOption">bool</param>
    ''' <param name="fileOption">FileInfo</param>
    ''' <param name="fileAccessOption">FileAccess</param>
    Private Sub RootCommand(intOption As Integer, boolOption As Boolean, fileOption As FileInfo, fileAccessOption As FileAccess)

        Dim fileOptionName As String = Nothing
        If fileOption IsNot Nothing Then
            If fileOption.Name IsNot Nothing Then
                fileOptionName = fileOption.Name
            End If
        End If

        Console.WriteLine(String.Format(
          "--int-option is: {0}, " +
          "--bool-option is: {1}, " +
          "--file-option is: {2}, " +
          "--file-access-option is: {3}",
          intOption, fileOption, fileOptionName, fileAccessOption.ToString()))
    End Sub
#End Region
#End Region

#Region "TEST"
    ''' <summary>Test</summary>
    ''' <param name="rootCommand">Command</param>
    Private Sub Test(rootCommand As Command)
        ' デバッグ実行時だけ実行
        If Not Debugger.IsAttached Then
            Return
        End If

#Region "rootCommand"
        rootCommand.Invoke("")
        ' --int-option
        rootCommand.Invoke("--int-option")
        rootCommand.Invoke("--int-option 123")
        rootCommand.Invoke("--int-option hoge")
        ' --bool-option
        rootCommand.Invoke("--bool-option")
        rootCommand.Invoke("--bool-option False")
        rootCommand.Invoke("--bool-option True")
        rootCommand.Invoke("--bool-option hoge")
        ' --file-option
        rootCommand.Invoke("--file-option ../Program.cs")
        ' --file-access-option
        rootCommand.Invoke("--file-access-option Read")
        rootCommand.Invoke("--file-access-option Write")
        rootCommand.Invoke("--file-access-option hoge")
#End Region

#Region "subCommand"
        ' subCommand1
        rootCommand.Invoke("cmd1 --an-int 123")
        ' subCommand2
        rootCommand.Invoke("cmd2 --a-string hoge")
        ' subCommandComplex
        rootCommand.Invoke("complex --an-int 123 --a-string hogehoge")
        ' subCommandInteractive
        rootCommand.Invoke("interactive --a-string hoge")
        ' subCommandDelay
        rootCommand.Invoke("delay --a-string hogehoge")
#End Region

    End Sub
#End Region

#Region "SELECT"
    ''' <summary>EnumMonth</summary>
    Private Enum EnumMonth As Byte
        January = 1
        February
        March
        April
        May
        June
        July
        August
        September
        October
        November
        December
    End Enum

    ''' <summary>GetPrefectureList</summary>
    ''' <param name="prefectureList">string[]</param>
    Private Sub GetPrefectureList(ByRef prefectureList As String())
        prefectureList = New String() {
            "北海道", "青森県", "岩手県", "宮城県", "秋田県", "山形県",
            "福島県", "茨城県", "栃木県", "群馬県", "埼玉県", "千葉県",
            "東京都", "神奈川県", "新潟県", "富山県", "石川県", "福井県",
            "山梨県", "長野県", "岐阜県", "静岡県", "愛知県", "三重県",
            "滋賀県", "京都府", "大阪府", "兵庫県", "奈良県", "和歌山県",
            "鳥取県", "島根県", "岡山県", "広島県", "山口県", "徳島県",
            "香川県", "愛媛県", "高知県", "福岡県", "佐賀県", "長崎県",
            "熊本県", "大分県", "宮崎県", "鹿児島県", "沖縄県"}
    End Sub
#End Region

    ''' <summary>ComplexType</summary>
    Public Class ComplexType
        ' public ComplexType(int anInt, string aString)
        ' {
        '     AnInt = anInt;
        '     AString = aString;
        ' }
        Public Property AnInt() As Integer
            Get
                Return m_AnInt
            End Get
            Set
                m_AnInt = Value
            End Set
        End Property
        Private m_AnInt As Integer
        Public Property AString() As String
            Get
                Return m_AString
            End Get
            Set
                m_AString = Value
            End Set
        End Property
        Private m_AString As String
    End Class

End Module
