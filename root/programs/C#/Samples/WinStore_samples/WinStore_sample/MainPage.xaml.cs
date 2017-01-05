using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using WinStore_sample.Views;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace WinStore_sample
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // DTO 汎用サービスインタフェースの CRUD サンプルの画面に切り替える
            this.ChangeScreen(new CRUDPage_mu());
        }

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、通常、ページを構成するために使用します。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ChangeScreen(Page targetPage)
        {
            this.MainFrame.Content = targetPage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // DTO 汎用サービスインタフェースの CRUD サンプルの画面に切り替える
            this.ChangeScreen(new CRUDPage_mu());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // XML 汎用サービスインタフェースの CRUD サンプルの画面に切り替える
            this.ChangeScreen(new CRUDPage_xl());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // JSON 汎用サービスインタフェースの CRUD サンプルの画面に切り替える
            this.ChangeScreen(new CRUDPage_jn());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // SOAP + Bean 汎用サービスインタフェースの CRUD サンプルの画面に切り替える
            this.ChangeScreen(new CRUDPage_sb());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // DTO サンプルの画面に切り替える
            this.ChangeScreen(new DTOPage());
        }
    }
}
