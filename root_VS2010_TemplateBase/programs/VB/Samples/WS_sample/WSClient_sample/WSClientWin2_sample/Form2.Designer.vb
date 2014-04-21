Imports Touryo.Infrastructure.Business.RichClient.Presentation

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
        Me.btnClose = New System.Windows.Forms.Button()
        Me.txtStatus = New System.Windows.Forms.TextBox()
        Me.txtGuid = New System.Windows.Forms.TextBox()
        Me.btnFormList = New System.Windows.Forms.Button()
        Me.btnFormCount = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        ' 
        ' btnClose
        ' 
        Me.btnClose.Location = New System.Drawing.Point(318, 12)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(78, 52)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "閉じる"
        Me.btnClose.UseVisualStyleBackColor = True
        ' 
        ' txtStatus
        ' 
        Me.txtStatus.Location = New System.Drawing.Point(12, 70)
        Me.txtStatus.Name = "txtStatus"
        Me.txtStatus.[ReadOnly] = True
        Me.txtStatus.Size = New System.Drawing.Size(384, 19)
        Me.txtStatus.TabIndex = 1
        ' 
        ' txtGuid
        ' 
        Me.txtGuid.Location = New System.Drawing.Point(12, 95)
        Me.txtGuid.Name = "txtGuid"
        Me.txtGuid.[ReadOnly] = True
        Me.txtGuid.Size = New System.Drawing.Size(384, 19)
        Me.txtGuid.TabIndex = 2
        ' 
        ' btnFormList
        ' 
        Me.btnFormList.Location = New System.Drawing.Point(12, 12)
        Me.btnFormList.Name = "btnFormList"
        Me.btnFormList.Size = New System.Drawing.Size(300, 23)
        Me.btnFormList.TabIndex = 3
        Me.btnFormList.Text = "Formを識別するIDをリストする。"
        Me.btnFormList.UseVisualStyleBackColor = True
        ' 
        ' btnFormCount
        ' 
        Me.btnFormCount.Location = New System.Drawing.Point(12, 41)
        Me.btnFormCount.Name = "btnFormCount"
        Me.btnFormCount.Size = New System.Drawing.Size(300, 23)
        Me.btnFormCount.TabIndex = 4
        Me.btnFormCount.Text = "全Formインスタンス数を表示する。"
        Me.btnFormCount.UseVisualStyleBackColor = True
        ' 
        ' Form2
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 12.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(408, 126)
        Me.Controls.Add(Me.btnFormCount)
        Me.Controls.Add(Me.btnFormList)
        Me.Controls.Add(Me.txtGuid)
        Me.Controls.Add(Me.txtStatus)
        Me.Controls.Add(Me.btnClose)
        Me.Name = "Form2"
        Me.Text = "Form2"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private btnClose As System.Windows.Forms.Button
    Private txtStatus As System.Windows.Forms.TextBox
    Private txtGuid As System.Windows.Forms.TextBox
    Private btnFormList As System.Windows.Forms.Button
    Private btnFormCount As System.Windows.Forms.Button

End Class
