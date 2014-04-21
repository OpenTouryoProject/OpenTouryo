using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WSClientSL_sample
{
    /// <summary>Silverlightのダイアログ画面</summary>
    public partial class DialogWindow : ChildWindow
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="Message">表示メッセージ</param>
        public DialogWindow(string Message)
        {
            InitializeComponent();

            this.textBlock1.Text = Message;
        }

        /// <summary>閉じるボタン</summary>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

