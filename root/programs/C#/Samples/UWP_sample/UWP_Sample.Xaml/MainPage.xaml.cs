using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace UWP_Sample.Xaml
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

        //private async void button_Click(object sender, RoutedEventArgs e)
        //{
        //    string message = "";

        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            Uri uri = new Uri("http://localhost:63877/SPA_Sample/api/SelectDT");

        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
        //            HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
        //        {
        //            { "ddlDap", "SQL" },
        //            { "ddlMode1", "individual" },
        //            { "ddlMode2", "static" },
        //            { "ddlExRollback", "-" }
        //        });

        //            HttpResponseMessage response = await client.PostAsync(uri, content);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                string result = await response.Content.ReadAsStringAsync();
        //                //result = result.Replace(@"\r\n", System.Environment.NewLine);
        //                result = result.Substring(1, result.Length - 1);
        //                DTTables dtTables = new DTTables();
        //                using (StringReader sr = new StringReader(result))
        //                {
        //                    dtTables.Load(sr);
        //                    message = string.Format("{0}件見つかりました", dtTables[0].Rows.Count);
        //                }
        //            }
        //            else
        //            {
        //                message = "HTTP Error! Status Code: " + response.StatusCode;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            message = ex.Message;
        //        }
        //        finally
        //        {
        //            this.textBlock.Text = message;
        //        }
        //    }
        //}

        private void toggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.splitView.IsPaneOpen = true;
        }

        private void toggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.splitView.IsPaneOpen = false;
        }

        private async void btnGetCount_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                try
                {
                    // Web API の URL
                    Uri uri = new Uri("http://localhost:63877/SPA_Sample/api/GetCount");

                    // Request Header を定義する
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    // Web API に送信するデータを構築する
                    HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "ddlDap", ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString() },
                        { "ddlMode1", ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString() },
                        { "ddlMode2", ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString() },
                        { "ddlExRollback", ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString() }
                    });

                    HttpResponseMessage response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        // 正常終了
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult);
                        message = dic["message"];
                    }
                    else
                    {
                        // エラー終了
                        message = "HTTP Error! Status Code: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    // 結果を表示
                    this.txtResult.Text = message;
                }
            }
        }

        private async void btnGetList_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                try
                {
                    // Web API の URL
                    Uri uri = new Uri("http://localhost:63877/SPA_Sample/api/SelectDT");

                    // Request Header を定義する
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    // Web API に送信するデータを構築する
                    HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "ddlDap", ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString() },
                        { "ddlMode1", ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString() },
                        { "ddlMode2", ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString() },
                        { "ddlExRollback", ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString() }
                    });

                    HttpResponseMessage response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        // 正常終了
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        List<Dictionary<string, string>> dic = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonResult);
                        this.lstRecords.ItemsSource = dic;

                        message = "正常終了しました";
                    }
                    else
                    {
                        // エラー終了
                        message = "HTTP Error! Status Code: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    // 結果を表示
                    this.txtResult.Text = message;
                }
            }
        }

        private async void btnGetRecord_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                try
                {
                    if (string.IsNullOrEmpty(this.txtShipperID.Text))
                    {
                        message = "検索する Shipper Id が入力されていません。１件表示の Shipper Id テキストボックスに、検索する Shipper Id を入力してください。";
                        return;
                    }

                    // Web API の URL
                    Uri uri = new Uri("http://localhost:63877/SPA_Sample/api/Select");

                    // Request Header を定義する
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    // Web API に送信するデータを構築する
                    HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "ddlDap", ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString() },
                        { "ddlMode1", ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString() },
                        { "ddlMode2", ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString() },
                        { "ddlExRollback", ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString() }, 
                        { "ShipperId", this.txtShipperID.Text }
                    });

                    HttpResponseMessage response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        // 正常終了
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult);

                        message = "正常終了しました";
                        this.txtCompanyName.Text = dic["companyName"];
                        this.txtPhone.Text = dic["phone"];
                    }
                    else
                    {
                        // エラー終了
                        message = "HTTP Error! Status Code: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    // 結果を表示
                    this.txtResult.Text = message;
                }
            }
        }

        private async void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                try
                {
                    if (string.IsNullOrEmpty(this.txtCompanyName.Text) || string.IsNullOrEmpty(this.txtPhone.Text))
                    {
                        message = "Company Name, Phone のいずれかが入力されていません。１件表示の Compnay Name, Phone テキストボックスに、追加する値を入力してください。（Shipper Id は、自動採番されます）";
                        return;
                    }

                    // Web API の URL
                    Uri uri = new Uri("http://localhost:63877/SPA_Sample/api/Insert");

                    // Request Header を定義する
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    // Web API に送信するデータを構築する
                    HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "ddlDap", ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString() },
                        { "ddlMode1", ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString() },
                        { "ddlMode2", ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString() },
                        { "ddlExRollback", ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString() },
                        { "CompanyName", this.txtCompanyName.Text },
                        { "Phone", this.txtPhone.Text }
                    });

                    HttpResponseMessage response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        // 正常終了
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult);
                        message = dic["message"];
                    }
                    else
                    {
                        // エラー終了
                        message = "HTTP Error! Status Code: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    // 結果を表示
                    this.txtResult.Text = message;
                }
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                try
                {
                    if (string.IsNullOrEmpty(this.txtShipperID.Text) || string.IsNullOrEmpty(this.txtCompanyName.Text) || string.IsNullOrEmpty(this.txtPhone.Text))
                    {
                        message = "Shipper Id, Company Name, Phone のいずれかが入力されていません。１件表示の Shipper Id テキストボックスに更新対象の Shipper Id の値を、Compnay Name, Phone テキストボックスに、更新する値をそれぞれ入力してください。";
                        return;
                    }

                    // Web API の URL
                    Uri uri = new Uri("http://localhost:63877/SPA_Sample/api/Update");

                    // Request Header を定義する
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    // Web API に送信するデータを構築する
                    HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "ddlDap", ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString() },
                        { "ddlMode1", ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString() },
                        { "ddlMode2", ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString() },
                        { "ddlExRollback", ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString() },
                        { "ShipperId", this.txtShipperID.Text }, 
                        { "CompanyName", this.txtCompanyName.Text },
                        { "Phone", this.txtPhone.Text }
                    });

                    HttpResponseMessage response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        // 正常終了
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult);
                        message = dic["message"];
                    }
                    else
                    {
                        // エラー終了
                        message = "HTTP Error! Status Code: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    // 結果を表示
                    this.txtResult.Text = message;
                }
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                try
                {
                    if (string.IsNullOrEmpty(this.txtShipperID.Text))
                    {
                        message = "削除する Shipper Id が入力されていません。１件表示の Shipper Id テキストボックスに削除対象の Shipper Id の値を入力してください。";
                        return;
                    }

                    // Web API の URL
                    Uri uri = new Uri("http://localhost:63877/SPA_Sample/api/Delete");

                    // Request Header を定義する
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    // Web API に送信するデータを構築する
                    HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "ddlDap", ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString() },
                        { "ddlMode1", ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString() },
                        { "ddlMode2", ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString() },
                        { "ddlExRollback", ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString() },
                        { "ShipperId", this.txtShipperID.Text }
                    });

                    HttpResponseMessage response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        // 正常終了
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult);
                        message = dic["message"];
                    }
                    else
                    {
                        // エラー終了
                        message = "HTTP Error! Status Code: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    // 結果を表示
                    this.txtResult.Text = message;
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // リストの内容をクリアする
            this.lstRecords.ItemsSource = null;
        }
    }
}
