<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

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
        Me.button7 = New System.Windows.Forms.Button
        Me.button6 = New System.Windows.Forms.Button
        Me.button5 = New System.Windows.Forms.Button
        Me.lblMSG = New System.Windows.Forms.Label
        Me.button4 = New System.Windows.Forms.Button
        Me.button3 = New System.Windows.Forms.Button
        Me.button2 = New System.Windows.Forms.Button
        Me.txtMSG = New System.Windows.Forms.TextBox
        Me.button1 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'button7
        '
        Me.button7.Location = New System.Drawing.Point(13, 147)
        Me.button7.Name = "button7"
        Me.button7.Size = New System.Drawing.Size(305, 23)
        Me.button7.TabIndex = 17
        Me.button7.Text = "エントリ削除"
        Me.button7.UseVisualStyleBackColor = True
        '
        'button6
        '
        Me.button6.Location = New System.Drawing.Point(13, 118)
        Me.button6.Name = "button6"
        Me.button6.Size = New System.Drawing.Size(305, 23)
        Me.button6.TabIndex = 16
        Me.button6.Text = "エントリ登録"
        Me.button6.UseVisualStyleBackColor = True
        '
        'button5
        '
        Me.button5.Location = New System.Drawing.Point(12, 89)
        Me.button5.Name = "button5"
        Me.button5.Size = New System.Drawing.Size(305, 23)
        Me.button5.TabIndex = 15
        Me.button5.Text = "へんなところにつなぐ"
        Me.button5.UseVisualStyleBackColor = True
        '
        'lblMSG
        '
        Me.lblMSG.AutoSize = True
        Me.lblMSG.Location = New System.Drawing.Point(10, 9)
        Me.lblMSG.Name = "lblMSG"
        Me.lblMSG.Size = New System.Drawing.Size(50, 12)
        Me.lblMSG.TabIndex = 14
        Me.lblMSG.Text = "メッセージ"
        '
        'button4
        '
        Me.button4.Location = New System.Drawing.Point(168, 60)
        Me.button4.Name = "button4"
        Me.button4.Size = New System.Drawing.Size(150, 23)
        Me.button4.TabIndex = 13
        Me.button4.Text = "WPF-UIInvoke"
        Me.button4.UseVisualStyleBackColor = True
        '
        'button3
        '
        Me.button3.Location = New System.Drawing.Point(12, 60)
        Me.button3.Name = "button3"
        Me.button3.Size = New System.Drawing.Size(150, 23)
        Me.button3.TabIndex = 12
        Me.button3.Text = "WinForm-UIInvoke"
        Me.button3.UseVisualStyleBackColor = True
        '
        'button2
        '
        Me.button2.Location = New System.Drawing.Point(168, 31)
        Me.button2.Name = "button2"
        Me.button2.Size = New System.Drawing.Size(150, 23)
        Me.button2.TabIndex = 11
        Me.button2.Text = "スレッドプール(Win)"
        Me.button2.UseVisualStyleBackColor = True
        '
        'txtMSG
        '
        Me.txtMSG.Location = New System.Drawing.Point(66, 6)
        Me.txtMSG.Name = "txtMSG"
        Me.txtMSG.Size = New System.Drawing.Size(251, 19)
        Me.txtMSG.TabIndex = 10
        '
        'button1
        '
        Me.button1.Location = New System.Drawing.Point(12, 31)
        Me.button1.Name = "button1"
        Me.button1.Size = New System.Drawing.Size(150, 23)
        Me.button1.TabIndex = 9
        Me.button1.Text = "スレッド(Win)"
        Me.button1.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(329, 177)
        Me.Controls.Add(Me.button7)
        Me.Controls.Add(Me.button6)
        Me.Controls.Add(Me.button5)
        Me.Controls.Add(Me.lblMSG)
        Me.Controls.Add(Me.button4)
        Me.Controls.Add(Me.button3)
        Me.Controls.Add(Me.button2)
        Me.Controls.Add(Me.txtMSG)
        Me.Controls.Add(Me.button1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents button7 As System.Windows.Forms.Button
    Private WithEvents button6 As System.Windows.Forms.Button
    Private WithEvents button5 As System.Windows.Forms.Button
    Private WithEvents lblMSG As System.Windows.Forms.Label
    Private WithEvents button4 As System.Windows.Forms.Button
    Private WithEvents button3 As System.Windows.Forms.Button
    Private WithEvents button2 As System.Windows.Forms.Button
    Private WithEvents txtMSG As System.Windows.Forms.TextBox
    Private WithEvents button1 As System.Windows.Forms.Button

End Class
