<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlChild
    Inherits System.Windows.Forms.UserControl

    'UserControl はコンポーネント一覧をクリーンアップするために dispose をオーバーライドします。
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

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnUCButton1 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnUCButton1
        '
        Me.btnUCButton1.Location = New System.Drawing.Point(3, 3)
        Me.btnUCButton1.Name = "btnUCButton1"
        Me.btnUCButton1.Size = New System.Drawing.Size(144, 23)
        Me.btnUCButton1.TabIndex = 1
        Me.btnUCButton1.Text = "button1"
        Me.btnUCButton1.UseVisualStyleBackColor = True
        '
        'UserControlChild
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnUCButton1)
        Me.Name = "UserControlChild"
        Me.Size = New System.Drawing.Size(150, 30)
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents btnUCButton1 As Button
End Class
