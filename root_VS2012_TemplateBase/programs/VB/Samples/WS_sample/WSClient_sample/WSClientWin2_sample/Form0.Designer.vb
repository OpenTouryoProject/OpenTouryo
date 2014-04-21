Imports Touryo.Infrastructure.Business.RichClient.Presentation

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form0
    Inherits MyBaseControllerWin

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnOpenForm1 = New System.Windows.Forms.Button
        Me.btnOpenForm3 = New System.Windows.Forms.Button
        Me.btnClose = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'btnOpenForm1
        '
        Me.btnOpenForm1.Location = New System.Drawing.Point(12, 12)
        Me.btnOpenForm1.Name = "btnOpenForm1"
        Me.btnOpenForm1.Size = New System.Drawing.Size(518, 23)
        Me.btnOpenForm1.TabIndex = 0
        Me.btnOpenForm1.Text = "Form1(スレッド制御と画面制御の動作確認サンプル)を開く"
        Me.btnOpenForm1.UseVisualStyleBackColor = True
        '
        'btnOpenForm3
        '
        Me.btnOpenForm3.Location = New System.Drawing.Point(12, 41)
        Me.btnOpenForm3.Name = "btnOpenForm3"
        Me.btnOpenForm3.Size = New System.Drawing.Size(518, 23)
        Me.btnOpenForm3.TabIndex = 1
        Me.btnOpenForm3.Text = "Form3(種々のコントロールのイベント動作サンプル)を開く"
        Me.btnOpenForm3.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(12, 70)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(518, 23)
        Me.btnClose.TabIndex = 2
        Me.btnClose.Text = "終了"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'Form0
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(542, 106)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnOpenForm3)
        Me.Controls.Add(Me.btnOpenForm1)
        Me.Name = "Form0"
        Me.Text = "Form0"
        Me.ResumeLayout(False)

    End Sub

    Private btnOpenForm1 As System.Windows.Forms.Button
    Private btnOpenForm3 As System.Windows.Forms.Button
    Private btnClose As System.Windows.Forms.Button

End Class
