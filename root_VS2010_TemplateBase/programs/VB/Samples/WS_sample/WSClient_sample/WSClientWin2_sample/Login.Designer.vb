Imports Touryo.Infrastructure.Business.RichClient.Presentation

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Login
    Inherits MyBaseControllerWin

    ''' <summary>
    ''' 必要なデザイナ変数です。
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' 使用中のリソースをすべてクリーンアップします。
    ''' </summary>
    ''' <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows フォーム デザイナで生成されたコード"

    ''' <summary>
    ''' デザイナ サポートに必要なメソッドです。このメソッドの内容を
    ''' コード エディタで変更しないでください。
    ''' </summary>
    Private Sub InitializeComponent()
        Me.btnButton1 = New System.Windows.Forms.Button()
        Me.textBox1 = New System.Windows.Forms.TextBox()
        Me.textBox2 = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        ' 
        ' btnButton1
        ' 
        Me.btnButton1.Location = New System.Drawing.Point(118, 12)
        Me.btnButton1.Name = "btnButton1"
        Me.btnButton1.Size = New System.Drawing.Size(75, 44)
        Me.btnButton1.TabIndex = 2
        Me.btnButton1.Text = "ログイン"
        Me.btnButton1.UseVisualStyleBackColor = True
        ' 
        ' textBox1
        ' 
        Me.textBox1.Location = New System.Drawing.Point(12, 12)
        Me.textBox1.Name = "textBox1"
        Me.textBox1.Size = New System.Drawing.Size(100, 19)
        Me.textBox1.TabIndex = 0
        ' 
        ' textBox2
        ' 
        Me.textBox2.Location = New System.Drawing.Point(12, 37)
        Me.textBox2.Name = "textBox2"
        Me.textBox2.Size = New System.Drawing.Size(100, 19)
        Me.textBox2.TabIndex = 1
        Me.textBox2.UseSystemPasswordChar = True
        ' 
        ' Login
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 12.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(202, 70)
        Me.Controls.Add(Me.textBox2)
        Me.Controls.Add(Me.textBox1)
        Me.Controls.Add(Me.btnButton1)
        Me.Name = "Login"
        Me.Text = "login"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private btnButton1 As System.Windows.Forms.Button
    Private textBox1 As System.Windows.Forms.TextBox
    Private textBox2 As System.Windows.Forms.TextBox
End Class
